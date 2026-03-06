using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public bool HasAssaulted { get; set; }
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
            Color dark = Team == 1 ? new Color(40, 60, 120) : new Color(120, 50, 50);
            if (IsBailed) color = Color.Yellow;
            if (IsSelected) color = Color.LightGreen;
            if (Health <= 0) color = Color.Gray;

            if (UnitData.Type == UnitType.Tank)
                DrawTank(spriteBatch, texture, color, dark);
            else if (UnitData.Type == UnitType.TankDestroyer)
                DrawTankDestroyer(spriteBatch, texture, color, dark);
            else if (UnitData.Type == UnitType.Gun)
                DrawGun(spriteBatch, texture, color, dark);
            else
                DrawInfantry(spriteBatch, texture, color, dark);

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

        private void DrawInfantry(SpriteBatch spriteBatch, Texture2D texture, Color color, Color dark)
        {
            // Squad: wedge of three figures facing Facing; small base arc
            float cos = MathF.Cos(Facing), sin = MathF.Sin(Facing);
            int r = 4;
            Vector2 fwd = new Vector2(cos, sin), side = new Vector2(-sin, cos);
            Vector2 p1 = Position - fwd * 6 - side * 5;
            Vector2 p2 = Position - fwd * 6 + side * 5;
            Vector2 p3 = Position + fwd * 4;
            spriteBatch.Draw(texture, new Rectangle((int)p1.X - r, (int)p1.Y - r, r * 2, r * 2), dark);
            spriteBatch.Draw(texture, new Rectangle((int)p2.X - r, (int)p2.Y - r, r * 2, r * 2), dark);
            spriteBatch.Draw(texture, new Rectangle((int)p3.X - r, (int)p3.Y - r, r * 2, r * 2), color);
            // Base arc (small ellipse behind squad)
            DrawRotatedRect(spriteBatch, texture, Position - fwd * 2, Facing, 18, 10, new Color(0, 0, 0, 120));
        }

        private void DrawTank(SpriteBatch spriteBatch, Texture2D texture, Color color, Color dark)
        {
            string name = UnitData.Name ?? "";
            float cos = MathF.Cos(Facing), sin = MathF.Sin(Facing);

            // Top-down: hull length (along tank) and width (across). Turret radius, barrel length.
            int hullLength = 34, hullWidth = 18, turretRadius = 9, barrelLength = 26;
            if (name.Contains("Tiger"))
            { hullLength = 40; hullWidth = 22; turretRadius = 12; barrelLength = 32; }
            else if (name.Contains("Panther"))
            { hullLength = 38; hullWidth = 20; turretRadius = 11; barrelLength = 30; }
            else if (name.Contains("StuG"))
            { hullLength = 36; hullWidth = 16; turretRadius = 0; barrelLength = 28; }
            else if (name.Contains("76mm"))
            { hullLength = 34; hullWidth = 18; turretRadius = 9; barrelLength = 28; }
            else if (name.Contains("Panzer IV"))
            { hullLength = 34; hullWidth = 20; turretRadius = 10; barrelLength = 26; }
            else if (name.Contains("Panzer III"))
            { hullLength = 30; hullWidth = 16; turretRadius = 8; barrelLength = 22; }
            else if (name.Contains("Sherman"))
            { hullLength = 32; hullWidth = 18; turretRadius = 9; barrelLength = 24; }

            // Hull: elongated rectangle (length along facing, width across)
            DrawRotatedRect(spriteBatch, texture, Position, Facing, hullLength, hullWidth, color);

            if (turretRadius > 0)
            {
                // Turret: circle (drawn as square) centered on hull
                DrawRotatedRect(spriteBatch, texture, Position, Facing, turretRadius * 2, turretRadius * 2, dark);
                // Barrel from front of turret to tip
                Vector2 barrelStart = Position + new Vector2(cos * turretRadius, sin * turretRadius);
                Vector2 barrelEnd = Position + new Vector2(cos * barrelLength, sin * barrelLength);
                DrawThickLine(spriteBatch, texture, barrelStart, barrelEnd, 4, Color.Black);
            }
            else
            {
                // Casemate (StuG): barrel from front of hull
                Vector2 barrelStart = Position + new Vector2(cos * (hullLength / 2), sin * (hullLength / 2));
                Vector2 barrelEnd = Position + new Vector2(cos * barrelLength, sin * barrelLength);
                DrawThickLine(spriteBatch, texture, barrelStart, barrelEnd, 5, Color.Black);
            }

            // Track strips (refined: two thin lines along hull sides)
            float perpX = -sin * (hullWidth / 2f + 2), perpY = cos * (hullWidth / 2f + 2);
            Vector2 backL = Position + new Vector2(-cos * (hullLength / 2f) + perpX, -sin * (hullLength / 2f) + perpY);
            Vector2 backR = Position + new Vector2(-cos * (hullLength / 2f) - perpX, -sin * (hullLength / 2f) - perpY);
            Vector2 frontL = Position + new Vector2(cos * (hullLength / 2f) + perpX, sin * (hullLength / 2f) + perpY);
            Vector2 frontR = Position + new Vector2(cos * (hullLength / 2f) - perpX, sin * (hullLength / 2f) - perpY);
            DrawThickLine(spriteBatch, texture, backL, frontL, 3, dark);
            DrawThickLine(spriteBatch, texture, backR, frontR, 3, dark);
        }

        private void DrawTankDestroyer(SpriteBatch spriteBatch, Texture2D texture, Color color, Color dark)
        {
            string name = UnitData.Name ?? "";
            float cos = MathF.Cos(Facing), sin = MathF.Sin(Facing);
            int hullLength = 32, hullWidth = 16, barrelLength = 28;
            if (name.Contains("Hellcat")) { hullLength = 30; hullWidth = 14; barrelLength = 30; }
            else if (name.Contains("Wolverine") || name.Contains("M10")) { hullLength = 34; hullWidth = 18; barrelLength = 28; }

            DrawRotatedRect(spriteBatch, texture, Position, Facing, hullLength, hullWidth, color);
            // Open-topped angular turret (wider than tall)
            DrawRotatedRect(spriteBatch, texture, Position, Facing, 20, 12, dark);
            Vector2 barrelStart = Position + new Vector2(cos * 10, sin * 10);
            Vector2 barrelEnd = Position + new Vector2(cos * barrelLength, sin * barrelLength);
            DrawThickLine(spriteBatch, texture, barrelStart, barrelEnd, 4, Color.Black);
        }

        private void DrawGun(SpriteBatch spriteBatch, Texture2D texture, Color color, Color dark)
        {
            string name = UnitData.Name ?? "";
            float cos = MathF.Cos(Facing), sin = MathF.Sin(Facing);
            bool howitzer = name.Contains("105");
            int shieldAlong = howitzer ? 22 : 20, shieldAcross = 14;
            int barrelLen = howitzer ? 20 : 26;

            // Gun shield rotated with facing (rect along barrel axis)
            DrawRotatedRect(spriteBatch, texture, Position, Facing, shieldAlong, shieldAcross, color);
            DrawRotatedRect(spriteBatch, texture, Position, Facing, shieldAlong / 2, shieldAcross / 2, dark);
            Vector2 barrelEnd = Position + new Vector2(cos * barrelLen, sin * barrelLen);
            DrawThickLine(spriteBatch, texture, Position, barrelEnd, howitzer ? 5 : 4, Color.DarkGray);
        }

        private static void DrawRotatedRect(SpriteBatch spriteBatch, Texture2D texture, Vector2 center, float angle, int width, int height, Color color)
        {
            // Origin 0.5,0.5 = center of 1x1 texture so rect is centered; scale gives size in pixels
            spriteBatch.Draw(texture, center, null, color, angle, new Vector2(0.5f, 0.5f), new Vector2(width, height), SpriteEffects.None, 0f);
        }

        private static void DrawThickLine(SpriteBatch spriteBatch, Texture2D texture, Vector2 a, Vector2 b, int thickness, Color color)
        {
            Vector2 d = b - a;
            float len = d.Length();
            if (len < 0.1f) return;
            Vector2 perp = new Vector2(-d.Y, d.X) / len;
            Vector2 h = perp * (thickness / 2f);
            Vector2 o = a + d * 0.5f;
            float angle = MathF.Atan2(d.Y, d.X);
            spriteBatch.Draw(texture, o, null, color, angle, new Vector2(0.5f, 0.5f), new Vector2(len, thickness), SpriteEffects.None, 0f);
        }

        public bool CanMoveTo(Vector2 newPosition)
        {
             return Vector2.Distance(Position, newPosition) <= UnitData.MovementDistance;
        }

        public void ResetTurn()
        {
            HasMoved = false;
            HasAssaulted = false;
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

        public string ResolveShooting(Unit target, List<Terrain> terrainList, List<SmokeMarker>? smokeMarkers = null)
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

            // Check Concealment (terrain + smoke)
            bool concealed = false;
            foreach (var t in terrainList)
            {
                if ((t.Type == TerrainType.Forest || t.Type == TerrainType.Building) && t.Bounds.Contains(target.Position))
                {
                    concealed = true;
                    break;
                }
            }
            if (!concealed && smokeMarkers != null)
            {
                foreach (var sm in smokeMarkers)
                {
                    if (sm.RemainingTurns > 0 && sm.Contains(target.Position))
                    {
                        concealed = true;
                        break;
                    }
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
            // Long range: harder to hit (Flames of War rule)
            if (weapon.LongRange > 0 && dist >= weapon.LongRange)
                toHit += weapon.LongRangeToHitModifier;

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

                if (target.UnitData.Type == UnitType.Tank || target.UnitData.Type == UnitType.TankDestroyer)
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
                else // Infantry or Gun (use Save)
                {
                    int saveRoll = Random.Shared.Next(1, 7);
                    // Hill: defensive position gives +1 to save (Flames of War gone to ground / dug in)
                    bool inHill = false;
                    foreach (var t in terrainList)
                    {
                        if (t.Type == TerrainType.Hill && t.Bounds.Contains(target.Position))
                        {
                            inHill = true;
                            break;
                        }
                    }
                    if (inHill) saveRoll++;
                    if (saveRoll < target.UnitData.Save)
                    {
                        destroyed = true;
                        log += "Hit! Dead! ";
                    }
                    else
                    {
                        log += inHill ? "Saved (in cover). " : "Saved. ";
                    }
                }

                if (destroyed)
                {
                    target.Health = 0;
                }
            }
            return log;
        }

        /// <summary>Artillery: fire at a point; template covers TemplateRadius. All units under template can be hit. Smoke salvo places marker only.</summary>
        public static string ResolveArtilleryShot(Unit shooter, Vector2 targetPoint, List<Unit> allUnits, List<Terrain> terrainList, bool smokeMode, out SmokeMarker? smokePlaced)
        {
            smokePlaced = null;
            Weapon? weapon = shooter.SelectedWeapon;
            if (weapon == null || !weapon.IsArtillery) return "Not an artillery weapon.";
            if (shooter.Health <= 0 || shooter.IsBailed) return "Shooter cannot fire.";
            if (shooter.FiredWeaponIndices.Contains(shooter.SelectedWeaponIndex)) return "Already fired this turn.";
            float dist = Vector2.Distance(shooter.Position, targetPoint);
            if (dist > weapon.Range) return "Target point out of range.";
            int rof = shooter.HasMoved ? weapon.MovingROF : weapon.HaltedROF;
            if (rof <= 0 && !smokeMode) return "No ROF.";

            if (smokeMode)
            {
                if (!weapon.CanSmoke) return "This weapon cannot fire smoke.";
                shooter.FiredWeaponIndices.Add(shooter.SelectedWeaponIndex);
                smokePlaced = new SmokeMarker(targetPoint, weapon.TemplateRadius, 2);
                return $"Smoke salvo! Marker placed for 2 turns.";
            }

            shooter.FiredWeaponIndices.Add(shooter.SelectedWeaponIndex);
            float r2 = weapon.TemplateRadius * weapon.TemplateRadius;
            var under = allUnits.Where(u => u.Health > 0 && Vector2.DistanceSquared(u.Position, targetPoint) <= r2).ToList();
            if (under.Count == 0) return "Artillery on target — no units under template.";

            int hits = 0;
            for (int i = 0; i < rof; i++)
            {
                if (Random.Shared.Next(1, 7) >= shooter.UnitData.Skill) hits++;
            }
            string log = $"Template ({weapon.TemplateRadius}\") → {under.Count} unit(s) under — {hits} hit(s). ";
            for (int i = 0; i < hits && under.Count > 0; i++)
            {
                var pick = under[Random.Shared.Next(under.Count)];
                bool destroyed;
                string snippet = ApplyOneHitFrom(shooter, pick, weapon, terrainList, out destroyed);
                log += snippet;
                if (destroyed)
                {
                    pick.Health = 0;
                    under.RemoveAll(u => u.Health <= 0);
                }
            }
            return log;
        }

        /// <summary>Apply a single hit from shooter's weapon to target (used by direct and artillery).</summary>
        private static string ApplyOneHitFrom(Unit shooter, Unit target, Weapon weapon, List<Terrain> terrainList, out bool destroyed)
        {
            destroyed = false;
            if (target.Health <= 0) return "";

            if (target.UnitData.Type == UnitType.Tank || target.UnitData.Type == UnitType.TankDestroyer)
            {
                if (weapon.AntiTank == 0) return "Bounce. ";
                int atRoll = Random.Shared.Next(1, 7) + weapon.AntiTank;
                int targetArmor = target.GetArmorAgainst(shooter.Position);
                int armorRoll = Random.Shared.Next(1, 7) + targetArmor;
                if (atRoll <= armorRoll) return "Bounce. ";
                int fpRoll = Random.Shared.Next(1, 7);
                if (fpRoll >= weapon.Firepower)
                {
                    destroyed = true;
                    return "Destroyed! ";
                }
                target.IsBailed = true;
                return "Bailed! ";
            }
            else
            {
                int saveRoll = Random.Shared.Next(1, 7);
                foreach (var t in terrainList)
                {
                    if (t.Type == TerrainType.Hill && t.Bounds.Contains(target.Position)) { saveRoll++; break; }
                }
                if (saveRoll < target.UnitData.Save)
                {
                    destroyed = true;
                    return "Kill! ";
                }
                return "Saved. ";
            }
        }

        /// <summary>Flames of War: assault (close combat). Call when attacker is in assault range of defender.</summary>
        public static string ResolveAssault(Unit attacker, Unit defender, out bool defenderDestroyed)
        {
            defenderDestroyed = false;
            if (attacker.Health <= 0 || attacker.IsBailed) return "Attacker cannot assault.";
            if (attacker.HasAssaulted) return "Already assaulted this turn.";
            if (defender.Health <= 0) return "Target already dead.";
            float dist = Vector2.Distance(attacker.Position, defender.Position);
            const float assaultRange = 40f; // base contact / assault range in pixels
            if (dist > assaultRange) return "Not in assault range! Move into contact first.";

            // Assault: each side rolls Skill; hits on roll >= Skill. Infantry vs infantry use Firepower for casualties.
            int attackerHits = 0, defenderHits = 0;
            int aDice = attacker.UnitData.Type == UnitType.Infantry ? 2 : 1; // infantry get 2 dice in assault
            int dDice = defender.UnitData.Type == UnitType.Infantry ? 2 : 1;
            for (int i = 0; i < aDice; i++)
            {
                int r = Random.Shared.Next(1, 7);
                if (r >= attacker.UnitData.Skill) attackerHits++;
            }
            for (int i = 0; i < dDice; i++)
            {
                int r = Random.Shared.Next(1, 7);
                if (r >= defender.UnitData.Skill) defenderHits++;
            }

            string log = $"Assault: Attacker {attackerHits} hits, Defender {defenderHits} hits. ";
            // Resolve on defender
            for (int i = 0; i < attackerHits && defender.Health > 0; i++)
            {
                int saveRoll = Random.Shared.Next(1, 7);
                if (defender.UnitData.Type == UnitType.Tank || defender.UnitData.Type == UnitType.TankDestroyer)
                {
                    if (saveRoll >= 5) { defender.IsBailed = true; log += "Tank bailed! "; }
                    else { defender.Health = 0; defenderDestroyed = true; log += "Tank destroyed! "; }
                }
                else
                {
                    if (saveRoll < defender.UnitData.Save) { defender.Health = 0; defenderDestroyed = true; log += "Kill! "; }
                    else log += "Saved. ";
                }
            }
            // Counterattack on attacker
            for (int i = 0; i < defenderHits && attacker.Health > 0; i++)
            {
                int saveRoll = Random.Shared.Next(1, 7);
                if (attacker.UnitData.Type == UnitType.Tank || attacker.UnitData.Type == UnitType.TankDestroyer)
                {
                    if (saveRoll >= 5) { attacker.IsBailed = true; log += "Attacker bailed! "; }
                    else { attacker.Health = 0; log += "Attacker destroyed! "; }
                }
                else
                {
                    if (saveRoll < attacker.UnitData.Save) { attacker.Health = 0; log += "Attacker killed! "; }
                    else log += "Attacker saved. ";
                }
            }
            attacker.HasAssaulted = true;
            return log;
        }

        /// <summary>True if this unit is within assault range of the other.</summary>
        public bool IsInAssaultRangeOf(Unit other)
        {
            return Vector2.Distance(Position, other.Position) <= 40f;
        }
    }
}
