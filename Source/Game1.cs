using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace WindsOfWar
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch = null!;
        private Texture2D _whiteTexture = null!;

        public enum TurnState
        {
            Starting,
            Movement,
            Shooting,
            Assault
        }

        private List<Unit> _units = new List<Unit>();
        private Unit? _selectedUnit;
        private int _currentTurn = 1;
        private TurnState _currentPhase = TurnState.Starting;

        private Rectangle _nextPhaseButtonRect = new Rectangle(10, 10, 150, 40);
        private MouseState _previousMouseState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Spawn initial units
            var riflemanData = new UnitData { Health = 50, AttackPower = 5, MovementSpeed = 250 };
            var tankData = new UnitData { Health = 200, AttackPower = 20, MovementSpeed = 150 };

            SpawnUnit(riflemanData, 1, new Vector2(100, 100));
            SpawnUnit(riflemanData, 1, new Vector2(100, 250));
            SpawnUnit(tankData, 1, new Vector2(200, 175));

            SpawnUnit(riflemanData, 2, new Vector2(900, 100));
            SpawnUnit(riflemanData, 2, new Vector2(900, 250));
            SpawnUnit(tankData, 2, new Vector2(800, 175));
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create a 1x1 white texture
            _whiteTexture = new Texture2D(GraphicsDevice, 1, 1);
            _whiteTexture.SetData(new[] { Color.White });
        }

        private void SpawnUnit(UnitData data, int team, Vector2 position)
        {
            var unit = new Unit(data, team, position);
            unit.Died += () => OnUnitDied(unit);
            _units.Add(unit);
        }

        private void OnUnitDied(Unit unit)
        {
            if (_selectedUnit == unit)
            {
                _selectedUnit = null;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Handle Input
            var mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                HandleLeftClick(mouseState.Position);
            }
            else if (mouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton == ButtonState.Released)
            {
                HandleRightClick(mouseState.Position);
            }

            _previousMouseState = mouseState;

            // Update Units
            // Create a copy to iterate safely
            var unitsToUpdate = new List<Unit>(_units);
            foreach (var unit in unitsToUpdate)
            {
                if (unit.Health <= 0)
                {
                    _units.Remove(unit);
                    continue;
                }
                unit.Update(gameTime);
            }

            // Update Window Title
            Window.Title = $"WindsOfWar - Player {_currentTurn}'s Turn - {_currentPhase} Phase";

            base.Update(gameTime);
        }

        private void HandleLeftClick(Point mousePos)
        {
            // Check UI
            if (_nextPhaseButtonRect.Contains(mousePos))
            {
                AdvancePhase();
                return;
            }

            // Check Units
            bool clickedUnit = false;
            foreach (var unit in _units)
            {
                if (unit.Bounds.Contains(mousePos))
                {
                    SelectUnit(unit);
                    clickedUnit = true;
                    break;
                }
            }

            if (!clickedUnit && _selectedUnit != null)
            {
                _selectedUnit.IsSelected = false;
                _selectedUnit = null;
            }
        }

        private void HandleRightClick(Point mousePos)
        {
            if (_selectedUnit != null && _selectedUnit.Team == _currentTurn && _currentPhase == TurnState.Movement)
            {
                _selectedUnit.MoveTo(new Vector2(mousePos.X, mousePos.Y));
            }
        }

        private void SelectUnit(Unit unit)
        {
            if (_selectedUnit != null && _selectedUnit.Team != unit.Team)
            {
                if (_currentPhase == TurnState.Shooting || _currentPhase == TurnState.Assault)
                {
                    _selectedUnit.Attack(unit);
                }
            }
            else if (unit.Team == _currentTurn)
            {
                if (_selectedUnit != null && _selectedUnit != unit)
                {
                    _selectedUnit.IsSelected = false;
                }
                _selectedUnit = unit;
                _selectedUnit.IsSelected = true;
            }
        }

        private void AdvancePhase()
        {
            _currentPhase++;
            if (_currentPhase > TurnState.Assault)
            {
                _currentPhase = TurnState.Starting;
                _currentTurn = _currentTurn == 1 ? 2 : 1;
            }

            if (_selectedUnit != null)
            {
                _selectedUnit.IsSelected = false;
                _selectedUnit = null;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // Draw Units
            foreach (var unit in _units)
            {
                unit.Draw(_spriteBatch, _whiteTexture);
            }

            // Draw UI Button
            _spriteBatch.Draw(_whiteTexture, _nextPhaseButtonRect, Color.DarkGray);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
