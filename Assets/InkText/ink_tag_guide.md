# Ink Tag Guide

This guide documents all the tags you can use in your Ink scripts to control the Visual Novel system.

## How to use Tags
Tags are special commands added to the end of a line in Ink (or on their own line) that tell the game what to do. They always start with a `#`.
Inside the tag, values are separated by underscores `_`.

**Example:**
```ink
Hello there! #chc_Sera_happy #nm_Sera #d_sera
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
**Syntax:** `#position_CharacterID_clear` or just `#chl_clear` / `#chc_clear` / `#chr_clear` to remove everyone.

**Examples:**
```ink
#chl_Sera_clear   // Removes Sera from the screen
#chc_clear        // Removes ALL characters
```

---

## Animation Tags

You can play animations on characters using the dedicated `an` tag or the inline `+` syntax.

### 1. Dedicated Animation Tag (`an`)
Plays an animation on a character without changing their sprite.

**Syntax:** `#an_CharacterID_AnimationName_OptionalArgs...`

**Example:**
```ink
#an_Sera_shake      // Sera shakes with default settings
#an_Sera_hop_50     // Sera hops with height 50
```

### 2. Inline Animation Syntax (`+`)
Combines a sprite change (or move) with an animation in a single tag. Use `+` to separate the sprite name from the animation.

**Syntax:** `#position_CharacterID_Sprite+Animation`

**Examples:**
```ink
#chl_Sera_Happy+Shake       // Changes Sera to Happy sprite at LEFT position AND Shakes
#chr_Dude+Hop               // Dude Hops on the right (keeping current sprite if 'Dude' is base)
#chc_Clown+Pop_1.2          // Clown Pops with strength 1.2 in center
```

### Available Animations

| Animation | Description | Arguments (Default) | Example |
| :--- | :--- | :--- | :--- |
| **shake** | Shakes horizontally | `Offset` (20), `Duration` (0.7) | `#an_Sera_shake` |
| **shakevertical** | Shakes vertically | `Offset` (20), `Duration` (0.7) | `#an_Sera_shakevertical` |
| **hop** | Jumps UP and back | `Height` (100), `Duration` (0.23) | `#an_Sera_hop` |
| **reversehop** | Jumps DOWN and back | `Height` (33), `Duration` (0.35) | `#an_Sera_reversehop` |
| **bounce** | Bounces scale (Jelly-like) | `Strength` (0.1), `Duration` (0.35) | `#an_Sera_bounce` |
| **pop** | Expands scale briefly | `Strength` (1.05), `Duration` (0.23) | `#an_Sera_pop` |
| **flash** | Flashes white | `Duration` (0.23) | `#an_Sera_flash` |
| **dodge** | Slides SIDEWAYS and back | `Distance` (100), `Duration` (0.35) | `#an_Sera_dodge` |

**Note:** Duration is in seconds.

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

## Dialogue Voice Tag (`d`)
Enables character voice playback for the current line. When set, the typewriter will play voice blips for each character revealed.

**Syntax:** `#d_CharacterName`

**Examples:**
```ink
Hello there! #d_sera               // This line plays Sera's voice blips
How are you? #nm_Sera #d_sera      // Common pattern: name + voice together
(The wind howls softly.)           // No #d tag = no voice (narration)
```

**Note:** The character name must match a key in the `VoicesMap` dictionary.

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
