# Comic Horror Fishing Buddies
A WIP multiplayer mod for DREDGE. Currently early alpha and might not work as intended. Requires the [Winch modloader](https://github.com/Hacktix/Winch).

![fishing buddies](https://github.com/xen-42/cosmic-horror-fishing-buddies/assets/22628069/d1cd9095-af1c-4296-84b4-d55f0e7a3980)

Huge thanks to JohnCorby for helping me set up Mirror with the mod. And shout-out to \_nebula and everyone who's contributed to [Quantum Space Buddies](https://github.com/misternebula/quantum-space-buddies), for the inspiration and code that I stole.

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

![fishing buddies 2](https://github.com/xen-42/cosmic-horror-fishing-buddies/assets/22628069/fe5177b1-babc-45a6-8070-2327776e938b)

#### What is not synced (roadmap):
- Location/quest specific details
- Events
- Enemies
- Waves
- Collisions?

#### Other todo:
- Implement Steam transport (maybe, if Epic has drawbacks)

## Dependencies
- [Winch](https://github.com/Hacktix/Winch)
- [Mirror](https://mirror-networking.com/)
  - [kcp2k](https://github.com/vis2k/kcp2k)
  - [EpicOnlineTransport](https://github.com/FakeByte/EpicOnlineTransport)
- [HarmonyX](https://github.com/BepInEx/HarmonyX)
