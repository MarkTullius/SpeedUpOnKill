# TimescaleShenanigans

Changes the timescale of the engine based upon enemy kills. Only affected by player kills, *not* ally kills or Forgive Me Please.

Currently two scaling styles available:

**Linear**
- Scales the timescale of the engine upon killing an enemy (by default 1 kill will increase, config option to set this to be less frequent)

**Random**
- Randomly chooses the timescale to set the engine to between the upper and lower bounds (set in config values)

# Changelog

**2.1.1**

-   Make Engi turret kills count
-   Change how the void coin counter works to ensure it updates with turret kills

**2.1.0**

-   Reset time scale to 100% upon run end
-   Change min value for Time Factor to 0.5% to prevent it being limited to a minimum of 50%
-   Add checkbox to allow the timescale to be reset at the start of each new stage
-   Set Void Coins to 100 on run start
-   Add Ceiling value to prevent the game crashing at high timescales and to negate the existing 300% timescale cap
-   Add Kill Factor to allow less frequent time scale changing
-   Split config into General, Linear and Random
-   Add sick as fuck icon
-   Make Risk of Options optional

**2.0.1**

-   Added Random scaling style
-   Utilised void coin counter to display current timescale % (rounding errors abound at present)

**2.0.0**

-   Recreated mod after SSD failure

**1.0.0**

-   Created mod
