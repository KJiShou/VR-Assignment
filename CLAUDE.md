# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity VR climbing game using the XR Interaction Toolkit. Players climb objects using VR controllers while managing stamina. The game features a chalk mechanic that temporarily boosts maximum stamina.

## Unity Development

- **Open in Unity**: Open `VR Assignment.sln` in Visual Studio or load the project folder in Unity Hub
- **Build**: Use File > Build Settings in Unity Editor
- **VR Testing**: Requires VR hardware or use XR Device Simulator (Window > XR > Device Simulator)

## Key Architecture

### Stamina System
`StaminaManager` is the central singleton managing stamina for both hands:
- Tracks `currentLeftStamina` / `currentRightStamina` and holding states
- Drains stamina when `isLeftHolding` / `isRightHolding` is true
- Recovers stamina when hands are free
- Auto-drops hands when stamina reaches zero via `CheckAutoDrop()`

### Climbing Interaction
`ClimbObject` attaches to any climbable object:
- Listens to XR interactable grab/release events
- Calls `StaminaManager.StartHoldingLeft/Right()` and `StopHoldingLeft/Right()`
- Provides `ForceReleaseLeftHand()` / `ForceReleaseRightHand()` for forced releases
- Has a `reducedStamina` field to control drain rate per object

### Hand Stamina UI
`HandStaminaBarUI` displays per-hand stamina bars:
- Blue bar = base stamina, White bar = chalk bonus, Red bar = missing stamina
- Controlled by StaminaManager's chalk active state and time remaining

### Player Controller
`Player.cs` enables/disables ray interactors when entering "TopPoint" trigger zones and handles teleportation to win screen.

## Scripts Location

- `Assets/Scripts/` - All C# scripts
- `Assets/Prefabs/` - Reusable prefabs (BrickHandle, StaminaCanvas, ClimbCube)
- `Assets/Scenes/` - Game scenes (MainMenu, Tutorial, Stage2)