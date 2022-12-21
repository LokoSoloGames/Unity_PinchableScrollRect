# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.5] - 2022-12-22
### Fixed
- Fix an incorrect zooming behaviour on ScreenSpace-Camera render mode.
### Updated
- Optimize Calculation during zoom deceleration.
- Update a proper way to handle stretched anchor content.

## [1.0.4] - 2022-09-15
### Fixed
- Fix an incorrect zooming behaviour on ScreenSpace-Camera & WorldSpace render mode.

## [1.0.3] - 2022-08-20
### Fixed
- Fix an incorrect zooming behaviour with Canvas Scalar.
### Added
- An Unity Event API for scale callback.
- Set Scale API Method for external control for zooming.

## [1.0.2] - 2022-07-16
### Fixed
- Fix an incorrect zooming behaviour when Content's RectTransform has a stretched anchor.

## [1.0.1] - 2022-05-30
### Added
- Inspector Shortcut Button to replace built-in ScrollRect to PinchableScrollRect Component.

## [1.0.0] - 2022-02-12
### Added
- PinchableScrollRect Component
- IPinchHandler Interface for extending pinching behaviours
- PinchInputDetector Component to convert pointer event to pinch event