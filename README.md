# Comic Horror Fishing Buddies

![GitHub all releases](https://img.shields.io/github/downloads/xen-42/cosmic-horror-fishing-buddies/total?style=for-the-badge)
![GitHub release (latest by date)](https://img.shields.io/github/downloads/xen-42/cosmic-horror-fishing-buddies/latest/total?style=for-the-badge)

A WIP multiplayer mod for DREDGE. Currently early alpha and might not work as intended. Installable using the [Dredge Mod Manager](https://github.com/DREDGE-Mods/DredgeModManager).

![cosmic horror fishing buddies](https://github.com/xen-42/cosmic-horror-fishing-buddies/assets/22628069/47687a63-0313-41dd-9d79-adcf58372c06)

Huge thanks to JohnCorby for helping me set up Mirror with the mod. And shout-out to \_nebula and everyone who's contributed to [Quantum Space Buddies](https://github.com/misternebula/quantum-space-buddies), for the inspiration ~~and code that I stole~~.

## How to Install
Download the DREDGE Mod Manager from [dredgemods.com](https://dredgemods.com/)!

## Known issues
If a player dredges up a quest related item, that dredge spot is synced to all other players as being empty. However, this means no other player will be able to get that quest item, even when they switch to single player.
For now, to correct this, you can install the [Quest Item Unlocker](https://dredgemods.com/mods/quest_item_unlocker/) mod which will reset all the dredge spots when you restart the game.

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
