# In-World Widgets: Concrete Implementation

Implementation details for four waypoint-attached in-world widgets: **Expandable Info Card**, **Media Billboard**, **3D Preview Orb**, and **Floating Label**. These integrate with the existing Waypoint / WaypointVisual / WaypointManager pipeline.

---

## Data Model Changes

Extend the Waypoint and (where needed) WaypointVisual data so widgets have content to display.

### Waypoint (`Assets/Scripts/Waypoint.cs`)

| Field           | Type                                                              | Purpose                                                                              |
| --------------- | ----------------------------------------------------------------- | ------------------------------------------------------------------------------------ |
| _(existing)_    | `id`, `name`, `desc`, `position`, `rotation`, `color`, `iconType` | No change.                                                                           |
| `imageRef`      | `Sprite` or `Texture2D` (optional)                                | Optional image for info card and/or media billboard.                                 |
| `videoClip`     | `VideoClip` (optional)                                            | Optional Unity VideoClip for media billboard.                                        |
| `previewPrefab` | `GameObject` (optional)                                           | Optional prefab to instantiate as 3D preview orb (e.g. robot part, equipment model). |

- Add nullable/optional refs so existing waypoints and creation flows remain valid without media.
- If using `Sprite`: assign in Inspector or via `WaypointListBuilder`; no URL loading in v1.
- If using `VideoClip`: assign in Inspector; keep video files in `Assets` (e.g. `Assets/StreamingAssets` or `Resources`).

### WaypointVisual (prefab / script references)

- Existing: `labelText`, `descriptionPanel`, `waypointNameText`, `descriptionText`, `selectionIndicator`, `shapeContainer`, `shapePrefabs`.
- New references (see below): **info card image**, **media display root**, **3D preview orb container**, **floating label container** (if separated from marker).

---

## Widget 1: Expandable Info Card

**Goal:** A 3D panel near the waypoint that shows title, description, and an optional image when the waypoint is selected (or within a distance threshold). Reuse and extend the existing description panel in `WaypointVisual`.

### Behavior

- **Show:** When the waypoint is selected (current behavior) and/or when the player is within a configurable distance (e.g. `showInfoDistance = 2.5f`).
- **Hide:** When the waypoint is deselected and (if using proximity) when the player moves beyond the distance.
- **Content:** Waypoint `name`, `desc`, and optional `imageRef` (thumbnail or small image on the card).

### Implementation

**1. Waypoint model**

- Add optional `imageRef` (e.g. `Sprite` or `Texture2D`) to `Waypoint` as above.

**2. WaypointVisual**

- Keep using `descriptionPanel` (GameObject), `waypointNameText`, `descriptionText`.
- Add an optional **Image** (Unity UI Image) child inside the description panel for the waypoint image.
  - In the prefab: add a `RawImage` or `Image` (e.g. below the title, above or beside the description).
  - In `WaypointVisual`: add a field `public RawImage infoCardImage;` (or `Image` if using Sprite).
- In `GenerateDescriptionPanel(Waypoint waypoint)` (or a new `RefreshInfoCard(Waypoint waypoint)`):
  - Set `waypointNameText.text = waypoint.name` and `descriptionText.text = waypoint.desc` (use `desc` to match existing code).
  - If `waypoint.imageRef != null` and `infoCardImage != null`, assign the texture/sprite and enable the image; otherwise disable or hide the image element.
- **Show/hide logic:** Keep `SetSelected(bool selected)` toggling `descriptionPanel.SetActive(selected)`. Optionally add proximity-based show:
  - In `Update()` or a dedicated update: get player position from `cameraTransform.position`, compute `waypointData.DistanceFrom(playerPosition)`. If distance &lt; `showInfoDistance`, set panel active; if using “show when selected OR close”, only hide when !selected and distance &gt; threshold. Avoid overlapping with menu selection (e.g. selected always shows; when not selected, proximity can still show).

**3. Optional: scale-in animation**

- On enable of the description panel, run a short scale-from-zero or fade-in (e.g. `CanvasGroup.alpha` or `transform.localScale`) via a simple coroutine or `DOTween` if available. Keeps behavior optional so the rest of the implementation stays minimal.

**4. Prefab hierarchy (conceptual)**

```
WaypointVisual (root)
├── ... existing (shapeContainer, selectionIndicator, labelText)
└── descriptionPanel (Canvas or panel GameObject)
    ├── waypointNameText (TMP)
    ├── infoCardImage (RawImage/Image)   ← NEW
    └── descriptionText (TMP)
```

### Integration

- `WaypointMenuController` already calls `SetSelected(true/false)` on the waypoint visual when the user selects/deselects in the menu; no change required for selection-driven show/hide.
- When waypoint data is updated (e.g. after edit), call existing `WaypointManager.UpdateWaypointVisual(waypointId)`; extend `UpdateAppearance` or add `RefreshInfoCard` so the info card text and image are refreshed from the current `Waypoint` data.

