# 🛠️ Haven Zone Creator

A powerful Zone Creation tool for **SPT mod developers** to create custom zones with ease for VCQL, Loose Loot, and more.

---

## 🌟 Features

- **Zone Information:**
    - Current Look Position: The position you were looking at when pressing 'Fetch Look Position'.
    - Current Look Rotation: The rotation you were looking at when pressing 'Fetch Look Position'.
    - Current Look Scale: The scale you were looking at when pressing 'Fetch Look Position'.
    - Look Position Cube Transparency: Transparency of the look position cube.

- **Zone Box Settings:**
    - Transform Speed: The speed at which the object is transformed.
    - Fetch Look Position: Fetches the position you are looking at.
    - Remove Look Position: Removes the look position cube.
    - Transform Positive X: Move Positive X axis.
    - Transform Negative X: Move Negative X axis.
    - Transform Positive Y: Positive Y axis.
    - Transform Negative Y: Negative Y axis.
    - Transform Positive Z: Positive Z axis.
    - Transform Negative Z: Negative Z axis.
    - Position: Position mode.
    - Scale: Change to Scale mode.
    - Rotate: Change to Rotate mode.
    - Lock X And Z Rotation Axes: Change to Lock X and Z rotation axes.

- **VCQL Zone Settings:**
    - Zone Id: The id of the zone.
    - Zone Name: The name of the zone.
    - Zone Type: The type of the zone.
    - Flare Type: The type of the flare.
    - Generate VCQL Zone: Generates the Zone in the VCQL Zone folder.

---

## 📥 Installation

1. Download and extract the mod into the main SPT folder.
2. Ensure the `.dll` file is placed inside the `BepInEx/plugins` directory.

---

## 🕹️ Usage

### Zone Creation

Cube Creation:
- **Fetch Look Position** to get the position you are looking at.
    - Numpad 0: Creates a cube at the look position. (can be a bit buggy due to EFT colliders)
    - Numpad Enter: Removes the cube.

PositionMode:
- Numpad 1: Move Positive X axis.
- Numpad 4: Move Negative X axis.
- Numpad 2: Positive Y axis.
- Numpad 5: Negative Y axis.
- Numpad 3: Positive Z axis.
- Numpad 6: Negative Z axis.

Rotation Mode:
- Numpad 1: Rotate Positive X axis.
- Numpad 4: Rotate Negative X axis.
- Numpad 2: Positive Y axis.
- Numpad 5: Negative Y axis.
- Numpad 3: Positive Z axis.
- Numpad 6: Negative Z axis.

Scale Mode:
- Numpad 1: Scale Positive X axis.
- Numpad 4: Scale Negative X axis.
- Numpad 2: Positive Y axis.
- Numpad 5: Negative Y axis.
- Numpad 3: Positive Z axis.
- Numpad 6: Negative Z axis.

Toggle Modes:
- Numpad 7: Position mode.
- Numpad 8: Rotation mode.
- Numpad 9: Scale mode.

Generating Jsons:
- **Generate VCQL Zone** to generate the zone in the VCQL Zone folder with the specified information you entered.
- **Generate Loose Loot Zone** to generate the zone in the Loose Loot Zone folder with the specified information you entered.

- Use the **F12 menu** to tailor settings, such as bot behavior or player enhancements.

---

## ❗ Notes

- The **Fetch Look Position** feature can be a bit buggy due to EFT colliders.

---

## 📜 License

This mod is distributed **for free use**. Feel free to modify, but proper credit is appreciated.

---

**Enjoy creating custom zones!** 🎉

---