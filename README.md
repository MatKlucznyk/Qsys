# Qsys
Qsys library for S#P and SIMPL

Note: Releases exclusively consist of compiled modules at the moment of their release. Within this repository, you can find exemplar programs situated in the SIMPL directory as well as the Q-Sys Designer File directory. Please bear in mind that these exemplary programs encompass the most up-to-date branch of compiled modules.

---

## Added Modules

### Qsys Nv Decoder
**SIMPL+ Module:** `Qsys Nv Decoder.usp`

Controls a QSC NV Series (e.g. NV-21-HU, NV-32-H) HDMI over IP decoder endpoint via a named component in Q-SYS Designer. Allows analog input source selection with feedback on a selected HDMI output.

| Signal | Type | Description |
|---|---|---|
| `Source` | Analog Input | HDMI input source index to select |
| `CurrentSource` | Analog Output | Currently selected input source feedback |

**Parameters:** `CoreID`, `ComponentName`, `OutputNumber` (HDMI output to control, 0-2, default 1)

Replaces the former `Qsys Nv21h Decoder` and `Qsys Nv32h Decoder` modules. For an NV-21-HU, leave `OutputNumber` at 1.

---

### Qsys Nvm Decoder
**SIMPL+ Module:** `Qsys Nvm Decoder.usp`

Controls a QSC NVM Series HDMI over IP decoder endpoint via a named component in Q-SYS Designer. Allows analog input source selection with feedback, plus HDMI video freeze, video mute, and output enable.

| Signal | Type | Description |
|---|---|---|
| `VideoFreezeOn` / `VideoFreezeOff` / `VideoFreezeToggle` | Digital Input | Set, clear, or toggle HDMI video freeze |
| `VideoMuteOn` / `VideoMuteOff` / `VideoMuteToggle` | Digital Input | Set, clear, or toggle HDMI video mute |
| `HdmiEnabledOn` / `HdmiEnabledOff` / `HdmiEnabledToggle` | Digital Input | Enable, disable, or toggle the HDMI output |
| `Source` | Analog Input | HDMI input source index to select |
| `VideoFreezeIsOn` / `VideoFreezeIsOff` | Digital Output | Video freeze state feedback |
| `VideoMuteIsOn` / `VideoMuteIsOff` | Digital Output | Video mute state feedback |
| `HdmiEnabledIsOn` / `HdmiEnabledIsOff` | Digital Output | HDMI output enabled state feedback |
| `CurrentSource` | Analog Output | Currently selected input source feedback |

**Parameters:** `CoreID`, `ComponentName`

---

### Qsys Camera Presets
**SIMPL+ Module:** `Qsys Camera Presets.usp`

Full camera preset management for up to 6 cameras with 16 presets each. Supports preset recall and save, pan/tilt/zoom progress feedback, camera tracking, router output selection, and preset file load/save.

**Inputs:** Camera select (1–6), Preset recall/save (1–16), Tracking on/off, Router output (1–11), Load/Save/Recalibrate/Refresh

**Outputs:** Camera select feedback, Preset button feedback, Pan & tilt progress, Tracking feedback, Router output feedback, Camera names, Preset names, Status text

**Parameters:** `CoreID`, `ComponentName`
