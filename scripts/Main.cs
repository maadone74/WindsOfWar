using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
    public enum TurnState
    {
        Starting,
        Movement,
        Shooting,
        Assault,
        End
    }

    private PackedScene _unitScene = GD.Load<PackedScene>("res://scenes/unit.tscn");
    private UnitData _riflemanData;
    private UnitData _tankData;
    private Unit _selectedUnit;
    private int _currentTurn = 1;
    private TurnState _currentPhase = TurnState.Starting;

    private Label _turnLabel;
    private Button _nextPhaseButton;
    private Label _unitInfoLabel;
    private RichTextLabel _combatLogLabel;

    // Minimal log for now
    private List<string> _combatLog = new List<string>();

    public override void _Ready()
    {
        _riflemanData = new UnitData {
            UnitName = "Rifle Squad",
            IsTank = false,
            Health = 5,
            MovementSpeed = 150,
            Skill = 4,
            Save = 3,
            WeaponRange = 400,
            ROF = 3,
            AntiTank = 2,
            Firepower = 6
        };
        _tankData = new UnitData {
            UnitName = "Sherman Tank",
            IsTank = true,
            Health = 3,
            MovementSpeed = 250,
            Skill = 4,
            ArmorFront = 6,
            ArmorSide = 4,
            ArmorTop = 1,
            WeaponRange = 600,
            ROF = 2,
            AntiTank = 10,
            Firepower = 3
        };

        SpawnUnit(_riflemanData, 1, new Vector2(100, 100));
        SpawnUnit(_riflemanData, 1, new Vector2(100, 250));
        SpawnUnit(_tankData, 1, new Vector2(200, 175));

        SpawnUnit(_riflemanData, 2, new Vector2(900, 100));
        SpawnUnit(_riflemanData, 2, new Vector2(900, 250));
        SpawnUnit(_tankData, 2, new Vector2(800, 175));

        _turnLabel = GetNode<Label>("CanvasLayer/TurnLabel");
        _nextPhaseButton = GetNode<Button>("CanvasLayer/NextPhaseButton");
        _unitInfoLabel = GetNode<Label>("CanvasLayer/UnitPanel/UnitInfoLabel");
        _combatLogLabel = GetNode<RichTextLabel>("CanvasLayer/LogPanel/CombatLogLabel");

        UpdateTurnLabel();
    }

    public void SpawnUnit(UnitData data, int team, Vector2 position)
    {
        var unit = _unitScene.Instantiate<Unit>();
        unit.UnitData = data;
        unit.Team = team;
        unit.Position = position;
        unit.Selected += OnUnitSelected;
        unit.Died += OnUnitDied;
        AddChild(unit);
    }

    public void OnUnitDied()
    {
        if (_selectedUnit != null && _selectedUnit.Health <= 0)
        {
            _selectedUnit = null;
        }
    }

    public void OnUnitSelected(Unit unit)
    {
        if (_selectedUnit != null && _selectedUnit.Team != unit.Team)
        {
            if (_currentPhase == TurnState.Shooting)
            {
                ResolveShooting(_selectedUnit, unit);
            }
            else if (_currentPhase == TurnState.Assault)
            {
                ResolveAssault(_selectedUnit, unit);
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
            _unitInfoLabel.Text = _selectedUnit.GetStatsString();
            Log($"Selected {unit.UnitData.UnitName}");
        }
    }

    private void ResolveShooting(Unit attacker, Unit defender)
    {
        Log($"--- {attacker.UnitData.UnitName} shoots at {defender.UnitData.UnitName} ---");

        float dist = attacker.Position.DistanceTo(defender.Position);
        if (dist > attacker.UnitData.WeaponRange)
        {
            Log($"Out of range! ({dist:F0} > {attacker.UnitData.WeaponRange})");
            return;
        }

        // Line of Sight Check
        var spaceState = GetWorld2D().DirectSpaceState;
        var query = PhysicsRayQueryParameters2D.Create(attacker.Position, defender.Position);
        var exclude = new Godot.Collections.Array<Rid>();
        exclude.Add(attacker.GetRid());
        exclude.Add(defender.GetRid());
        query.Exclude = exclude;

        var result = spaceState.IntersectRay(query);
        if (result.Count > 0)
        {
            Log("Line of Sight Blocked!");
            return;
        }

        // To Hit Roll
        int hitRoll = (int)(GD.Randi() % 6 + 1);
        int requiredSkill = attacker.UnitData.Skill;
        Log($"To Hit: Rolled {hitRoll} vs Skill {requiredSkill}+");

        if (hitRoll < requiredSkill)
        {
            Log("Missed!");
            return;
        }

        Log("Hit!");

        // Save Roll
        if (defender.UnitData.IsTank)
        {
            int atk = attacker.UnitData.AntiTank;
            int armor = defender.UnitData.ArmorFront; // Simplified: Always Front
            int saveRoll = (int)(GD.Randi() % 6 + 1);
            int totalArmor = saveRoll + armor;

            Log($"Armor Save: Rolled {saveRoll} + Armor {armor} = {totalArmor} vs AT {atk}");

            if (totalArmor < atk)
            {
                // Penetrated
                int fpRoll = (int)(GD.Randi() % 6 + 1);
                int fp = attacker.UnitData.Firepower;
                Log($"Penetrated! Firepower Test: Rolled {fpRoll} vs FP {fp}+");

                if (fpRoll >= fp)
                {
                    Log("Target Destroyed!");
                    defender.TakeDamage(defender.Health);
                }
                else
                {
                    Log("Crew Bailed Out! (Taking 1 damage for simplified rules)");
                    defender.TakeDamage(1);
                }
            }
            else if (totalArmor == atk)
            {
                Log("Glancing Hit! (No damage for now)");
            }
            else
            {
                Log("Ricochet! No effect.");
            }
        }
        else
        {
            // Infantry Save
            int saveRoll = (int)(GD.Randi() % 6 + 1);
            int save = defender.UnitData.Save;
            Log($"Infantry Save: Rolled {saveRoll} vs Save {save}+");

            if (saveRoll < save)
            {
                Log("Save Failed! Unit hit.");
                defender.TakeDamage(1);
            }
            else
            {
                Log("Saved!");
            }
        }
    }

    private void ResolveAssault(Unit attacker, Unit defender)
    {
        Log($"--- {attacker.UnitData.UnitName} assaults {defender.UnitData.UnitName} ---");
        float dist = attacker.Position.DistanceTo(defender.Position);
        if (dist > 50) // Melee range
        {
            Log("Too far to assault!");
            return;
        }

        // Simple 4+ to hit in melee for now
        int roll = (int)(GD.Randi() % 6 + 1);
        if (roll >= 4)
        {
             Log("Melee Hit! Defender takes 1 damage.");
             defender.TakeDamage(1);
        }
        else
        {
             Log("Melee Miss!");
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButtonEvent)
        {
            if (mouseButtonEvent.ButtonIndex == MouseButton.Left && mouseButtonEvent.Pressed)
            {
                var spaceState = GetWorld2D().DirectSpaceState;
                var query = new PhysicsPointQueryParameters2D();
                query.Position = GetGlobalMousePosition();
                query.CollideWithAreas = true;
                query.CollideWithBodies = true;

                var result = spaceState.IntersectPoint(query);

                if (result.Count == 0)
                {
                    if (_selectedUnit != null)
                    {
                        _selectedUnit.IsSelected = false;
                        _selectedUnit = null;
                        _unitInfoLabel.Text = "Select a Unit";
                        Log("Deselected unit.");
                    }
                }
            }
            else if (mouseButtonEvent.ButtonIndex == MouseButton.Right && mouseButtonEvent.Pressed)
            {
                if (_selectedUnit != null && _selectedUnit.Team == _currentTurn && _currentPhase == TurnState.Movement)
                {
                    _selectedUnit.MoveTo(GetGlobalMousePosition());
                    Log($"{_selectedUnit.UnitData.UnitName} moving.");
                }
            }
        }
    }

    private void UpdateTurnLabel()
    {
        _turnLabel.Text = $"Player {_currentTurn}'s Turn - {_currentPhase} Phase";
        Log($"--- Phase Changed to {_currentPhase} ---");
    }

    private void _on_next_phase_pressed()
    {
        _currentPhase++;
        if (_currentPhase > TurnState.End)
        {
            _currentPhase = TurnState.Starting;
            _currentTurn = _currentTurn == 1 ? 2 : 1;
            Log($"=== Player {_currentTurn}'s Turn Started ===");
        }
        else if (_currentPhase == TurnState.End)
        {
            // Auto advance from End to Start? Or wait for click?
            // Let's make it wait for click to finish turn.
        }

        if (_selectedUnit != null)
        {
            _selectedUnit.IsSelected = false;
            _selectedUnit = null;
            _unitInfoLabel.Text = "Select a Unit";
        }

        UpdateTurnLabel();
    }

    private void Log(string message)
    {
        GD.Print(message);
        _combatLog.Add(message);
        // Limit log size
        if (_combatLog.Count > 20) _combatLog.RemoveAt(0);

        if (_combatLogLabel != null)
        {
            _combatLogLabel.Text = string.Join("\n", _combatLog);
            _combatLogLabel.ScrollToLine(_combatLogLabel.GetLineCount() - 1);
        }
    }
}
