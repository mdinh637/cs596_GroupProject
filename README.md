# cs596 Group Project

## Group name

**[GROUP NAME]**

## Members and roles

| Member | Role |
|--------|------|
| **[Name]** | **[Short role — e.g. enemy wave logic and combat balancing]** |
| **[Name]** | **[...]** |
| **[Name]** | **[...]** |
| **[Name]** | **[...]** |
| **[Name]** | **[...]** |

| Contributor | Role |
|------------|------|
| **Laura Wetherhold / FluffShady** | **Character import, humanoid animation setup, waypoint prototype + docs** |
| **Michael Dinh** | **Implementing ally troop and enemy logic, troop placement, combat system** |

## Game description

**[Game title]** is **[one-sentence pitch — e.g. a 3D lane-style tactics game where you spawn units into lanes and defend your base]**.

**Gameplay.** **[Paragraph: what the player does moment-to-moment — spawning, lane choice, combat, towers, etc.]**

**Goals.** **[Paragraph: win/lose conditions and what “success” looks like in a match.]**

**Course elements.** We plan to incorporate **AI-controlled unit movement on waypoint paths, lane-style spawning/placement, collision/combat logic, and player interaction systems (camera/UI/tower placement)**. Prototype work currently includes **imported and retargeted humanoid character animations, an idle/walk Animator setup, and a working A -> B waypoint movement prototype with placeholder environment geometry**.

## Technical notes (prototype)

- Character import and waypoint path: see `Assets/Docs/CharacterImportAndWaypoints.md`.
- Waypoint movement: `Assets/Scripts/Movement/WaypointMover.cs`.
- Current prototype status from my work:
  - imported KayKit character assets and textures into project structure
  - configured humanoid rig/avatar mapping and shared Rig_Medium animation clips
  - created Animator states/transitions (`Idle`/`Walk`) driven by `Speed`
  - wired `WaypointMover` + Animator and verified lane-style A -> B movement
  - documented setup, troubleshooting, and reusable character workflow in project docs

## Help / risks

**[Optional: e.g. “We may need TA help on NavMesh vs. simple waypoints for lane following.”]**

## Repository

**[https://github.com/mdinh637/cs596_GroupProject](https://github.com/mdinh637/cs596_GroupProject)**
