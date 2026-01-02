using System;
using System.Drawing;
using System.Collections.Generic;

namespace graphSNA.Model.Foundation
{
    /// <summary>
    /// Force-directed graph layout algorithm using Fruchterman-Reingold approach.
    /// Positions nodes by simulating repulsion between all nodes and attraction along edges.
    /// Like magnets: nodes push each other away, but edges pull connected nodes together.
    /// </summary>
    public class Layout
    {
        private double k;          // Ideal spring length (optimal distance between nodes)
        private int AreaWidth;     // Canvas width
        private int AreaHeight;    // Canvas height

        public void CalculateLayout(Graph graph, int width, int height, int iterations = 100)
        {
            AreaWidth = width;
            AreaHeight = height;

            // Center point - nodes will be pulled toward this (gravity)
            float centerX = width / 2f;
            float centerY = height / 2f;

            // Calculate ideal spring length based on area and node count
            // Formula: k = sqrt(area / nodeCount) * compressionFactor
            // Smaller k = nodes closer together
            double area = width * height;
            k = Math.Sqrt(area / (graph.Nodes.Count + 1)) * 0.6;

            // Temperature controls how much nodes can move per iteration
            // Starts high (big movements) and decreases (fine-tuning)
            double temperature = width / 10.0;

            // Stores movement vector for each node in current iteration
            Dictionary<Node, PointF> displacements = new Dictionary<Node, PointF>();

            // Main simulation loop - runs 100 times by default
            for (int i = 0; i < iterations; i++)
            {
                // ============================================
                // STEP 1: REPULSION - All nodes push each other away
                // Like same-pole magnets repelling
                // ============================================
                foreach (var v in graph.Nodes)
                {
                    displacements[v] = PointF.Empty;  // Reset displacement
                    
                    foreach (var u in graph.Nodes)
                    {
                        if (v == u) continue;  // Skip self
                        
                        // Calculate distance between nodes
                        double dx = v.Location.X - u.Location.X;
                        double dy = v.Location.Y - u.Location.Y;
                        double dist = Math.Sqrt(dx * dx + dy * dy);  // Pythagorean theorem
                        
                        if (dist < 0.1) dist = 0.1;  // Prevent division by zero

                        // Repulsion formula: force = k² / distance
                        // Closer nodes = stronger repulsion
                        double force = (k * k) / dist;
                        
                        // Add force to displacement (direction: away from other node)
                        displacements[v] = new PointF(
                            displacements[v].X + (float)(dx / dist * force),
                            displacements[v].Y + (float)(dy / dist * force)
                        );
                    }
                }

                // ============================================
                // STEP 2: ATTRACTION - Connected nodes pull each other
                // Like springs connecting nodes
                // ============================================
                foreach (var edge in graph.Edges)
                {
                    Node v = edge.Source;
                    Node u = edge.Target;

                    double dx = v.Location.X - u.Location.X;
                    double dy = v.Location.Y - u.Location.Y;
                    double dist = Math.Sqrt(dx * dx + dy * dy);

                    if (dist < 0.1) dist = 0.1;

                    // Weight factor: higher edge weight = stronger attraction
                    // Squared weight makes difference more dramatic
                    // Weight=1 -> factor=21, Weight=0.5 -> factor=6
                    double weightFactor = 1.0 + (edge.Weight * edge.Weight * 20.0);

                    // Attraction formula: force = (distance² / k) * weightFactor
                    double force = ((dist * dist) / k) * weightFactor;

                    float dispX = (float)(dx / dist * force);
                    float dispY = (float)(dy / dist * force);

                    // Move both nodes toward each other
                    displacements[v] = new PointF(displacements[v].X - dispX, displacements[v].Y - dispY);
                    displacements[u] = new PointF(displacements[u].X + dispX, displacements[u].Y + dispY);
                }

                // ============================================
                // STEP 3: GRAVITY - Pull all nodes toward center
                // Prevents graph from drifting to infinity
                // ============================================
                foreach (var v in graph.Nodes)
                {
                    double dx = centerX - v.Location.X;
                    double dy = centerY - v.Location.Y;
                    double dist = Math.Sqrt(dx * dx + dy * dy);

                    // Gravity strength (0.20 * k)
                    double gravityForce = 0.20 * k;

                    if (dist > 0)
                    {
                        displacements[v] = new PointF(
                            displacements[v].X + (float)(dx / dist * gravityForce),
                            displacements[v].Y + (float)(dy / dist * gravityForce)
                        );
                    }
                }

                // ============================================
                // STEP 4: Apply displacements to node positions
                // ============================================
                foreach (var v in graph.Nodes)
                {
                    PointF disp = displacements[v];
                    double length = Math.Sqrt(disp.X * disp.X + disp.Y * disp.Y);

                    if (length > 0)
                    {
                        // Limit movement by temperature (prevents oscillation)
                        double limitedLength = Math.Min(length, temperature);

                        // Normalize and scale displacement
                        disp.X = (float)(disp.X / length * limitedLength);
                        disp.Y = (float)(disp.Y / length * limitedLength);

                        // Update node position
                        int newX = (int)(v.Location.X + disp.X);
                        int newY = (int)(v.Location.Y + disp.Y);

                        v.Location = new Point(newX, newY);
                    }
                }

                // Cool down: reduce temperature by 5% each iteration
                // Early iterations: big movements, later: small adjustments
                temperature *= 0.95;
            }
        }

        private const int NodeSize = 30;
    }
}