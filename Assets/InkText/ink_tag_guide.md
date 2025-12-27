# Ink Tag Guide

This guide documents all the tags you can use in your Ink scripts to control the Visual Novel system.

## How to use Tags
Tags are special commands added to the end of a line in Ink (or on their own line) that tell the game what to do. They always start with a `#`.
Inside the tag, values are separated by underscores `_`.

**Example:**
```ink
Hello there! #ch_Sera_happy
```

---

## Character Tags (`ch`, `chl`, `chc`, `chr`)
Control character sprites on screen.

### Showing a Character
*   **#ch**: Show character in the **Center** (Default).
*   **#chl**: Show character on the **Left**.
*   **#chc**: Show character in the **Center**.
*   **#chr**: Show character on the **Right**.

**Syntax:** `#position_CharacterID_SpriteName`
If `CharacterID` and `SpriteName` are the same, you can just type it once.

**Examples:**
```ink
#ch_Sera_happy          // Shows Sera in center with 'happy' sprite
#chl_Sera_sad           // Moves Sera to left and changes to 'sad' sprite
#chr_Samurai            // Shows Samurai on right
```

### Using Aliases (Multiple characters sharing sprites)
You can assign a specific unique ID to a character while using a shared sprite asset.
**Syntax:** `#position_UniqueID:SpriteName_Emotion`

**Example:**
```ink
#chl_Guard1:Samurai_idle  // Shows "Guard1" using "Samurai_idle" sprite
#chr_Guard2:Samurai_alert // Shows "Guard2" using "Samurai_alert" sprite
```

### Removing Characters
**Syntax:** `#position_CharacterID_clear` or just `#ch_clear` to remove everyone.

**Examples:**
```ink
#chl_Sera_clear   // Removes Sera from the screen
#ch_clear         // Removes ALL characters
```

---

## Animation Tag (`an`)
Plays a special animation on a specific character.

**Syntax:** `#an_CharacterID_AnimationName_OptionalArgs...`

### Available Animations

| Animation | Description | Arguments | Example |
| :--- | :--- | :--- | :--- |
| **shake** | Shakes horizontally | `Intensity` (def: 10), `Duration` (def: 1) | `#an_Sera_shake_10_0.5` |
| **shakevertical** | Shakes vertically | `Intensity` (def: 10), `Duration` (def: 1) | `#an_Sera_shakevertical` |
| **hop** | Hops up | `Height` (def: 30), `Duration` (def: 0.5) | `#an_Sera_hop` |
| **reversehop** | Hops down | `Height` (def: 15), `Duration` (def: 0.5) | `#an_Sera_reversehop` |
| **punch** | "Punches" (scales up) | `Strength` (def: 0.1), `Duration` (def: 0.5) | `#an_Sera_punch` |
| **flash** | Flashes white | `Duration` (def: 0.5) | `#an_Sera_flash` |
| **dodge** | Slides side-to-side | `Distance` (def: 20), `Duration` (def: 0.5) | `#an_Sera_dodge` |

**Note:** If you don't provide arguments, the defaults will be used.

---

## Name Tag (`nm`)
Updates the name displayed in the name box.

**Syntax:** `#nm_NameToShow`

**Examples:**
```ink
#nm_Sera      // Sets name to "Sera"
#nm_The Boss  // Sets name to "The Boss"
#nm_none      // Hides the name box
```

---

## Audio & Visual Tags

### Background (`bg`)
Changes the background image.
**Syntax:** `#bg_BackgroundName`
**Example:** `#bg_park_morning`

### Music (`ms`)
Plays background music.
**Syntax:** `#ms_SongName`
**Example:** `#ms_battle_theme`

### Ambience (`ab`)
Plays ambient looping sound (rain, wind, etc).
**Syntax:** `#ab_AmbienceName`
**Example:** `#ab_rain_heavy`

### Sound Effects (`sx`)
Plays a one-shot sound effect.
**Syntax:** `#sx_SoundName`
**Example:** `#sx_door_slam`
