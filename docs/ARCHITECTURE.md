# HMDMQP-2025 System Architecture

A VR waypoint navigation application for Meta Quest built with Unity.

---

## Project Overview

**HMDMQP-2025** is a spatial VR application that allows users to create, manage, visualize, and navigate to waypoints in a 3D environment using Meta Quest hardware.

### Technology Stack
- **Engine**: Unity (URP v17.2)
- **XR Framework**: Meta XR SDK v78.0.0
- **Input**: Unity Input System + OVR Input
- **UI**: TextMesh Pro + Canvas-based UI

---

## System Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              INPUT LAYER                                    │
│                                                                             │
│   ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐        │
│   │  VRMenuToggle   │    │   OVR Input     │    │  Input Actions  │        │
│   │  (menu open)    │    │  (thumbstick/   │    │  (Unity Input   │        │
│   │                 │    │   trigger)      │    │   System)       │        │
│   └────────┬────────┘    └────────┬────────┘    └────────┬────────┘        │
└────────────┼──────────────────────┼──────────────────────┼─────────────────┘
             │                      │                      │
             ▼                      ▼                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                           UI / MENU LAYER                                   │
│                                                                             │
│   ┌─────────────────────────────────────────────────────────────────┐      │
│   │                   WaypointMenuController                         │      │
│   │   ┌──────────┐   ┌──────────┐   ┌──────────┐                    │      │
│   │   │  Create  │◄─►│  Delete  │◄─►│   Edit   │  ◄── Button Row    │      │
│   │   │  Button  │   │  Button  │   │  Button  │                    │      │
│   │   └──────────┘   └──────────┘   └──────────┘                    │      │
│   │         │                             │                          │      │
│   │         ▼                             ▼                          │      │
│   │   ┌─────────────────────────────────────────────────────┐       │      │
│   │   │           Waypoint List (paginated)                 │       │      │
│   │   │   ┌─────────────┐  ┌─────────────┐                  │       │      │
│   │   │   │WaypointList │  │WaypointList │  ...             │       │      │
│   │   │   │   Item 1    │  │   Item 2    │                  │       │      │
│   │   │   └─────────────┘  └─────────────┘                  │       │      │
│   │   └─────────────────────────────────────────────────────┘       │      │
│   └─────────────────────────────────────────────────────────────────┘      │
│                                                                             │
│   ┌────────────────────┐    ┌──────────────────────┐                       │
│   │ConfirmDialogCtrl   │    │CustomizationMenuCtrl │                       │
│   │(Yes/No dialogs)    │    │(color/shape editing) │                       │
│   └────────────────────┘    └──────────────────────┘                       │
└─────────────────────────────────────┬───────────────────────────────────────┘
                                      │
                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                        WAYPOINT CORE SYSTEM                                 │
│                                                                             │
│   ┌─────────────────────────────────────────────────────────────────┐      │
│   │                    WaypointManager (Singleton)                   │      │
│   │                                                                  │      │
│   │   • CreateWaypoint(position, name)                              │      │
│   │   • DeleteWaypoint(id)                                          │      │
│   │   • GetWaypoint(id)                                             │      │
│   │   • UpdateWaypoint(id, properties)                              │      │
│   │                                                                  │      │
│   │   ┌─────────────────────────────────────────┐                   │      │
│   │   │         List<Waypoint> waypoints        │                   │      │
│   │   └─────────────────────────────────────────┘                   │      │
│   └─────────────────────────────────────────────────────────────────┘      │
│                                      │                                      │
│                                      ▼                                      │
│   ┌─────────────────────────────────────────────────────────────────┐      │
│   │                      Waypoint (Data Model)                       │      │
│   │                                                                  │      │
│   │   • id: string (UUID)           • color: Color                  │      │
│   │   • name: string                • iconType: IconType            │      │
│   │   • description: string         • position: Vector3             │      │
│   │   • rotation: Quaternion                                        │      │
│   └─────────────────────────────────────────────────────────────────┘      │
└─────────────────────────────────────┬───────────────────────────────────────┘
                                      │
                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                       VISUALIZATION LAYER                                   │
