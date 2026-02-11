using Godot;

public partial class Unit : CharacterBody2D
{
    [Signal]
    public delegate void DiedEventHandler();

    [Signal]
    public delegate void SelectedEventHandler(Unit unit);

    [Export]
    public UnitData UnitData { get; set; }

    [Export]
    public int Team { get; set; } = 1;

    public int Health;
    public Vector2 TargetPosition;

    private bool _isSelected = false;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                QueueRedraw(); // Redraw selection circle/range
                var sprite = GetNode<Sprite2D>("Sprite2D");
                if (_isSelected)
                {
                    sprite.Modulate = new Color(0, 1, 0); // Green
                }
                else
                {
                    sprite.Modulate = Team == 1 ? new Color(1, 1, 1) : new Color(1, 0, 0); // White or Red
                }
            }
        }
    }

    private Sprite2D _sprite;

    public override void _Ready()
    {
        if (UnitData != null)
        {
            Health = UnitData.Health;
        }
        TargetPosition = Position;

        var healthBar = GetNode<ProgressBar>("HealthBar");
        healthBar.MaxValue = Health;
        healthBar.Value = Health;

        _sprite = GetNode<Sprite2D>("Sprite2D");
        IsSelected = false; // To initialize the color
    }

    public override void _Process(double delta)
    {
        if (Position.DistanceTo(TargetPosition) > 5)
        {
            var direction = Position.DirectionTo(TargetPosition);
            Velocity = direction * UnitData.MovementSpeed;
            MoveAndSlide();
        }
        else
        {
            Velocity = Vector2.Zero;
        }
    }

    public override void _Draw()
    {
        if (IsSelected)
        {
            // Draw Selection Circle
            DrawCircle(Vector2.Zero, 30, new Color(0, 1, 0, 0.3f));

            // Draw Range Circle if in Shooting Phase (context dependent, but for now always if selected)
            // We can check Main phase? Or just always draw for now.
            if (UnitData != null)
            {
                DrawArc(Vector2.Zero, UnitData.WeaponRange, 0, Mathf.Tau, 32, new Color(1, 0, 0, 0.5f), 2.0f);
            }
        }
    }

    public void MoveTo(Vector2 newPosition)
    {
        TargetPosition = newPosition;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        var healthBar = GetNode<ProgressBar>("HealthBar");
        healthBar.Value = Health;

        if (Health <= 0)
        {
            EmitSignal(SignalName.Died);
            QueueFree();
        }
    }

    // Removed old Attack method, logic will be handled by Main/CombatManager

    private void _on_input_event(Node viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.ButtonIndex == MouseButton.Left && mouseButtonEvent.Pressed)
        {
            EmitSignal(SignalName.Selected, this);
        }
    }

    public string GetStatsString()
    {
        if (UnitData == null) return "No Data";

        string stats = $"Name: {UnitData.UnitName}\n";
        stats += $"Type: {(UnitData.IsTank ? "Tank" : "Infantry")}\n";
        stats += $"Health: {Health}/{UnitData.Health}\n";
        stats += $"Move: {UnitData.MovementSpeed}\n";
        stats += $"Skill: {UnitData.Skill}+\n";

        if (UnitData.IsTank)
        {
            stats += $"Armor: F{UnitData.ArmorFront} S{UnitData.ArmorSide} T{UnitData.ArmorTop}\n";
        }
        else
        {
            stats += $"Save: {UnitData.Save}+\n";
        }

        stats += $"Weapon: Range {UnitData.WeaponRange}, ROF {UnitData.ROF}, AT {UnitData.AntiTank}, FP {UnitData.Firepower}+";

        return stats;
    }
}
