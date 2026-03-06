using Microsoft.Xna.Framework;

namespace WindsOfWar
{
    /// <summary>Smoke marker from artillery smoke salvo. Blocks LOS and gives concealment; lasts a number of turns.</summary>
    public class SmokeMarker
    {
        public Vector2 Position { get; set; }
        public float Radius { get; set; } = 70f;
        public int RemainingTurns { get; set; } = 2;

        public SmokeMarker(Vector2 position, float radius, int durationTurns = 2)
        {
            Position = position;
            Radius = radius;
            RemainingTurns = durationTurns;
        }

        public bool Contains(Vector2 point)
        {
            return Vector2.DistanceSquared(Position, point) <= Radius * Radius;
        }
    }
}