---

## Widget 2: Media Billboard (Image / Video)

**Goal:** An in-world quad (or panel) that shows an image or plays a video when the waypoint is selected or the user is within range. Separate from the small info card so large media has its own widget.

### Behavior

- **Show:** When the waypoint is selected, or when the player is within a configurable distance (e.g. `showMediaDistance = 3f`).
- **Hide:** When deselected and beyond distance.
- **Content:** If `Waypoint.videoClip` is set, play video on a quad; else if `Waypoint.imageRef` (or a dedicated “billboard image”) is set, show texture/sprite on a quad. If neither is set, the media billboard stays hidden.

### Implementation

**1. Waypoint model**

- Add optional `videoClip` (Unity `VideoClip`) and optional `imageRef` (or reuse same `imageRef` as info card) to `Waypoint`.

**2. New component: WaypointMediaDisplay**

- **Location:** `Assets/Scripts/WaypointMediaDisplay.cs`.
- **Responsibilities:**
  - Hold references: a quad (or panel) with `MeshRenderer` + material for image, and a `UnityEngine.Video.VideoPlayer` (+ `RenderTexture` + `Renderer.material`) for video.
  - Initialize from a `Waypoint`: if `videoClip != null`, set up `VideoPlayer` and render to a quad; else if image ref exists, set quad material texture/sprite.
  - Show/hide the display root based on “is selected” and/or “player within range” (same pattern as info card: get camera position, compare with waypoint position).
  - For video: `Play()` when shown, `Pause()` or `Stop()` when hidden; optionally loop.
- **References (inspector):**
  - `Transform displayRoot` (or `GameObject`) — the quad or panel to show/hide.
  - `MeshRenderer imageQuad` (for image mode).
  - `VideoPlayer videoPlayer` (for video mode).
  - `RenderTexture videoRenderTarget` (video output).
  - Optional: `float showMediaDistance`, `bool showOnlyWhenSelected`.

**3. WaypointVisual**

- Add a field: `public WaypointMediaDisplay mediaDisplay;` (optional).
- In `Initialize(Waypoint waypoint, ...)`: if `mediaDisplay != null`, call e.g. `mediaDisplay.Initialize(waypoint, playerCameraTransform)`.
- In `SetSelected(bool selected)`: if `mediaDisplay != null`, call e.g. `mediaDisplay.SetVisible(selected)` so selection-driven visibility is consistent. WaypointMediaDisplay can also implement proximity logic internally.

**4. WaypointManager / CreateVisual**

- No change required if `WaypointMediaDisplay` lives on the same waypoint prefab as `WaypointVisual` and is initialized from `WaypointVisual.Initialize`. Ensure the waypoint prefab includes the media quad and `WaypointMediaDisplay` component.

**5. Video setup (Unity)**

- Create a `RenderTexture` (e.g. 1920×1080) and assign it to `VideoPlayer.targetTexture`.
- Quad material: use an Unlit or URP shader that samples the RenderTexture so the video is visible in world space.
- VideoPlayer: set source to `VideoClip`, loop on/off as desired, play on show and pause on hide.

**6. Prefab hierarchy (conceptual)**

```
WaypointVisual (root)
├── ... existing
├── descriptionPanel (Widget 1)
└── mediaDisplayRoot (new)
    ├── Quad (MeshRenderer + material for image/video)
    └── WaypointMediaDisplay (component; references Quad, VideoPlayer, RenderTexture)
```

### Integration

- Selection: `WaypointMenuController` → `SetSelected` on `WaypointVisual` → `WaypointMediaDisplay.SetVisible(true/false)`.
- When waypoint is updated (e.g. new media assigned), re-initialize or refresh `WaypointMediaDisplay` from the updated `Waypoint` (e.g. from `UpdateWaypointVisual` or from a new `WaypointMediaDisplay.Refresh(Waypoint w)` called after edit).

---

## Widget 3: 3D Preview Orb

**Goal:** A small in-world 3D model (e.g. robot arm, equipment, or product) that rotates near the waypoint when selected or when the player is within range. Gives visitors a quick “this is what it looks like” preview without opening the full info card.

### Behavior

- **Show:** When the waypoint is selected, or when the player is within a configurable distance (e.g. `showPreviewDistance = 3f`).
- **Hide:** When deselected and beyond distance.
- **Content:** If `Waypoint.previewPrefab` is set, instantiate it (or a reference is assigned in prefab) and display; otherwise the orb stays hidden.
- **Animation:** Optional slow rotation around the vertical axis (e.g. `transform.Rotate(0, rotationSpeed * Time.deltaTime, 0)`) so the model is viewable from all sides. Scale and position are configurable so the orb reads as a compact preview, not full-size.

