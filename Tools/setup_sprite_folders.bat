@echo off
echo ========================================
echo  Haunted Castle Escape - Sprite Setup
echo ========================================
echo.

set SPRITES_DIR=%~dp0..\Assets\Resources\Sprites

echo Creating sprite folder structure...
echo.

:: Environment folders
mkdir "%SPRITES_DIR%\Environment\Floors" 2>nul
mkdir "%SPRITES_DIR%\Environment\Walls" 2>nul
mkdir "%SPRITES_DIR%\Environment\Doors" 2>nul
mkdir "%SPRITES_DIR%\Environment\Props" 2>nul

:: Character folders
mkdir "%SPRITES_DIR%\Characters\Wizard" 2>nul
mkdir "%SPRITES_DIR%\Characters\Knight" 2>nul
mkdir "%SPRITES_DIR%\Characters\Serf" 2>nul

:: Enemy folders
mkdir "%SPRITES_DIR%\Enemies\Ghost" 2>nul
mkdir "%SPRITES_DIR%\Enemies\Skeleton" 2>nul
mkdir "%SPRITES_DIR%\Enemies\Bat" 2>nul
mkdir "%SPRITES_DIR%\Enemies\Spider" 2>nul
mkdir "%SPRITES_DIR%\Enemies\Mummy" 2>nul
mkdir "%SPRITES_DIR%\Enemies\Demon" 2>nul
mkdir "%SPRITES_DIR%\Enemies\Vampire" 2>nul
mkdir "%SPRITES_DIR%\Enemies\Witch" 2>nul
mkdir "%SPRITES_DIR%\Enemies\Werewolf" 2>nul
mkdir "%SPRITES_DIR%\Enemies\Reaper" 2>nul

:: Item folders
mkdir "%SPRITES_DIR%\Items\Food" 2>nul
mkdir "%SPRITES_DIR%\Items\Keys" 2>nul
mkdir "%SPRITES_DIR%\Items\Special" 2>nul
mkdir "%SPRITES_DIR%\Items\Treasure" 2>nul

:: Effect folders
mkdir "%SPRITES_DIR%\Effects\Projectiles" 2>nul
mkdir "%SPRITES_DIR%\Effects\Impacts" 2>nul

:: UI folders
mkdir "%SPRITES_DIR%\UI\HUD" 2>nul
mkdir "%SPRITES_DIR%\UI\Buttons" 2>nul

echo.
echo Folder structure created!
echo.
echo ========================================
echo  SPRITE CHECKLIST
echo ========================================
echo.
echo ENVIRONMENT (Priority: HIGH)
echo ---------------------------
if exist "%SPRITES_DIR%\Environment\Floors\stone_floor.png" (echo [X] stone_floor.png) else (echo [ ] stone_floor.png)
if exist "%SPRITES_DIR%\Environment\Floors\wood_floor.png" (echo [X] wood_floor.png) else (echo [ ] wood_floor.png)
if exist "%SPRITES_DIR%\Environment\Floors\dungeon_floor.png" (echo [X] dungeon_floor.png) else (echo [ ] dungeon_floor.png)
if exist "%SPRITES_DIR%\Environment\Floors\tower_floor.png" (echo [X] tower_floor.png) else (echo [ ] tower_floor.png)
echo.
if exist "%SPRITES_DIR%\Environment\Walls\stone_wall.png" (echo [X] stone_wall.png) else (echo [ ] stone_wall.png)
if exist "%SPRITES_DIR%\Environment\Walls\dungeon_wall.png" (echo [X] dungeon_wall.png) else (echo [ ] dungeon_wall.png)
if exist "%SPRITES_DIR%\Environment\Walls\brick_wall.png" (echo [X] brick_wall.png) else (echo [ ] brick_wall.png)
echo.
if exist "%SPRITES_DIR%\Environment\Doors\wooden_door.png" (echo [X] wooden_door.png) else (echo [ ] wooden_door.png)
if exist "%SPRITES_DIR%\Environment\Doors\iron_door.png" (echo [X] iron_door.png) else (echo [ ] iron_door.png)
if exist "%SPRITES_DIR%\Environment\Doors\secret_door.png" (echo [X] secret_door.png) else (echo [ ] secret_door.png)
echo.
echo CHARACTERS (Priority: MEDIUM)
echo -----------------------------
if exist "%SPRITES_DIR%\Characters\Wizard\wizard_idle.png" (echo [X] wizard_idle.png) else (echo [ ] wizard_idle.png)
if exist "%SPRITES_DIR%\Characters\Knight\knight_idle.png" (echo [X] knight_idle.png) else (echo [ ] knight_idle.png)
if exist "%SPRITES_DIR%\Characters\Serf\serf_idle.png" (echo [X] serf_idle.png) else (echo [ ] serf_idle.png)
echo.
echo ENEMIES (Priority: MEDIUM)
echo --------------------------
if exist "%SPRITES_DIR%\Enemies\Ghost\ghost_idle.png" (echo [X] ghost_idle.png) else (echo [ ] ghost_idle.png)
if exist "%SPRITES_DIR%\Enemies\Skeleton\skeleton_idle.png" (echo [X] skeleton_idle.png) else (echo [ ] skeleton_idle.png)
if exist "%SPRITES_DIR%\Enemies\Bat\bat_idle.png" (echo [X] bat_idle.png) else (echo [ ] bat_idle.png)
if exist "%SPRITES_DIR%\Enemies\Spider\spider_idle.png" (echo [X] spider_idle.png) else (echo [ ] spider_idle.png)
if exist "%SPRITES_DIR%\Enemies\Mummy\mummy_idle.png" (echo [X] mummy_idle.png) else (echo [ ] mummy_idle.png)
if exist "%SPRITES_DIR%\Enemies\Demon\demon_idle.png" (echo [X] demon_idle.png) else (echo [ ] demon_idle.png)
if exist "%SPRITES_DIR%\Enemies\Vampire\vampire_idle.png" (echo [X] vampire_idle.png) else (echo [ ] vampire_idle.png)
if exist "%SPRITES_DIR%\Enemies\Witch\witch_idle.png" (echo [X] witch_idle.png) else (echo [ ] witch_idle.png)
echo.
echo ITEMS (Priority: LOW)
echo ---------------------
if exist "%SPRITES_DIR%\Items\Food\chicken.png" (echo [X] chicken.png) else (echo [ ] chicken.png)
if exist "%SPRITES_DIR%\Items\Food\bread.png" (echo [X] bread.png) else (echo [ ] bread.png)
if exist "%SPRITES_DIR%\Items\Keys\key_red.png" (echo [X] key_red.png) else (echo [ ] key_red.png)
if exist "%SPRITES_DIR%\Items\Keys\key_blue.png" (echo [X] key_blue.png) else (echo [ ] key_blue.png)
if exist "%SPRITES_DIR%\Items\Keys\key_green.png" (echo [X] key_green.png) else (echo [ ] key_green.png)
if exist "%SPRITES_DIR%\Items\Special\cross.png" (echo [X] cross.png) else (echo [ ] cross.png)
if exist "%SPRITES_DIR%\Items\Special\spellbook.png" (echo [X] spellbook.png) else (echo [ ] spellbook.png)
echo.
echo ========================================
echo.
echo See Docs\MIDJOURNEY_SPRITE_BATCH.md for prompts!
echo.
pause
