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
    public int AttackPower;
    public int MovementSpeed;
    public Vector2 TargetPosition;

    private bool _isSelected = false;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
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

    private Sprite2D _sprite;

    public override void _Ready()
    {
        if (UnitData != null)
        {
            Health = UnitData.Health;
            AttackPower = UnitData.AttackPower;
            MovementSpeed = UnitData.MovementSpeed;
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
            Velocity = direction * MovementSpeed;
            MoveAndSlide();
        }
        else
        {
            Velocity = Vector2.Zero;
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

    public void Attack(Unit target)
    {
        var damage = (int)(GD.Randi() % (AttackPower + 1));
        target.TakeDamage(damage);
    }

    private void _on_input_event(Node viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.ButtonIndex == MouseButton.Left && mouseButtonEvent.Pressed)
        {
            EmitSignal(SignalName.Selected, this);
        }
    }
}
