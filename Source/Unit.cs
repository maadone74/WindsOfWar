using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace WindsOfWar
{
    public class Unit
    {
        public event Action? Died;

        public UnitData UnitData { get; set; }
        public int Team { get; set; } = 1;
        public int Health { get; set; }

        public Vector2 Position { get; set; }
        public Vector2 TargetPosition { get; set; }
        public bool IsSelected { get; set; }
        public bool HasMoved { get; set; }

        public Rectangle Bounds => new Rectangle((int)Position.X - 16, (int)Position.Y - 16, 32, 32);

        public Unit(UnitData data, int team, Vector2 position)
        {
            UnitData = data;
            Team = team;
            Position = position;
            TargetPosition = position;
            Health = data.Health;
        }

        private bool _isDead = false;

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Vector2.Distance(Position, TargetPosition) > 2)
            {
                Vector2 direction = Vector2.Normalize(TargetPosition - Position);
                float visualSpeed = 150f;
                Position += direction * visualSpeed * dt;

                if (Vector2.Distance(Position, TargetPosition) < 2)
                    Position = TargetPosition;
            }

            if (Health <= 0 && !_isDead)
            {
                _isDead = true;
                Died?.Invoke();
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            Color color = Team == 1 ? Color.CornflowerBlue : Color.IndianRed;
            if (IsSelected) color = Color.LightGreen;
            if (Health <= 0) color = Color.Gray; // Dead

            if (UnitData.Type == UnitType.Tank)
            {
                Rectangle rect = new Rectangle((int)Position.X - 16, (int)Position.Y - 16, 32, 32);
                spriteBatch.Draw(texture, rect, color);

                Rectangle turret = new Rectangle((int)Position.X - 8, (int)Position.Y - 8, 16, 16);
                spriteBatch.Draw(texture, turret, Color.DarkGray);

                Rectangle barrel = new Rectangle((int)Position.X, (int)Position.Y - 2, 24, 4);
                if (Team == 2) barrel.X -= 24;
                spriteBatch.Draw(texture, barrel, Color.Black);
            }
            else
            {
                // Infantry - small square
                Rectangle infRect = new Rectangle((int)Position.X - 10, (int)Position.Y - 10, 20, 20);
                spriteBatch.Draw(texture, infRect, color);
            }

            if (IsSelected)
            {
                // Simple selection corners
                int s = 16;
                int t = 2; // thickness
                // Top Left
                spriteBatch.Draw(texture, new Rectangle((int)Position.X - s - 4, (int)Position.Y - s - 4, 8, t), Color.White);
                spriteBatch.Draw(texture, new Rectangle((int)Position.X - s - 4, (int)Position.Y - s - 4, t, 8), Color.White);
                // Top Right
                spriteBatch.Draw(texture, new Rectangle((int)Position.X + s - 4, (int)Position.Y - s - 4, 8, t), Color.White);
                spriteBatch.Draw(texture, new Rectangle((int)Position.X + s + 4 - t, (int)Position.Y - s - 4, t, 8), Color.White);
                // Bottom Left
                spriteBatch.Draw(texture, new Rectangle((int)Position.X - s - 4, (int)Position.Y + s + 4 - t, 8, t), Color.White);
                spriteBatch.Draw(texture, new Rectangle((int)Position.X - s - 4, (int)Position.Y + s - 4, t, 8), Color.White);
                // Bottom Right
                spriteBatch.Draw(texture, new Rectangle((int)Position.X + s - 4, (int)Position.Y + s + 4 - t, 8, t), Color.White);
                spriteBatch.Draw(texture, new Rectangle((int)Position.X + s + 4 - t, (int)Position.Y + s - 4, t, 8), Color.White);
            }
        }

        public bool CanMoveTo(Vector2 newPosition)
        {
             return Vector2.Distance(Position, newPosition) <= UnitData.MovementDistance;
        }

        public void MoveTo(Vector2 newPosition)
        {
            if (!HasMoved && CanMoveTo(newPosition))
            {
                TargetPosition = newPosition;
                HasMoved = true;
            }
        }

        public string ResolveShooting(Unit target)
        {
            if (Health <= 0) return "Unit is dead.";
            if (target.Health <= 0) return "Target is already dead.";

            float dist = Vector2.Distance(Position, target.Position);
            if (dist > UnitData.Range) return "Out of Range!";

            int rof = HasMoved ? UnitData.MovingROF : UnitData.HaltedROF;
            if (rof <= 0) return "No ROF!";

            Random rand = new Random();
            int hits = 0;
            string log = "";

            for(int i=0; i<rof; i++)
            {
                int roll = rand.Next(1, 7);
                if (roll >= UnitData.Skill) hits++;
            }

            if (hits == 0) return "Miss!";
            log += $"Hit {hits}x! ";

            for(int i=0; i<hits; i++)
            {
                if (target.Health <= 0) break;

                bool destroyed = false;
                bool bailed = false;

                if (target.UnitData.Type == UnitType.Tank)
                {
                    if (UnitData.AntiTank == 0)
                    {
                        log += "Bounce. ";
                    }
                    else
                    {
                        // Tank vs Tank Combat (Equation of War)
                        int atRoll = rand.Next(1, 7) + UnitData.AntiTank;
                        int armorRoll = rand.Next(1, 7) + target.UnitData.FrontArmor; // Simplified to Front Armor

                        if (atRoll > armorRoll)
                        {
                            // Penetrated
                            int fpRoll = rand.Next(1, 7);
                            if (fpRoll >= UnitData.Firepower)
                            {
                                destroyed = true;
                                log += "Penetrated! Destroyed! ";
                            }
                            else
                            {
                                bailed = true;
                                log += "Penetrated! Bailed! ";
                            }
                        }
                        else
                        {
                            log += "Bounce. ";
                        }
                    }
                }
                else // Infantry
                {
                    int saveRoll = rand.Next(1, 7);
                    if (saveRoll < target.UnitData.Save)
                    {
                        destroyed = true;
                        log += "Hit! Dead! ";
                    }
                    else
                    {
                        log += "Saved. ";
                    }
                }

                if (destroyed)
                {
                    target.Health = 0;
                }
            }
            return log;
        }
    }
}
