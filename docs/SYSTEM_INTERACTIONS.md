# System Interactions

How the major systems communicate and depend on each other.

---

## System Dependency Graph

```
                           ┌─────────────────────────────────────┐
                           │           USER INPUT                │
                           │  (OVR Thumbstick, Trigger, Buttons) │
                           └──────────────────┬──────────────────┘
                                              │
                                              ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                                                                             │
│                         WAYPOINT MENU CONTROLLER                            │
│                              (Central Hub)                                  │
│                                                                             │
└───────┬──────────────────┬──────────────────┬──────────────────┬───────────┘
        │                  │                  │                  │
        │ creates/         │ updates          │ sets             │ sets
        │ deletes          │ selection        │ target           │ target
        │                  │                  │                  │
        ▼                  ▼                  ▼                  ▼
┌───────────────┐  ┌───────────────┐  ┌───────────────┐  ┌───────────────┐
│   WAYPOINT    │  │   WAYPOINT    │  │    COMPASS    │  │     LINE      │
│   MANAGER     │  │    VISUAL     │  │    MANAGER    │  │   CONNECTOR   │
│               │  │               │  │               │  │               │
│ • CRUD ops    │  │ • 3D render   │  │ • Direction   │  │ • Guide line  │
│ • Data store  │  │ • Billboard   │  │ • Bearing     │  │ • Visual aid  │
│ • Visuals     │  │ • Selection   │  │               │  │               │
└───────┬───────┘  └───────────────┘  └───────────────┘  └───────────────┘
        │
        │ provides waypoint list
        │
        ▼
┌───────────────┐
│    MINIMAP    │
│  CONTROLLER   │
│               │
│ • Icons       │
│ • Positions   │
│ • Distances   │
└───────────────┘
```

---

## Inter-System Communication

### WaypointMenuController → WaypointManager

```
┌─────────────────────────┐                    ┌─────────────────────────┐
│ WaypointMenuController  │                    │     WaypointManager     │
│                         │                    │                         │
│  CreateWaypoint() ──────┼───────────────────►│ CreateWaypoint(pos,name)│
│                         │                    │   → returns Waypoint    │
│                         │                    │                         │
│  DeleteWaypoint() ──────┼───────────────────►│ DeleteWaypoint(id)      │
│                         │                    │   → removes from list   │
│                         │                    │                         │
│  GetWaypoint() ─────────┼───────────────────►│ GetWaypoint(id)         │
│                         │                    │   → returns Waypoint    │
│                         │                    │                         │
│  UpdateWaypoint() ──────┼───────────────────►│ Update properties       │
│                         │                    │   → modifies waypoint   │
└─────────────────────────┘                    └─────────────────────────┘
```

### WaypointManager → WaypointVisual

```
┌─────────────────────────┐                    ┌─────────────────────────┐
│     WaypointManager     │                    │     WaypointVisual      │
│                         │                    │                         │
│  CreateVisual() ────────┼───────────────────►│ Instantiate prefab      │
│                         │                    │ Initialize(waypoint)    │
│                         │                    │                         │
│  DestroyVisual() ───────┼───────────────────►│ Destroy gameObject      │
│                         │                    │                         │
│  GetVisual() ───────────┼───────────────────►│ Returns visual ref      │
└─────────────────────────┘                    └─────────────────────────┘
```

### WaypointMenuController → Navigation Aids

```
┌─────────────────────────┐
│ WaypointMenuController  │
│                         │
│  On waypoint selected:  │
│                         │
│  ┌───────────────────┐  │         ┌─────────────────────────┐
│  │                   ├──┼────────►│     CompassManager      │
│  │ CompassManager    │  │         │                         │
│  │ .Waypoint = obj   │  │         │ Updates compass marker  │
│  │                   │  │         │ to show bearing         │
│  └───────────────────┘  │         └─────────────────────────┘
│                         │
│  ┌───────────────────┐  │         ┌─────────────────────────┐
│  │                   ├──┼────────►│ WaypointLineConnector   │
│  │ LineConnector     │  │         │                         │
│  │ .SetTarget(wp)    │  │         │ Draws line from camera  │
│  │                   │  │         │ to selected waypoint    │
│  └───────────────────┘  │         └─────────────────────────┘
│                         │
└─────────────────────────┘
```

### MinimapController → WaypointManager

```
┌─────────────────────────┐                    ┌─────────────────────────┐
│   MinimapController     │                    │     WaypointManager     │
│                         │                    │                         │
│  Every LateUpdate():    │                    │                         │
│                         │    reads           │                         │
│  WaypointManager ───────┼───────────────────►│ .Waypoints (List)       │
│    .Waypoints           │                    │                         │
│                         │                    │ Returns all waypoints   │
│  For each waypoint:     │                    │ for icon positioning    │
│    → Create/update icon │                    │                         │
│    → Calculate position │                    │                         │
│                         │                    │                         │
└─────────────────────────┘                    └─────────────────────────┘
```

---

## Event Flow Timeline

