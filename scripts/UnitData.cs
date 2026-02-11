using Godot;

public partial class UnitData : Resource
{
    [Export]
    public string UnitName { get; set; } = "Unit";

    [Export]
    public bool IsTank { get; set; } = false;

    [Export]
    public int MovementSpeed { get; set; } = 200;

    [Export]
    public int Health { get; set; } = 100; // Structure Points or Team Size

    // Combat Stats
    [Export]
    public int Skill { get; set; } = 4; // To Hit (e.g. 4+)

    [Export]
    public int Save { get; set; } = 3; // Infantry Save (e.g. 3+)

    [Export]
    public int ArmorFront { get; set; } = 0; // Tank Front Armor

    [Export]
    public int ArmorSide { get; set; } = 0; // Tank Side Armor

    [Export]
    public int ArmorTop { get; set; } = 0; // Tank Top Armor

    [Export]
    public int AntiTank { get; set; } = 0; // AT rating

    [Export]
    public int Firepower { get; set; } = 6; // Firepower rating (e.g. 5+)

    [Export]
    public int ROF { get; set; } = 1; // Rate of Fire

    [Export]
    public float WeaponRange { get; set; } = 400.0f;
}
