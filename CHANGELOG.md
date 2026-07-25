# Changelog

All notable changes to this project will be documented in this file.

## [0.2.6]
- **Compatibility:** Restored AssemblyName to `ProjectMER` (from `FlaProjectMER`) to resolve loading issues with dependent plugins such as `MEROptimizer`.

## [0.2.5]
- **Pickup:** Added support for weapon attachments code serialization and deserialization.
- **Pickup:** Added support for Number of Uses (Uses property) to allow finite or infinite item spawns.
- **Pickup:** Added Locked property support to turn standard pickups into interactable buttons.

## [0.2.4]
- **Waypoint:** Added Priority property support (0-255) for bot navigation node configurations.

## [0.2.3]
- **Debug:** Replaced `Logger.Debug` calls inside debug blocks with `Logger.Info` to bypass LabAPI's framework debug log suppression.

## [0.2.2]
- **Interactable:** Fixed Animator lookup fallback chain (self -> children -> parents).
- **Core:** Added version metadata configuration check.

## [0.2.1]
- **Version:** Added `/version` utility commands (`mp version`, `mp ver`, `mp v`) to query active build version in RA console.

## [0.2.0]
- **Interactable:** Changed `TargetAnimator` property to `TargetObject` of type GameObject for simpler drag-and-drop operations in Unity Editor.
- **Core:** Prevented server crash due to duplicate Object IDs in schematic block deserialization.

## [1.0.0]
- Forked ProjectMER to FlaProjectMER.
- Added local dependency reference configurations.
- Integrated GitHub Actions CI/CD workflow for automated building and releases.
