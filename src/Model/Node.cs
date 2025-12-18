using System;
using System.Drawing; // Required for Point (Location)

namespace graphSNA.Model
{
    /// <summary>
    /// Represents a user (vertex) in the social network graph.
    /// </summary>
    public class Node
    {
        public string Id { get; set; }
        // Unique identifier or name of the user (e.g., "Ali")
        public string Name { get; set; }

        // Numerical features required for dynamic weight calculation
        public double Activity { get; set; }        // Feature I (Aktiflik)
        public double Interaction { get; set; }     // Feature II (Etkileşim)
        // Bu artık dışarıdan gelmiyor, biz hesaplayacağız.
        public double ConnectionCount { get; set; } // Feature III (Bağlantı Sayısı)

        public System.Drawing.Point Location { get; set; }
        public System.Drawing.Color Color { get; set; } = System.Drawing.Color.LightBlue;

        // Constructor
        public Node(string id, string name, double activity, double interaction)
        {
            this.Id = id;
            this.Name = name;
            this.Activity = activity;
            this.Interaction = interaction;
            // ConnectionCount varsayılan 0 başlar
        }

        // Override ToString to display the Name easily in ListBoxes or Debugging
        // public override string ToString() => Name;
        public override string ToString()
        {
            return Name;
        }
    }
}