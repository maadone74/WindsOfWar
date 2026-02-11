using System.Collections.Generic;

namespace WindsOfWar
{
    public static class UnitLibrary
    {
        public static List<UnitData> GetAllUnits()
        {
            var units = new List<UnitData>();

            // --- AMERICANS ---
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

            // --- GERMANS ---
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

            return units;
        }
    }
}
