using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindsOfWar
{
    public enum TerrainType
    {
        Clear,
        Forest,
        Building,
        River,
        Hill
    }

    public class Terrain
    {
        public Rectangle Bounds { get; set; }
        public TerrainType Type { get; set; }
        public Color Color { get; set; }

        public Terrain(Rectangle bounds, TerrainType type)
        {
            Bounds = bounds;
            Type = type;
            switch (type)
            {
                case TerrainType.Forest: Color = Color.ForestGreen; break;
                case TerrainType.Building: Color = Color.Gray; break;
                case TerrainType.River: Color = Color.Blue; break;
                case TerrainType.Hill: Color = Color.SaddleBrown; break;
                default: Color = Color.Transparent; break;
            }
        }
    }
}
