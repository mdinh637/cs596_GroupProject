# Bare minimum: one character, idle/walk, A → B

Checklist for the check-in demo (import → animate → `WaypointMover` → short clip).

---

## 1) Copy files into the Unity project

From `KayKit_Adventurers_2.0_FREE` on your PC, copy into the repo under something like:

`Assets/Art/KayKit_Adventurers/` with subfolders:
- `Models/Characters/`
- `AnimatorControllers/`
- `Textures/`
- `Animations/`

**Character (pick one):**

- `Characters/fbx/Knight.fbx`  
- `Characters/fbx/knight_texture.png` (same folder; needed for materials)

(Alternatives: `Barbarian.fbx`, `Ranger.fbx`, etc. — same steps.)

**Animations (shared “Rig_Medium” clips — match the medium-sized characters):**

- `Animations/fbx/Rig_Medium/Rig_Medium_MovementBasic.fbx` — walk / run style clips  
- `Animations/fbx/Rig_Medium/Rig_Medium_General.fbx` — idle / misc (use whatever reads as idle in the Import list)

This pack ships **FBX**, not `.blend`; Unity imports FBX directly.

---

## 2) Import settings (Project window)

**A. Character model (`Knight.fbx`)**

1. Select `Knight.fbx`.
2. **Rig** tab: **Animation Type** = **Humanoid**, **Avatar Definition** = **Create From This Model**, Apply.
3. **Model** tab: scale 1; if the character is huge/tiny in scene, adjust **Scale Factor** once and Apply.

Note: `Knight.fbx` may show **No animation data available in this model** on the Animation tab. That is expected for this pack because clips come from the separate `Rig_Medium_*.fbx` files.

**B. Animation FBX files (`Rig_Medium_*.fbx`)**

For each animation file:

1. **Rig** tab: **Animation Type** = **Humanoid**.
2. **Avatar Definition** = **Copy From Other Avatar** → drag the **Avatar** from `Knight.fbx` (expand the asset; the Avatar is the sub-asset), Apply.  
   If Unity complains, try **Create From This Model** on the animation file, then switch to Copy From Other Avatar after the Knight avatar exists.
3. **Animation** tab: enable **Loop Time** on clips that should loop (**Walk**, **Idle**, etc.), Apply.

Note: warnings like **translation animation will be discarded** are common with Humanoid retargeting and usually fine for this prototype if clips still preview/play.

---

## 3) Animator (idle + walk, driven by `Speed`)

1. Right-click in Project → **Create → Animator Controller** (e.g. `Knight_Locomotion`).
2. Double-click the controller to open the Animator window.
3. In the Animator window, in the **Parameters** tab (next to Layers), click the **+** button → choose **Float** → name it `Speed` (exact name to match `WaypointMover`).
4. Create states:
   - right-click empty space → **Create State → Empty** (make `Idle`)
   - right-click empty space → **Create State → Empty** (make `Walk`)
5. Assign animation clips to each state:
   - click `Idle` state, then in Inspector set **Motion** = idle clip (example: `Idle_A`)
   - click `Walk` state, then in Inspector set **Motion** = walk clip
6. Make transitions:
   - right-click `Idle` state → **Make Transition** → click `Walk`
   - right-click `Walk` state → **Make Transition** → click `Idle`
7. Edit transitions (click each transition arrow):
   - uncheck **Has Exit Time**
   - keep **Transition Duration** short (`0.05`–`0.1`)
   - add conditions:
     - `Idle -> Walk`: `Speed` **Greater** `0.1`
     - `Walk -> Idle`: `Speed` **Less** `0.1`
8. Set default state: right-click `Idle` → **Set as Layer Default State**.

9. On your character in the hierarchy (prefab or instance): add **Animator**, assign **Controller** = `Knight_Locomotion`.
10. Avatar is usually auto-filled from the model import. If not, drag the character avatar from the model asset into the Animator's **Avatar** field.

Prefab step (if you dragged a model into the scene):

1. Create folder `Assets/Prefabs` if needed.
2. Drag the character object from **Hierarchy** into `Assets/Prefabs`.
3. Unity creates a prefab asset; continue wiring Animator/WaypointMover on that prefab instance.

---

## 4) Scene setup: ground, two waypoints, move A → B

