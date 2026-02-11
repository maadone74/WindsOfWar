using Godot;

public partial class Main : Node2D
{
    public enum TurnState
    {
        Starting,
        Movement,
        Shooting,
        Assault
    }

    private PackedScene _unitScene = GD.Load<PackedScene>("res://scenes/unit.tscn");
    private UnitData _riflemanData;
    private UnitData _tankData;
    private Unit _selectedUnit;
    private int _currentTurn = 1;
    private TurnState _currentPhase = TurnState.Starting;

    private Label _turnLabel;
    private Button _nextPhaseButton;

    public override void _Ready()
    {
        _riflemanData = new UnitData { Health = 50, AttackPower = 5, MovementSpeed = 250 };
        _tankData = new UnitData { Health = 200, AttackPower = 20, MovementSpeed = 150 };

        SpawnUnit(_riflemanData, 1, new Vector2(100, 100));
        SpawnUnit(_riflemanData, 1, new Vector2(100, 250));
        SpawnUnit(_tankData, 1, new Vector2(200, 175));

        SpawnUnit(_riflemanData, 2, new Vector2(900, 100));
        SpawnUnit(_riflemanData, 2, new Vector2(900, 250));
        SpawnUnit(_tankData, 2, new Vector2(800, 175));

        _turnLabel = GetNode<Label>("TurnLabel");
        _nextPhaseButton = GetNode<Button>("NextPhaseButton");
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

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButtonEvent)
        {
            if (mouseButtonEvent.ButtonIndex == MouseButton.Left && mouseButtonEvent.Pressed)
            {
                if (_selectedUnit != null && GetNodeOrNull<Unit>(GetPathTo(GetNodeAt(mouseButtonEvent.Position))) == null)
                {
                    _selectedUnit.IsSelected = false;
                    _selectedUnit = null;
                }
            }
            else if (mouseButtonEvent.ButtonIndex == MouseButton.Right && mouseButtonEvent.Pressed)
            {
                if (_selectedUnit != null && _selectedUnit.Team == _currentTurn && _currentPhase == TurnState.Movement)
                {
                    _selectedUnit.MoveTo(GetGlobalMousePosition());
                }
            }
        }
    }

    private void UpdateTurnLabel()
    {
        _turnLabel.Text = $"Player {_currentTurn}'s Turn - {_currentPhase} Phase";
    }

    private void _on_next_phase_pressed()
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

        UpdateTurnLabel();
    }
}
