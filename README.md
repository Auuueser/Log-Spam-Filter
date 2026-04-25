# Log Spam Filter

Log Spam Filter is a BepInEx plugin for Lethal Company that suppresses selected high-frequency Unity and BepInEx log messages. It is intended for heavily modded test profiles where repeated debug output makes `LogOutput.log` difficult to inspect.

The plugin only filters log output. It does not modify enemy AI, spawning, networking, physics, save data, or gameplay state.

## Features

- Filters selected `UnityEngine.Debug` and `UnityEngine.Logger` messages before they are written.
- Wraps existing BepInEx log listeners so selected plugin log entries can also be suppressed.
- Uses exact, prefix, and contained-text rules to keep broad matching limited to known spam patterns.
- Emits its plugin version and changelog summary on startup.

## Current Version

`1.2.7`

## Filter Coverage

The current rule set includes repeated logs from these areas:

- Bracken / Flowerman anger and speed debug output
- Cadaver / baby bird distance and scream timer debug output
- MouthDog noise targeting and cruiser collision debug output
- Stingray movement and audio debug output
- HoarderBug target-object debug output
- OpenBodyCams cosmetic collection timing output
- Spawn planner probability debug output
- Rope, targeting, and boolean debug messages
- Common Unity warning spam for negative collider scale and missing audio spatializer setup

## Requirements

- Lethal Company
- BepInEx 5.x
- .NET SDK capable of building `net472`

## Build

The project does not store local game or mod-manager paths. Provide them at build time:

```powershell
dotnet build .\src\LogSpamFilter.csproj `
  -p:BepInExCoreDir="D:\path\to\BepInEx\core" `
  -p:GameManagedDir="D:\Steam\steamapps\common\Lethal Company\Lethal Company_Data\Managed"
```

The output assembly is `LogSpamFilter.dll`.

## Installation

Place `LogSpamFilter.dll` in a BepInEx plugins folder, for example:

```text
BepInEx/plugins/LogSpamFilter/LogSpamFilter.dll
```

## Notes

This plugin cannot suppress OpenXR loader messages emitted before BepInEx plugins and patchers can intercept managed logging. Those messages originate from Unity/OpenXR startup initialization and require a game startup configuration change to prevent at the source.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.
