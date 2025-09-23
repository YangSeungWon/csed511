# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Unity VR assignment implementing a virtual environment with keyboard/mouse controls (Assignment 1), designed for future HMD compatibility (Assignment 2+).

Unity Version: 6000.0.34f1

## Project Architecture

### Core Controllers
All controllers are attached to a single Player GameObject for centralized control:

- **PlayerMovementController** (Assets/Scripts/PlayerMovementController.cs): WASD movement with 3 speeds (walk/run/dash via Shift/Ctrl)
- **TeleportationController** (Assets/Scripts/TeleportationController.cs): Camera-based raycast teleportation with visual indicator (T key)
- **InteractionController** (Assets/Scripts/InteractionController.cs): Pickup/drop/throw mechanics for rigid objects (E to pickup, right-click to throw)
- **EnvironmentSetup** (Assets/Scripts/EnvironmentSetup.cs): Procedural environment generation with walls, obstacles, and interactable objects

### Input Mapping
- Movement: WASD keys
- Speed modifiers: Shift (run), Ctrl (dash)
- Camera: Mouse look
- Teleport: Hold T key, release to teleport
- Interact: E (pickup/drop), Right-click (throw)
- Cursor unlock: Escape key

### Scene Structure
- Main scene: Assets/Scenes/SampleScene.unity
- Player prefab created at runtime by EnvironmentSetup
- Environment objects generated procedurally with colliders
- Pickupable objects tagged as "Pickupable" with varying physics properties

## Building and Running

### Unity Editor
1. Open project in Unity 6000.0.34f1 or later
2. Open SampleScene from Assets/Scenes/
3. Press Play to test in editor

### Build for Standalone
```
File > Build Settings >
Select target platform >
Add Open Scenes >
Build
```

### Export Package
```
Assets > Export Package >
Select all project files >
Export as Assignment1.unitypackage
```

## HMD Compatibility Considerations

When extending for VR (Assignment 2+):
- Camera system in PlayerMovementController needs VR camera rig replacement
- Input system should transition to VR controller inputs
- Teleportation arc visualization for VR controllers
- Interaction distance and hold position adjustments for VR hands
- UI elements need VR-compatible rendering

## Testing Focus Areas

1. Movement speeds differentiation (walk/run/dash)
2. Teleportation indicator visibility and accuracy
3. Object pickup physics (mass differences between objects)
4. Collision detection between held objects and environment
5. Throw force consistency across different object masses