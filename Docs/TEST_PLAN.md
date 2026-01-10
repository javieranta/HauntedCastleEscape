# Haunted Castle Escape - Comprehensive Test Plan

## Overview
This document outlines the comprehensive testing strategy for the game after fixing the Time.timeScale freeze issues.

---

## Critical Issues Fixed

### Root Cause: Infinite Loop Coroutines
The game was freezing because coroutines using `Time.deltaTime` or `WaitForSeconds()` become infinite loops when `Time.timeScale = 0`.

### Files Fixed (17 total):

1. **VisualEffectsManager.cs** - 6 coroutines fixed
   - DamageNumberRoutine
   - HealNumberRoutine
   - HitFlashRoutine
   - ScreenShakeRoutine
   - ParticleBurstRoutine
   - TextPopupRoutine

2. **CombatFeedbackManager.cs** - All coroutines fixed + HitStop disabled
   - MeleeSwingTrailRoutine
   - ImpactRingRoutine
   - ImpactLinesRoutine

3. **Enemy.cs** - 3 coroutines fixed
   - DamageFlash
   - DeathAnimation
   - DamageFlashRoutine

4. **EnemyAbilities.cs** - 3 coroutines fixed
   - PhaseSequence
   - TeleportSequence
   - LungeSequence

5. **PlayerHealth.cs** - 3 coroutines fixed
   - RespawnRoutine
   - GameOverRoutine
   - DamageFlashRoutine

6. **EnemySpawner.cs** - 1 coroutine fixed
   - SpawnRoomEnemiesDelayed

7. **FloorIndicatorUI.cs** - 3 coroutines fixed
   - ShowFloorChangeSequence
   - ShowBriefSequence
   - FadeOut

8. **TutorialHintUI.cs** - 2 coroutines fixed
   - FadeIn
   - FadeOut

9. **DynamicLightingSystem.cs** - 1 coroutine fixed
   - LightFlashRoutine

10. **MinimapUI.cs** - 1 coroutine fixed
    - CenterOnRoom coroutine

11. **ScoreDisplayUI.cs** - 1 coroutine fixed
    - FadeCombo

12. **ItemPickup.cs** - 1 coroutine fixed
    - PickupAnimation

13. **PlayerAnimator.cs** - 1 coroutine fixed
    - DamageFlash

14. **Projectile.cs** - 1 coroutine fixed
    - HitEffectRoutine

### Safety Mechanisms Added:
- **TimeScaleWatchdog.cs** - Auto-resets Time.timeScale if stuck at 0 for > 2 seconds
- **PlayerController.cs** - Aggressive timeScale reset after 1.5 seconds

---

## Test Cases

### 1. Player Movement Tests

#### 1.1 Basic Movement
- [ ] Move up (W / Up Arrow)
- [ ] Move down (S / Down Arrow)
- [ ] Move left (A / Left Arrow)
- [ ] Move right (D / Right Arrow)
- [ ] Diagonal movement works
- [ ] Movement speed feels consistent

#### 1.2 Movement After Events
- [ ] Movement works after taking damage
- [ ] Movement works after room transition
- [ ] Movement works after floor transition
- [ ] Movement works after pause/unpause
- [ ] Movement works after respawn

### 2. Attack System Tests

#### 2.1 Basic Attack
- [ ] Space key fires projectile
- [ ] Left mouse click fires projectile
- [ ] Projectile moves in facing direction
- [ ] Projectile deals damage to enemies
- [ ] Attack cooldown works

#### 2.2 Attack Visual Effects
- [ ] Projectile is visible
- [ ] Hit effect appears on enemy hit
- [ ] Screen shake on enemy hit

### 3. Enemy Encounter Tests

#### 3.1 Enemy Behavior
- [ ] Enemies spawn in rooms
- [ ] Enemies move (patrol/chase)
- [ ] Enemies detect player
- [ ] Enemies deal damage on contact

#### 3.2 Specific Enemy Abilities
- [ ] Bat sonic scream works WITHOUT freeze
- [ ] Ghost phase ability works
- [ ] Vampire teleport works
- [ ] Witch magic bolts work
- [ ] All enemy abilities complete without freeze

### 4. Room Transition Tests

#### 4.1 Door Transitions
- [ ] Door triggers work
- [ ] Fade out happens
- [ ] New room loads
- [ ] Fade in happens
- [ ] Player positioned correctly
- [ ] NO FREEZE during transition

#### 4.2 Floor Transitions
- [ ] Stairs work
- [ ] Floor indicator shows
- [ ] Floor changes correctly
- [ ] NO FREEZE during floor change

### 5. Visual Effects Tests

#### 5.1 Damage Effects
- [ ] Damage numbers appear
- [ ] Damage numbers fade out
- [ ] Screen shake works
- [ ] Effects complete without freeze

#### 5.2 Particle Effects
- [ ] Particle bursts work
- [ ] Particles fade out properly
- [ ] No infinite particles

### 6. Combat Feedback Tests

#### 6.1 Hit Effects
- [ ] Enemy flash on hit
- [ ] Impact effects appear
- [ ] Effects complete without freeze

#### 6.2 Death Effects
- [ ] Enemy death animation plays
- [ ] Player death animation plays
- [ ] Respawn works correctly

### 7. UI Tests

#### 7.1 HUD
- [ ] Health bar updates
- [ ] Score updates
- [ ] Combo display works
- [ ] All UI updates without freeze

#### 7.2 Tutorial Hints
- [ ] Hints fade in
- [ ] Hints fade out
- [ ] No freeze on hints

### 8. Pause System Tests

#### 8.1 Pause Menu
- [ ] ESC opens pause menu
- [ ] Game pauses (Time.timeScale = 0)
- [ ] Resume works
- [ ] Time.timeScale returns to 1
- [ ] All effects continue after unpause

### 9. Stress Tests

#### 9.1 Extended Play
- [ ] Play for 5+ minutes without freeze
- [ ] Multiple room transitions
- [ ] Multiple enemy encounters
- [ ] Multiple deaths and respawns

#### 9.2 Rapid Actions
- [ ] Rapid attacking doesn't freeze
- [ ] Rapid movement doesn't freeze
- [ ] Multiple effects at once work

### 10. Recovery Tests

#### 10.1 Reset Key
- [ ] R key resets player state
- [ ] TimeScaleWatchdog triggers if needed
- [ ] Game recovers from any stuck state

---

## Expected Results

After all fixes:
1. **No freezing** during normal gameplay
2. **No freezing** when enemies use abilities
3. **No freezing** during room/floor transitions
4. **No freezing** during visual effects
5. **Player can always move** (except during legitimate pauses)
6. **Attack system works** consistently
7. **Game recovers** from any Time.timeScale issues via watchdog

---

## Automated Verification

To verify fixes are in place, run these grep commands:

```bash
# Should return NO matches (all WaitForSeconds converted to WaitForSecondsRealtime)
grep -r "yield return new WaitForSeconds\([^R]" Assets/Scripts/

# Should return NO matches (all elapsed += Time.deltaTime converted)
grep -r "elapsed += Time\.deltaTime" Assets/Scripts/
```

---

## Known Limitations

1. **Hit Stop disabled** - The freeze-frame effect on hit is disabled to prevent issues
2. **Slow Motion disabled** - Kill slow motion is disabled for stability
3. **Effects use unscaled time** - Visual effects continue during pause (acceptable tradeoff)

---

## Sign-off

- [ ] All test cases passed
- [ ] No freezing in 10+ minute play session
- [ ] Attack and movement work consistently
- [ ] Ready for production

Date: _______________
Tester: _______________
