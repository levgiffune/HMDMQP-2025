# Data Flow Diagrams

Visual representation of how data flows through the HMDMQP-2025 systems.

---

## Waypoint Creation Flow

```
┌──────────────────────────────────────────────────────────────────────────┐
│                        USER CREATES A WAYPOINT                           │
└──────────────────────────────────────────────────────────────────────────┘

         User Input                    Menu Logic
    ┌─────────────────┐           ┌─────────────────┐
    │  Navigate to    │           │ selectedIndex   │
    │  Create Button  │──────────►│    = -1         │
    │  (thumbstick)   │           │ (Create btn)    │
    └─────────────────┘           └────────┬────────┘
                                           │
    ┌─────────────────┐                    │
    │  Press Trigger  │──────────────────► │
    │  (confirm)      │                    ▼
    └─────────────────┘           ┌─────────────────┐
                                  │ConfirmSelection()│
                                  │                 │
                                  │ if index == -1  │
                                  │ CreateWaypoint()│
                                  └────────┬────────┘
                                           │
                                           ▼
                        ┌─────────────────────────────────┐
                        │        WaypointManager          │
                        │                                 │
                        │  spawnPos = camera.forward * 2  │
                        │                                 │
                        │  new Waypoint(                  │
                        │      id: UUID,                  │
                        │      position: spawnPos,        │
                        │      name: "Waypoint N"         │
                        │  )                              │
                        └────────────────┬────────────────┘
                                         │
              ┌──────────────────────────┼──────────────────────┐
              │                          │                      │
              ▼                          ▼                      ▼
    ┌─────────────────┐       ┌─────────────────┐    ┌─────────────────┐
    │  Add to List    │       │ CreateVisual()  │    │ Update Menu     │
    │                 │       │                 │    │                 │
    │ waypoints.Add() │       │ Instantiate     │    │AddWaypointTo    │
    │                 │       │ waypointPrefab  │    │   List()        │
    └─────────────────┘       └────────┬────────┘    └─────────────────┘
                                       │
                                       ▼
                            ┌─────────────────────┐
                            │   WaypointVisual    │
                            │                     │
                            │  Initialize()       │
                            │  • Set position     │
                            │  • Set color        │
                            │  • Set shape        │
                            └─────────────────────┘
```

---

## Waypoint Selection Flow

```
┌──────────────────────────────────────────────────────────────────────────┐
│                        USER SELECTS A WAYPOINT                           │
└──────────────────────────────────────────────────────────────────────────┘

    User navigates to waypoint in list
                │
                ▼
    ┌───────────────────────┐
    │  Press Trigger        │
    │  on Waypoint Item     │
    └───────────┬───────────┘
                │
                ▼
    ┌───────────────────────────────────────────────────────────────┐
    │                 WaypointMenuController                        │
    │                                                               │
    │   SelectWaypoint(waypointId)                                  │
    │     │                                                         │
    │     ├──► Get Waypoint from WaypointManager                    │
    │     │                                                         │
    │     ├──► Get WaypointVisual                                   │
    │     │                                                         │
    │     └──► Update UI state                                      │
    └───────────┬───────────────────────────────────────────────────┘
                │
    ┌───────────┼───────────────────────────────────────────┐
    │           │           │             │                 │
    ▼           ▼           ▼             ▼                 ▼
┌─────────┐ ┌─────────┐ ┌─────────┐ ┌─────────────┐ ┌─────────────┐
│Waypoint │ │Waypoint │ │Compass  │ │ Line        │ │ UI List     │
│Visual   │ │ListItem │ │Manager  │ │ Connector   │ │ Highlight   │
│         │ │         │ │         │ │             │ │             │
│SetSelect│ │ Show    │ │.Waypoint│ │.SetTarget() │ │ Show        │
│ed(true) │ │ Active  │ │ = obj   │ │             │ │ Border      │
│         │ │ Border  │ │         │ │             │ │             │
│• Show   │ │         │ │ Updates │ │ Draws line  │ │             │
│  ring   │ │         │ │ compass │ │ from camera │ │             │
│• Show   │ │         │ │ marker  │ │ to waypoint │ │             │
│  info   │ │         │ │ position│ │             │ │             │
│  card   │ │         │ │         │ │             │ │             │
│• Toggle │ │         │ │         │ │             │ │             │
│  media  │ │         │ │         │ │             │ │             │
│• Toggle │ │         │ │         │ │             │ │             │
│  preview│ │         │ │         │ │             │ │             │
└─────────┘ └─────────┘ └─────────┘ └─────────────┘ └─────────────┘
```

---

## Edit Mode Flow

