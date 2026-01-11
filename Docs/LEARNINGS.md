# Development Learnings & Hard-Won Solutions

This document captures critical learnings and solutions to difficult problems encountered during development. **Read this file when stuck on a problem** - the solution may already be documented here.

---

## 1. CompareTag Causes Freeze When Tag Doesn't Exist

**Problem:** Game freezes when using `CompareTag("SomeTag")` in collision handlers.

**Root Cause:** Unity's `CompareTag()` method can cause freezes/hangs when the tag doesn't exist in the project's Tag Manager. Unlike accessing `.tag` property directly, `CompareTag` has different internal behavior that causes issues with non-existent tags.

**Solution:** Use string comparison instead of `CompareTag`:

```csharp
// BAD - Freezes if "Wall" tag doesn't exist in project
if (other.CompareTag("Wall"))

// GOOD - Works even if tag doesn't exist
string tag = other.tag;
if (tag == "Wall")
```

**Files affected:** Any script using `CompareTag()` in `OnTriggerEnter2D`, `OnCollisionEnter2D`, etc.

---

## 2. Time.deltaTime Division Causes Physics Corruption

**Problem:** Game freezes with NaN/Infinity values corrupting physics.

**Root Cause:** Dividing by `Time.deltaTime` when `Time.timeScale = 0` produces Infinity/NaN values that corrupt Rigidbody2D velocities.

**Solution:** Guard all divisions by Time.deltaTime:

```csharp
// BAD - Division by zero when timeScale = 0
velocity = distance / Time.deltaTime;

// GOOD - Guard against zero
if (Time.deltaTime > 0.0001f)
{
    velocity = distance / Time.deltaTime;
}
```

---

## 3. Coroutines Freeze When Using Time.deltaTime with timeScale = 0

**Problem:** Coroutines with while loops hang forever when `Time.timeScale = 0`.

**Root Cause:** `Time.deltaTime` returns 0 when timeScale is 0, so `elapsed += Time.deltaTime` never increases.

**Solution:** Use unscaled time:

```csharp
// BAD - Infinite loop when timeScale = 0
while (elapsed < duration)
{
    elapsed += Time.deltaTime;
    yield return null;
}

// GOOD - Works regardless of timeScale
while (elapsed < duration)
{
    elapsed += Time.unscaledDeltaTime;
    yield return null;
}

// Also use WaitForSecondsRealtime instead of WaitForSeconds
yield return new WaitForSecondsRealtime(1f);
```

---

## 4. Runtime Texture Creation Causes Performance Issues

**Problem:** Game stutters or freezes when creating textures at runtime.

**Root Cause:** Creating `new Texture2D()` and calling `tex.Apply()` is expensive. Doing this every frame or for every spawned object causes severe performance issues.

**Solution:** Cache textures/sprites statically:

```csharp
// BAD - Creates new texture for each projectile
void CreateVisual()
{
    var tex = new Texture2D(16, 16);
    // ... fill pixels ...
    tex.Apply();
    sprite = Sprite.Create(tex, ...);
}

// GOOD - Create once, reuse forever
private static Sprite _cachedSprite;

Sprite GetCachedSprite()
{
    if (_cachedSprite == null)
    {
        var tex = new Texture2D(16, 16);
        // ... fill pixels ...
        tex.Apply();
        _cachedSprite = Sprite.Create(tex, ...);
    }
    return _cachedSprite;
}
```

---

## 5. Isolating Freeze Causes - Systematic Debugging

**Problem:** Game freezes but unclear what causes it.

**Method:** Binary search through code by disabling/enabling components:

1. Disable entire feature → Does it freeze?
   - No → Problem is in that feature
   - Yes → Problem is elsewhere

2. Within the feature, disable half the code → Does it freeze?
   - Repeat until isolated to specific line/method

3. For component-based issues:
   - Create empty GameObject → Freeze?
   - Add Component A → Freeze?
   - Add Component B → Freeze?
   - Continue until found

**Example from this project:**
```
Empty GameObject → No freeze
+ Rigidbody2D → No freeze
+ CircleCollider2D → No freeze
+ UltraSimpleProjectile → FREEZE!
  - Empty component → No freeze
  - With OnTriggerEnter2D (just Destroy) → No freeze
  - With CompareTag → FREEZE! ← Found it!
```

