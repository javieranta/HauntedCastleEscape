# Quick Sprite Generation Guide

## The Problem
Procedurally generated sprites look like "computer graphics" no matter how we improve the code.
Your knight looks great because Midjourney was trained on millions of real images.

## The Solution
Generate ALL sprites with Midjourney and import them. Here's the fastest workflow:

---

## PRIORITY 1: Floor & Wall Tiles (Biggest Visual Impact)

### Stone Floor Tile (Copy this EXACTLY into Midjourney):
```
seamless tileable dark medieval castle stone floor texture, worn granite flagstones with mortar lines, top-down view, game asset, 2D texture, realistic lighting, --tile --ar 1:1 --v 6
```

### Stone Wall Tile:
```
seamless tileable medieval castle stone brick wall texture, gray limestone blocks with dark mortar, front view, game asset, 2D texture, realistic lighting, --tile --ar 1:1 --v 6
```

### Wood Floor Tile:
```
seamless tileable old wooden plank floor texture, dark oak boards with visible grain, top-down view, game asset, 2D texture, --tile --ar 1:1 --v 6
```

---

## PRIORITY 2: Props (4 sprites)

### Torch:
```
medieval wall torch with orange flame, wooden handle with metal bracket, dark fantasy game art style, transparent background, 2D game sprite, front view --ar 1:2 --v 6
```

### Barrel:
```
old wooden barrel with metal bands, medieval game asset, transparent background, 2D game sprite, front view, dark fantasy style --ar 1:1 --v 6
```

### Cobweb:
```
detailed spider cobweb, white silk threads, dark fantasy style, transparent background, 2D game asset, corner decoration --ar 1:1 --v 6
```

### Wooden Door:
```
heavy medieval wooden door with iron hinges and handle, castle dungeon style, dark wood planks, transparent background, 2D game sprite --ar 3:4 --v 6
```

---

## PRIORITY 3: Enemies (Generate after floors/walls look good)

### Spider:
```
giant black spider, red eyes, hairy legs, dark fantasy monster, top-down view, 2D game sprite, transparent background --ar 1:1 --v 6
```

### Ghost:
```
floating translucent ghost, pale blue glow, tattered ethereal form, dark fantasy, 2D game sprite, transparent background --ar 1:1 --v 6
```

### Skeleton:
```
walking skeleton warrior, bone white, dark fantasy style, front view, 2D game sprite, transparent background --ar 1:1 --v 6
```

---

## Import Steps (For EACH sprite):

1. **Generate in Midjourney** - Use the prompts above
2. **Upscale** - Click U1/U2/U3/U4 to upscale your favorite
3. **Download** - Save the image
4. **Remove background** (if needed) - Use remove.bg
5. **Copy to Unity folder:**
   - Floor tiles → `Assets/Resources/Sprites/Environment/Floors/`
   - Wall tiles → `Assets/Resources/Sprites/Environment/Walls/`
   - Props → `Assets/Resources/Sprites/Environment/Props/`
   - Enemies → `Assets/Resources/Sprites/Enemies/`

6. **In Unity** - Select the imported sprite and set:
   - **Filter Mode**: Bilinear (smooth) or Trilinear
   - **Compression**: None (for best quality)
   - **Max Size**: 2048
   - Click Apply

---

## File Naming Convention

```
Assets/Resources/Sprites/
├── Environment/
│   ├── Floors/
│   │   └── floor.png          (or floor_stone.png, floor_wood.png)
│   ├── Walls/
│   │   └── wall.png           (or wall_stone.png, wall_brick.png)
│   ├── Doors/
│   │   └── door_closed.png
│   │   └── door_open.png
│   └── Props/
│       ├── torch.png
│       ├── barrel.png
│       └── cobweb.png
├── Enemies/
│   ├── spider.png
│   ├── ghost.png
│   └── skeleton.png
└── Characters/
    ├── Knight/
    │   └── knight_idle.png    (already done!)
    ├── Wizard/
    │   └── wizard_idle.png
    └── Serf/
        └── serf_idle.png
```

---

## Estimated Time
- Floor + Wall tiles: 10 minutes (biggest visual improvement!)
- 4 props: 15 minutes
- 3 enemies: 10 minutes
- **Total: ~35 minutes to transform the entire game's look**

---

## Tips for Best Results

1. **Use `--tile` for floor/wall textures** - Makes them seamlessly tileable
2. **Use `--ar 1:1` for most sprites** - Square aspect ratio
3. **Add "transparent background"** for characters/items
4. **Add "2D game sprite"** to get flat art suitable for games
5. **Use `--v 6`** for latest Midjourney version
