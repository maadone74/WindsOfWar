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
                Points = 2,
                Weapons = new List<Weapon> { new Weapon { Name = "Rifle", Range = 400, HaltedROF = 1, MovingROF = 1, AntiTank = 0, Firepower = 6 } }
            });

            units.Add(new UnitData
            {
                Name = "Sherman M4",
                Type = UnitType.Tank,
                Health = 1,
                MovementDistance = 250,
                Skill = 4, Save = 3, FrontArmor = 6, SideArmor = 4,
                Points = 10,
                Weapons = new List<Weapon>
                {
                    new Weapon { Name = "75mm Gun", Range = 600, LongRange = 300, HaltedROF = 2, MovingROF = 1, AntiTank = 10, Firepower = 3 },
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
                Points = 12,
                Weapons = new List<Weapon>
                {
                    new Weapon { Name = "76mm Gun", Range = 800, LongRange = 400, HaltedROF = 2, MovingROF = 1, AntiTank = 12, Firepower = 3 },
                    new Weapon { Name = ".50 cal MG", Range = 400, HaltedROF = 3, MovingROF = 1, AntiTank = 2, Firepower = 6 }
                }
            });

            units.Add(new UnitData
            {
                Name = "M10 Wolverine",
                Type = UnitType.TankDestroyer,
                Health = 1,
                MovementDistance = 260,
                Skill = 4, Save = 3, FrontArmor = 4, SideArmor = 2,
                Points = 9,
                Weapons = new List<Weapon>
                {
                    new Weapon { Name = "3in Gun", Range = 800, LongRange = 400, HaltedROF = 2, MovingROF = 1, AntiTank = 11, Firepower = 3 },
                    new Weapon { Name = ".50 cal", Range = 400, HaltedROF = 2, MovingROF = 1, AntiTank = 2, Firepower = 6 }
                }
            });

            units.Add(new UnitData
            {
                Name = "M18 Hellcat",
                Type = UnitType.TankDestroyer,
                Health = 1,
                MovementDistance = 280,
                Skill = 4, Save = 3, FrontArmor = 3, SideArmor = 2,
                Points = 8,
                Weapons = new List<Weapon>
                {
                    new Weapon { Name = "76mm Gun", Range = 800, LongRange = 400, HaltedROF = 2, MovingROF = 1, AntiTank = 12, Firepower = 3 },
                    new Weapon { Name = ".50 cal", Range = 400, HaltedROF = 2, MovingROF = 1, AntiTank = 2, Firepower = 6 }
                }
            });

            units.Add(new UnitData
            {
                Name = ".30cal MG Team (USA)",
                Type = UnitType.Infantry,
                Health = 1,
                MovementDistance = 120,
                Skill = 4, Save = 3,
                Points = 2,
                Weapons = new List<Weapon> { new Weapon { Name = "MG", Range = 500, HaltedROF = 3, MovingROF = 1, AntiTank = 0, Firepower = 6 } }
            });

            // US Gun (artillery)
            units.Add(new UnitData
            {
                Name = "M2 105mm (USA)",
                Type = UnitType.Gun,
                Health = 1,
                MovementDistance = 100,
                Skill = 4, Save = 3,
                Points = 8,
                Weapons = new List<Weapon>
                {
                    new Weapon { Name = "105mm Howitzer", Range = 1200, LongRange = 600, HaltedROF = 1, MovingROF = 0, AntiTank = 6, Firepower = 2, IsArtillery = true, TemplateRadius = 70, CanSmoke = true },
                    new Weapon { Name = "MG", Range = 400, HaltedROF = 2, MovingROF = 0, AntiTank = 2, Firepower = 6 }
                }
            });

            // --- GERMANS ---
            units.Add(new UnitData
            {
                Name = "Grenadier (GER)",
                Type = UnitType.Infantry,
                Health = 1,
                MovementDistance = 150,
                Skill = 3, Save = 3,
                Points = 3,
                Weapons = new List<Weapon> { new Weapon { Name = "Rifle/MG", Range = 400, HaltedROF = 2, MovingROF = 1, AntiTank = 0, Firepower = 6 } }
            });

            units.Add(new UnitData
            {
                Name = "MG42 Team (GER)",
                Type = UnitType.Infantry,
                Health = 1,
                MovementDistance = 120,
                Skill = 3, Save = 3,
                Points = 3,
                Weapons = new List<Weapon> { new Weapon { Name = "MG42", Range = 500, HaltedROF = 4, MovingROF = 1, AntiTank = 0, Firepower = 6 } }
            });

            units.Add(new UnitData
            {
                Name = "Panzer III L",
                Type = UnitType.Tank,
                Health = 1,
                MovementDistance = 250,
                Skill = 3, Save = 3, FrontArmor = 5, SideArmor = 3,
                Points = 8,
                Weapons = new List<Weapon>
                {
                    new Weapon { Name = "5cm Gun", Range = 600, LongRange = 300, HaltedROF = 2, MovingROF = 1, AntiTank = 9, Firepower = 3 },
                    new Weapon { Name = "MG", Range = 400, HaltedROF = 3, MovingROF = 1, AntiTank = 2, Firepower = 6 }
                }
            });

            units.Add(new UnitData
            {
                Name = "Panzer IV H",
                Type = UnitType.Tank,
                Health = 1,
                MovementDistance = 250,
                Skill = 3, Save = 3, FrontArmor = 6, SideArmor = 3,
                Points = 10,
                Weapons = new List<Weapon>
                {
                    new Weapon { Name = "7.5cm Gun", Range = 800, LongRange = 400, HaltedROF = 2, MovingROF = 1, AntiTank = 11, Firepower = 3 },
                    new Weapon { Name = "MG", Range = 400, HaltedROF = 3, MovingROF = 1, AntiTank = 2, Firepower = 6 }
                }
            });

            units.Add(new UnitData
            {
                Name = "Tiger I E",
                Type = UnitType.Tank,
                Health = 1,
                MovementDistance = 200,
                Skill = 3, Save = 3, FrontArmor = 9, SideArmor = 8,
                Points = 18,
                Weapons = new List<Weapon>
                {
                    new Weapon { Name = "8.8cm Gun", Range = 1000, LongRange = 500, HaltedROF = 2, MovingROF = 1, AntiTank = 14, Firepower = 3 },
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
                Points = 16,
                Weapons = new List<Weapon>
                {
                    new Weapon { Name = "7.5cm L/70", Range = 1000, LongRange = 500, HaltedROF = 2, MovingROF = 1, AntiTank = 14, Firepower = 3 },
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
                Points = 11,
                Weapons = new List<Weapon>
                {
                    new Weapon { Name = "7.5cm Gun", Range = 800, LongRange = 400, HaltedROF = 2, MovingROF = 1, AntiTank = 11, Firepower = 3 },
                    new Weapon { Name = "MG", Range = 400, HaltedROF = 3, MovingROF = 1, AntiTank = 2, Firepower = 6 }
                }
            });

            // German Gun (anti-tank)
            units.Add(new UnitData
            {
                Name = "PaK 40 7.5cm (GER)",
                Type = UnitType.Gun,
                Health = 1,
                MovementDistance = 80,
                Skill = 3, Save = 3,
                Points = 7,
                Weapons = new List<Weapon>
                {
                    new Weapon { Name = "7.5cm PaK 40", Range = 800, LongRange = 400, HaltedROF = 2, MovingROF = 0, AntiTank = 12, Firepower = 3 },
                    new Weapon { Name = "MG", Range = 400, HaltedROF = 2, MovingROF = 0, AntiTank = 2, Firepower = 6 }
                }
            });

            units.Add(new UnitData
            {
                Name = "8.8cm Flak (GER)",
                Type = UnitType.Gun,
                Health = 1,
                MovementDistance = 90,
                Skill = 3, Save = 3,
                Points = 10,
                Weapons = new List<Weapon>
                {
                    new Weapon { Name = "8.8cm", Range = 1000, LongRange = 500, HaltedROF = 2, MovingROF = 0, AntiTank = 13, Firepower = 3, IsArtillery = true, TemplateRadius = 65 },
                    new Weapon { Name = "MG", Range = 400, HaltedROF = 2, MovingROF = 0, AntiTank = 2, Firepower = 6 }
                }
            });

            return units;
        }
    }
}
