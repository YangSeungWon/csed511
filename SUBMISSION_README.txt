CSED511 Assignment 1 - Virtual Environment Setup
Student: [Your Name]
Date: September 2025

== PROJECT STATUS ==
✅ All requirements implemented and tested

== FEATURES IMPLEMENTED ==

1. ENVIRONMENT (20 points)
   - 5+ 3D objects: walls, obstacles, ramp, platform
   - All objects have colliders
   - Reasonable layout with varied terrain

2. CONTINUOUS MOVEMENT (20 points)
   - WASD keyboard movement
   - 3 speeds implemented:
     * Walk: 3 units/sec (default)
     * Run: 6 units/sec (hold Shift)
     * Dash: 10 units/sec (hold Ctrl)

3. TELEPORTATION (20 points)
   - Camera-based raycast system
   - Visual line indicator (green=valid, red=invalid)
   - Cylinder destination marker
   - Mouse click to confirm teleport

4. INTERACTION (40 points)
   - Pickup with E key
   - Drop with E key (when holding)
   - Throw with right-click
   - 6 pickupable objects with different physics:
     * Light/Heavy cubes (0.5f vs 5f mass)
     * Small/Large spheres (bouncy material)
     * Cylinder and Capsule variants

== CONTROLS ==
- Move: WASD
- Look: Mouse
- Speed: Shift (run), Ctrl (dash)
- Teleport: Aim and left-click
- Pickup/Drop: E
- Throw: Right-click (while holding)
- Unlock cursor: Escape

== TO BUILD & SUBMIT ==
1. Open project in Unity 6000.0.34f1 or later
2. Open SampleScene from Assets/Scenes/
3. File > Build Settings > Build
4. Assets > Export Package > Select all > Export as Assignment1.unitypackage
5. Create Assignment 1.zip containing:
   - Unity project folder
   - Assignment1.unitypackage
6. Upload to shared drive and share link

== HMD COMPATIBILITY NOTES ==
- Camera system designed for easy VR rig replacement
- Input mapping structured for controller transition
- Interaction distances calibrated for hand tracking
- Teleportation ready for arc visualization