│                                                                             │
│   ┌───────────────────┐  ┌───────────────────┐  ┌───────────────────┐      │
│   │   WaypointVisual  │  │WaypointLineConnect│  │ SelectionIndicator│      │
│   │                   │  │       or          │  │                   │      │
│   │ • 3D shape render │  │ • Line from camera│  │ • Rotation effect │      │
│   │ • Billboard effect│  │   to waypoint     │  │ • Pulse animation │      │
│   │ • Color updates   │  │ • Guide line viz  │  │                   │      │
│   │ • Selection ring  │  │                   │  │                   │      │
│   └───────────────────┘  └───────────────────┘  └───────────────────┘      │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────┐
│                       NAVIGATION AIDS LAYER                                 │
│                                                                             │
│   ┌─────────────────────────────┐    ┌─────────────────────────────┐       │
│   │      CompassManager         │    │     MinimapController       │       │
│   │                             │    │                             │       │
│   │  ┌───────────────────┐      │    │   ┌─────────────────┐       │       │
│   │  │    Compass UI     │      │    │   │    Minimap      │       │       │
│   │  │   ┌─────────┐     │      │    │   │      ┌─┐        │       │       │
│   │  │   │N  E  S  │     │      │    │   │    ◄─┼─┼──►     │       │       │
│   │  │   │    ▼    │     │      │    │   │      └─┘        │       │       │
│   │  │   └─────────┘     │      │    │   │  ● waypoints    │       │       │
│   │  └───────────────────┘      │    │   └─────────────────┘       │       │
│   │                             │    │                             │       │
│   │  • UV scroll by heading     │    │  • Bird's-eye view         │       │
│   │  • Waypoint marker on arc   │    │  • Player icon rotation    │       │
│   │                             │    │  • Dynamic icon creation   │       │
│   └─────────────────────────────┘    └─────────────────────────────┘       │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────┐
│                         VR / XR LAYER                                       │
│                                                                             │
│   ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐   │
│   │ OVRCameraRig │  │ControllerRay │  │EnablePassthru│  │ OVRCameraPopup│  │
│   │              │  │              │  │              │  │              │   │
│   │ VR camera    │  │ Pointer viz  │  │ AR passthru  │  │ UI follow    │   │
│   └──────────────┘  └──────────────┘  └──────────────┘  └──────────────┘   │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## Core Systems

### 1. Waypoint System
The central feature of the application - manages spatial markers in the VR world.

| Component | Responsibility |
|-----------|---------------|
| **Waypoint** | Data model: id, name, position, color, iconType |
| **WaypointManager** | Singleton managing waypoint lifecycle (CRUD) |
| **WaypointVisual** | 3D rendering with billboard effect |
| **WaypointLineConnector** | Guide line from camera to selected waypoint |

### 2. UI/Menu System
VR-optimized menu interface with thumbstick navigation.

| Component | Responsibility |
|-----------|---------------|
| **VRMenu** | Canvas that follows camera (2m in front) |
| **VRMenuToggle** | Input binding to show/hide menu |
| **WaypointMenuController** | Navigation state, Create/Delete/Edit actions |
| **WaypointListItem** | Individual list item with edit panel |

### 3. Compass System
Real-time orientation showing waypoint direction.

| Component | Responsibility |
|-----------|---------------|
| **CompassManager** | Updates compass based on player heading and waypoint bearing |

### 4. Minimap System
Bird's-eye view of player and waypoints.

| Component | Responsibility |
|-----------|---------------|
| **MinimapController** | Positions minimap, manages waypoint icons, calculates polar coordinates |

---

## Design Patterns

| Pattern | Usage |
|---------|-------|
| **Singleton** | WaypointManager, WaypointMenuController, CompassManager, MinimapController |
| **Model-View** | Waypoint (data) ↔ WaypointVisual (view) |
| **Factory** | WaypointManager creates waypoints and visuals |
| **DontDestroyOnLoad** | Managers persist across scenes |

---

## Folder Structure

```
Assets/
├── Scripts/           # 21 C# source files
├── Scenes/            # Main scene: HMDMQP.unity
├── Prefabs/           # 9 prefabs (waypoints, UI elements)
├── Materials/         # Color/shape materials
├── Sprites/           # UI graphics
├── Resources/         # Runtime-loaded assets
├── XR/                # XR configuration
├── Oculus/            # Meta SDK assets
└── TextMesh Pro/      # Font resources
```
