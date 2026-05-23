# Changelog

All notable changes to this project are documented here.

## 1.2.8 - 2026-05-23

- Added filtering for repeated Unity cascade shadow atlasing failure logs.

## 1.2.7 - 2026-04-25

- Added filters for MouthDog noise targeting logs.
- Added filters for cruiser collision debug logs involving `MouthDogModel`.
- Added filtering for MouthDog hit-force log entries.

## 1.2.6 - 2026-04-13

- Added filters for `goUp:` debug output.
- Added exact-match filtering for `True, False` to avoid broad boolean-message suppression.

## 1.2.5 - 2026-04-13

- Added filtering for repeated OpenBodyCams cosmetic collection timing logs.
- Kept matching constrained to known OpenBodyCams timing text to avoid suppressing unrelated `Collected` messages.

## 1.2.4 - 2026-04-13

- Added filters for rope-position and targetability debug logs.

## 1.2.3 - 2026-04-08

- Added filters for Bracken / Flowerman anger-meter and speed-increase debug output.
- Added filters for Cadaver / baby bird distance and scream-timer debug output.

## 1.2.2 - 2026-04-04

- Removed OpenXR loader filtering rules from the managed plugin after confirming those messages are emitted before this plugin can reliably intercept them.

## 1.2.1 - 2026-04-04

- Added filters for repeated Unity negative-scale `BoxCollider` warnings.
- Added filters for repeated audio spatializer initialization warnings.
- Added filters for repeated door-code and outside-AI-node refresh logs.

## 1.2.0 - 2026-04-04

- Added startup version and changelog summary logging.
- Added BepInEx listener wrapping so selected plugin log entries can be filtered in addition to Unity log calls.

## 1.1.x - 2026-04

- Added filtering for Stingray, HoarderBug, Puma, spawn planner, and early repeated debug logs.
- Added support for multiple Unity `Debug` and `Logger` overloads.
