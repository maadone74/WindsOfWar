using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace WindsOfWar
{
    /// <summary>
    /// Objective marker for scenario victory (hold at game end or for points).
    /// </summary>
    public class Objective
    {
        public Rectangle Bounds { get; set; }
        public string Label { get; set; } = "Objective";

        public Objective(Rectangle bounds, string label = "Objective")
        {
            Bounds = bounds;
            Label = label;
        }
    }

    public class Scenario
    {
        public string Name { get; set; }
        public List<Terrain> TerrainList { get; set; } = new List<Terrain>();
        public Rectangle P1Deployment { get; set; }
        public Rectangle P2Deployment { get; set; }
        /// <summary>Objective zones; control by having a unit in the bounds.</summary>
        public List<Objective> Objectives { get; set; } = new List<Objective>();

        public Scenario(string name, Rectangle p1Dep, Rectangle p2Dep)
        {
            Name = name;
            P1Deployment = p1Dep;
            P2Deployment = p2Dep;
        }
    }
}
