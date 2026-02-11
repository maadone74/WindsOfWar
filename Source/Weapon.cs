namespace WindsOfWar
{
    public class Weapon
    {
        public string Name { get; set; } = "Weapon";
        public int Range { get; set; } = 400;
        public int HaltedROF { get; set; } = 1;
        public int MovingROF { get; set; } = 1;
        public int AntiTank { get; set; } = 0;
        public int Firepower { get; set; } = 6;
    }
}
