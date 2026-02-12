using System.Collections.Generic;

namespace WindsOfWar
{
    public static class GermanUnits
    {
        public static List<UnitData> GetUnits()
        {
            var units = new List<UnitData>();

            // Infantry
            units.Add(new UnitData
            {
                Name = "Grenadier (GER)",
                Type = UnitType.Infantry,
                Health = 1,
                MovementDistance = 150,
                Skill = 3, Save = 3, // Veterans
                Weapons = new List<Weapon> { new Weapon { Name = "Rifle/MG", Range = 400, HaltedROF = 2, MovingROF = 1, AntiTank = 0, Firepower = 6 } }
            });

            units.Add(new UnitData
            {
                Name = "Panzergrenadier",
                Type = UnitType.Infantry,
                Health = 1,
                MovementDistance = 150,
                Skill = 3, Save = 3, // Veterans
                Weapons = new List<Weapon> { new Weapon { Name = "MG42", Range = 400, HaltedROF = 3, MovingROF = 2, AntiTank = 2, Firepower = 6 } }
            });

            // Tanks
            units.Add(new UnitData
            {
                Name = "Panzer IV H",
                Type = UnitType.Tank,
                Health = 1,
                MovementDistance = 250,
                Skill = 3, Save = 3, FrontArmor = 6, SideArmor = 3,
                Weapons = new List<Weapon>
                {
                    new Weapon { Name = "7.5cm Gun", Range = 800, HaltedROF = 2, MovingROF = 1, AntiTank = 11, Firepower = 3 },
                    new Weapon { Name = "MG", Range = 400, HaltedROF = 3, MovingROF = 1, AntiTank = 2, Firepower = 6 }
                }
            });

            units.Add(new UnitData
            {
                Name = "StuG III G",
                Type = UnitType.Tank,
                Health = 1,
                MovementDistance = 250,
                Skill = 3, Save = 3, FrontArmor = 7, SideArmor = 3,
                Weapons = new List<Weapon>
                {
                    new Weapon { Name = "7.5cm Gun", Range = 800, HaltedROF = 2, MovingROF = 1, AntiTank = 11, Firepower = 3 },
                    new Weapon { Name = "MG", Range = 400, HaltedROF = 3, MovingROF = 1, AntiTank = 2, Firepower = 6 }
                }
            });

            units.Add(new UnitData
            {
                Name = "Panther A",
                Type = UnitType.Tank,
                Health = 1,
                MovementDistance = 250,
                Skill = 3, Save = 3, FrontArmor = 10, SideArmor = 5,
                Weapons = new List<Weapon>
                {
                    new Weapon { Name = "7.5cm L/70", Range = 1000, HaltedROF = 2, MovingROF = 1, AntiTank = 14, Firepower = 3 },
                    new Weapon { Name = "MG", Range = 400, HaltedROF = 3, MovingROF = 1, AntiTank = 2, Firepower = 6 }
                }
            });

            units.Add(new UnitData
            {
                Name = "Tiger I E",
                Type = UnitType.Tank,
                Health = 1,
                MovementDistance = 200, // Slow
                Skill = 3, Save = 3, FrontArmor = 9, SideArmor = 8,
                Weapons = new List<Weapon>
                {
                    new Weapon { Name = "8.8cm Gun", Range = 1000, HaltedROF = 2, MovingROF = 1, AntiTank = 14, Firepower = 3 },
                    new Weapon { Name = "MG", Range = 400, HaltedROF = 3, MovingROF = 1, AntiTank = 2, Firepower = 6 }
                }
            });

             units.Add(new UnitData
            {
                Name = "Tiger II (King Tiger)",
                Type = UnitType.Tank,
                Health = 1,
                MovementDistance = 200, // Slow
                Skill = 3, Save = 3, FrontArmor = 16, SideArmor = 8,
                Weapons = new List<Weapon>
                {
                    new Weapon { Name = "8.8cm KwK43", Range = 1000, HaltedROF = 2, MovingROF = 1, AntiTank = 16, Firepower = 3 },
                    new Weapon { Name = "MG", Range = 400, HaltedROF = 3, MovingROF = 1, AntiTank = 2, Firepower = 6 }
                }
            });

            // Guns
            units.Add(new UnitData
            {
                Name = "Pak 40 Anti-tank Gun",
                Type = UnitType.Gun,
                Health = 1,
                MovementDistance = 50,
                Skill = 3, Save = 3,
                Weapons = new List<Weapon>
                {
                    new Weapon { Name = "7.5cm PaK40", Range = 800, HaltedROF = 2, MovingROF = 1, AntiTank = 12, Firepower = 3 }
                }
            });

            return units;
        }
    }
}
