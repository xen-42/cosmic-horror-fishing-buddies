# Comic Horror Fishing Buddies

![GitHub all releases](https://img.shields.io/github/downloads/xen-42/cosmic-horror-fishing-buddies/total?style=for-the-badge)
![GitHub release (latest by date)](https://img.shields.io/github/downloads/xen-42/cosmic-horror-fishing-buddies/latest/total?style=for-the-badge)
[![Support me on Patreon](https://img.shields.io/endpoint.svg?url=https%3A%2F%2Fshieldsio-patreon.vercel.app%2Fapi%3Fusername%3Dxen42%26type%3Dpatrons&style=for-the-badge)](https://patreon.com/xen42)

A WIP multiplayer mod for DREDGE. Currently early alpha and might not work as intended. Installable using the [Dredge Mod Manager](https://github.com/DREDGE-Mods/DredgeModManager).

Unfortunately connecting via EOS does **NOT** work with the Epic Games store version. This means you will have to manually set up port forwarding or use a VPN connection service. When connecting via IP adress the port used is **7777**.

![cosmic horror fishing buddies](https://github.com/xen-42/cosmic-horror-fishing-buddies/assets/22628069/47687a63-0313-41dd-9d79-adcf58372c06)

Huge thanks to JohnCorby for helping me set up Mirror with the mod. And shout-out to \_nebula and everyone who's contributed to [Quantum Space Buddies](https://github.com/misternebula/quantum-space-buddies), for the inspiration ~~and code that I stole~~.

## How to Install
Download the DREDGE Mod Manager from [dredgemods.com](https://dredgemods.com/)!

## Features (spoilers)
#### What is synced:
- Player movement
- Engine sounds & ship wake
- Time
  - Fast forwarding - all players must be fast-forwarding too or at least in port for time to speed up
- Fishing spots
  - Stock is synced but "special" status isn't currently
- Abilities
  - Light
  - Foghorn
  - Manifest
  - Banish (has effects but only works locally)
  - Atrophy
  - Haste (missing vfx)
  - Trawl net
  - Bait
  - Crab pots (inventory not yet synced)
- Ship appearance
  - Rigidbody boat assets (net, tires) don't animate yet
- World Events
  - Only the occurrence of events is currently synced, will also have to sync the movement of whatever is spawned by the event

![fishing buddies 2](https://github.com/xen-42/cosmic-horror-fishing-buddies/assets/22628069/fe5177b1-babc-45a6-8070-2327776e938b)

#### What is not synced (roadmap):
- Location/quest specific details
- Weather
- Enemies
- Waves
- Collisions?

## Dependencies (included)
- [Winch](https://github.com/DREDGE-Mods/Winch)
- [Mirror](https://mirror-networking.com/)
  - [kcp2k](https://github.com/vis2k/kcp2k)
  - [EpicOnlineTransport](https://github.com/FakeByte/EpicOnlineTransport)
- [HarmonyX](https://github.com/BepInEx/HarmonyX)
