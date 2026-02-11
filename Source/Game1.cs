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

        public enum GameState
        {
            SplashScreen,
            Gameplay
        }

        private GameState _currentGameState = GameState.SplashScreen;
        private int _playerSide = 1; // 1 = Americans, 2 = Germans
        private Rectangle _startGameButtonRect = new Rectangle(362, 300, 300, 50);
        private Rectangle _toggleSideButtonRect = new Rectangle(362, 360, 300, 50);

        private List<Unit> _units = new List<Unit>();
        private Unit? _selectedUnit;
        private int _currentTurn = 1;
        private TurnState _currentPhase = TurnState.Starting;

        private Rectangle _nextPhaseButtonRect = new Rectangle(10, 10, 150, 40);
        private MouseState _previousMouseState;

        private string _combatLog = "Welcome to WindsOfWar!";

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 768;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        private void StartGame()
        {
            _units.Clear();
            _currentTurn = 1;
            _currentPhase = TurnState.Starting;
            _combatLog = "Welcome to WindsOfWar!";
            _selectedUnit = null;

            // Spawn initial units
            var riflemanData = new UnitData
            {
                Name = "Rifle Team",
                Type = UnitType.Infantry,
                Health = 1,
                MovementDistance = 150,
                Range = 400,
                HaltedROF = 1,
                MovingROF = 1,
                AntiTank = 0,
                Firepower = 6,
                Skill = 4,
                Save = 3
            };

            var shermanData = new UnitData
            {
                Name = "Sherman",
                Type = UnitType.Tank,
                Health = 1,
                MovementDistance = 250,
                Range = 600,
                HaltedROF = 2,
                MovingROF = 1,
                AntiTank = 10,
                Firepower = 3,
                Skill = 4,
                Save = 3,
                FrontArmor = 6,
                SideArmor = 4
            };

            var panzerData = new UnitData
            {
                Name = "Panzer IV",
                Type = UnitType.Tank,
                Health = 1,
                MovementDistance = 250,
                Range = 800,
                HaltedROF = 2,
                MovingROF = 1,
                AntiTank = 11,
                Firepower = 3,
                Skill = 4,
                Save = 3,
                FrontArmor = 6,
                SideArmor = 3
            };

            // Team 1 (Allies)
            SpawnUnit(riflemanData, 1, new Vector2(100, 100));
            SpawnUnit(riflemanData, 1, new Vector2(100, 250));
            SpawnUnit(shermanData, 1, new Vector2(200, 175));

            // Team 2 (Axis)
            SpawnUnit(riflemanData, 2, new Vector2(900, 100));
            SpawnUnit(riflemanData, 2, new Vector2(900, 250));
            SpawnUnit(panzerData, 2, new Vector2(800, 175));

            _currentGameState = GameState.Gameplay;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create a 1x1 white texture
            _whiteTexture = new Texture2D(GraphicsDevice, 1, 1);
            _whiteTexture.SetData(new[] { Color.White });

            // Initialize Font
            SimpleFont.Initialize();
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

            if (_currentGameState == GameState.SplashScreen)
            {
                if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                {
                    Point mousePos = mouseState.Position;
                    if (_startGameButtonRect.Contains(mousePos))
                    {
                        StartGame();
                    }
                    else if (_toggleSideButtonRect.Contains(mousePos))
                    {
                        _playerSide = (_playerSide == 1) ? 2 : 1;
                    }
                }
            }
            else
            {
                if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                {
                    HandleLeftClick(mouseState.Position);
                }
                else if (mouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton == ButtonState.Released)
                {
                    HandleRightClick(mouseState.Position);
                }

                // Update Units
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
            }

            _previousMouseState = mouseState;

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
                // Deselect only if clicking empty space and not targeting
                // Actually, let's keep it simple: click empty space -> deselect
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
                // Targeting Enemy
                if (_currentPhase == TurnState.Shooting)
                {
                    if (_selectedUnit.Team == _currentTurn)
                    {
                        _combatLog = _selectedUnit.ResolveShooting(unit);
                    }
                }
            }
            else if (unit.Team == _currentTurn)
            {
                // Selecting Own Unit
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

                // Reset Movement Flags for the new turn's active team
                foreach (var unit in _units)
                {
                    if (unit.Team == _currentTurn)
                        unit.HasMoved = false;
                }

                // Automatically skip Starting Phase to Movement? Or keep it for rally/remount
                // Let's keep it.
            }

            if (_selectedUnit != null)
            {
                _selectedUnit.IsSelected = false;
                _selectedUnit = null;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            if (_currentGameState == GameState.SplashScreen)
            {
                GraphicsDevice.Clear(Color.Black);
            }
            else
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);
            }

            _spriteBatch.Begin();

            if (_currentGameState == GameState.SplashScreen)
            {
                // Draw Title
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, "WINDS OF WAR", new Vector2(300, 100), Color.Red, 8);

                // Draw Start Game Button
                _spriteBatch.Draw(_whiteTexture, _startGameButtonRect, Color.DarkGray);
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, "START GAME", new Vector2(_startGameButtonRect.X + 20, _startGameButtonRect.Y + 15), Color.White, 3);

                // Draw Toggle Side Button
                _spriteBatch.Draw(_whiteTexture, _toggleSideButtonRect, Color.DarkGray);
                string sideText = _playerSide == 1 ? "SIDE: AMERICANS" : "SIDE: GERMANS";
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, sideText, new Vector2(_toggleSideButtonRect.X + 20, _toggleSideButtonRect.Y + 15), Color.White, 3);
            }
            else
            {
                // Draw Units
                foreach (var unit in _units)
                {
                    unit.Draw(_spriteBatch, _whiteTexture);
                }

                // Draw UI Panel
                Rectangle uiRect = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, 60);
                _spriteBatch.Draw(_whiteTexture, uiRect, Color.Black * 0.7f);

                // Draw Phase Button
                _spriteBatch.Draw(_whiteTexture, _nextPhaseButtonRect, Color.DarkGray);
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, "NEXT PHASE", new Vector2(_nextPhaseButtonRect.X + 10, _nextPhaseButtonRect.Y + 10), Color.White, 2);

                // Draw Turn Info
                string turnInfo = $"P{_currentTurn} : {_currentPhase}";
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, turnInfo, new Vector2(180, 15), Color.White, 3);

                // Draw Selected Unit Info
                if (_selectedUnit != null)
                {
                    string info = $"{_selectedUnit.UnitData.Name} | Move: {_selectedUnit.UnitData.MovementDistance} | Range: {_selectedUnit.UnitData.Range} | AT: {_selectedUnit.UnitData.AntiTank} | FP: {_selectedUnit.UnitData.Firepower}+";
                    SimpleFont.DrawString(_spriteBatch, _whiteTexture, info, new Vector2(10, 70), Color.White, 2);

                    // Debug Moved status
                    if (_selectedUnit.HasMoved)
                        SimpleFont.DrawString(_spriteBatch, _whiteTexture, "(MOVED)", new Vector2(10, 90), Color.Yellow, 2);
                    else if (_currentPhase == TurnState.Movement && _selectedUnit.Team == _currentTurn)
                        SimpleFont.DrawString(_spriteBatch, _whiteTexture, "(Right Click to Move)", new Vector2(10, 90), Color.LightGreen, 2);
                }

                // Draw Combat Log
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, _combatLog, new Vector2(10, GraphicsDevice.Viewport.Height - 30), Color.Yellow, 2);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