```
TIME ──────────────────────────────────────────────────────────────────────►

User opens menu
    │
    ▼
┌─────────────┐
│VRMenuToggle │──► VRMenu.Show()
└─────────────┘
    │
    ▼
User navigates to Create button
    │
    ▼
┌──────────────────────────┐
│WaypointMenuController    │──► Update button highlight
│  selectedIndex = -1      │
└──────────────────────────┘
    │
    ▼
User presses trigger
    │
    ▼
┌──────────────────────────┐     ┌──────────────────────────┐
│WaypointMenuController    │────►│WaypointManager           │
│  ConfirmSelection()      │     │  CreateWaypoint()        │
└──────────────────────────┘     └────────────┬─────────────┘
                                              │
    ┌─────────────────────────────────────────┼─────────────────────────────┐
    │                                         │                             │
    ▼                                         ▼                             ▼
┌──────────────────┐              ┌──────────────────┐           ┌──────────────────┐
│ Add to waypoints │              │ CreateVisual()   │           │ Add to menu list │
│ list             │              │                  │           │                  │
└──────────────────┘              └────────┬─────────┘           └──────────────────┘
                                           │
                                           ▼
                              ┌──────────────────────────┐
                              │ WaypointVisual           │
                              │   Initialize()           │
                              │   → Set position         │
                              │   → Set appearance       │
                              └──────────────────────────┘
    │
    ▼
Next frame: MinimapController.LateUpdate()
    │
    ▼
┌──────────────────────────┐
│ New waypoint detected    │
│ → Create minimap icon    │
│ → Position on minimap    │
└──────────────────────────┘
```

---

## Singleton Access Pattern

All major systems use singletons for cross-system communication:

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                            SINGLETON ACCESS                                 │
└─────────────────────────────────────────────────────────────────────────────┘

    Any Script                              Singleton Instance
    ─────────                               ──────────────────

    WaypointManager.Instance ──────────────► WaypointManager
                                             • waypoints list
                                             • CreateWaypoint()
                                             • DeleteWaypoint()

    WaypointMenuController.Instance ───────► WaypointMenuController
                                             • selectedIndex
                                             • isEditing
                                             • SelectWaypoint()

    CompassManager.Instance ───────────────► CompassManager
                                             • Waypoint target
                                             • compass texture

    MinimapController.Instance ────────────► MinimapController
                                             • waypoint icons
                                             • player position

    WaypointLineConnector.Instance ────────► WaypointLineConnector
                                             • target waypoint
                                             • line renderer
```

---

## VR Input Mapping

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                          VR CONTROLLER INPUT                                │
└─────────────────────────────────────────────────────────────────────────────┘

    LEFT CONTROLLER                         RIGHT CONTROLLER
    ───────────────                         ────────────────

         ┌───┐                                   ┌───┐
         │ Y │                                   │ B │ ◄── Cancel edit
         └───┘                                   └───┘
    ┌─────────────┐                         ┌─────────────┐
    │  Thumbstick │ ◄── Toggle popup        │  Thumbstick │ ◄── Navigate menu
    └─────────────┘                         └─────────────┘
         ┌───┐                                   ┌───┐
         │ X │                                   │ A │
         └───┘                                   └───┘

    ╭─────────────╮                         ╭─────────────╮
    │   Trigger   │                         │   Trigger   │ ◄── Confirm/Select
    ╰─────────────╯                         ╰─────────────╯

    ╭─────────────╮                         ╭─────────────╮
    │    Grip     │                         │    Grip     │
    ╰─────────────╯                         ╰─────────────╯


    INPUT ACTION                            RESPONSE
    ────────────                            ────────

    Right Thumbstick Up        ──►          Navigate up in menu
    Right Thumbstick Down      ──►          Navigate down in menu
    Right Thumbstick Left      ──►          Navigate left / Cycle color
    Right Thumbstick Right     ──►          Navigate right / Cycle shape
    Right Index Trigger        ──►          Confirm selection / Edit
    Right B Button             ──►          Cancel edit mode
    Left Thumbstick Press      ──►          Toggle popup visibility
    Menu Toggle Action         ──►          Show/Hide VR menu
```

---

## Data Persistence

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        CURRENT DATA PERSISTENCE                             │
└─────────────────────────────────────────────────────────────────────────────┘

    ┌──────────────────┐
    │  WaypointManager │
    │                  │
    │  List<Waypoint>  │ ◄── In-memory only (not persisted to disk)
    │  waypoints       │
    │                  │
    └──────────────────┘

    ┌──────────────────┐
    │WaypointListBuilder│
    │                  │
    │  Pre-defined     │ ◄── Inspector-assigned waypoints loaded at start
    │  waypoints from  │
    │  inspector       │
    │                  │
    └──────────────────┘


    NOTE: Waypoints are stored in memory during runtime.
          Data is lost when the application closes.

          Future enhancement: Add serialization to JSON/PlayerPrefs
          for persistent storage across sessions.
```
