using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace WindsOfWar
{
    public class Unit
    {
        public event Action? Died;
        public event Action<string>? OnSoundTriggered;

        public UnitData UnitData { get; set; }
        public int Team { get; set; } = 1;
        public int Health { get; set; }

        public Vector2 Position { get; set; }
        public Vector2 TargetPosition { get; set; }
        public bool IsSelected { get; set; }
        public bool HasMoved { get; set; }
        public bool IsBailed { get; set; }
        public HashSet<int> FiredWeaponIndices { get; set; } = new HashSet<int>();
        public int SelectedWeaponIndex { get; set; } = 0;

        public Weapon? SelectedWeapon
        {
            get
            {
                if (UnitData.Weapons == null || UnitData.Weapons.Count == 0) return null;
                if (SelectedWeaponIndex < 0 || SelectedWeaponIndex >= UnitData.Weapons.Count) SelectedWeaponIndex = 0;
                return UnitData.Weapons[SelectedWeaponIndex];
            }
        }

        public void CycleWeapon()
        {
            if (UnitData.Weapons == null || UnitData.Weapons.Count == 0) return;
            SelectedWeaponIndex++;
            if (SelectedWeaponIndex >= UnitData.Weapons.Count)
                SelectedWeaponIndex = 0;
        }

        public Rectangle Bounds => new Rectangle((int)Position.X - 16, (int)Position.Y - 16, 32, 32);

        public float Facing { get; set; }

        public Unit(UnitData data, int team, Vector2 position)
        {
            UnitData = data;
            Team = team;
            Position = position;
            TargetPosition = position;
            Health = data.Health;
            Facing = Team == 2 ? MathF.PI : 0f;
        }

        private bool _isDead = false;

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Vector2.Distance(Position, TargetPosition) > 2)
            {
                Vector2 direction = Vector2.Normalize(TargetPosition - Position);
                Facing = MathF.Atan2(direction.Y, direction.X);
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
            if (IsBailed) color = Color.Yellow; // Bailed
            if (IsSelected) color = Color.LightGreen;
            if (Health <= 0) color = Color.Gray; // Dead

            if (UnitData.Type == UnitType.Tank)
            {
                Rectangle rect = new Rectangle((int)Position.X - 16, (int)Position.Y - 16, 32, 32);
                spriteBatch.Draw(texture, rect, color);

                // Turret (Rotated)
                spriteBatch.Draw(texture, Position, null, Color.DarkGray, Facing, new Vector2(0.5f, 0.5f), new Vector2(16, 16), SpriteEffects.None, 0f);

                // Barrel (Rotated)
                spriteBatch.Draw(texture, Position, null, Color.Black, Facing, new Vector2(0f, 0.5f), new Vector2(24, 4), SpriteEffects.None, 0f);
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

        public void ResetTurn()
        {
            HasMoved = false;
            FiredWeaponIndices.Clear();
        }

        public bool Remount()
        {
            if (!IsBailed) return true;

            // Simple Skill Check to Remount
            int roll = Random.Shared.Next(1, 7);
            if (roll >= UnitData.Skill)
            {
                IsBailed = false;
                return true;
            }
            return false;
        }

        public int GetArmorAgainst(Vector2 shooterPosition)
        {
             // Calculate angle to shooter
             Vector2 direction = shooterPosition - Position;
             float angleToShooter = MathF.Atan2(direction.Y, direction.X);

             // Normalize angles to -PI to PI
             float diff = angleToShooter - Facing;
             while (diff > MathF.PI) diff -= 2 * MathF.PI;
             while (diff < -MathF.PI) diff += 2 * MathF.PI;

             // Check Front Arc (+/- 45 degrees, i.e., PI/4)
             if (Math.Abs(diff) <= MathF.PI / 4)
             {
                 return UnitData.FrontArmor;
             }
             return UnitData.SideArmor;
        }

        public void MoveTo(Vector2 newPosition)
        {
            if (IsBailed) return;

            if (!HasMoved && CanMoveTo(newPosition))
            {
                TargetPosition = newPosition;
                HasMoved = true;
                OnSoundTriggered?.Invoke("move");
            }
        }

        public string ResolveShooting(Unit target, List<Terrain> terrainList)
        {
            if (Health <= 0) return "Unit is dead.";
            if (IsBailed) return "Unit is Bailed Out and cannot shoot.";
            if (target.Health <= 0) return "Target is already dead.";

            if (FiredWeaponIndices.Contains(SelectedWeaponIndex))
                return "This weapon already fired this turn!";

            Weapon? weapon = SelectedWeapon;
            if (weapon == null) return "No Weapon!";

            float dist = Vector2.Distance(Position, target.Position);
            if (dist > weapon.Range) return "Out of Range!";

            int rof = HasMoved ? weapon.MovingROF : weapon.HaltedROF;
            if (rof <= 0) return "No ROF!";

            // Check Concealment
            bool concealed = false;
            foreach (var t in terrainList)
            {
                if ((t.Type == TerrainType.Forest || t.Type == TerrainType.Building) && t.Bounds.Contains(target.Position))
                {
                    concealed = true;
                    break;
                }
            }

            int hits = 0;
            string log = "";
            if (concealed) log += "(Concealed +1 to Hit) ";

            // Mark as shot
            FiredWeaponIndices.Add(SelectedWeaponIndex);

            // Update Facing to target
            Vector2 direction = target.Position - Position;
            Facing = MathF.Atan2(direction.Y, direction.X);

            OnSoundTriggered?.Invoke("shoot");

            int toHit = UnitData.Skill;
            if (concealed) toHit++;

            for(int i=0; i<rof; i++)
            {
                int roll = Random.Shared.Next(1, 7);
                if (roll >= toHit) hits++;
            }

            if (hits == 0) return "Miss!";
            log += $"Hit {hits}x! ";

            for(int i=0; i<hits; i++)
            {
                if (target.Health <= 0) break;

                bool destroyed = false;

                if (target.UnitData.Type == UnitType.Tank)
                {
                    if (weapon.AntiTank == 0)
                    {
                        log += "Bounce. ";
                    }
                    else
                    {
                        // Tank vs Tank Combat (Equation of War)
                        int atRoll = Random.Shared.Next(1, 7) + weapon.AntiTank;
                        int targetArmor = target.GetArmorAgainst(Position);
                        int armorRoll = Random.Shared.Next(1, 7) + targetArmor;

                        if (atRoll > armorRoll)
                        {
                            // Penetrated
                            int fpRoll = Random.Shared.Next(1, 7);
                            if (fpRoll >= weapon.Firepower)
                            {
                                destroyed = true;
                                log += "Penetrated! Destroyed! ";
                            }
                            else
                            {
                                target.IsBailed = true;
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
                    int saveRoll = Random.Shared.Next(1, 7);
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
