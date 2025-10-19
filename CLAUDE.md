# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Unity VR assignment implementing a virtual environment with both desktop and VR HMD support. The project has evolved from Assignment 1 (keyboard/mouse controls) to Assignment 2+ (full VR support with XR Interaction Toolkit).

Unity Version: 2021.3.45f2

## Project Architecture

### Dual Control Systems

The project supports both desktop and VR modes through parallel controller systems:

**Desktop Controllers** (Assignment 1 - Legacy):
- **PlayerMovementController** (Assets/Scripts/PlayerMovementController.cs): WASD movement with 3 speeds (walk/run/dash via Shift/Ctrl)
- **TeleportationController** (Assets/Scripts/TeleportationController.cs): Camera-based raycast teleportation with visual indicator (T key)
- **InteractionController** (Assets/Scripts/InteractionController.cs): Pickup/drop/throw mechanics for rigid objects (E to pickup, right-click to throw)

**VR Controllers** (Assignment 2):
- **VRMovementSpeedController** (Assets/Scripts/VRMovementSpeedController.cs): Manages movement speeds (walk/run/dash) for ActionBasedContinuousMoveProvider
- **VRObjectSpawner** (Assets/Scripts/VRObjectSpawner.cs): Procedurally spawns interactable objects with varying physics properties (mass, bounciness) using XRGrabInteractable
- **VRTriggerStateMonitor** (Assets/Scripts/VRTriggerStateMonitor.cs): Monitors VR controller trigger and grip button states (NotPressed/HalfPressed/FullyPressed)
- **VRObjectInfoMonitor** (Assets/Scripts/VRObjectInfoMonitor.cs): Tracks currently grabbed objects and displays their properties
- **VRUIManager** (Assets/Scripts/VRUIManager.cs): Central UI controller that manages speed display, trigger status, object info, and spawner buttons
- **CrosshairUI** (Assets/Scripts/CrosshairUI.cs): Visual feedback for interaction systems

**Mini-Game System** (Assignment 3 - Target Shooting):
- **GunController** (Assets/Scripts/GunController.cs): Handles gun grabbing, trigger detection, bullet spawning, haptic/audio feedback. Each gun fires a different colored bullet (red/blue/white)
- **Bullet** (Assets/Scripts/Bullet.cs): Bullet physics with collision detection, self-destruct after 10s, awards points based on color when hitting targets
- **Target** (Assets/Scripts/Target.cs): Auto-destructs after 5 seconds, destroyed on bullet collision
- **TargetSpawner** (Assets/Scripts/TargetSpawner.cs): Spawns targets every 5 seconds at predefined spawn points
- **ScoreManager** (Assets/Scripts/ScoreManager.cs): Singleton that tracks score, awards points based on bullet color (Red: +50, Blue: +30, White: +10), updates UI

### Key Dependencies

The VR system relies on:
- **XR Interaction Toolkit** (2.6.5): Core VR interaction framework
- **XR Plugin Management** (4.5.2): VR platform management
- **OpenXR** (1.13.2): Cross-platform VR runtime
- **Oculus XR Plugin** (3.4.1): Meta Quest support
- **Input System** (1.11.2): New Unity input system for VR controllers
- **TextMeshPro** (3.0.9): UI text rendering

### Scene Structure

- Main scene: Assets/Scenes/SampleScene.unity
- VR rig: XR Origin (XR Rig) with ActionBasedControllers for left/right hands
- Objects tagged as "Pickupable" have XRGrabInteractable components
- World-space UI canvas attached to VR rig for HUD display

### VR Interaction Pattern

1. Objects are spawned via VRObjectSpawner with configurable physics properties
2. XRGrabInteractable components enable VR controller grabbing using grip/trigger buttons
3. VRTriggerStateMonitor tracks controller input states (analog trigger values)
4. VRObjectInfoMonitor detects grab/release events and displays object metadata
5. VRUIManager orchestrates all UI updates in world-space canvas

## Building and Running

### Unity Editor
1. Open project in Unity 2021.3.45f2
2. Open SampleScene from Assets/Scenes/
3. Press Play to test (VR mode requires connected HMD, otherwise uses XR Device Simulator)

### VR Testing Without HMD
The project includes XR Device Simulator from XR Interaction Toolkit samples. Enable it in XR Origin for keyboard/mouse VR simulation during development.