### Implementation

**1. Waypoint model**

- Add optional `previewPrefab` (`GameObject`) to `Waypoint`. Assign in Inspector or via `WaypointListBuilder`; prefab should be a lightweight mesh (or LOD) suitable for in-world preview. No runtime loading in v1.

**2. New component: WaypointPreviewOrb**

- **Location:** `Assets/Scripts/WaypointPreviewOrb.cs`.
- **Responsibilities:**
  - Hold references: a container `Transform` (or `GameObject`) as the parent for the instantiated preview, and optional rotation/scale settings.
  - Initialize from a `Waypoint`: if `previewPrefab != null`, instantiate under the container (or assign a pre-placed model reference); set local scale (e.g. `previewScale = 0.3f`) and local position so the orb sits beside or above the marker.
  - Show/hide the container based on “is selected” and/or “player within range” (same pattern as info card and media: camera position vs waypoint position).
  - In `Update()` when visible: rotate the container (or the instantiated model’s root) around the up axis at a configurable speed (e.g. `rotationSpeedDegreesPerSecond = 20f`).
- **References (inspector):**
  - `Transform orbRoot` (or `GameObject`) — container to show/hide and to parent the instantiated prefab.
  - Optional: `float showPreviewDistance`, `bool showOnlyWhenSelected`, `float previewScale`, `float rotationSpeedDegreesPerSecond`.

**3. WaypointVisual**

- Add a field: `public WaypointPreviewOrb previewOrb;` (optional).
- In `Initialize(Waypoint waypoint, ...)`: if `previewOrb != null`, call e.g. `previewOrb.Initialize(waypoint, playerCameraTransform)`.
- In `SetSelected(bool selected)`: if `previewOrb != null`, call e.g. `previewOrb.SetVisible(selected)` so selection-driven visibility is consistent. WaypointPreviewOrb can also implement proximity logic internally.

**4. WaypointManager / CreateVisual**

- No change required if `WaypointPreviewOrb` lives on the same waypoint prefab as `WaypointVisual` and is initialized from `WaypointVisual.Initialize`. Ensure the waypoint prefab includes the orb container and `WaypointPreviewOrb` component. When waypoint data is updated (e.g. preview prefab changed), re-initialize or refresh the orb (e.g. destroy previous instance and instantiate new prefab from updated `Waypoint`).

**5. Prefab / instantiation**

- Preview prefabs should be self-contained (meshes + materials). Keep poly count modest for performance when multiple waypoints show orbs. If the waypoint prefab uses a pre-placed model instead of runtime instantiation, `WaypointPreviewOrb` can optionally support an “assign reference in prefab” path and only use `previewPrefab` when a new prefab is set at edit time.

**6. Prefab hierarchy (conceptual)**

```
WaypointVisual (root)
├── ... existing
├── descriptionPanel (Widget 1)
├── mediaDisplayRoot (Widget 2)
└── previewOrbRoot (new)
    ├── (instantiated preview prefab or pre-placed model)
    └── WaypointPreviewOrb (component; references orbRoot, optional scale/rotation)
```

### Integration

- Selection: `WaypointMenuController` → `SetSelected` on `WaypointVisual` → `WaypointPreviewOrb.SetVisible(true/false)`.
- When waypoint is updated (e.g. preview prefab assigned or changed), re-initialize or refresh `WaypointPreviewOrb` from the updated `Waypoint` (e.g. from `UpdateWaypointVisual` or `WaypointPreviewOrb.Refresh(Waypoint w)`).

---

## Widget 4: Floating Label (Title Bar)

**Goal:** A short, always-on label (waypoint name) floating above or beside the marker, with optional distance-based visibility/LOD so it doesn’t clutter the view at long range.

### Behavior

- **Content:** Waypoint `name`; optionally a small category icon (if you add category to the model later).
- **Placement:** Offset above the waypoint (e.g. `labelOffset = (0, 0.3f, 0)` in local space) so it floats above the 3D marker. Keep billboard behavior so the label faces the camera (WaypointVisual already billboards the whole transform; if the label is a child, it will follow).
- **Visibility (LOD):** Always on, or distance-based: show label when distance &lt; `labelVisibleDistance` (e.g. 5 m); hide when beyond. Optionally fade alpha by distance.

### Implementation

**1. Waypoint model**

- No new fields required for name-only label. Optional: add `category` or `iconType` (already exists) for a small icon next to the label later.

**2. WaypointVisual (existing labelText)**

