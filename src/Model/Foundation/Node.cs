using System;
using System.Drawing; // Required for Point (Location)

namespace graphSNA.Model.Foundation
{
    /// <summary>
    ///  Represents a user (vertex) in the social network graph.
    /// </summary>
    public class Node
    {
        // Unique identifier of each user (e.g., "1", "2", "3", ...)
        public string Id { get; set; }
        // Nonunique name of the user (e.g., "Ali")
        public string Name { get; set; }
        // Numerical features required for dynamic weight calculation
        public float Activity { get; set; }        // Feature I (Aktiflik)
        public float Interaction { get; set; }     // Feature II (Etkileşim)
        public long ConnectionCount { get; set; } // Feature III (Bağlantı Sayısı)

        public Point Location { get; set; }
        public Color Color { get; set; } = Color.LightBlue;

        // Constructor
        public Node(string id, string name, float activity, float interaction)
        {
            Id = id;
            Name = name;
            Activity = activity;
            Interaction = interaction;
            // ConnectionCount == 0 by default, will be set when edges are added
        }

        // Override ToString to display the Name easily in ListBoxes or Debugging
        public override string ToString() => Name;
    }
}