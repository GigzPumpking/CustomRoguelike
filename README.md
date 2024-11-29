# CustomRoguelike

**CustomRoguelike** is a Unity3D project that lets players personalize their roguelike experience by customizing in-game sprites. Players can easily replace the visuals for key game elements by adding their own images to the `UserAssets` folder, following specific naming conventions.

---

## Customizing Sprites

To customize the in-game sprites:

1. Navigate to the game's `UserAssets` folder.
2. Place your custom images in the folder.
3. Ensure your images are correctly named to replace specific in-game objects:
   - `Player` - for the player character sprite.
   - `Enemy` - for enemy sprites.
   - `Jump` - for the jumping action sprite.
   - `Dash` - for the dashing action sprite.
   - `Slam` - for the slamming action sprite.
4. Supported file types:
   - `.png`
   - `.jpg`
   - `.jpeg`

> ⚠️ **Note:** Images must match the above names exactly (case-sensitive). Only correctly named files will replace the corresponding sprites in-game.

---

## Debug Tools & Commands

CustomRoguelike includes tools for debugging and advanced interaction:

### Debug Panel
- Press **\`** (backtick) to toggle the debug panel ON or OFF.
- The debug panel lists various debug details regarding the custom asset loader.

### Command System (Currently contains no actual commands)
- Press **T** to open the command text panel.
- Press **ESC** to close the command text panel.

### Quitting the Game
- Press **Q** to quit the game at any time.

---

## Getting Started

1. Download and run the game.
2. Open the `UserAssets` folder to add your custom sprites (optional).
3. Launch the game and enjoy your personalized roguelike experience (there is no roguelike experience yet...)!
4. Use the debug tools for testing or exploring additional features (there are none yet).

---

## Technical Details

- **Engine**: Unity3D
- **Supported Platforms**: macOS (eventually, Windows)
- **Version**: TBA

---

## Future Plans

- Command system to be updated in the future with commands for spawning enemies or adjusting player stats
- Organize debug panel better if appending messages from other scripts in the future
- Allow users to customize skills and make their own custom skills
- Allow users to customize enemies and make their own custom enemies
- Allow user to fill a folder with various images and randomize all assets in game with said images
- Have in-game file browser to directly set images for certain entities in game
- No longer restrict user to naming image files in specific ways
- Custom audio + music approached in a similar way to the sprites

