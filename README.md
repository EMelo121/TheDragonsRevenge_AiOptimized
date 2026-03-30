This project is a turn-based RPG inspired by classic titles such as early Final Fantasy games. It was developed in Unity using C# and focuses on overworld exploration, 
turn-based combat, and menu-driven player interaction.
The primary goal for revisiting this project is to demonstrate proficiency in using generative AI as a development tool for improving code quality, architecture, 
and maintainability.

**Core Features:**
_Overworld System:_
-Player movement and interaction
-Enemy encounter tracking
-Item collection and persistence
-Scene transitions
_Combat System:_
-Turn-based battle flow
-Player and enemy stat systems
-Animation-driven attacks
-Experience and leveling system
-Multi-enemy combat support
_Menu System:_
-UI-driven interaction
-Inventory and progression display
-Combat feedback panels

**Tech Stack:**
Unity (2D)
C#
Visual Studio
Git / GitHub

**Major Refactor Highlights:**

**1. Enemy System Architecture Overhaul**
_Before:_
-Each enemy script contained:
-Damage logic
-UI updates
-Mana handling
-Player interaction logic
-~250–350 lines per enemy script
_After:_
-Introduced a shared base class: EnemyBattleStats
-Centralized:
-Damage handling
-UI updates
-Mana logic
-Combat interactions

_Result:_
-Enemy scripts reduced to ~25–40 lines each
-Significantly improved scalability and readability

**2. Codebase Reduction**
-Reduced total combat system code by ~35–45%
-Eliminated large amounts of duplicated logic
-Simplified future feature expansion

**3. Multi-Enemy Experience System Fix**
_Before:_
Experience rewards were calculated from a single enemy
_After:_
Experience is now correctly summed across all enemies in battle

**4. Turn Order Optimization**
_Before:_
Separate logic branches for 1, 2, and 3 enemies
_After:_
Unified loop-based system for determining turn order

**5. Performance Improvements**
-Removed unnecessary per-frame calculations
-Optimized scene-based level tracking
-Reduced repeated object lookups

Author:
Emily Melo
