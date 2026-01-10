# Controls

## Default Control Scheme (Keyboard & Mouse)

| Action | Primary | Secondary |
|--------|---------|-----------|
| Move Up | W | ↑ Arrow |
| Move Down | S | ↓ Arrow |
| Move Left | A | ← Arrow |
| Move Right | D | → Arrow |
| Attack | Space | Left Mouse |
| Interact/Pickup | E | Right Mouse |
| Drop Item | Q | - |
| Pause | Escape | - |

## Classic Control Scheme (Sinclair-style)

| Action | Key |
|--------|-----|
| Move Up | R |
| Move Down | E |
| Move Left | Q |
| Move Right | W |
| Attack | T |
| Interact/Pickup | Z |
| Pause | Escape |

## Gamepad Support

| Action | Button |
|--------|--------|
| Move | Left Stick / D-Pad |
| Attack | A (South) |
| Interact | X (West) |
| Drop Item | B (East) |
| Pause | Start |

## Input Configuration

Input actions are defined in `Assets/Settings/GameInputActions.inputactions` and loaded at runtime from `Assets/Resources/GameInputActions.inputactions`.

### Modifying Controls

1. Open `Assets/Settings/GameInputActions.inputactions` in Unity
2. Select the action to modify
3. Add/remove/change bindings as needed
4. Save the asset
5. Copy to `Assets/Resources/` if changed

### Control Schemes

Three control schemes are available:
- **Keyboard&Mouse**: Modern WASD + mouse controls
- **Gamepad**: Standard controller support
- **Classic**: Original ZX Spectrum-style keys

### Switching Schemes at Runtime

```csharp
// Get the input handler
var inputHandler = PlayerController.Instance.GetComponent<PlayerInputHandler>();

// Switch to classic controls
inputHandler.SwitchControlScheme("Classic");

// Switch back to modern
inputHandler.SwitchControlScheme("Keyboard&Mouse");
```

## Special Interactions

### Secret Passages
- Stand near a secret passage (Bookcase/Clock/Barrel)
- Press Interact (E) if your character can use it:
  - **Wizard**: Bookcase passages
  - **Knight**: Clock passages
  - **Serf**: Barrel passages

### Locked Doors
- Approach a colored locked door
- If you have the matching key, press Interact (E)
- Key is consumed and door opens

### Items
- Walk over items to auto-pickup (if inventory has space)
- Or press Interact (E) near an item
- Press Drop (Q) to drop the oldest item
- Food is consumed by pressing the corresponding slot key (1-3)

### Floor Transitions
- Walk onto stairs or trapdoors
- Press Interact (E) to move between floors
