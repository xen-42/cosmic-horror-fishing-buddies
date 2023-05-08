# Comic Horror Fishing Buddies
A multiplayer mod for DREDGE

Huge thanks to JohnCorby for helping me set up Mirror with the mod. And shout-out to \_nebula and everyone who's contributed to [Quantum Space Buddies](https://github.com/misternebula/quantum-space-buddies), for the inspiration and code that I stole.

## Features
What is synced (some spoilers here):
- Player movement
- Engine sounds & ship wake
- Time
  - Fast forwarding - all players must be fast-forwarding too or at least in port for time to speed up
- Fishing 
  - Regular spots (not bait/crab pots)
- Abilities
  - Light
  - Foghorn
  - Manifest
  - Banish (has effects but only works locally)
  - Atrophy
  - Haste (missing vfx)
  - Trawl net
- Ship appearance
  - Missing fish containers and rudder

What is not synced (roadmap):
- Abilities
  - Crab pots
  - Bait
- Events
- Enemies
- Waves
- Collisions?

Other todo:
- Implement Epic transport
- Implement Steam transport (maybe, if Epic has drawbacks)

## Dependencies
- [Winch](https://github.com/Hacktix/Winch)
- [Mirror](https://mirror-networking.com/)
  - [kcp2k](https://github.com/vis2k/kcp2k)
  - [EpicOnlineTransport](https://github.com/FakeByte/EpicOnlineTransport)
- [HarmonyX](https://github.com/BepInEx/HarmonyX)
