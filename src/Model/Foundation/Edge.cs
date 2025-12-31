using System;

namespace graphSNA.Model.Foundation
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
            float Diff_Act = Math.Abs(Source.Activity - Target.Activity);
            float Diff_Int = Math.Abs(Source.Interaction - Target.Interaction);
            float Diff_ConCount = Math.Abs(Source.ConnectionCount - Target.ConnectionCount);
            Weight = 1 / ( 1 + Math.Sqrt( Diff_Act * Diff_Act + Diff_Int * Diff_Int + Diff_ConCount * Diff_ConCount ) );
        }
    }
}