```
┌──────────────────────────────────────────────────────────────────────────┐
│                        USER EDITS A WAYPOINT                             │
└──────────────────────────────────────────────────────────────────────────┘

    ┌─────────────────────┐
    │  Select a waypoint  │
    │  Press trigger again│
    └──────────┬──────────┘
               │
               ▼
    ┌─────────────────────────────────────┐
    │         EnterEditMode()             │
    │                                     │
    │  • Cache original color             │
    │  • Cache original iconType          │
    │  • Show edit panel on list item     │
    │  • isEditing = true                 │
    └──────────┬──────────────────────────┘
               │
               ▼
    ┌─────────────────────────────────────────────────────────────────────┐
    │                         EDIT PREVIEW LOOP                           │
    │                                                                     │
    │   Thumbstick Left/Right ──► Cycle color index ──► Preview color    │
    │                                                                     │
    │   Thumbstick Up/Down ────► Cycle shape index ──► Preview shape     │
    │                                                                     │
    │   ┌─────────────┐                         ┌─────────────────────┐  │
    │   │  Preview    │                         │   WaypointVisual    │  │
    │   │  Changes    │ ───────────────────────►│   UpdateAppearance  │  │
    │   │  (realtime) │                         │   (temporary)       │  │
    │   └─────────────┘                         └─────────────────────┘  │
    │                                                                     │
    └───────────────────────────────┬─────────────────────────────────────┘
                                    │
              ┌─────────────────────┴─────────────────────┐
              │                                           │
              ▼                                           ▼
    ┌─────────────────────┐                    ┌─────────────────────┐
    │  Press Trigger      │                    │  Press B Button     │
    │  (Confirm)          │                    │  (Cancel)           │
    └──────────┬──────────┘                    └──────────┬──────────┘
               │                                          │
               ▼                                          ▼
    ┌─────────────────────┐                    ┌─────────────────────┐
    │   ConfirmEdit()     │                    │   CancelEdit()      │
    │                     │                    │                     │
    │ • Update Waypoint   │                    │ • Restore original  │
    │   color & iconType  │                    │   color & iconType  │
    │ • Save to manager   │                    │ • Reset visual      │
    │ • Update visual     │                    │                     │
    │ • isEditing = false │                    │ • isEditing = false │
    └─────────────────────┘                    └─────────────────────┘
```

---

## Minimap Update Cycle

```
┌──────────────────────────────────────────────────────────────────────────┐
│                     MINIMAP UPDATE (Every Frame)                         │
└──────────────────────────────────────────────────────────────────────────┘

    ┌─────────────────────────────────────────────────────────────────────┐
    │                    MinimapController.LateUpdate()                   │
    └───────────────────────────────┬─────────────────────────────────────┘
                                    │
              ┌─────────────────────┼─────────────────────────────────┐
              │                     │                                 │
              ▼                     ▼                                 ▼
    ┌─────────────────┐   ┌─────────────────────┐         ┌─────────────────┐
    │PositionMinimap()│   │UpdateWaypointIcons()│         │CleanupDeleted() │
    │                 │   │                     │         │                 │
    │ offset from     │   │ For each waypoint:  │         │ Remove icons    │
    │ camera position │   │                     │         │ for deleted     │
    │                 │   │                     │         │ waypoints       │
    └─────────────────┘   └──────────┬──────────┘         └─────────────────┘
                                     │
                                     ▼
                    ┌───────────────────────────────────┐
                    │     For Each Waypoint             │
                    │                                   │
                    │  ┌─────────────────────────────┐  │
                    │  │ Calculate position:         │  │
                    │  │                             │  │
                    │  │ deltaX = wp.x - player.x    │  │
                    │  │ deltaZ = wp.z - player.z    │  │
                    │  │                             │  │
                    │  │ distance = sqrt(dX² + dZ²)  │  │
                    │  │ angle = atan2(dX, dZ)       │  │
                    │  │                             │  │
                    │  │ clampedDist = min(dist,     │  │
                    │  │               worldRadius)  │  │
                    │  │                             │  │
                    │  │ minimapDist = (clampedDist  │  │
                    │  │   / worldRadius) * mapRadius│  │
                    │  │                             │  │
                    │  │ x = sin(angle) * mapDist    │  │
                    │  │ y = cos(angle) * mapDist    │  │
                    │  └─────────────────────────────┘  │
                    │                                   │
                    │  Update icon RectTransform        │
                    │  localPosition = (x, y, 0)        │
                    │                                   │
                    └───────────────────────────────────┘
```

---

## Compass Update Cycle

