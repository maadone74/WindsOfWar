namespace WindsOfWar
{
    public enum UnitType
    {
        Infantry,
        Tank,
        Gun
    }

    public class UnitData
    {
        public string Name { get; set; } = "Unit";
        public UnitType Type { get; set; } = UnitType.Infantry;
        public int Health { get; set; } = 1;

        // Movement
        public int MovementDistance { get; set; } = 150; // Pixels per turn

        // Shooting
        public int Range { get; set; } = 400;
        public int HaltedROF { get; set; } = 1;
        public int MovingROF { get; set; } = 1;
        public int AntiTank { get; set; } = 0; // 0 means no AT (bullet)
        public int Firepower { get; set; } = 6; // 1+ (best) to 6+ (worst)

        // Defense
        public int Skill { get; set; } = 4; // 4+ To Hit (Trained)
        public int Save { get; set; } = 3; // 3+ Infantry Save

        // Armor (for Tanks)
        public int FrontArmor { get; set; } = 0;
        public int SideArmor { get; set; } = 0;
        public int TopArmor { get; set; } = 0;
    }
}
