using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace WindsOfWar
{
    public class Scenario
    {
        public string Name { get; set; }
        public List<Terrain> TerrainList { get; set; } = new List<Terrain>();
        public Rectangle P1Deployment { get; set; }
        public Rectangle P2Deployment { get; set; }

        public Scenario(string name, Rectangle p1Dep, Rectangle p2Dep)
        {
            Name = name;
            P1Deployment = p1Dep;
            P2Deployment = p2Dep;
        }
    }
}