---

## 6. LayerMask.NameToLayer Returns -1 for Missing Layers

**Problem:** Setting `gameObject.layer` to an invalid value causes issues.

**Solution:** Always validate layer exists:

```csharp
int layer = LayerMask.NameToLayer("MyLayer");
if (layer >= 0 && layer < 32)
{
    gameObject.layer = layer;
}
else
{
    gameObject.layer = 0; // Default layer
}
```

---

## 7. Validate Physics Values Before Applying

**Problem:** NaN or Infinity values in Rigidbody2D.velocity corrupt physics simulation.

**Solution:** Always validate before setting:

```csharp
Vector2 newVelocity = direction * speed;

if (float.IsNaN(newVelocity.x) || float.IsNaN(newVelocity.y) ||
    float.IsInfinity(newVelocity.x) || float.IsInfinity(newVelocity.y))
{
    newVelocity = Vector2.zero;
}

// Also clamp to reasonable values
float maxSpeed = 20f;
if (newVelocity.sqrMagnitude > maxSpeed * maxSpeed)
{
    newVelocity = newVelocity.normalized * maxSpeed;
}

rb.velocity = newVelocity;
```

---

## 8. Normalized Zero Vector Produces NaN

**Problem:** Normalizing a zero-length vector produces NaN.

**Solution:** Check magnitude before normalizing:

```csharp
// BAD - NaN if positions are identical
Vector2 direction = (target - source).normalized;

// GOOD - Fallback for zero vector
Vector2 direction = (target - source);
if (direction.sqrMagnitude < 0.01f)
{
    direction = Vector2.right; // Default direction
}
else
{
    direction = direction.normalized;
}
```

---

## 9. Windows Reserved Filenames Cause Unity Import Loops

**Problem:** Unity gets stuck in infinite import loop, console shows errors about importing files named `nul`, `con`, `com1`, `aux`, `prn`, etc.

**Root Cause:** Windows has reserved device names that cannot be used as filenames. If a file with these names somehow gets created (often from cross-platform projects or bad asset imports), Unity will fail to import them repeatedly.

**Reserved Windows filenames:**
- `nul`, `con`, `prn`, `aux`
- `com1` through `com9`
- `lpt1` through `lpt9`

**Solution:** Delete using Python with extended path syntax:

```python
import os

# Extended path syntax bypasses Windows reserved name restrictions
file_path = r"C:\path\to\project\Assets\nul"
extended_path = "\\\\?\\" + file_path

if os.path.exists(extended_path):
    os.remove(extended_path)
    print(f"Deleted: {extended_path}")
```

**Alternative:** Use command prompt with `del \\?\C:\path\to\nul`

---

## 10. Unity Caches Scene Files Aggressively

**Problem:** After editing scene files externally (or via scripts), Unity still shows old content. Camera settings, GameObjects, etc. don't update.

**Root Cause:** Unity keeps scenes loaded in memory and doesn't automatically detect external changes to .unity files.

**Symptoms:**
- Camera background color doesn't change despite editing scene file
- GameObjects missing from hierarchy despite being in scene file
- Old settings persist after file modifications

**Solution:** Force Unity to reload from disk:

```
Method 1: Reopen Scene
- File > Open Scene > Select the scene
- Or double-click the scene in Project window
- If asked to save, click "Don't Save" to discard cached version

Method 2: Reimport
- Right-click scene in Project window
- Select "Reimport"

Method 3: Nuclear Option
- Close Unity completely
- Delete Library/SceneCache folder (if exists)
- Reopen project
```

**Prevention:** When making bulk external edits, close Unity first.

---

## 11. Incomplete .meta Files Break Asset Import

**Problem:** Sprites/textures show as generic icons in Unity, don't import as sprites.

**Root Cause:** The .meta file only has the header (fileFormatVersion and guid) but missing the TextureImporter section.

**Symptoms:**
- PNG files show as generic file icons, not sprite previews
- Resources.Load returns null for sprites
- Console may show import warnings

**Solution:** Ensure .meta files have complete TextureImporter:

