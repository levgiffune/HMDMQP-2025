1. Waypoints
Visual: Large header with a distinct color diamond shape.
Interaction: No direct add or edit functionality.
2. Intro Card (On Start)
Content: Controls overview + Tour/FreeRoam mode selection.
Navigation: Thumbstick up/down to toggle between Tour and Free Roam.
Confirm: Press 'A' to confirm selection and enter the chosen mode.
Note: B does nothing on IntroCard — A is the only way to proceed.
3. Tour Mode
Function: Tracks the current waypoint in tourOrder sequence.
Start Popup: "Let's start with {waypoint name}. Follow the red line." (Press 'B' to close).
Navigation: Red line + compass points to current waypoint.
Waypoint Proximity (e.g., at 2m):
3D Model (Meshy model).
Info Card with name and description.
Description Card (with image carousel and short description).
Action: Press 'A' to move on to the next waypoint.
Next Waypoint: Transition popup appears ("Next: {name}") — Press 'B' to close.
End Tour: Popup card ("Tour Complete!") — Press 'A' to switch to Free Roam mode.
4. Free Roam Mode
Menu:
Action: Trigger to open/close.
Content: List of available waypoints.
Navigation: Thumbstick up/down to highlight entries.
Selection: Press 'A' to select a waypoint.
On Select (persistent until new waypoint selected):
3D Model (Meshy model).
Info Card with name and description.
Description Card (with image carousel and short description).
Compass + line connector point to selected waypoint.
5. Compass
Function: Replaces the minimap.
Direction: Points to selected waypoint in Free Roam mode, or to current waypoint in Tour mode.
6. All Components
Menu
Compass
Waypoint Visual (Title/Color Diamond)
Description Card
3D Model (Model and Info Card)
Intro Card
Popup Card
UI Prefab Creation Guide
Step-by-step instructions for creating all 5 UI prefabs in Unity Editor.
TIP
All world-space canvases use Scale 0.001, 0.001, 0.001 so that pixel sizes map to millimeters. A 400×300 canvas ≈ 40cm × 30cm in world space.

1. PopupCard Prefab
Used for tour transitions ("Next waypoint…", "Tour Complete!").
Create the GameObject
Right-click in Hierarchy → UI → Canvas → rename to
PopupCard
Select
PopupCard, in Inspector:
Canvas → Render Mode: World Space
Rect Transform → Width: 500, Height: 300, Scale: 0.001, 0.001, 0.001
Remove Graphic Raycaster (no pointer interaction)
Add Component →
PopupCard (your script)
Build the hierarchy
PopupCard (Canvas + PopupCard script)
├── Background (UI → Image)
│   Color: dark semi-transparent (0, 0, 0, 0.85)
│   Rect: stretch to fill parent
├── Title (UI → Text - TextMeshPro)
│   Font Size: 36, Bold, White
│   Alignment: Center Top
│   Rect: top-anchored, Height ~60, left/right padding 20
├── Message (UI → Text - TextMeshPro)
│   Font Size: 24, White
│   Alignment: Center Middle
│   Rect: stretch middle area, padding 20 all sides
└── AButtonPrompt (empty GameObject)
   ├── AButtonText (UI → Text - TextMeshPro)
   │   Default text: "Press A to continue"
   │   Font Size: 20, Color: yellow
   │   Alignment: Center Bottom
   │   Rect: bottom-anchored, Height ~40
   └── (AButtonPrompt starts active — script toggles it)
Wire Inspector refs on
PopupCard component
Field
Drag from
titleText
Title TMP
messageText
Message TMP
aButtonPrompt
AButtonPrompt GameObject
aButtonText
AButtonText TMP

Save prefab
Drag PopupCard from Hierarchy into Assets/Prefabs/
Keep in scene (starts active — self-hides via canvas.enabled in Start())
Assign to TourManager.popupCard in Inspector

2. IntroCard Prefab
Shown once at app start. Controls overview + Tour/FreeRoam selection.
Create the GameObject
Right-click in Hierarchy → UI → Canvas → rename to IntroCard
Inspector:
Canvas → Render Mode: World Space
Rect Transform → Width: 600, Height: 450, Scale: 0.001, 0.001, 0.001
Remove Graphic Raycaster
Add Component → IntroCard (your script)
Build the hierarchy
IntroCard (Canvas + IntroCard script)
├── Background (UI → Image)
│   Color: (0, 0, 0, 0.9)
│   Rect: stretch to fill
├── ControlsText (UI → Text - TextMeshPro)
│   Text: "Welcome!\n\nControls:\n• Trigger — Open menu\n• Thumbstick — Navigate\n• A — Select"
│   Font Size: 22, White
│   Alignment: Top Left
│   Rect: top half of card, padding 30
├── Divider (UI → Image)
│   Color: white, Height: 2
│   Rect: horizontal line at vertical center
├── TourOption (empty GameObject)
│   ├── TourLabel (UI → Text - TextMeshPro)
│   │   Text: "▶  Guided Tour"
│   │   Font Size: 28, White
│   │   Rect: left-aligned, below divider
│   └── (no extra children needed)
└── FreeRoamOption (empty GameObject)
   ├── FreeRoamLabel (UI → Text - TextMeshPro)
   │   Text: "▶  Free Roam"
   │   Font Size: 28, White
   │   Rect: left-aligned, below TourOption
   └── (no extra children needed)
Wire Inspector refs on IntroCard component
Field
Drag from
controlsText
ControlsText TMP
tourOption
TourOption GameObject
freeRoamOption
FreeRoamOption GameObject
tourLabel
TourLabel TMP
freeRoamLabel
FreeRoamLabel TMP

