# Qsys
Qsys library for S#P and SIMPL

Note: Releases exclusively consist of compiled modules at the moment of their release. Within this repository, you can find exemplar programs situated in the SIMPL directory as well as the Q-Sys Designer File directory. Please bear in mind that these exemplary programs encompass the most up-to-date branch of compiled modules.

---

## Added Modules

### Qsys Nv21h Decoder
**SIMPL+ Module:** `Qsys Nv21h Decoder.usp`

Controls the QSC NV-21-HU HDMI over IP decoder endpoint via a named component in Q-SYS Designer. Allows analog input source selection with feedback.

| Signal | Type | Description |
|---|---|---|
| `Source` | Analog Input | HDMI input source index to select |
| `CurrentSource` | Analog Output | Currently selected input source feedback |

**Parameters:** `CoreID`, `ComponentName`

---

### Qsys Camera Presets
**SIMPL+ Module:** `Qsys Camera Presets.usp`

Full camera preset management for up to 6 cameras with 16 presets each. Supports preset recall and save, pan/tilt/zoom progress feedback, camera tracking, router output selection, and preset file load/save.

**Inputs:** Camera select (1–6), Preset recall/save (1–16), Tracking on/off, Router output (1–11), Load/Save/Recalibrate/Refresh

**Outputs:** Camera select feedback, Preset button feedback, Pan & tilt progress, Tracking feedback, Router output feedback, Camera names, Preset names, Status text

**Parameters:** `CoreID`, `ComponentName`
