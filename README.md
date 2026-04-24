# CS596 Group Project

## Group name: 5Stack

## Game Name: Battle Knights

## Members and roles

| Member | Role |
|--------|------|
| **Husain Patanwala** | **Asset research / game concept design / tower+ability upgrade system + projectile mechanics** |
| **Glory Kanda** | **Handles playtesting, bug documentation, game balance feedback, and final submission materials.** |
| **Michael Dinh** | **Implementing ally troop and enemy logic, troop placement, combat system** |
| **Laura Wetherhold** | **Character import, humanoid animation setup, waypoint prototype + init docs** |
| **Alejandro Alvarado** | **Enemy tower functionality, HUD and UI** |

## Game description

**[Battle Knights]** is a 3D lane-based strategy game where the player manages their troops in the lane. Taking reference to an ARAM-style gameplay and theme from League of Legends, the goal is to deploy troops to push down the lane and destroy the enemy base located on the opposite end, while protecting your own base in the process. 

The main mechanic involves managing the player's currency and deploying troops in an effective manner. The player will passively generate currency with the possibility of upgrading it over time, while also being able to obtain currency from defeating enemy troops and objectives. 

Through the usage of the currency, the player will be able to place troops anywhere in a designated area in front of their base that will follow a set path towards the enemy base. When units encounter an opposing unit within their field of range, they will lock onto the target and close the distance to enter attack range using AI logic. In the case where the unit in sight is defeated or no longer in range of sight, the troop will continue down its original path towards the enemy base. The player wins when they destroy the enemy base, but will lose if the enemy manages to destroy the player's base first. 

### Graphics
In order to focus on gameplay, we chose to search for open source, free 3d assets to enhance the visual aspect of the game. Decided to use assets mainly from KayKit developer on itch.io. 

### SFX
The sound effects are to be determined based on what interactions we have in the game. For example, currently the tower shoots projectile that appear like cannonballs. We could have it so that on impact if the projectile explodes we attach an explosion sfx. 

### X-Factor (Multiple enemeies/units and AI and Custom shaders (maybe))

Current allied unit designs include:
  - Tank, high health, high target dmg, slow atk spd, short atk range
  - Basic soldier, balanced stats, short atk range
  - Archer, long-ranged atks, lower dmg and hp
  - Assassin, untargetable until initiating combat, still damageable from aoe even when untargetable
For enemy units:
  - Lower-tier soldier, weaker stats all around but more frequent
  - Tank, high health, average aoe dmg, slow atk spd, short atk range
  - Air unit, targetable only by ranged atks, swift, low hp
The elements in class we plan to incorporate include AI behavior and possibly shaders for a ground design. The core gameplay is what we hope to achieve, but the troops and certain mechanics are not yet set in stone.

### Fun factor
This game is a mix of ARAM, tower defense, and strategy. The player will have to decide what to spend their experience points on: unlocking new characters/troops, upgrading tower efficacy, or other upgrades. Also during the battle itself they have to actively think and and deal with different enemy combos sent at them.

## Gameplay 

**[one-sentence pitch — e.g. a 3D lane-style tactics game where you spawn units into lanes and defend your base]**.

**Gameplay.** **[Paragraph: what the player does moment-to-moment — spawning, lane choice, combat, towers, etc.]**

**Goals.** **[Paragraph: win/lose conditions and what “success” looks like in a match.]**

**Course elements.** We plan to incorporate **AI-controlled unit movement on waypoint paths, lane-style spawning/placement, collision/combat logic, and player interaction systems (camera/UI/tower placement)**. Prototype work currently includes **imported and retargeted humanoid character animations, an idle/walk Animator setup, and a working A -> B waypoint movement prototype with placeholder environment geometry**.

## Help / risks

**[Optional: e.g. “We may need TA help on NavMesh vs. simple waypoints for lane following.”]**

## Repository

**[https://github.com/mdinh637/cs596_GroupProject](https://github.com/mdinh637/cs596_GroupProject)**
