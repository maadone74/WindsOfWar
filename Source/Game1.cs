using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace WindsOfWar
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch = null!;
        private Texture2D _whiteTexture = null!;
        private Dictionary<string, SoundEffect> _sounds = new Dictionary<string, SoundEffect>();
        private List<Terrain> _terrainList = new List<Terrain>();

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
            ScenarioSelect,
            ForceSetup,
            Gameplay
        }

        private GameState _currentGameState = GameState.SplashScreen;
        private List<Scenario> _scenarios = new List<Scenario>();
        private Scenario? _selectedScenario;
        private List<UnitData> _availableUnits = new List<UnitData>();
        private int _playerSide = 1; // 1 = Americans, 2 = Germans
        private Rectangle _startGameButtonRect = new Rectangle(362, 300, 300, 50);
        private Rectangle _toggleSideButtonRect = new Rectangle(362, 360, 300, 50);

        private List<Unit> _units = new List<Unit>();
        private Unit? _selectedUnit;
        private int _currentTurn = 1;
        private int _gameTurnNumber = 1;
        private TurnState _currentPhase = TurnState.Starting;

        private Rectangle _nextPhaseButtonRect = new Rectangle(10, 10, 150, 40);
        private MouseState _previousMouseState;
        private KeyboardState _previousKeyboardState;

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

            // Initialize Scenarios
            var s1 = new Scenario("Open Field", new Rectangle(50, 50, 900, 150), new Rectangle(50, 550, 900, 150));
            s1.TerrainList.Add(new Terrain(new Rectangle(400, 300, 100, 100), TerrainType.Forest));
            s1.TerrainList.Add(new Terrain(new Rectangle(600, 100, 50, 50), TerrainType.Building));
            _scenarios.Add(s1);

            var s2 = new Scenario("River Crossing", new Rectangle(50, 50, 900, 100), new Rectangle(50, 600, 900, 100));
            s2.TerrainList.Add(new Terrain(new Rectangle(0, 350, 1024, 60), TerrainType.River));
            s2.TerrainList.Add(new Terrain(new Rectangle(100, 450, 80, 80), TerrainType.Forest));
            s2.TerrainList.Add(new Terrain(new Rectangle(800, 200, 80, 80), TerrainType.Forest));
            _scenarios.Add(s2);

            _availableUnits = UnitLibrary.GetAllUnits();
        }

        private void GoToScenarioSelect()
        {
            _currentGameState = GameState.ScenarioSelect;
        }

        private void SelectScenario(Scenario scenario)
        {
            _selectedScenario = scenario;
            _terrainList = new List<Terrain>(scenario.TerrainList);
            _units.Clear(); // Clear units for setup
            _currentGameState = GameState.ForceSetup;
        }

        private void AddUnit(UnitData data, int team)
        {
            // Find a spot in deployment zone
            Rectangle zone = (team == 1) ? _selectedScenario.P1Deployment : _selectedScenario.P2Deployment;
            Random rand = new Random();
            int x = rand.Next(zone.X, zone.X + zone.Width);
            int y = rand.Next(zone.Y, zone.Y + zone.Height);
            SpawnUnit(data, team, new Vector2(x, y));
        }

        private void StartGameplay()
        {
            _currentTurn = 1;
            _gameTurnNumber = 1;
            _currentPhase = TurnState.Starting;
            _combatLog = "Battle Started!";
            _selectedUnit = null;
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

            // Load Sounds
            try
            {
                _sounds["shoot"] = Content.Load<SoundEffect>("shoot");
                _sounds["move"] = Content.Load<SoundEffect>("move");
            }
            catch
            {
                // Sounds missing, ignore
                System.Diagnostics.Debug.WriteLine("Warning: Sound files missing.");
            }
        }

        public void PlaySound(string name)
        {
            if (_sounds.TryGetValue(name, out var sound))
            {
                sound.Play();
            }
        }

        private void SpawnUnit(UnitData data, int team, Vector2 position)
        {
            var unit = new Unit(data, team, position);
            unit.Died += () => OnUnitDied(unit);
            unit.OnSoundTriggered += PlaySound;
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
            var keyboardState = Keyboard.GetState();

            if (_selectedUnit != null && _selectedUnit.Team == _currentTurn)
            {
                if (keyboardState.IsKeyDown(Keys.W) && _previousKeyboardState.IsKeyUp(Keys.W))
                {
                    _selectedUnit.CycleWeapon();
                }
            }

            if (_currentGameState == GameState.SplashScreen)
            {
                if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                {
                    Point mousePos = mouseState.Position;
                    if (_startGameButtonRect.Contains(mousePos))
                    {
                        GoToScenarioSelect();
                    }
                    else if (_toggleSideButtonRect.Contains(mousePos))
                    {
                        _playerSide = (_playerSide == 1) ? 2 : 1;
                    }
                }
            }
            else if (_currentGameState == GameState.ScenarioSelect)
            {
                if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                {
                    Point mousePos = mouseState.Position;
                    int y = 150;
                    foreach(var s in _scenarios)
                    {
                         Rectangle btn = new Rectangle(300, y, 400, 40);
                         if (btn.Contains(mousePos))
                         {
                             SelectScenario(s);
                             break;
                         }
                         y += 60;
                    }
                }
            }
            else if (_currentGameState == GameState.ForceSetup)
            {
                 if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                {
                    Point mousePos = mouseState.Position;

                    // P1 Buttons (Left)
                    int y = 100;
                    foreach(var u in _availableUnits)
                    {
                        Rectangle btn = new Rectangle(20, y, 200, 30);
                        if (btn.Contains(mousePos)) AddUnit(u, 1);
                        y += 40;
                    }

                    // P2 Buttons (Right)
                    y = 100;
                    foreach(var u in _availableUnits)
                    {
                        Rectangle btn = new Rectangle(800, y, 200, 30);
                        if (btn.Contains(mousePos)) AddUnit(u, 2);
                        y += 40;
                    }

                    // Start Game Button
                    Rectangle startBtn = new Rectangle(400, 600, 200, 50);
                    if (startBtn.Contains(mousePos)) StartGameplay();
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
            _previousKeyboardState = keyboardState;

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
                        _combatLog = _selectedUnit.ResolveShooting(unit, _terrainList);
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

                if (_currentTurn == 1) _gameTurnNumber++;

                // Reset Movement Flags for the new turn's active team
                foreach (var unit in _units)
                {
                    if (unit.Team == _currentTurn)
                    {
                        unit.ResetTurn();
                    }
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
            else if (_currentGameState == GameState.ScenarioSelect)
            {
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, "SELECT SCENARIO", new Vector2(300, 50), Color.White, 5);
                int y = 150;
                foreach(var s in _scenarios)
                {
                     Rectangle btn = new Rectangle(300, y, 400, 40);
                     _spriteBatch.Draw(_whiteTexture, btn, Color.DarkGray);
                     SimpleFont.DrawString(_spriteBatch, _whiteTexture, s.Name, new Vector2(320, y+10), Color.White, 2);
                     y += 60;
                }
            }
            else if (_currentGameState == GameState.ForceSetup)
            {
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, "SETUP FORCES", new Vector2(400, 20), Color.White, 3);

                // Draw Zones (Debug/Visual)
                if (_selectedScenario != null)
                {
                    _spriteBatch.Draw(_whiteTexture, _selectedScenario.P1Deployment, Color.Green * 0.3f);
                    _spriteBatch.Draw(_whiteTexture, _selectedScenario.P2Deployment, Color.Red * 0.3f);
                }

                // Draw P1 Buttons
                int y = 100;
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, "Player 1 (USA)", new Vector2(20, 70), Color.CornflowerBlue, 2);
                foreach(var u in _availableUnits)
                {
                    Rectangle btn = new Rectangle(20, y, 200, 30);
                    _spriteBatch.Draw(_whiteTexture, btn, Color.DarkBlue);
                    SimpleFont.DrawString(_spriteBatch, _whiteTexture, u.Name, new Vector2(25, y+5), Color.White, 1);
                    y += 40;
                }

                // Draw P2 Buttons
                y = 100;
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, "Player 2 (GER)", new Vector2(800, 70), Color.IndianRed, 2);
                foreach(var u in _availableUnits)
                {
                    Rectangle btn = new Rectangle(800, y, 200, 30);
                    _spriteBatch.Draw(_whiteTexture, btn, Color.DarkRed);
                    SimpleFont.DrawString(_spriteBatch, _whiteTexture, u.Name, new Vector2(805, y+5), Color.White, 1);
                    y += 40;
                }

                // Draw Units (so we can see them placed)
                foreach (var unit in _units)
                {
                    unit.Draw(_spriteBatch, _whiteTexture);
                }

                // Start Button
                Rectangle startBtn = new Rectangle(400, 600, 200, 50);
                _spriteBatch.Draw(_whiteTexture, startBtn, Color.Green);
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, "START BATTLE", new Vector2(420, 615), Color.White, 2);
            }
            else
            {
                // Draw Terrain
                foreach (var terrain in _terrainList)
                {
                    _spriteBatch.Draw(_whiteTexture, terrain.Bounds, terrain.Color);
                }

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
                string turnInfo = $"Turn {_gameTurnNumber} | P{_currentTurn} : {_currentPhase}";
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, turnInfo, new Vector2(180, 15), Color.White, 3);

                // Draw Selected Unit Info
                if (_selectedUnit != null)
                {
                    string info = $"{_selectedUnit.UnitData.Name} | Move: {_selectedUnit.UnitData.MovementDistance}";
                    Weapon? weapon = _selectedUnit.SelectedWeapon;
                    if (weapon != null)
                    {
                        info += $" | WPN: {weapon.Name} | Rng: {weapon.Range} | AT: {weapon.AntiTank} | FP: {weapon.Firepower}+";
                    }

                    SimpleFont.DrawString(_spriteBatch, _whiteTexture, info, new Vector2(10, 70), Color.White, 2);

                    // Debug Moved status
                    if (_selectedUnit.HasMoved)
                        SimpleFont.DrawString(_spriteBatch, _whiteTexture, "(MOVED)", new Vector2(10, 90), Color.Yellow, 2);
                    else if (_currentPhase == TurnState.Movement && _selectedUnit.Team == _currentTurn)
                        SimpleFont.DrawString(_spriteBatch, _whiteTexture, "(Right Click to Move)", new Vector2(10, 90), Color.LightGreen, 2);

                    // Shot status and Weapon Hint
                    if (_selectedUnit.FiredWeaponIndices.Contains(_selectedUnit.SelectedWeaponIndex))
                        SimpleFont.DrawString(_spriteBatch, _whiteTexture, "(WEAPON FIRED)", new Vector2(10, 110), Color.Red, 2);
                    else if (_selectedUnit.Team == _currentTurn)
                        SimpleFont.DrawString(_spriteBatch, _whiteTexture, "(Press W to Cycle Weapon)", new Vector2(10, 110), Color.LightGray, 2);
                }

                // Draw Combat Log
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, _combatLog, new Vector2(10, GraphicsDevice.Viewport.Height - 30), Color.Yellow, 2);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