1. Open your scene (e.g. `SampleScene`).
2. Ground: use your existing plane or a cube scaled as a floor.
3. Place the character near one end of the “lane.”
4. Create two empties: **GameObject → Create Empty** → name `Waypoint_A`, `Waypoint_B`.  
   Position them on the floor (same Y as feet; small offset is OK).
5. Put the character at **Waypoint_A** (or just in front of it).
6. On the **root** GameObject that moves (same object as **Animator**): **Add Component → Waypoint Mover**.
7. **Waypoints** array: size **2**, element 0 = `Waypoint_A`, element 1 = `Waypoint_B`.
8. **Move Speed** ~ `2`–`4`; **Arrival Threshold** ~ `0.15`.
9. Drag the character's **Animator component** into the `WaypointMover.Animator` slot; set **Speed Float Parameter** = `Speed`.
10. Press **Play**: the unit should walk **A → B** and stop (script disables itself at the end unless **Loop Path** is on).

Quick check if animation does not switch:

- the character object has an **Animator** component
- `Animator.Controller` is set to `Knight_Locomotion`
- `WaypointMover.Animator` references that same Animator
- `WaypointMover.Speed Float Parameter` exactly matches Animator parameter name (`Speed`)

---

## 5) Record a 20–30s clip + five bullets for the team / TA

**Recording (quick options on Windows):**

- **Win + G** → Xbox Game Bar → Capture → **Record last…** / Start recording, or  
- **Win + Alt + R** (if enabled), or OBS.

Capture the **Game** view (or full screen Play). Show: character walks from A to B once, clearly.

**Five bullets you can paste (edit names if you used another class):**

1. Copied KayKit assets into `Assets/Art/KayKit_Adventurers/` subfolders (`Models/Characters`, `Textures`, `Animations`, and `AnimatorControllers`).
2. Set the character to **Humanoid** rig and animation FBX files to **Humanoid** with **Copy From Other Avatar** (Knight), with **Loop Time** on idle/walk clips.
3. Built a small **Animator Controller** with **Idle** / **Walk** states and a float **Speed** parameter for transitions.
4. Placed two empty transforms **Waypoint_A** and **Waypoint_B**, attached **`WaypointMover`** to the character root, and assigned the Animator so **Speed** drives walk while moving.
5. Demo shows the character following a simple **A → B** path for lane-style prototyping (no NavMesh yet).

---

## 6) Adding a new character (without duplicating animation files)

You can reuse the same `Rig_Medium_*.fbx` animation files for multiple medium characters.

How it works:

- each character (`Knight`, `Mage`, `Ranger`, etc.) has its own Avatar from **Create From This Model**
- animation files use **Copy From Other Avatar** so Unity can retarget clips
- at runtime, each character's **Animator + Avatar** retargets the same clips to that character

Important clarification:

- you do **not** need separate animation FBX copies for each character prefab
- you only need one shared animation set, as long as rigs are compatible (Humanoid + same rig type)

Simple workflow for a new character:

1. Import the new character FBX and texture.
2. Set new character Rig = **Humanoid**, Avatar Definition = **Create From This Model**, Apply.
3. Make a prefab for that character.
4. Add/assign **Animator** on that prefab.
5. Use the same Animator Controller (`Knight_Locomotion` or a renamed shared one).
6. Keep using the same shared animation files.

Optional:

- if you want to preview animation import against the new character in the animation FBX import settings, temporarily change **Copy From Other Avatar** source to that new character avatar
- this does not mean you need new animation files; it is only a source-avatar setting for import/preview

---

## 7) What I contributed for this check-in

- contributor: Laura Wetherhold / FluffShady
- imported one KayKit character and texture into the Unity project
- set up Humanoid rig + avatar mapping for character and shared animation files
- built an Animator Controller with `Idle` / `Walk` states and `Speed` transitions
- created/used `WaypointMover` so the character moves from `Waypoint_A` to `Waypoint_B`
- wired Animator + WaypointMover together so movement drives animation
- documented the import/wiring workflow and troubleshooting notes in this file

Current prototype status:

- one character can walk A -> B with idle/walk animation switching
- setup is reusable for more medium-rig characters without duplicating animation files

---

## Repo reference

- Script: `Assets/Scripts/Movement/WaypointMover.cs`

## known limitations (prototype)

- movement is straight-line segments between points, not navmesh  
- no root motion; position is driven by the script  
- y follows waypoint height; keep waypoints on the floor