### Build for Standalone VR
```
File > Build Settings >
Select platform (PC/Mac/Android for Quest)
Player Settings > XR Plug-in Management > Enable OpenXR
Add Open Scenes >
Build
```

### Meta Quest Deployment
1. Switch platform to Android in Build Settings
2. Enable OpenXR in XR Plug-in Management (Android tab)
3. Configure Oculus feature set in OpenXR settings
4. Build and deploy via ADB or direct USB connection

## Input Mappings

**Desktop Mode** (Legacy):
- Movement: WASD keys
- Speed modifiers: Shift (run), Ctrl (dash)
- Camera: Mouse look
- Teleport: Hold T key, release to teleport
- Interact: E (pickup/drop), Right-click (throw)
- Cursor unlock: Escape key

**VR Mode**:
- Movement: Left thumbstick (continuous movement via ActionBasedContinuousMoveProvider)
- Rotation: Right thumbstick (snap turn or continuous)
- Speed toggle: UI button in world-space HUD
- Grab objects: Grip button on either controller
- Teleport: Built into XR Interaction Toolkit (if TeleportationProvider configured)
- Object spawn: UI buttons in world-space HUD

## Development Notes

### Adding New Spawnable Objects
Edit VRObjectSpawner.spawnableObjects list with SpawnableObject entries specifying:
- Primitive type (Cube/Sphere/Capsule/Cylinder)
- Scale, mass, bounciness, color
- PhysicMaterialCombine mode for bounce behavior

### Modifying Movement Speeds
Adjust VRMovementSpeedController fields: walkSpeed, runSpeed, dashSpeed. These sync to ActionBasedContinuousMoveProvider.moveSpeed.

### Extending UI Displays
VRUIManager.Update() polls monitor components each frame. Add new monitor scripts following the pattern of VRTriggerStateMonitor (expose public properties and getter methods).

### Setting Up Assignment 3 Mini-Game

**Gun Setup**:
1. Import 3 gun models from Unity Asset Store
2. Add XRGrabInteractable component to each gun
3. Add GunController component and configure:
   - Set bulletColor (Red/Blue/White)
   - Assign bulletPrefab (sphere primitive with Rigidbody)
   - Assign muzzlePoint (child Transform at barrel end)
   - Assign gunShotSound AudioClip
   - Configure haptic settings (intensity: 0.5, duration: 0.1)

**Bullet Prefab**:
1. Create sphere primitive (scale 0.1)
2. Add Rigidbody (useGravity: false, Continuous collision detection)
3. Add Bullet component
4. Save as prefab in Assets/Prefabs/

**Target Setup**:
1. Create target prefab (cube/sphere with distinct appearance)
2. Add Collider and Rigidbody (isKinematic: true)
3. Add Target component
4. Save as prefab

**Scene Setup**:
1. Create empty GameObject for TargetSpawner
2. Configure spawn points (manually or use auto-generation)
3. Assign target prefab
4. Set spawnInterval to 5 seconds
5. Create ScoreManager GameObject
6. Add world-space Canvas for score UI
7. Connect ScoreManager to UI TextMeshProUGUI elements

**Scoring System**:
- Red bullets: 50 points per hit
- Blue bullets: 30 points per hit
- White bullets: 10 points per hit
- Targets self-destruct after 5 seconds (no points)
- Score UI updates immediately on hit

## Testing Focus Areas

**Assignment 2 (VR Interaction)**:
1. VR controller trigger/grip state detection accuracy
2. Object grab/release mechanics with different mass values
3. Physics material behavior (bounciness combinations)
4. Movement speed cycling via UI button
5. Object spawning in front of player view
6. World-space UI readability and button interaction
7. XR Device Simulator functionality for desktop testing

**Assignment 3 (Mini-Game)**:
1. Gun grabbing with Grip button from either controller
2. Bullet spawning on Trigger press with correct color (red/blue/white)
3. Haptic feedback intensity and duration on firing
4. Gunshot audio playback
5. Target spawning every 5 seconds at predefined locations
6. Target self-destruct after 5 seconds
7. Bullet-target collision detection and destruction
8. Score increment based on bullet color (+50 red, +30 blue, +10 white)
9. Score UI immediate update after hits
10. Bullet self-destruct after 10 seconds to prevent clutter