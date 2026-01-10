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

## Quick Reference: Common Freeze Causes

| Symptom | Likely Cause | Solution |
|---------|--------------|----------|
| Freeze on collision | `CompareTag` with missing tag | Use `tag ==` instead |
| Freeze when timeScale=0 | `Time.deltaTime` in loops | Use `Time.unscaledDeltaTime` |
| Freeze on spawn | Runtime texture creation | Cache textures/sprites |
| Freeze with physics | NaN/Infinity in velocity | Validate before applying |
| Freeze on division | Division by zero | Guard with `if (x > 0)` |

---

**Last Updated:** 2026-01-10
