# WindsOfWar

A Flames of War–style tactical miniatures game built with MonoGame. Command American or German forces in turn-based battles with movement, shooting, assault, and artillery template fire.

## How to Run

```bash
dotnet run
```

Requires .NET 8 and MonoGame.

## Game Flow

1. **Splash** — Choose your side (Americans / Germans) and Start Game.
2. **Scenario Select** — Pick a scenario (Open Field, River Crossing).
3. **Force Setup** — Add units for each side (click unit names). Build to the **100-point** limit. Start Battle when ready.
4. **Gameplay** — Turns alternate; each turn has four phases: **Starting** → **Movement** → **Shooting** → **Assault**.

## Phases

- **Starting** — Rally and remount (bailed tanks may try to remount).
- **Movement** — Select a friendly unit, right-click to move. **Green circle** = move range.
- **Shooting** — Select a friendly unit, press **W** to cycle weapons. **Red circle** = weapon range.
  - **Direct fire:** Left-click an enemy to fire. Range, concealment (forest/building/smoke), and long-range modifiers apply.
  - **Artillery (template):** If the selected weapon is artillery (e.g. 105mm Howitzer, 8.8cm Flak), **click anywhere on the map** within range to place the template. All units under the template can be hit; hits are rolled (ROF dice vs Skill) and applied to units in the blast area.
- **Assault** — Move into base contact with an enemy, then in Assault phase select your unit and click the enemy to resolve close combat.

## Artillery & Smoke

- **Template fire** — Artillery weapons (M2 105mm, 8.8cm Flak) fire at a **point**, not a unit. A **template** (blast circle) is placed at the clicked location; any unit under the template is eligible to be hit. A yellow preview shows the template at the mouse when in range.
- **Smoke salvo** — The 105mm Howitzer can fire smoke instead of HE: press **S** to toggle **Smoke** mode, then click on the map. A **smoke marker** is placed (no damage). Units inside smoke are **concealed** (+1 to hit for shooters). Smoke lasts **2 turns** then is removed.

## Units

- **Infantry** — Rifle teams, Grenadiers, MG teams; use Save when hit; get 2 dice in assault.
- **Tanks** — Sherman, Panzer IV, Tiger, Panther, StuG; front/side armor; can bail or be destroyed.
- **Tank destroyers** — M10 Wolverine, M18 Hellcat; open-topped, good AT.
- **Guns** — M2 105mm (US, artillery + smoke), PaK 40, 8.8cm Flak (GER, artillery); long range; use Save when hit.

Each unit has a points cost shown in setup (e.g. Tiger 18pt, Rifle Team 2pt).

## Terrain

- **Forest / Building** — Concealment (+1 to hit for shooter).
- **Hill** — Defensive position (+1 to save for infantry/gun in the hill).
- **Smoke** — Concealment; placed by smoke salvos.
- **River** — Blocks movement (scenario-dependent).

## Objectives

Scenarios include **objective markers** (gold zones). Control = having a unit inside the zone; contested if both sides have units there. The UI shows **Objectives P1:X P2:X** each turn.

## Controls

| Action | Input |
|--------|--------|
| Move unit | Right-click (Movement phase) |
| Direct fire | Select friend, click enemy (Shooting phase) |
| Artillery template fire | Select artillery unit, click map within red circle |
| Smoke salvo | **S** to toggle (if weapon can smoke), then click map |
| Assault | Select friend in contact, click enemy (Assault phase) |
| Cycle weapon | **W** |
| Next phase | Click **NEXT PHASE** |
| Back / quit | **Esc** |

## Tech

- **.NET 8**, **MonoGame 3.8** (Desktop GL)
- C#; procedural font and fallback sounds if audio/content missing