- The project already has `public TextMeshPro labelText` and sets `labelText.text = waypoint.name` in `Initialize`. Use this as the floating label.
- **Position:** Ensure the label’s transform is offset above the waypoint (e.g. in the prefab, place the label child at local position `(0, 0.3f, 0)` or similar). If it’s currently at origin, adjust the prefab so the label floats above the marker.
- **Billboard:** The root `WaypointVisual` does `LookAt(cameraTransform)` in `Update()`, so a child label will follow. If the label should always face the camera independently (e.g. label is in world space elsewhere), add a small script that makes only the label transform look at camera; otherwise current setup is enough.
- **Distance LOD (optional):** In `WaypointVisual.Update()`:
  - Compute `distance = waypointData.DistanceFrom(cameraTransform.position)`.
  - If `labelVisibleDistance > 0`: set `labelText.gameObject.SetActive(distance < labelVisibleDistance)`.
  - Optional: set `labelText.alpha = Mathf.Lerp(0f, 1f, 1f - (distance - minDist) / (labelVisibleDistance - minDist))` for a simple fade.
- Add inspector fields: `float labelVisibleDistance` (0 = always on), `Vector3 labelOffset` (if you set position from code; otherwise use prefab placement).

**3. Optional: category icon**

- If you add a `category` (enum or string) to `Waypoint` later, add a small sprite or icon next to the label (e.g. a child `Image` or SpriteRenderer) and set its sprite by category. For this document, the floating label is name-only unless you extend the model.

**4. Prefab hierarchy (conceptual)**

```
WaypointVisual (root, billboards to camera)
├── shapeContainer
├── selectionIndicator
├── labelText (TMP, child offset e.g. (0, 0.3f, 0))   ← Floating label
├── descriptionPanel (Widget 1)
├── mediaDisplayRoot (Widget 2)
└── previewOrbRoot (Widget 3)
```

### Integration

- No change in `WaypointMenuController` or `WaypointManager` for the label itself; `Initialize` already sets the text. When waypoint name is edited, `UpdateWaypointVisual` → `UpdateAppearance`; extend so that the label text is refreshed (e.g. in `UpdateAppearance` set `labelText.text = waypointData.name` again, or ensure `waypointData` is updated and a single place sets `labelText.text` from it).

---

## Summary Table

| Widget                  | Data (Waypoint)                           | Component / Prefab                                           | Trigger                              |
| ----------------------- | ----------------------------------------- | ------------------------------------------------------------ | ------------------------------------ |
| 1. Expandable Info Card | `name`, `desc`, optional `imageRef`       | WaypointVisual + descriptionPanel + optional RawImage/Image  | Selected and/or proximity            |
| 2. Media Billboard      | optional `videoClip`, optional `imageRef` | WaypointMediaDisplay (new) + quad + VideoPlayer              | Selected and/or proximity            |
| 3. 3D Preview Orb       | optional `previewPrefab`                  | WaypointPreviewOrb (new) + orb root + instantiated prefab    | Selected and/or proximity            |
| 4. Floating Label       | `name` (existing)                         | WaypointVisual.labelText (existing), position + optional LOD | Always on or distance &lt; threshold |

### Suggested implementation order

1. **Widget 4 (Floating label)** — Prefab tweak + optional LOD in `WaypointVisual`; no new script.
2. **Widget 1 (Info card)** — Add `imageRef` to Waypoint, add image UI to description panel, optional proximity in `WaypointVisual`.
3. **Widget 2 (Media billboard)** — Add `videoClip`/image to Waypoint, new `WaypointMediaDisplay` script, quad + VideoPlayer in prefab, hook into `SetSelected` and optional proximity.
4. **Widget 3 (3D Preview Orb)** — Add `previewPrefab` to Waypoint, new `WaypointPreviewOrb` script, orb root in prefab, instantiate prefab and rotate when visible; hook into `SetSelected` and optional proximity.

---

## Files to Create or Modify

| Action   | File                                                                                                                                                                      |
| -------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Modify   | `Assets/Scripts/Waypoint.cs` — add optional `imageRef`, `videoClip`, `previewPrefab`.                                                                                     |
| Modify   | `Assets/Scripts/WaypointVisual.cs` — info card image, optional proximity for panel; floating label offset/LOD; references to WaypointMediaDisplay and WaypointPreviewOrb. |
| Create   | `Assets/Scripts/WaypointMediaDisplay.cs` — show/hide quad, image/video display, optional proximity.                                                                       |
| Create   | `Assets/Scripts/WaypointPreviewOrb.cs` — show/hide orb root, instantiate preview prefab, optional rotation and proximity.                                                 |
| Modify   | Waypoint prefab — add info card image UI, media display root + quad + VideoPlayer, preview orb root, position label.                                                      |
| Optional | `WaypointManager.UpdateWaypointVisual` or WaypointVisual — refresh media display and preview orb when waypoint data changes.                                              |

This keeps the existing architecture (WaypointManager, WaypointMenuController, selection flow) intact and adds the four widgets in a way that matches the current codebase (e.g. `desc`, `WaypointVisual.Initialize`, `SetSelected`).
