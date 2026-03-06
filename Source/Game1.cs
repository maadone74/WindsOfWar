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
        private const int PointsLimit = 100;

        private List<SmokeMarker> _smokeMarkers = new List<SmokeMarker>();
        private bool _artillerySmokeMode;
        private Vector2? _lastArtilleryImpact;
        private double _lastArtilleryImpactTime = -999;
        private Point _mousePosition;
        private GameTime? _gameTime;

        private Rectangle _nextPhaseButtonRect = new Rectangle(12, 12, 160, 44);
        private MouseState _previousMouseState;
        private KeyboardState _previousKeyboardState;

        private string _combatLog = "Welcome to WindsOfWar!";

        // Modern UI palette
        private static readonly Color UIPanel = new Color(28, 32, 40);
        private static readonly Color UIPanelBorder = new Color(55, 62, 76);
        private static readonly Color UIButton = new Color(45, 52, 65);
        private static readonly Color UIButtonAccent = new Color(56, 132, 255);
        private static readonly Color UISuccess = new Color(34, 160, 80);
        private static readonly Color UIText = new Color(238, 240, 245);
        private static readonly Color UITextMuted = new Color(150, 156, 168);
        private static readonly Color UIDanger = new Color(200, 70, 70);
        private const int UIBarHeight = 68;
        private const int UICombatLogHeight = 36;

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

            // Initialize Scenarios (Flames of War style: deployment zones, terrain, objectives)
            var s1 = new Scenario("Open Field", new Rectangle(50, 50, 900, 150), new Rectangle(50, 550, 900, 150));
            s1.TerrainList.Add(new Terrain(new Rectangle(400, 300, 100, 100), TerrainType.Forest));
            s1.TerrainList.Add(new Terrain(new Rectangle(600, 100, 50, 50), TerrainType.Building));
            s1.Objectives.Add(new Objective(new Rectangle(450, 350, 80, 60), "Centre"));
            _scenarios.Add(s1);

            var s2 = new Scenario("River Crossing", new Rectangle(50, 50, 900, 100), new Rectangle(50, 600, 900, 100));
            s2.TerrainList.Add(new Terrain(new Rectangle(0, 350, 1024, 60), TerrainType.River));
            s2.TerrainList.Add(new Terrain(new Rectangle(100, 450, 80, 80), TerrainType.Forest));
            s2.TerrainList.Add(new Terrain(new Rectangle(800, 200, 80, 80), TerrainType.Forest));
            s2.TerrainList.Add(new Terrain(new Rectangle(450, 280, 120, 50), TerrainType.Hill));
            s2.Objectives.Add(new Objective(new Rectangle(200, 320, 70, 50), "Bridge"));
            s2.Objectives.Add(new Objective(new Rectangle(750, 380, 70, 50), "Crossing"));
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
            if (_selectedScenario == null) return;
            // Find a spot in deployment zone
            Rectangle zone = (team == 1) ? _selectedScenario.P1Deployment : _selectedScenario.P2Deployment;
            int x = Random.Shared.Next(zone.X, zone.X + zone.Width);
            int y = Random.Shared.Next(zone.Y, zone.Y + zone.Height);
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
                System.Diagnostics.Debug.WriteLine("Loading sounds from files...");
                
                string shootPath = "Content/tank-shoot.wav";
                string movePath = "Content/tank-move.wav";
                
                System.Diagnostics.Debug.WriteLine($"  Current directory: {Directory.GetCurrentDirectory()}");
                System.Diagnostics.Debug.WriteLine($"  Looking for: {Path.GetFullPath(shootPath)}");
                System.Diagnostics.Debug.WriteLine($"  File exists: {File.Exists(shootPath)}");
                System.Diagnostics.Debug.WriteLine($"  Looking for: {Path.GetFullPath(movePath)}");
                System.Diagnostics.Debug.WriteLine($"  File exists: {File.Exists(movePath)}");
                
                if (File.Exists(shootPath))
                {
                    byte[] shootData = File.ReadAllBytes(shootPath);
                    System.Diagnostics.Debug.WriteLine($"  Read {shootData.Length} bytes from tank-shoot.wav");
                    
                    using (var stream = new MemoryStream(shootData))
                    {
                        _sounds["shoot"] = SoundEffect.FromStream(stream);
                        System.Diagnostics.Debug.WriteLine("  Loaded: tank-shoot.wav");
                    }
                }
                
                if (File.Exists(movePath))
                {
                    byte[] moveData = File.ReadAllBytes(movePath);
                    System.Diagnostics.Debug.WriteLine($"  Read {moveData.Length} bytes from tank-move.wav");
                    
                    using (var stream = new MemoryStream(moveData))
                    {
                        _sounds["move"] = SoundEffect.FromStream(stream);
                        System.Diagnostics.Debug.WriteLine("  Loaded: tank-move.wav");
                    }
                }
                
                if (_sounds.Count > 0)
                    System.Diagnostics.Debug.WriteLine($"Sounds loaded successfully! Total: {_sounds.Count}");
                else
                    throw new Exception("No sound files loaded");
            }
            catch (Exception ex)
            {
                // Sounds missing or audio unavailable — try procedural, then disable sound
                System.Diagnostics.Debug.WriteLine($"Warning: Failed to load sounds: {ex.Message}");
                try
                {
                    _sounds["shoot"] = SoundGenerator.CreateNoise(500);
                    _sounds["move"] = SoundGenerator.CreateTone(150, 800);
                    System.Diagnostics.Debug.WriteLine("Using procedurally generated sounds.");
                }
                catch (Exception ex2)
                {
                    // No audio hardware or init failed — run without sound
                    System.Diagnostics.Debug.WriteLine($"Audio disabled: {ex2.Message}");
                }
            }
        }

        public void PlaySound(string name)
        {
            System.Diagnostics.Debug.WriteLine($"PlaySound called: {name}");
            if (_sounds.TryGetValue(name, out var sound))
            {
                System.Diagnostics.Debug.WriteLine($"  Sound object: {sound}");
                System.Diagnostics.Debug.WriteLine($"  Sound duration: {sound.Duration}");
                System.Diagnostics.Debug.WriteLine($"  Playing sound: {name}");
                try
                {
                    sound.Play();
                    System.Diagnostics.Debug.WriteLine($"  Play() called successfully");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"  ERROR playing sound: {ex.Message}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"  Sound not found: {name}. Available sounds: {string.Join(", ", _sounds.Keys)}");
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
            // Handle Input
            var mouseState = Mouse.GetState();
            var keyboardState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || (keyboardState.IsKeyDown(Keys.Escape) && _previousKeyboardState.IsKeyUp(Keys.Escape)))
            {
                if (_currentGameState == GameState.SplashScreen)
                {
                    Exit();
                }
                else if (_currentGameState == GameState.ScenarioSelect)
                {
                    _currentGameState = GameState.SplashScreen;
                }
                else if (_currentGameState == GameState.ForceSetup)
                {
                    _currentGameState = GameState.ScenarioSelect;
                }
                else if (_currentGameState == GameState.Gameplay)
                {
                    _currentGameState = GameState.ForceSetup;
                    _units.Clear(); // Reset units if going back to setup
                }
            }

            if (_selectedUnit != null && _selectedUnit.Team == _currentTurn)
            {
                if (keyboardState.IsKeyDown(Keys.W) && _previousKeyboardState.IsKeyUp(Keys.W))
                    _selectedUnit.CycleWeapon();
                if (keyboardState.IsKeyDown(Keys.S) && _previousKeyboardState.IsKeyUp(Keys.S) && _selectedUnit.SelectedWeapon?.CanSmoke == true)
                    _artillerySmokeMode = !_artillerySmokeMode;
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
                    int y = 160;
                    foreach (var s in _scenarios)
                    {
                        Rectangle btn = new Rectangle(300, y, 420, 48);
                        if (btn.Contains(mousePos))
                        {
                            SelectScenario(s);
                            break;
                        }
                        y += 64;
                    }
                }
            }
            else if (_currentGameState == GameState.ForceSetup)
            {
                 if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                {
                    Point mousePos = mouseState.Position;
                    int y = 96;
                    foreach (var u in _availableUnits)
                    {
                        if (new Rectangle(20, y, 220, 36).Contains(mousePos)) { AddUnit(u, 1); break; }
                        y += 44;
                    }
                    y = 96;
                    foreach (var u in _availableUnits)
                    {
                        if (new Rectangle(800, y, 220, 36).Contains(mousePos)) { AddUnit(u, 2); break; }
                        y += 44;
                    }
                    if (new Rectangle(398, 598, 228, 52).Contains(mousePos)) StartGameplay();
                }
            }
            else
            {
                _mousePosition = mouseState.Position;
                _gameTime = gameTime;

                if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                    HandleLeftClick(mouseState.Position, gameTime);
                else if (mouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton == ButtonState.Released)
                    HandleRightClick(mouseState.Position);

                if (_lastArtilleryImpact.HasValue && gameTime.TotalGameTime.TotalSeconds - _lastArtilleryImpactTime > 2.0)
                    _lastArtilleryImpact = null;

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

        private void HandleLeftClick(Point mousePos, GameTime gameTime)
        {
            if (_nextPhaseButtonRect.Contains(mousePos))
            {
                AdvancePhase();
                return;
            }

            // Artillery: fire at point (template) if selected weapon is artillery and click is in range
            if (_selectedUnit != null && _selectedUnit.Team == _currentTurn && _currentPhase == TurnState.Shooting)
            {
                Weapon? w = _selectedUnit.SelectedWeapon;
                if (w != null && w.IsArtillery)
                {
                    Vector2 target = new Vector2(mousePos.X, mousePos.Y);
                    float dist = Vector2.Distance(_selectedUnit.Position, target);
                    if (dist <= w.Range && !_selectedUnit.FiredWeaponIndices.Contains(_selectedUnit.SelectedWeaponIndex))
                    {
                        string log = Unit.ResolveArtilleryShot(_selectedUnit, target, _units, _terrainList, _artillerySmokeMode, out SmokeMarker? smokePlaced);
                        _combatLog = log;
                        if (smokePlaced != null)
                            _smokeMarkers.Add(smokePlaced);
                        if (log.StartsWith("Template") || log.StartsWith("Smoke"))
                        {
                            _lastArtilleryImpact = target;
                            _lastArtilleryImpactTime = gameTime.TotalGameTime.TotalSeconds;
                        }
                        return;
                    }
                }
            }

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

        private int GetPointsForTeam(int team)
        {
            int total = 0;
            foreach (var u in _units)
                if (u.Team == team && u.Health > 0) total += u.UnitData.Points;
            return total;
        }

        private (int p1, int p2) GetObjectivesControlled()
        {
            int p1 = 0, p2 = 0;
            if (_selectedScenario?.Objectives == null) return (p1, p2);
            foreach (var obj in _selectedScenario.Objectives)
            {
                bool p1Has = false, p2Has = false;
                foreach (var u in _units)
                {
                    if (u.Health <= 0) continue;
                    if (!obj.Bounds.Contains(u.Position)) continue;
                    if (u.Team == 1) p1Has = true;
                    else p2Has = true;
                }
                if (p1Has && !p2Has) p1++;
                if (p2Has && !p1Has) p2++;
            }
            return (p1, p2);
        }

        private void SelectUnit(Unit unit)
        {
            if (_selectedUnit != null && _selectedUnit.Team != unit.Team)
            {
                // Targeting Enemy
                if (_currentPhase == TurnState.Shooting && _selectedUnit.Team == _currentTurn)
                {
                    _combatLog = _selectedUnit.ResolveShooting(unit, _terrainList, _smokeMarkers);
                }
                else if (_currentPhase == TurnState.Assault && _selectedUnit.Team == _currentTurn)
                {
                    _combatLog = Unit.ResolveAssault(_selectedUnit, unit, out _);
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

                if (_currentTurn == 1)
                {
                    _gameTurnNumber++;
                    foreach (var sm in _smokeMarkers.ToList())
                    {
                        sm.RemainingTurns--;
                        if (sm.RemainingTurns <= 0) _smokeMarkers.Remove(sm);
                    }
                }

                // Reset Movement Flags for the new turn's active team
                foreach (var unit in _units)
                {
                    if (unit.Team == _currentTurn)
                    {
                        unit.ResetTurn();
                        // Try to remount
                        if (unit.IsBailed)
                        {
                            if (unit.Remount())
                            {
                                _combatLog = $"{unit.UnitData.Name} Remounted!";
                            }
                            else
                            {
                                _combatLog = $"{unit.UnitData.Name} Failed to Remount.";
                            }
                        }
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
                GraphicsDevice.Clear(new Color(72, 88, 110)); // Muted slate-blue battlefield
            }

            _spriteBatch.Begin();

            if (_currentGameState == GameState.SplashScreen)
            {
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, "WINDS OF WAR", new Vector2(280, 120), UIText, 8);

                DrawPanel(_startGameButtonRect, UISuccess, true);
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, "START GAME", new Vector2(_startGameButtonRect.X + 24, _startGameButtonRect.Y + 16), UIText, 3);

                DrawPanel(_toggleSideButtonRect, UIButton, true);
                string sideText = _playerSide == 1 ? "SIDE: AMERICANS" : "SIDE: GERMANS";
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, sideText, new Vector2(_toggleSideButtonRect.X + 24, _toggleSideButtonRect.Y + 16), UIText, 3);
            }
            else if (_currentGameState == GameState.ScenarioSelect)
            {
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, "Select scenario", new Vector2(380, 48), UITextMuted, 2);
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, "SELECT SCENARIO", new Vector2(300, 72), UIText, 5);
                int y = 160;
                foreach (var s in _scenarios)
                {
                    Rectangle btn = new Rectangle(300, y, 420, 48);
                    DrawPanel(btn, UIButton, true);
                    SimpleFont.DrawString(_spriteBatch, _whiteTexture, s.Name, new Vector2(328, y + 14), UIText, 2);
                    y += 64;
                }
            }
            else if (_currentGameState == GameState.ForceSetup)
            {
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, "Setup forces", new Vector2(430, 18), UITextMuted, 1);
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, "SETUP FORCES", new Vector2(388, 38), UIText, 3);

                if (_selectedScenario != null)
                {
                    _spriteBatch.Draw(_whiteTexture, _selectedScenario.P1Deployment, new Color(34, 160, 80, 70));
                    _spriteBatch.Draw(_whiteTexture, _selectedScenario.P2Deployment, new Color(200, 70, 70, 70));
                }

                int p1pts = GetPointsForTeam(1);
                int p2pts = GetPointsForTeam(2);
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, $"P1: {p1pts} / {PointsLimit} pts", new Vector2(24, 58), p1pts <= PointsLimit ? UIText : UIDanger, 2);
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, $"P2: {p2pts} / {PointsLimit} pts", new Vector2(804, 58), p2pts <= PointsLimit ? UIText : UIDanger, 2);

                int y = 96;
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, "Player 1 · USA", new Vector2(24, 76), UITextMuted, 1);
                foreach (var u in _availableUnits)
                {
                    Rectangle btn = new Rectangle(20, y, 220, 36);
                    DrawPanel(btn, UIButton, true);
                    SimpleFont.DrawString(_spriteBatch, _whiteTexture, $"{u.Name}  ·  {u.Points}pt", new Vector2(32, y + 10), UIText, 1);
                    y += 44;
                }

                y = 96;
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, "Player 2 · GER", new Vector2(804, 76), UITextMuted, 1);
                foreach (var u in _availableUnits)
                {
                    Rectangle btn = new Rectangle(800, y, 220, 36);
                    DrawPanel(btn, UIButton, true);
                    SimpleFont.DrawString(_spriteBatch, _whiteTexture, $"{u.Name}  ·  {u.Points}pt", new Vector2(808, y + 10), UIText, 1);
                    y += 44;
                }

                foreach (var unit in _units)
                    unit.Draw(_spriteBatch, _whiteTexture);

                Rectangle startBtn = new Rectangle(398, 598, 228, 52);
                DrawPanel(startBtn, UISuccess, true);
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, "START BATTLE", new Vector2(448, 614), UIText, 2);
            }
            else
            {
                // Draw Terrain
                foreach (var terrain in _terrainList)
                {
                    _spriteBatch.Draw(_whiteTexture, terrain.Bounds, terrain.Color);
                }

                // Draw movement and weapon range circles for selected unit (ruler)
                if (_selectedUnit != null && _selectedUnit.Health > 0)
                {
                    var pos = _selectedUnit.Position;
                    DrawRangeCircle(pos, _selectedUnit.UnitData.MovementDistance, Color.Lime * 0.35f, 2);
                    if (_selectedUnit.SelectedWeapon != null)
                        DrawRangeCircle(pos, _selectedUnit.SelectedWeapon.Range, new Color(220, 50, 50) * 0.4f, 2);
                }

                // Smoke markers (concealment, block LOS)
                foreach (var sm in _smokeMarkers)
                {
                    if (sm.RemainingTurns <= 0) continue;
                    DrawFilledCircle(sm.Position, sm.Radius, new Color(180, 180, 190, 140));
                    DrawRangeCircle(sm.Position, sm.Radius, new Color(200, 200, 210, 180), 2);
                }

                // Last artillery impact template (show for 2 seconds)
                if (_lastArtilleryImpact.HasValue && _selectedUnit?.SelectedWeapon?.IsArtillery == true)
                {
                    float r = _selectedUnit.SelectedWeapon.TemplateRadius;
                    DrawFilledCircle(_lastArtilleryImpact.Value, r, new Color(200, 80, 50, 90));
                    DrawRangeCircle(_lastArtilleryImpact.Value, r, new Color(220, 60, 40, 220), 2);
                }

                // Artillery template preview at mouse when in range
                if (_currentPhase == TurnState.Shooting && _selectedUnit != null && _selectedUnit.Team == _currentTurn && _selectedUnit.SelectedWeapon?.IsArtillery == true && !_selectedUnit.FiredWeaponIndices.Contains(_selectedUnit.SelectedWeaponIndex))
                {
                    Vector2 mouse = new Vector2(_mousePosition.X, _mousePosition.Y);
                    float dist = Vector2.Distance(_selectedUnit.Position, mouse);
                    if (dist <= _selectedUnit.SelectedWeapon.Range && dist > 0)
                    {
                        float tr = _selectedUnit.SelectedWeapon.TemplateRadius;
                        DrawFilledCircle(mouse, tr, new Color(255, 200, 100, 70));
                        DrawRangeCircle(mouse, tr, new Color(255, 180, 80, 180), 2);
                    }
                }

                // Draw Objectives (Flames of War mission objectives)
                if (_selectedScenario?.Objectives != null)
                {
                    foreach (var obj in _selectedScenario.Objectives)
                    {
                        _spriteBatch.Draw(_whiteTexture, obj.Bounds, Color.Gold * 0.35f);
                        int x = obj.Bounds.X, y = obj.Bounds.Y, w = obj.Bounds.Width, h = obj.Bounds.Height;
                        _spriteBatch.Draw(_whiteTexture, new Rectangle(x, y, w, 2), Color.Gold);
                        _spriteBatch.Draw(_whiteTexture, new Rectangle(x, y + h - 2, w, 2), Color.Gold);
                        _spriteBatch.Draw(_whiteTexture, new Rectangle(x, y, 2, h), Color.Gold);
                        _spriteBatch.Draw(_whiteTexture, new Rectangle(x + w - 2, y, 2, h), Color.Gold);
                        SimpleFont.DrawString(_spriteBatch, _whiteTexture, obj.Label, new Vector2(x, y - 14), Color.Gold, 1);
                    }
                }

                // Draw Units
                foreach (var unit in _units)
                {
                    unit.Draw(_spriteBatch, _whiteTexture);
                }

                // Top bar
                Rectangle uiRect = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, UIBarHeight);
                _spriteBatch.Draw(_whiteTexture, uiRect, UIPanel);
                _spriteBatch.Draw(_whiteTexture, new Rectangle(0, UIBarHeight - 1, GraphicsDevice.Viewport.Width, 1), UIPanelBorder);

                DrawPanel(_nextPhaseButtonRect, UIButtonAccent, true);
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, "NEXT PHASE", new Vector2(_nextPhaseButtonRect.X + 20, _nextPhaseButtonRect.Y + 12), UIText, 2);

                var (obj1, obj2) = GetObjectivesControlled();
                string turnInfo = $"Turn {_gameTurnNumber}   ·   Player {_currentTurn}   ·   {_currentPhase}   ·   Objectives  P1: {obj1}  P2: {obj2}";
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, turnInfo, new Vector2(192, 22), UIText, 2);

                if (_selectedUnit != null)
                {
                    Rectangle card = new Rectangle(12, UIBarHeight + 8, 520, 72);
                    DrawPanel(card, UIPanel, true);

                    string info = $"{_selectedUnit.UnitData.Name}   ·   Move {_selectedUnit.UnitData.MovementDistance}";
                    Weapon? weapon = _selectedUnit.SelectedWeapon;
                    if (weapon != null)
                        info += $"   ·   {weapon.Name}  Rng {weapon.Range}  AT{weapon.AntiTank}  FP{weapon.Firepower}+";
                    SimpleFont.DrawString(_spriteBatch, _whiteTexture, info, new Vector2(24, UIBarHeight + 18), UIText, 2);
                    SimpleFont.DrawString(_spriteBatch, _whiteTexture, "Green = move   Red = weapon range", new Vector2(24, UIBarHeight + 36), UITextMuted, 1);

                    if (_selectedUnit.HasMoved)
                        SimpleFont.DrawString(_spriteBatch, _whiteTexture, "Moved", new Vector2(24, UIBarHeight + 52), UITextMuted, 1);
                    else if (_currentPhase == TurnState.Movement && _selectedUnit.Team == _currentTurn)
                        SimpleFont.DrawString(_spriteBatch, _whiteTexture, "Right-click to move", new Vector2(24, UIBarHeight + 52), UISuccess, 1);

                    if (_currentPhase == TurnState.Assault && _selectedUnit.Team == _currentTurn)
                        SimpleFont.DrawString(_spriteBatch, _whiteTexture, _selectedUnit.HasAssaulted ? "Assaulted" : "Click enemy in contact to assault", new Vector2(280, UIBarHeight + 52), _selectedUnit.HasAssaulted ? UITextMuted : UIText, 1);
                    else if (_selectedUnit.FiredWeaponIndices.Contains(_selectedUnit.SelectedWeaponIndex))
                        SimpleFont.DrawString(_spriteBatch, _whiteTexture, "Weapon fired", new Vector2(280, UIBarHeight + 52), UITextMuted, 1);
                    else if (_selectedUnit.Team == _currentTurn)
                    {
                        if (_selectedUnit.SelectedWeapon?.IsArtillery == true)
                        {
                            SimpleFont.DrawString(_spriteBatch, _whiteTexture, "Click map = template fire" + (_selectedUnit.SelectedWeapon.CanSmoke ? "  ·  S = Smoke" : ""), new Vector2(280, UIBarHeight + 52), UIText, 1);
                            if (_selectedUnit.SelectedWeapon.CanSmoke && _artillerySmokeMode)
                                SimpleFont.DrawString(_spriteBatch, _whiteTexture, "[SMOKE SALVO]", new Vector2(500, UIBarHeight + 52), Color.Orange, 1);
                        }
                        else
                            SimpleFont.DrawString(_spriteBatch, _whiteTexture, "W = cycle weapon", new Vector2(280, UIBarHeight + 52), UITextMuted, 1);
                    }
                }

                int logY = GraphicsDevice.Viewport.Height - UICombatLogHeight;
                _spriteBatch.Draw(_whiteTexture, new Rectangle(0, logY, GraphicsDevice.Viewport.Width, UICombatLogHeight), UIPanel);
                _spriteBatch.Draw(_whiteTexture, new Rectangle(0, logY, GraphicsDevice.Viewport.Width, 1), UIPanelBorder);
                SimpleFont.DrawString(_spriteBatch, _whiteTexture, _combatLog, new Vector2(16, logY + 10), UIText, 2);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawPanel(Rectangle r, Color fill, bool border)
        {
            _spriteBatch.Draw(_whiteTexture, r, fill);
            if (border)
            {
                _spriteBatch.Draw(_whiteTexture, new Rectangle(r.X, r.Y, r.Width, 1), UIPanelBorder);
                _spriteBatch.Draw(_whiteTexture, new Rectangle(r.X, r.Bottom - 1, r.Width, 1), UIPanelBorder);
                _spriteBatch.Draw(_whiteTexture, new Rectangle(r.X, r.Y, 1, r.Height), UIPanelBorder);
                _spriteBatch.Draw(_whiteTexture, new Rectangle(r.Right - 1, r.Y, 1, r.Height), UIPanelBorder);
            }
        }

        private void DrawFilledCircle(Vector2 center, float radius, Color color)
        {
            const int segments = 36;
            float step = MathF.Tau / segments;
            float sliceW = 2f * radius * MathF.Sin(step / 2f);
            for (int i = 0; i < segments; i++)
            {
                float a = i * step + step / 2f;
                Vector2 pos = center + (radius * 0.5f) * new Vector2(MathF.Cos(a), MathF.Sin(a));
                _spriteBatch.Draw(_whiteTexture, pos, null, color, a, new Vector2(0.5f, 0.5f), new Vector2(radius, sliceW), SpriteEffects.None, 0f);
            }
        }

        /// <summary>Draw a circle outline for movement/weapon range (ruler).</summary>
        private void DrawRangeCircle(Vector2 center, float radius, Color color, int thickness)
        {
            const int segments = 64;
            float step = MathF.Tau / segments;
            for (int i = 0; i < segments; i++)
            {
                float a0 = i * step, a1 = (i + 1) * step;
                Vector2 p0 = center + radius * new Vector2(MathF.Cos(a0), MathF.Sin(a0));
                Vector2 p1 = center + radius * new Vector2(MathF.Cos(a1), MathF.Sin(a1));
                Vector2 d = p1 - p0;
                float len = d.Length();
                if (len < 0.001f) continue;
                Vector2 mid = (p0 + p1) * 0.5f;
                float angle = MathF.Atan2(d.Y, d.X);
                _spriteBatch.Draw(_whiteTexture, mid, null, color, angle, new Vector2(0.5f, 0.5f), new Vector2(len, thickness), SpriteEffects.None, 0f);
            }
        }
    }
}