```
┌──────────────────────────────────────────────────────────────────────────┐
│                     COMPASS UPDATE (Every Frame)                         │
└──────────────────────────────────────────────────────────────────────────┘

    ┌─────────────────────────────────────────────────────────────────────┐
    │                    CompassManager.Update()                          │
    └───────────────────────────────┬─────────────────────────────────────┘
                                    │
              ┌─────────────────────┴─────────────────────┐
              │                                           │
              ▼                                           ▼
    ┌─────────────────────────┐               ┌─────────────────────────┐
    │  Update Compass Scroll  │               │ Update Waypoint Marker  │
    │                         │               │                         │
    │  playerRotY =           │               │ if (targetWaypoint)     │
    │    camera.eulerAngles.y │               │                         │
    │                         │               │   dirToWaypoint =       │
    │  uvOffset =             │               │     wp.pos - player.pos │
    │    playerRotY / 360     │               │                         │
    │                         │               │   angleToWP =           │
    │  compassTexture.uvRect  │               │     SignedAngle(        │
    │    .x = uvOffset        │               │       forward,          │
    │                         │               │       dirToWaypoint)    │
    │  ┌───────────────────┐  │               │                         │
    │  │  N   E   S   W    │  │               │   markerX =             │
    │  │  ─────────────    │  │               │     (angleToWP / 180)   │
    │  │     scrolls ──►   │  │               │     * arcWidth          │
    │  └───────────────────┘  │               │                         │
    │                         │               │   marker.localPos.x     │
    └─────────────────────────┘               │     = markerX           │
                                              │                         │
                                              │  ┌─────────────────┐    │
                                              │  │    N   ▼   S    │    │
                                              │  │  ◄─────────────►│    │
                                              │  │     marker      │    │
                                              │  └─────────────────┘    │
                                              └─────────────────────────┘
```

---

## Input Navigation State Machine

```
┌──────────────────────────────────────────────────────────────────────────┐
│                    MENU NAVIGATION STATE MACHINE                         │
└──────────────────────────────────────────────────────────────────────────┘

                    Thumbstick Up
                         │
                         ▼
    ┌──────────────────────────────────────────────────────────────────┐
    │                                                                  │
    │   ┌─────────┐       ┌─────────┐       ┌─────────┐               │
    │   │ CREATE  │◄─────►│ DELETE  │◄─────►│  EDIT   │   Buttons    │
    │   │ idx=-1  │       │ idx=-2  │       │ idx=-3  │   (Row 1)    │
    │   └────┬────┘       └─────────┘       └────┬────┘               │
    │        │    Thumbstick Left/Right          │                    │
    │        │                                   │                    │
    │        └───────────────┬───────────────────┘                    │
    │                        │                                        │
    │              Thumbstick Down                                    │
    │                        │                                        │
    │                        ▼                                        │
    │   ┌────────────────────────────────────────────────────────┐   │
    │   │              WAYPOINT LIST (paginated)                 │   │
    │   │                                                        │   │
    │   │    ┌──────────────┐                                    │   │
    │   │    │  Waypoint 1  │  idx=0                             │   │
    │   │    └──────────────┘                                    │   │
    │   │           │  Thumbstick Down                           │   │
    │   │           ▼                                            │   │
    │   │    ┌──────────────┐                                    │   │
    │   │    │  Waypoint 2  │  idx=1                             │   │
    │   │    └──────────────┘                                    │   │
    │   │           │  Thumbstick Down                           │   │
    │   │           ▼                                            │   │
    │   │    ┌──────────────┐                                    │   │
    │   │    │  Waypoint 3  │  idx=2                             │   │
    │   │    └──────────────┘                                    │   │
    │   │           │  Thumbstick Down                           │   │
    │   │           ▼                                            │   │
    │   │    ┌──────────────┐                                    │   │
    │   │    │  Waypoint 4  │  idx=3                             │   │
    │   │    └──────────────┘                                    │   │
    │   │           │                                            │   │
    │   │           │  At bottom of page?                        │   │
    │   │           │  ──► Next Page (pagination)                │   │
    │   │                                                        │   │
    │   └────────────────────────────────────────────────────────┘   │
    │                                                                  │
    └──────────────────────────────────────────────────────────────────┘

    ACTIONS:
    ┌─────────────────────────────────────────────────────────────────┐
    │                                                                 │
    │   Index Trigger on CREATE  ──►  CreateWaypoint()               │
    │   Index Trigger on DELETE  ──►  DeleteSelectedWaypoint()       │
    │   Index Trigger on EDIT    ──►  EnterEditMode()                │
    │   Index Trigger on LIST    ──►  SelectWaypoint() / EditMode    │
    │   B Button in Edit Mode    ──►  CancelEdit()                   │
    │                                                                 │
    └─────────────────────────────────────────────────────────────────┘
```
