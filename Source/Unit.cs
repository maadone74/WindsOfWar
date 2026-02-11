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
        public int AttackPower { get; set; }
        public int MovementSpeed { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 TargetPosition { get; set; }

        public bool IsSelected { get; set; }

        public Rectangle Bounds => new Rectangle((int)Position.X - 16, (int)Position.Y - 16, 32, 32);

        public Unit(UnitData data, int team, Vector2 position)
        {
            UnitData = data;
            Team = team;
            Position = position;
            TargetPosition = position;
            Health = data.Health;
            AttackPower = data.AttackPower;
            MovementSpeed = data.MovementSpeed;
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Vector2.Distance(Position, TargetPosition) > 5)
            {
                Vector2 direction = Vector2.Normalize(TargetPosition - Position);
                Position += direction * MovementSpeed * dt;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            Color color = Team == 1 ? Color.White : Color.Red;
            if (IsSelected)
            {
                color = Color.Green;
            }

            // Draw unit (centered)
            spriteBatch.Draw(texture, new Rectangle((int)Position.X - 16, (int)Position.Y - 16, 32, 32), color);

            // Draw health bar (simple implementation)
            int healthWidth = (int)((float)Health / UnitData.Health * 32);
            spriteBatch.Draw(texture, new Rectangle((int)Position.X - 16, (int)Position.Y - 20, 32, 4), Color.Gray);
            spriteBatch.Draw(texture, new Rectangle((int)Position.X - 16, (int)Position.Y - 20, healthWidth, 4), Color.Lime);
        }

        public void MoveTo(Vector2 newPosition)
        {
            TargetPosition = newPosition;
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                Died?.Invoke();
            }
        }

        public void Attack(Unit target)
        {
            Random rand = new Random();
            int damage = rand.Next(AttackPower + 1);
            target.TakeDamage(damage);
        }
    }
}