```yaml
fileFormatVersion: 2
guid: <unique-guid-here>
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {}
  serializedVersion: 12
  mipmaps:
    mipMapMode: 0
    enableMipMap: 0
    sRGBTexture: 1
    # ... full settings ...
  textureType: 8          # 8 = Sprite
  spriteMode: 1           # 1 = Single
  spritePixelsToUnits: 256
  # ... platform settings ...
```

**Key settings for sprites:**
- `textureType: 8` (Sprite type)
- `spriteMode: 1` (Single sprite)
- `isReadable: 1` (if you need to read pixels)
- `textureCompression: 0` (none) or `1` (low quality)

---

## 12. EditorBuildSettings Scene GUIDs Must Match

**Problem:** Scenes don't load, or wrong scenes load. Build settings show scenes but they don't work.

**Root Cause:** `ProjectSettings/EditorBuildSettings.asset` contains GUIDs for each scene. If these don't match the actual scene .meta file GUIDs, Unity can't find the scenes.

**Solution:** Get correct GUIDs from scene .meta files:

```bash
# Check scene GUID
cat Assets/Scenes/MyScene.unity.meta | grep guid
# Output: guid: 3d3b5006b5f83d34f8addb96c3214a0b
```

Then update EditorBuildSettings.asset:
```yaml
m_Scenes:
- enabled: 1
  path: Assets/Scenes/MyScene.unity
  guid: 3d3b5006b5f83d34f8addb96c3214a0b  # Must match .meta file!
```

---

## Quick Reference: Common Freeze Causes

| Symptom | Likely Cause | Solution |
|---------|--------------|----------|
| Freeze on collision | `CompareTag` with missing tag | Use `tag ==` instead |
| Freeze when timeScale=0 | `Time.deltaTime` in loops | Use `Time.unscaledDeltaTime` |
| Freeze on spawn | Runtime texture creation | Cache textures/sprites |
| Freeze with physics | NaN/Infinity in velocity | Validate before applying |
| Freeze on division | Division by zero | Guard with `if (x > 0)` |

## Quick Reference: Unity Import Issues

