namespace WindsOfWar
{
    public class Weapon
    {
        public string Name { get; set; } = "Weapon";
        public int Range { get; set; } = 400;
        /// <summary>Range at which long-range modifier applies (0 = no long range).</summary>
        public int LongRange { get; set; } = 0;
        /// <summary>Extra to-hit penalty when firing at long range (e.g. 1 = need one higher roll).</summary>
        public int LongRangeToHitModifier { get; set; } = 1;
        public int HaltedROF { get; set; } = 1;
        public int MovingROF { get; set; } = 1;
        public int AntiTank { get; set; } = 0;
        public int Firepower { get; set; } = 6;

        /// <summary>If true, fire at a point using template; any unit under template can be hit.</summary>
        public bool IsArtillery { get; set; }
        /// <summary>Template radius in pixels (blast area). Used when IsArtillery is true.</summary>
        public int TemplateRadius { get; set; } = 60;
        /// <summary>If true, this artillery can fire a smoke salvo instead of HE (place smoke marker, no damage).</summary>
        public bool CanSmoke { get; set; }
    }
}
