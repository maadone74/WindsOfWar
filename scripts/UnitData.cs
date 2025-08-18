using Godot;

public partial class UnitData : Resource
{
    [Export]
    public int Health { get; set; } = 100;

    [Export]
    public int AttackPower { get; set; } = 10;

    [Export]
    public int MovementSpeed { get; set; } = 200;
}
