# .github/copilot-instructions.md

## Mod Overview and Purpose

**Mod Name**: Red Alert 2 (Continued)

This mod is an extension and update of the original Red Alert 2 mod by AKreedz, incorporating additional features such as Trait Value support. It aims to enrich RimWorld gameplay by introducing elements from the classic Red Alert 2 and Mental Omega universe, including new factions, units, defenses, and unique mechanics. The mod is designed to provide players with a fresh and challenging experience by introducing the factions of the Soviet Union, Allies, and Yuri as enemies with distinctive traits and capabilities.

## Key Features and Systems

- **Factions**: Introducing new enemy factions - Soviet, Allied, and Yuri. Each faction comes with its own unique units and mechanics.

- **Enemy Units**:
  - Infantry: Conscript, G.I, Initiate, Tesla Trooper, and more.
  - Special Units: Chrono Legionnaire, Tanya, Desolator, Yuri.
  - Vehicles: Apocalypse Tank, Rhino Tank, Grizzly Tank.

- **Defense Architectures**:
  - Buildings include Sentinel Gun, Pillbox, Tesla Coil, Prism Tower, and Grand Cannon, providing robust defense options.

- **Unique Mechanisms**:
  - Super Weapons, Soviet technological advancements, and the use of Faction Tech Cores for unlocking new capabilities.

- **Miscellanies**:
  - New building options like Tesla Reactor, Nuclear Reactor, Barracks, Ore Refinery, and Grinder enhance strategy elements.
  - Factions have unique musical themes that play during invasions.

## Coding Patterns and Conventions

- Organized by class responsibility, such as `Comp` classes for component functionality and `Harmony` classes for patches.
- Follow naming conventions such as `PascalCase` for classes and methods.
- Group related classes and ensure file names are descriptive of their purpose.

## XML Integration

- XML is used for defining game objects, textures, and other resources.
- Ensure XML files are well-structured and validate them using available tools to prevent in-game errors.
- Maintain a clear hierarchy for better readability and management.

## Harmony Patching

- Utilize Harmony to modify base game methods without altering original code directly.
- Patches are typically organized within `Harmony_[Feature].cs` files, encapsulating modifications and enhancements.
- Ensure patches are as non-invasive as possible to maintain compatibility with other mods.

## Suggestions for Copilot

- Assist in generating boilerplate code for C# classes and methods.
- Help with creating Harmony patches by suggesting which methods to prefix, postfix, or transpile.
- Generate XML templates for new content such as items, buildings, or factions.
- Suggest tests and validate gaming logic for complex interactions in the mod.
- Assist with best practices for coding patterns in Unity and RimWorld specifically.

This documentation aims to guide contributors and AI tools in maintaining and extending the Red Alert 2 (Continued) mod effectively by following established conventions and practices. The focus is on modular, maintainable, and expandable coding architectures. Happy modding!


This instruction file outlines the main objectives, features, and structure of the Red Alert 2 (Continued) mod, providing a comprehensive guide for developers and AI assistants to contribute effectively.
