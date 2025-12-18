using System;

namespace graphSNA.Model
{
    /// <summary>
    ///  Represents a connection (link) between two nodes.
    ///  The graph is undirected and weighted.
    /// </summary>
    public class Edge
    {
        public Node Source { get; set; } // Starting Node
        public Node Target { get; set; } // Ending Node
        public double Weight { get; set; } // Cost of the connection

        public Edge(Node source, Node target)
        {
            Source = source;
            Target = target;
            CalculateWeight(); // Automatically calculate weight upon creation
        }

        /// <summary>
        ///  Calculates the weight based on the formula.
        ///  Formula: Weight = 1 / ( 1 + Sqrt( (Diff_Activity)^2 + (Diff_Interaction)^2 + (Diff_ConnectionCount)^2 ) )
        /// </summary>
        public void CalculateWeight()
        {
            // TODO: Implement the specific formula from Section 4.3
            // For now, we assign a default value of 1.0 to test the graph structure.
            this.Weight = 1.0;
        }
    }
}