| Symptom | Likely Cause | Solution |
|---------|--------------|----------|
| Infinite import loop | Windows reserved filename | Delete with `\\?\` path |
| Scene not updating | Unity cache | Reopen scene from disk |
| Sprites show as icons | Incomplete .meta file | Add full TextureImporter |
| Scenes won't load | Wrong GUIDs in BuildSettings | Match GUIDs from .meta |
| Blue screen in Game view | No camera rendering | Open correct scene, press Play |

---

**Last Updated:** 2026-01-11

---

## 13. Duplicate Visual Systems Cause "Fake Doors"

**Problem:** All rooms appear to have 4 doors, but some are "fake" - they don't transition to other rooms when entered.

**Root Cause:** Two separate visual systems were creating room visuals:
1. `RoomVisuals` component (attached to each Room) - creates floor, walls, doors
2. `RoomVisualizer` singleton - ALSO created floor, walls, doors

Both systems checked `roomData.northDoor?.exists == true` before creating doors, but having duplicate visuals caused confusion and visual glitches.

**Solution:** Separate responsibilities:
- `RoomVisuals` handles floor, walls, and door gaps (attached to Room)
- `RoomVisualizer` ONLY handles decorations (torches, stairs, furniture)

```csharp
// In RoomVisualizer.CreateRoomVisuals() - DON'T create floor/walls
public void CreateRoomVisuals(RoomData roomData)
{
    // NOTE: Floor and Walls are created by RoomVisuals component.
    // RoomVisualizer only handles decorations to avoid duplicate visuals.

    // Do NOT call CreateFloor() or CreateWalls() here!
    CreateDecorations(roomData);  // Only decorations
}
```

**Files affected:**
- `RoomVisualizer.cs` - Removed CreateFloor/CreateWalls calls
- `RoomVisuals.cs` - Handles all floor/wall/door visuals

---

## 14. Dark Fallback Colors Cause Floor Darkness

**Problem:** Basement floor appears completely dark even with good ambient lighting.

**Root Cause:** When Midjourney sprite files fail to load, the fallback procedural colors are very dark:
```csharp
// OLD - Too dark!
Color floorColor = floorLevel switch
{
    0 => new Color(0.25f, 0.22f, 0.2f),  // Basement - almost black
    // ...
};
```

Combined with vignette overlay, this makes the basement unplayable.

**Solution:** Use much brighter fallback colors:
```csharp
// NEW - Playable brightness
Color floorColor = floorLevel switch
{
    0 => new Color(0.55f, 0.48f, 0.52f),  // Basement - visible purple
    1 => new Color(0.55f, 0.48f, 0.42f),  // Castle - warm
    2 => new Color(0.6f, 0.58f, 0.55f),   // Tower - bright
    _ => new Color(0.5f, 0.45f, 0.4f)
};
```

Also reduce vignette intensity for basement:
```csharp
basementAtmosphere = new FloorAtmosphere
{
    vignetteIntensity = 0.15f,  // Very minimal for visibility
    // ...
};
```

**Files affected:**
- `PlaceholderSpriteGenerator.cs` - Brighter fallback floor colors
- `AtmosphereManager.cs` - Reduced basement vignette intensity

---

## 15. All Sprite Generators Need Bright Basement Colors

**Problem:** Basement remains dark despite fixing fallback colors in PlaceholderSpriteGenerator, because multiple procedural sprite generators are called in a chain and ALL have dark basement colors.

**Root Cause:** The sprite loading fallback chain tries multiple generators:
1. Midjourney sprites (may not load)
2. HDSmoothSpriteGenerator
3. UltraHDSpriteGenerator
4. PhotorealisticSpriteGenerator
5. EnhancedSpriteGenerator
6. PlaceholderSpriteGenerator fallback

Each generator had its own dark basement colors. If Midjourney sprites fail, ANY of these could be used, and they all had colors like (0.22, 0.2, 0.18) for basement floors - almost black.

**Solution:** Brighten ALL sprite generators' basement colors to ~0.45-0.55 range:

```csharp
// Example brightened basement colors (floor 0)
case 0: // Basement - BRIGHTENED for visibility
    baseColor = new Color(0.48f, 0.42f, 0.45f);
    lightColor = new Color(0.62f, 0.55f, 0.58f);
    darkColor = new Color(0.38f, 0.32f, 0.35f);
    groutColor = new Color(0.28f, 0.24f, 0.26f);
    break;
```

**Files affected (ALL brightened for basement/floor 0):**
- `HDSmoothSpriteGenerator.cs` - GetStoneFloorTile colors
- `UltraHDSpriteGenerator.cs` - GetFloorTileSprite and GetWallSprite for case 0
- `PhotorealisticSpriteGenerator.cs` - GetFloorTileSprite and GetWallSprite for case 0
- `EnhancedSpriteGenerator.cs` - Palettes.BasementStone, BasementStoneDark, BasementStoneLight
- `PlaceholderSpriteGenerator.cs` - Fallback floor colors

---

## 16. Door Visual Creation Was Empty

**Problem:** Doors disappeared from rooms after disabling duplicate visual systems.

**Root Cause:** `RoomVisuals.CreateDoorway()` method was empty/commented out. It was originally supposed to create door sprites in wall gaps, but the implementation was missing. After disabling `RoomVisualizer`'s floor/wall/door creation (to fix fake doors), no system was creating door visuals.

**Solution:** Implement actual door sprite creation in RoomVisuals.CreateDoorway():

```csharp
private void CreateDoorway(string name, Vector2 position, bool isVertical, Color color)
{
    var doorObj = new GameObject(name);
    doorObj.transform.SetParent(transform);
    doorObj.transform.localPosition = position;

    var sr = doorObj.AddComponent<SpriteRenderer>();
    Sprite doorSprite = PlaceholderSpriteGenerator.GetDoorSprite(false, "");
    sr.sprite = doorSprite;
    sr.sortingLayerName = "Walls";
    sr.sortingOrder = 2;
    sr.drawMode = SpriteDrawMode.Simple;

    // Scale door to fit gap
    Vector2 doorSize = isVertical
        ? new Vector2(wallThickness * 1.5f, doorWidth)
        : new Vector2(doorWidth, wallThickness * 1.5f);

    if (doorSprite != null)
    {
        float scaleX = doorSize.x / doorSprite.bounds.size.x;
        float scaleY = doorSize.y / doorSprite.bounds.size.y;
        doorObj.transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }
}
```

**Files affected:**
- `RoomVisuals.cs` - CreateDoorway implementation

---

## 17. TextMeshPro AddComponent at Runtime Triggers TMP Importer Freeze

**Problem:** Game freezes when enemy projectile hits player. TMP Importer dialog appears and blocks the game.

**Root Cause:** Using `AddComponent<TextMeshPro>()` at runtime triggers Unity's TextMeshPro Importer window. This window is modal and blocks all game input until manually closed. Unlike other component types, TMP has special import requirements.

**Symptoms:**
- Game freezes immediately on first enemy hit
- "TMP Importer" window appears in Unity Editor
- Cannot interact with game until window is closed
- Happens in both Editor and builds (in builds, just freezes with no dialog)

**Affected code patterns:**
```csharp
// BAD - Triggers TMP Importer dialog and freezes game
var textObj = new GameObject("DamageNumber");
var tmp = textObj.AddComponent<TextMeshPro>();  // FREEZE!
tmp.text = "10";
```

**Solution:** Replace TextMeshPro with SpriteRenderer-based visuals:

```csharp
// GOOD - No TMP dependency, no freeze
var indicatorObj = new GameObject("DamageIndicator");
var sr = indicatorObj.AddComponent<SpriteRenderer>();
sr.sprite = GetCachedDamageSprite();  // Use cached sprite
sr.color = Color.red;
sr.sortingLayerName = "UI";
sr.sortingOrder = 1000;

