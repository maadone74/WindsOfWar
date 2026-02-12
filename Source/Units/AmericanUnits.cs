using System.Collections.Generic;

namespace WindsOfWar
{
    public static class AmericanUnits
    {
        public static List<UnitData> GetUnits()
        {
            var units = new List<UnitData>();

            // Infantry
            units.Add(new UnitData
            {
                Name = "Rifle Team (USA)",
                Type = UnitType.Infantry,
                Health = 1,
                MovementDistance = 150,
                Skill = 4, Save = 3,
                Weapons = new List<Weapon> { new Weapon { Name = "Rifle", Range = 400, HaltedROF = 1, MovingROF = 1, AntiTank = 0, Firepower = 6 } }
            });

            units.Add(new UnitData
            {
                Name = "Parachute Rifle",
                Type = UnitType.Infantry,
                Health = 1,
                MovementDistance = 150,
                Skill = 3, Save = 3, // Fearless Veteran
                Weapons = new List<Weapon> { new Weapon { Name = "Rifle/MG", Range = 400, HaltedROF = 2, MovingROF = 1, AntiTank = 2, Firepower = 6 } }
            });

            // Tanks
            units.Add(new UnitData
            {
                Name = "Sherman M4",
                Type = UnitType.Tank,
                Health = 1,
                MovementDistance = 250,
                Skill = 4, Save = 3, FrontArmor = 6, SideArmor = 4,
                Weapons = new List<Weapon>
                {
                    new Weapon { Name = "75mm Gun", Range = 600, HaltedROF = 2, MovingROF = 1, AntiTank = 10, Firepower = 3 },
                    new Weapon { Name = ".50 cal MG", Range = 400, HaltedROF = 3, MovingROF = 1, AntiTank = 2, Firepower = 6 }
                }
            });

            units.Add(new UnitData
            {
                Name = "Sherman 76mm",
                Type = UnitType.Tank,
                Health = 1,
                MovementDistance = 250,
                Skill = 4, Save = 3, FrontArmor = 7, SideArmor = 4,
                Weapons = new List<Weapon>
                {
                    new Weapon { Name = "76mm Gun", Range = 800, HaltedROF = 2, MovingROF = 1, AntiTank = 12, Firepower = 3 },
                    new Weapon { Name = ".50 cal MG", Range = 400, HaltedROF = 3, MovingROF = 1, AntiTank = 2, Firepower = 6 }
                }
            });

            units.Add(new UnitData
            {
                Name = "M10 Tank Destroyer",
                Type = UnitType.Tank,
                Health = 1,
                MovementDistance = 250,
                Skill = 4, Save = 3, FrontArmor = 4, SideArmor = 2,
                Weapons = new List<Weapon>
                {
                    new Weapon { Name = "3-inch Gun", Range = 800, HaltedROF = 2, MovingROF = 1, AntiTank = 12, Firepower = 3 },
                    new Weapon { Name = ".50 cal MG", Range = 400, HaltedROF = 3, MovingROF = 1, AntiTank = 2, Firepower = 6 }
                }
            });

            units.Add(new UnitData
            {
                Name = "M5 Stuart",
                Type = UnitType.Tank,
                Health = 1,
                MovementDistance = 300,
                Skill = 4, Save = 3, FrontArmor = 4, SideArmor = 2,
                Weapons = new List<Weapon>
                {
                    new Weapon { Name = "37mm Gun", Range = 600, HaltedROF = 2, MovingROF = 1, AntiTank = 7, Firepower = 4 },
                    new Weapon { Name = "MG", Range = 400, HaltedROF = 3, MovingROF = 1, AntiTank = 2, Firepower = 6 }
                }
            });

            // Guns
             units.Add(new UnitData
            {
                Name = "105mm Field Battery",
                Type = UnitType.Gun,
                Health = 1,
                MovementDistance = 50,
                Skill = 4, Save = 3,
                Weapons = new List<Weapon>
                {
                    new Weapon { Name = "105mm Howitzer", Range = 1200, HaltedROF = 1, MovingROF = 1, AntiTank = 9, Firepower = 2 }
                }
            });

            return units;
        }
    }
}