Save prefab
Drag into Assets/Prefabs/
Keep in scene — GameModeManager needs a reference to it

3. DescriptionCard Prefab
Shows at waypoint proximity: image carousel + description text.
Create the GameObject
Right-click in Hierarchy → UI → Canvas → rename to DescriptionCard
Inspector:
Canvas → Render Mode: World Space
Rect Transform → Width: 450, Height: 350, Scale: 0.001, 0.001, 0.001
Remove Graphic Raycaster
Add Component → DescriptionCard (your script)
Build the hierarchy
DescriptionCard (Canvas + DescriptionCard script)
├── Background (UI → Image)
│   Color: (0.1, 0.1, 0.1, 0.9)
│   Rect: stretch to fill
├── CarouselImage (UI → Raw Image)
│   Rect: top portion, Height ~200, padding 10
│   Leave texture blank (set at runtime)
├── PrevButton (UI → Image, optional arrow "‹")
│   Rect: left side of carousel, small 30×30
├── NextButton (UI → Image, optional arrow "›")
│   Rect: right side of carousel, small 30×30
├── DescriptionText (UI → Text - TextMeshPro)
│   Font Size: 20, White
│   Alignment: Top Left
│   Rect: below carousel, padding 15
└── AdvancePrompt (UI → Text - TextMeshPro)
   Text: "Press A to move on →"
   Font Size: 18, Color: yellow
   Alignment: Center Bottom
   Rect: bottom-anchored, Height ~30
   Starts INACTIVE (script toggles in Tour mode)
Wire Inspector refs on DescriptionCard component
Field
Drag from
arouselImage
CarouselImage RawImage
descriptionText
DescriptionText TMP
advancePrompt
AdvancePrompt TMP
nextButton
NextButton GameObject
prevButton
PrevButton GameObject

Save prefab
Drag into Assets/Prefabs/
This will be a child of the Waypoint prefab, placed by WaypointVisual

4. InfoCard Prefab
Small name+description panel near the 3D model.
Create the GameObject
Right-click in Hierarchy → UI → Canvas → rename to InfoCard
Inspector:
Canvas → Render Mode: World Space
Rect Transform → Width: 300, Height: 120, Scale: 0.001, 0.001, 0.001
Remove Graphic Raycaster
Add Component → InfoCard (your script)
Build the hierarchy
InfoCard (Canvas + InfoCard script)
├── Background (UI → Image)
│   Color: (0, 0, 0, 0.8)
│   Rect: stretch to fill, rounded corners if using sprite
├── NameText (UI → Text - TextMeshPro)
│   Font Size: 26, Bold, White
│   Alignment: Center Top
│   Rect: top half, padding 10
└── DescriptionText (UI → Text - TextMeshPro)
   Font Size: 16, Light gray
   Alignment: Center Top
   Overflow: Ellipsis
   Rect: bottom half, padding 10
Wire Inspector refs on InfoCard component
Field
Drag from
nameText
NameText TMP
descriptionText
DescriptionText TMP

Save prefab
Drag into Assets/Prefabs/
This will be a child of the Waypoint prefab, near the WaypointPreviewOrb

5. FreeRoamListItem Prefab
Single row in the Free Roam waypoint list.
Create the GameObject
Right-click in Hierarchy → UI → Create Empty → rename to FreeRoamListItem
Add Rect Transform: Width: 400, Height: 50
Add Component → FreeRoamListEntry (your script)
Build the hierarchy
FreeRoamListItem (RectTransform + FreeRoamListEntry script)
├── HighlightImage (UI → Image)
│   Color: (1, 1, 1, 0.15) — subtle white overlay
│   Rect: stretch to fill parent
│   Starts INACTIVE
├── ActiveImage (UI → Image)
│   Color: (0.2, 0.8, 0.2, 0.3) — green tint
│   Rect: stretch to fill parent
│   Starts INACTIVE
└── NameText (UI → Text - TextMeshPro)
   Font Size: 22, White
   Alignment: Middle Left
   Rect: stretch, left padding 15
Wire Inspector refs on FreeRoamListEntry component
Field
Drag from
nameText
NameText TMP
highlightImage
HighlightImage Image
activeImage
ActiveImage Image

Save prefab
Drag into Assets/Prefabs/
Assign to FreeRoamMenu.listItemPrefab in Inspector

Where Each Prefab Lives in the Scene
Scene Root
├── [XR Rig / OVR Camera Rig]
├── GameModeManager          ← empty GO, add GameModeManager script
├── IntroCard                ← instance in scene, starts active
├── TourManager              ← empty GO, starts INACTIVE
├── PopupCard                ← instance in scene, starts active (self-hides)
├── Menu (existing)          ← VRMenu + VRMenuToggle, starts INACTIVE
│   └── FreeRoamMenu         ← child of Menu
├── WaypointManager (existing)
├── WaypointLineConnector (existing)
├── Compass (existing)
└── Directional Light (existing)
IMPORTANT
After placing all scene objects, wire Inspector refs:

GameModeManager:
introCard → IntroCard in scene
tourManager → TourManager in scene
freeRoamMenu → FreeRoamMenu under Menu
vrMenu → Menu's VRMenu component

TourManager:
popupCard → PopupCard in scene
lineConnector → WaypointLineConnector
compass → Compass

VRMenuToggle (on Menu):
vrMenu → Menu's VRMenu component
toggleMenuAction → mapped to trigger (use started phase, not performed)