// Cache sprites to avoid runtime texture creation
private static Sprite _cachedDamageSprite;
private Sprite GetCachedDamageSprite()
{
    if (_cachedDamageSprite != null) return _cachedDamageSprite;

    int size = 16;
    var tex = new Texture2D(size, size);
    // Create simple shape...
    tex.Apply();
    _cachedDamageSprite = Sprite.Create(tex, ...);
    return _cachedDamageSprite;
}
```

**Alternative Solutions:**
1. Pre-create TMP components in prefabs (not at runtime)
2. Use Unity's legacy Text component (also not recommended for runtime creation)
3. Use object pooling with pre-instantiated TMP objects

**Files affected:**
- `CombatFeedbackManager.cs` - ShowComboCounter, ComboCounterRoutine
- `VisualEffectsManager.cs` - ShowDamageNumber, ShowHealNumber, ShowTextPopup

---

## 18. Floor Numbering in TestRoomSetup Must Match RoomDatabase Convention

**Problem:** Starting room showed RED test marker (floor 0 = basement) instead of GREEN (floor 1 = castle).

**Root Cause:** TestRoomSetup.cs was creating the starting room "room_center" with `floorNumber = 0`, but the game convention expects:
- Floor 0 = Basement (RED test marker) - dungeons, dark, dangerous
- Floor 1 = Castle (GREEN test marker) - starting area
- Floor 2 = Tower (BLUE test marker) - upper floors

**Solution:** Ensure floor numbers match the convention:

```csharp
// Starting room should be floor 1 (Castle), not floor 0 (Basement)
var centerRoom = CreateRoomData("room_center", "Grand Hall", 1, true, false);
                                                           // ↑ This was 0, should be 1
```

Also ensure stairs lead to correct floor rooms:
```csharp
// Stairs UP from floor 1 → floor 2 (Tower)
centerRoom.stairsUp = new FloorTransition {
    destinationRoomId = "room_f2_center",  // floor 2 room
    position = new Vector2(5f, 2f)  // Right side
};

// Stairs DOWN from floor 1 → floor 0 (Basement)
centerRoom.stairsDown = new FloorTransition {
    destinationRoomId = "room_dungeon_1",  // floor 0 room
    position = new Vector2(-5f, 2f)  // Left side (separate from stairs up!)
};
```

**Files affected:**
- `TestRoomSetup.cs` - All castle rooms changed from floor 0 to floor 1

---

## 19. Invalid Layer Assignment Can Cause Black Screen on Room Transition

**Problem:** Room transitions result in completely black screen, even though logs show room was built successfully.

**Root Cause:** During room building, code like `gameObject.layer = LayerMask.NameToLayer("Hazards")` returns -1 when the layer doesn't exist. Assigning -1 to `gameObject.layer` causes Unity error: "A game object can only be in one layer. The layer needs to be in the range [0...31]"

This error can disrupt the room building process in subtle ways, even if it doesn't throw an exception that stops the build entirely.

**Solution:** Always validate layer exists before assignment:

```csharp
// BAD - Layer might not exist
gameObject.layer = LayerMask.NameToLayer("Hazards");  // Returns -1 if missing!

