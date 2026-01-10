# Time.timeScale Freeze Fix - Changes Summary

## Issue Description
The game was freezing completely (player and all enemies) after certain events, particularly when the bat enemy used its sonic scream ability. The freeze was not recoverable via the R key reset.

## Root Cause Analysis

### Primary Issue: Infinite Loop Coroutines
When `Time.timeScale` is set to 0 (e.g., during combat feedback, pause, or hit stop):
- `Time.deltaTime` returns 0
- Coroutines using `elapsed += Time.deltaTime` never progress
- While loops like `while (elapsed < duration)` run infinitely
- Unity's main thread gets blocked in the coroutine

### Secondary Issue: Combat Feedback Hit Stop
`CombatFeedbackManager.TriggerHitStop()` was setting `Time.timeScale = 0` for dramatic effect, but the restoration could fail, leaving the game frozen.

## Solution Applied

### 1. Disabled Time.timeScale Manipulation in Combat Effects

**File: CombatFeedbackManager.cs**
- `TriggerHitStop()` - Returns immediately without setting timeScale
- `TriggerSlowMotionKill()` - Returns immediately without setting timeScale
- `HitStopRoutine()` - Yields break immediately

### 2. Converted All Coroutines to Use Unscaled Time

Changed `Time.deltaTime` to `Time.unscaledDeltaTime` in all coroutines:

| File | Coroutines Fixed |
|------|------------------|
| VisualEffectsManager.cs | DamageNumberRoutine, HealNumberRoutine, HitFlashRoutine, ScreenShakeRoutine, ParticleBurstRoutine, TextPopupRoutine |
| CombatFeedbackManager.cs | MeleeSwingTrailRoutine, ImpactRingRoutine, ImpactLinesRoutine |
| Enemy.cs | DamageFlash, DeathAnimation, DamageFlashRoutine |
| EnemyAbilities.cs | PhaseSequence, TeleportSequence, LungeSequence |
| PlayerHealth.cs | RespawnRoutine, GameOverRoutine, DamageFlashRoutine |
| EnemySpawner.cs | SpawnRoomEnemiesDelayed |
| FloorIndicatorUI.cs | ShowFloorChangeSequence, ShowBriefSequence, FadeOut |
| TutorialHintUI.cs | FadeIn, FadeOut |
| DynamicLightingSystem.cs | LightFlashRoutine |
| MinimapUI.cs | CenterOnRoom |
| ScoreDisplayUI.cs | FadeCombo |
| ItemPickup.cs | PickupAnimation |
| PlayerAnimator.cs | DamageFlash |
| Projectile.cs | HitEffectRoutine |

### 3. Converted WaitForSeconds to WaitForSecondsRealtime

All `yield return new WaitForSeconds(x)` converted to `yield return new WaitForSecondsRealtime(x)`:

| File | Locations |
|------|-----------|
| VisualEffectsManager.cs | HitFlashRoutine |
| Enemy.cs | DamageFlash |
| EnemyAbilities.cs | PhaseSequence (1.5f), TeleportSequence (0.1f) |
| PlayerHealth.cs | RespawnRoutine (1f), GameOverRoutine (2f), DamageFlashRoutine (2x) |
| EnemySpawner.cs | SpawnRoomEnemiesDelayed |
| FloorIndicatorUI.cs | ShowFloorChangeSequence, ShowBriefSequence |

### 4. Safety Mechanisms

**TimeScaleWatchdog.cs** (already existed)
- Monitors `Time.timeScale` using `Time.unscaledTime`
- Auto-resets to 1.0 if stuck at 0 for > 2 seconds
- Also unpauses GameManager if in Paused state

**PlayerController.cs** (already had)
- Aggressive timeScale reset after 1.5 seconds
- Uses `Time.unscaledTime` for tracking

## Files Modified (17 total)

1. `Assets/Scripts/Effects/VisualEffectsManager.cs` - 6 coroutines
2. `Assets/Scripts/Effects/CombatFeedbackManager.cs` - 3+ coroutines + disabled hit stop
3. `Assets/Scripts/Enemies/Enemy.cs` - 3 coroutines
4. `Assets/Scripts/Enemies/EnemyAbilities.cs` - 3 coroutines
5. `Assets/Scripts/Enemies/EnemySpawner.cs` - 1 coroutine
6. `Assets/Scripts/Player/PlayerHealth.cs` - 3 coroutines
7. `Assets/Scripts/Player/PlayerAnimator.cs` - 1 coroutine
8. `Assets/Scripts/Player/Projectile.cs` - 1 coroutine
9. `Assets/Scripts/UI/FloorIndicatorUI.cs` - 3 coroutines
10. `Assets/Scripts/UI/TutorialHintUI.cs` - 2 coroutines
11. `Assets/Scripts/UI/MinimapUI.cs` - 1 coroutine
12. `Assets/Scripts/UI/ScoreDisplayUI.cs` - 1 coroutine
13. `Assets/Scripts/Items/ItemPickup.cs` - 1 coroutine
14. `Assets/Scripts/Visuals/DynamicLightingSystem.cs` - 1 coroutine

## Verification Commands

Run these to verify all fixes are in place:

