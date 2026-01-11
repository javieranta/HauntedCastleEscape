# Claude Next Session Instructions

**Last Updated:** 2026-01-11
**Project:** Haunted Castle Escape (Unity 2D game)

---

## IMMEDIATE CONTEXT - READ THIS FIRST

You are continuing development on a Unity 2D game called "Haunted Castle Escape" - a clean-room remake inspired by the 1983 ZX Spectrum game "Atic Atac".

### What Was Just Fixed (Session 6)

The **basement darkness bug** was finally solved after multiple debugging sessions:

**Root Cause:** `Hazard.cs` line 60 was calling `LayerMask.NameToLayer("Hazards")` which returned -1 because the "Hazards" layer doesn't exist in Unity's layer settings. Assigning -1 to `gameObject.layer` caused the error: "A game object can only be in one layer. The layer needs to be in the range [0...31]"

**Fixes Applied:**
1. `Hazard.cs` - Added layer validation with fallback to Default layer
2. `Hazard.cs` - Fixed `CompareTag` to use string comparison (prevents silent failures)
3. `RoomManager.cs` - Added `DelayedFadeCheck()` safety coroutine (force-clears stuck fades after 1 second)
4. `FloorTransitionTrigger.cs` - Added comprehensive `[STAIRS DEBUG]` logging

---

## CRITICAL FILES TO KNOW

| File | Purpose |
|------|---------|
| `Docs/LEARNINGS.md` | **READ FIRST when stuck** - Solutions to hard problems |
| `Docs/SESSION_HANDOFF.md` | Session-by-session fix history |
| `CLAUDE.md` (project root) | Project guidelines, architecture |
| `Assets/Scripts/Services/RoomManager.cs` | Room transitions, the heart of gameplay flow |
| `Assets/Scripts/Rooms/FloorTransitionTrigger.cs` | Stairs/trapdoor collision handling |

---

## KNOWN PATTERNS THAT CAUSE ISSUES

### 1. CompareTag Freezes/Fails Silently
```csharp
// BAD - Freezes or fails if tag doesn't exist
if (other.CompareTag("Player"))

// GOOD - Always works
if (other.tag == "Player" || other.gameObject.name.ToLower().Contains("player"))
```

### 2. LayerMask.NameToLayer Returns -1
```csharp
// BAD - Returns -1 if layer doesn't exist
gameObject.layer = LayerMask.NameToLayer("MyLayer");

// GOOD - Validate first
int layer = LayerMask.NameToLayer("MyLayer");
if (layer >= 0 && layer <= 31) gameObject.layer = layer;
else gameObject.layer = 0; // Default
```

### 3. TextMeshPro at Runtime Freezes Game
Never use `AddComponent<TextMeshPro>()` at runtime - use SpriteRenderer instead.

### 4. Time.deltaTime Division When Paused
Always guard: `if (Time.deltaTime > 0.0001f)`

---

## CURRENT GAME STATE

### What Works
- Player movement and combat
- Room transitions (doors, stairs, trapdoors)
- Floor system (0=Basement/RED, 1=Castle/GREEN, 2=Tower/BLUE)
- Enemy AI and projectiles
- Visual effects (damage indicators, etc.)
- Basement visibility (JUST FIXED)

### What's Partially Working
- Audio system (disabled for testing)
- Some enemy types may have issues

### What's Not Implemented Yet
- Full procedural castle generation
- Complete game loop (3 key pieces → Great Key → Victory)
- Win/lose conditions
- Scoring system

---

## HOW TO DEBUG ROOM TRANSITIONS

1. **Watch Unity Console** for these log patterns:
   - `[STAIRS DEBUG]` - Trigger collision detection
   - `[FLOOR TRANSITION]` - Transition initiation
   - `[RoomManager]` - Room building process
   - `[POST-TRANSITION]` - After transition diagnostics

2. **If screen goes black:**
   - Check for `[RoomManager] SAFETY: Fade still at X` (fade stuck)
   - Check for layer assignment errors
   - Look for exceptions in room building

3. **Key diagnostic in RoomManager:**
   - `RunVisibilityDiagnostics()` - Logs camera, room, and renderer state
   - `RunPostTransitionDiagnostics()` - Runs after fade completes

---

## UNITY PROJECT SPECIFICS

### Layers Defined (in LayerSetup.cs)
- 8: Player
- 9: Enemies
- 10: Projectiles
- 11: Items
- 12: Walls
- 13: Doors
- 14: Triggers

**NOTE:** "Hazards" layer is NOT defined - code now falls back to Default (0)

### Sorting Layers Used
- Background
- Walls
- Items
- Enemies
- Player
- Projectiles
- UI
- Lighting

**NOTE:** Some may not be defined in Unity - code should handle gracefully

---

## FILES WITH RECENT CHANGES (Session 6)

| File | Change |
|------|--------|
| `Assets/Scripts/Rooms/Hazard.cs` | Layer validation, CompareTag fix |
| `Assets/Scripts/Services/RoomManager.cs` | DelayedFadeCheck safety coroutine |
| `Assets/Scripts/Rooms/FloorTransitionTrigger.cs` | Debug logging, CompareTag fix |
| `Assets/Scripts/Services/TransitionManager.cs` | Added CurrentFadeAlpha, ForceTransparent() |
| `Docs/LEARNINGS.md` | Added Learning #19 (layer assignment) |
| `Docs/SESSION_HANDOFF.md` | Updated with Session 6 fixes |

---

## POTENTIAL NEXT STEPS

1. **Add "Hazards" layer in Unity** - Currently falls back to Default, but proper layer would be better
2. **Clean up debug logging** - Remove or reduce verbose `[STAIRS DEBUG]` logs after confirming stability
3. **Test all floor transitions** - Verify stairs up/down work correctly
4. **Continue game implementation** - Keys, inventory, win condition

---

## STRUGGLES & LESSONS LEARNED

### What Made This Bug Hard to Find
1. The layer error didn't throw an exception that stopped room building
2. The error message appeared in logs but wasn't obviously connected to black screen
3. Multiple red herrings: camera position, fade timing, lighting system
4. Needed to search Unity Editor log directly (`C:\Users\javie\AppData\Local\Unity\Editor\Editor.log`)

### Debugging Approach That Worked
1. Added extensive logging at every step of room transition
2. Added POST-TRANSITION diagnostics to verify state after fade
3. Searched Unity Editor log for specific error patterns using grep
4. Found the layer error, traced it to Hazard.cs

---

## HOW TO START NEXT SESSION

1. **Read this file first** (`Docs/CLAUDE_NEXT_SESSION.md`)
2. **Check `Docs/LEARNINGS.md`** if encountering any freeze/crash issues
3. **Run the game** and verify basement transition still works
4. **Ask user** what they want to work on next

---

## QUICK COMMANDS

```bash
# Check Unity Editor log for errors
# Location: C:\Users\javie\AppData\Local\Unity\Editor\Editor.log

# Search for specific patterns in log
grep -i "error" "C:\Users\javie\AppData\Local\Unity\Editor\Editor.log" | tail -50

# Check project structure
ls Assets/Scripts/
```

---

**Remember:** Always read `Docs/LEARNINGS.md` when stuck - the solution is probably already documented there!
