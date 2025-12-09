using System;
using System.Drawing; // Required for Point (Location)

namespace graphSNA.Model
{
    /// <summary>
    /// Represents a user (vertex) in the social network graph.
    /// </summary>
    public class Node
    {
        // Unique identifier or name of the user (e.g., "Ali")
        public string Name { get; set; }

        // Numerical features required for dynamic weight calculation
        public double Activity { get; set; }        // Feature I (Aktiflik)
        public double Interaction { get; set; }     // Feature II (Etkileşim)
        public double ConnectionCount { get; set; } // Feature III (Bağlantı Sayısı)

        // Visual coordinates for drawing on the canvas
        public Point Location { get; set; }

        // Constructor
        public Node(string name, double activity, double interaction, double connectionCount)
        {
            Name = name;
            Activity = activity;
            Interaction = interaction;
            ConnectionCount = connectionCount;

            // Default location (will be updated by the visualizer later)
            Location = new Point(0, 0);
        }

        // Override ToString to display the Name easily in ListBoxes or Debugging
        public override string ToString()
        {
            return Name;
        }
    }
}