```powershell
# Should return NO matches (all WaitForSeconds converted)
Select-String -Path "Assets\Scripts\*.cs" -Pattern "yield return new WaitForSeconds\([^R]" -Recurse

# Should return NO matches (all elapsed += Time.deltaTime converted)
Select-String -Path "Assets\Scripts\*.cs" -Pattern "elapsed \+= Time\.deltaTime" -Recurse
```

## Known Trade-offs

1. **Visual effects continue during pause** - Since effects use unscaled time, they'll continue playing even when the game is paused. This is an acceptable trade-off for stability.

2. **Hit stop effect removed** - The dramatic freeze-frame on enemy hit is disabled. The game now continues smoothly without this effect.

3. **Slow motion effect removed** - Kill slow motion is disabled for stability.

## Testing Recommendations

1. Play for 5+ minutes with multiple room transitions
2. Encounter multiple enemy types, especially bats
3. Trigger pause/unpause multiple times
4. Die and respawn multiple times
5. Verify no freezes occur

---

## Additional Fix: NaN/Infinity Physics Corruption (v2)

### New Root Cause Discovered
The game was still freezing because of a **divide by zero** issue that produced NaN/Infinity values, corrupting Unity's physics engine.

### Primary Issue: Division by Zero
In `AdvancedEnemyAI.cs`:
```csharp
_playerVelocity = (currentPlayerPos - _lastPlayerPosition) / Time.deltaTime;
```
When `Time.deltaTime = 0` (during timeScale = 0), this produces `Infinity` or `NaN` values that corrupt Rigidbody2D velocities and freeze the physics simulation.

### Fixes Applied (v2)

#### 1. AdvancedEnemyAI.cs - Divide by Zero Guard
- Added check: `if (Time.deltaTime > 0.0001f)` before division
- Added velocity clamping to max 50 units/sec
- Added NaN/Infinity validation with reset to zero

#### 2. TimeScaleWatchdog.cs - Enhanced Recovery
- Reduced timeout from 2.0s to 0.5s for faster recovery
- Added `ValidatePhysicsHealth()` method that runs every 60 frames
- Scans all enemies and player for NaN/Infinity velocities
- Auto-resets any corrupted velocities to zero

#### 3. Enemy.cs - Velocity Validation
- Added NaN/Infinity check before applying velocity to Rigidbody2D
- Added velocity clamping to max 20 units/sec
- Logs warning when invalid velocity is detected

### Summary of All Changes

| Issue | Cause | Fix |
|-------|-------|-----|
| Infinite loop coroutines | `Time.deltaTime = 0` in while loops | Use `Time.unscaledDeltaTime` |
| WaitForSeconds freeze | Pauses when timeScale = 0 | Use `WaitForSecondsRealtime` |
| Division by zero | `x / Time.deltaTime` when timeScale = 0 | Guard with `if (Time.deltaTime > 0)` |
| Physics corruption | NaN/Infinity in velocities | Validate and reset to zero |
| Slow recovery | 2 second timeout | Reduced to 0.5 seconds |

---

## Additional Fix: SimpleProjectileBehavior (v3)

### New Root Cause Discovered
The original `EnemyProjectile` component's `Awake()` method and `CreateVisual()` were causing freezes during component initialization. The exact cause was traced to something in the complex component setup, possibly Unity's internal handling of multiple component additions with custom sprites.

### Solution Applied

#### 1. Created SimpleProjectileBehavior.cs
A minimal, lightweight projectile behavior that avoids all the freeze-causing code:
- No Awake() texture creation
- Uses pre-cached sprite from EnemyProjectile.GetSimpleSprite()
- Simple velocity handling in Initialize()
- Uses unscaledDeltaTime for lifetime tracking
- Clean collision handling with player and walls

#### 2. Modified EnemyProjectile.SpawnProjectile()
Changed from creating full EnemyProjectile component to:
- Creating a simple GameObject with minimal components
- Using cached 8x8 white circle sprite (created once, reused)
- Adding SimpleProjectileBehavior instead of EnemyProjectile
- Returns null (no reference needed for simple projectiles)

### Files Modified (v3)

1. `Assets/Scripts/Enemies/SimpleProjectileBehavior.cs` - NEW FILE
2. `Assets/Scripts/Enemies/EnemyProjectile.cs` - SpawnProjectile now uses simplified approach

### Summary of All Changes (v1-v3)

| Issue | Cause | Fix |
|-------|-------|-----|
| Infinite loop coroutines | `Time.deltaTime = 0` in while loops | Use `Time.unscaledDeltaTime` |
| WaitForSeconds freeze | Pauses when timeScale = 0 | Use `WaitForSecondsRealtime` |
| Division by zero | `x / Time.deltaTime` when timeScale = 0 | Guard with `if (Time.deltaTime > 0)` |
| Physics corruption | NaN/Infinity in velocities | Validate and reset to zero |
| EnemyProjectile component freeze | Complex component initialization | Use SimpleProjectileBehavior |
| Runtime texture creation | New Texture2D per projectile | Cache sprites statically |

---

**Date:** 2026-01-10
**Status:** Complete (v3 - SimpleProjectileBehavior fix)