// GOOD - Validate with fallback
int layer = LayerMask.NameToLayer("Hazards");
if (layer >= 0 && layer <= 31)
{
    gameObject.layer = layer;
}
else
{
    gameObject.layer = 0;  // Default layer
    Debug.LogWarning($"Layer 'Hazards' not found, using Default layer");
}
```

**Also helpful:** Add a safety fade check after room transitions:
```csharp
// After room transition, verify fade cleared
StartCoroutine(DelayedFadeCheck());

private IEnumerator DelayedFadeCheck()
{
    yield return new WaitForSecondsRealtime(1.0f);
    if (TransitionManager.Instance.CurrentFadeAlpha > 0.05f)
    {
        Debug.LogError("Fade stuck! Force clearing...");
        TransitionManager.Instance.ForceTransparent();
    }
}
```

**Files affected:**
- `Hazard.cs` - Layer assignment validation added
- `RoomManager.cs` - DelayedFadeCheck safety coroutine added

---

## 20. Reading Unity Editor Log Directly for Hidden Errors

**Problem:** Some Unity errors don't appear prominently in the Console window, or get lost in a flood of messages.

**Solution:** Read the Unity Editor log file directly:

```
Location: C:\Users\[Username]\AppData\Local\Unity\Editor\Editor.log
```

**Useful search patterns:**
```bash
# Find all errors
grep -i "error" Editor.log | tail -100

# Find specific error messages
grep "layer" Editor.log
grep "range 0 to 31" Editor.log

# Find stack traces for specific scripts
grep "Hazard.cs" Editor.log
```

**Key insight:** The Editor.log contains EVERYTHING - including errors that might be swallowed or not displayed prominently. When debugging mysterious issues, always check this file.

---

## 21. Add Safety Checks for Critical Game State

**Problem:** Edge cases and interrupted coroutines can leave the game in bad states (e.g., fade overlay stuck at black).

**Solution:** Add delayed safety checks that verify and correct state:

```csharp
// After any critical state change, add a delayed verification
StartCoroutine(DelayedStateCheck());

private IEnumerator DelayedStateCheck()
{
    // Wait for normal completion
    yield return new WaitForSecondsRealtime(1.0f);

    // Verify state is correct, fix if not
    if (TransitionManager.Instance.CurrentFadeAlpha > 0.05f)
    {
        Debug.LogError("State corrupted! Forcing correction...");
        TransitionManager.Instance.ForceTransparent();
    }
}
```

**Best practices:**
1. Use `WaitForSecondsRealtime` (not `WaitForSeconds`) to work even when timeScale=0
2. Log when safety checks trigger - indicates a bug to investigate
3. Add these checks for any state that, if stuck, would break the game

---

## 22. Systematic Logging for Complex Flows

**Problem:** When a multi-step process fails, it's hard to know which step caused the issue.

**Solution:** Add tagged logging at every step of the process:

```csharp
// Use consistent tags for filtering
Debug.LogWarning("[STAIRS DEBUG] Step 1: Trigger entered");
Debug.LogWarning("[STAIRS DEBUG] Step 2: Player detected");
Debug.LogWarning("[FLOOR TRANSITION] Initiating transition...");
Debug.LogWarning("[FLOOR TRANSITION] Room loaded: " + roomId);

// Separate post-completion diagnostics
Debug.LogWarning("[POST-TRANSITION] Fade alpha: " + fadeAlpha);
Debug.LogWarning("[POST-TRANSITION] Camera position: " + camPos);
```

**Benefits:**
1. Easy to filter logs by tag (grep "[STAIRS DEBUG]")
2. Clear sequence of what happened
3. Easy to spot where flow stopped

**Remember to remove/reduce verbose logging** once the issue is fixed!
