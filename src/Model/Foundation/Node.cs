using System;
using System.Drawing;

namespace graphSNA.Model.Foundation
{
    /// <summary>
    /// Represents a user (vertex) in the social network graph.
    /// Contains user properties and visual attributes for rendering.
    /// </summary>
    public class Node
    {
        // Unique identifier (e.g., "1", "2", "3")
        public string Id { get; set; }
        
        // Display name (e.g., "Ali", "Mehmet") - not unique
        public string Name { get; set; }
        
        // Numerical features for dynamic weight calculation
        public float Activity { get; set; }        // Feature I: User activity score (0-100)
        public float Interaction { get; set; }     // Feature II: User interaction score (0-100)
        public long ConnectionCount { get; set; }  // Feature III: Number of connections (degree)

        // Visual properties
        public Point Location { get; set; }                    // Position on canvas (X, Y)
        public Color Color { get; set; } = Color.LightBlue;    // Node color (changed by coloring algorithm)

        public Node(string id, string name, float activity, float interaction)
        {
            Id = id;
            Name = name;
            Activity = activity;
            Interaction = interaction;
            // ConnectionCount starts at 0, updated when edges are added
        }

        // Used by ListBox and debugging to display node name
        public override string ToString() => Name;
    }
}