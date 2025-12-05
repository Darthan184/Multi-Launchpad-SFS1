# Multi-Launchpad-SFS1

The mod adds support for alternate space centers to custom solar systems. In general, the planets that are expected to have an alternate space center should have a flat area at the top with the correct terrain altitude. If it doesn't (like the default solar system) it is possible to shift the location and height of the space center to find a flat-ish location but these tend to differ for different difficulties. As a result you will need to provide a space center for each difficulty unless you are lucky enough to have a flat area in the correct place.

To specify them you need to add an Alternate_Space_Center_Data.txt file:

![File Directory](Images/Directory.png)

The file is essentially a json array of space center definitions with a three of optional extra fields:

```
[
    {
      "enabled": 1,
      "difficulty":"all",
      "address": "Earth",
      "location": "Alternate",
      "angle": 91.0,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 26.2
      }
    }
    ,{

      "enabled": 2,
      "difficulty":"normal",
      "address": "Moon",
      "angle": 91.0,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 168.0
      }
    }
    ,{
      "enabled": 2,
      "difficulty":"hard",
      "address": "Moon",
      "angle": 91,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 395.0
      }
    }
    ,{
      "enabled": 2,
      "difficulty":"realistic",
      "address": "Moon",
      "angle": 91,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 2835.0
      }
    }
    ,{
      "enabled": 2,
      "address": "Venus",
      "angle": 90.0,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 20.0
      }
    }
]
```

## Fields 

**"address":**

Specifies the name of the planet where the space center is. If omitted will be for all planets with terrain. If an alternate launchpad is specfied more than once, the last one specified will be used.

**"inclInsignificant"**

When no address is specified, include planets that are not marked as 'significant', default false.

**"exclude"**;

When no address is specified, exclude planets in this array of names
        
**"location":**

Specifies the name of an alternate location on the same planet. If omitted will use the name "(default)".

**"enabled":**

0 - never enabled

1 - always enabled (default value if this field is omitted)

2 - enabled when a challenge is accomplished. N.B. if there is a planet called "Earth", a 'return safely' challenge will only be counted when recovered on Earth. Otherwise it is counted when the rocket is recovered at the planet with the current launchpad.

**"challenge_id":**

The challenge id to be used with enabled=2, if omitted will be the "landed on planet and returned safely" challenge. If no value was specified for "address" the string {planet} will be replaced with the name of the planet.

**"difficulty":**

"all" - used for all difficulties (default value  if this field is omitted)

"normal" - only used in the 'normal' difficulty

"hard" - only used in the 'hard' difficulty

"realistic" - only used in 'realistic' difficulty

**"angle":**

The location on the planet (in degrees) of the space center (not fully implemented by SFS, location is, orientation is not), default 90.

**"position_LaunchPad":**

Position of the launch pad. If omitted will use default values for its fields:

    **"horizontalPosition"**
    
    Horizontal position the launch pad relative to the space center (meters) +ve right, default 365

    **"height"**
    
    Vertical position of the launch pad relative to the planet radius +ve up, if omitted, will attempt to find the best value

## UI

If the mod is installed a window will appear in the space center view:

![UI](Images/UI.png)

The '\<' and '>' buttons at the top will switch the space center between the original (as specified in Space_Center_Data.txt) and the enabled alternates. The '\<' and '>' buttons at the bottom (only appear when the are multiple locations) will switch between locations on the planet.

## Known Limitations

Only the selected space center will exist in the game. Switching to a different one effectively 'teleports' the space center to a different location. As a result, if you launch a rocket, leave it on the launchpad, change the space center and resume the game, the rocket will fall to the ground.

The space center that is selected when entering a world is always the original (Space_Center_Data.txt) one.

You can only recover at the *currently selected* space center's  planet.

The "angle" field works as well as the "angle" field in Space_Center_Data.txt does. It specifies the position of the space center but not its orientation, so if moved away from 90 the launch pad will appear tilted. It is best to try and move it no more that a couple of degrees. A result of this is that any alternate locations need to be close to the 90 degree position - how close depends on how much tilt you can live with.

The space center screen sometimes looks a little odd, expecially for planets/moons without an atmosphere.

## Example

This is an Alternate_Space_Center_Data.txt for the default solar system with an alternate Earth launch pad 1 degree West (about 6km in Normal) from the default launch pad. It defines alternate space centers for every planet and moon apart from Captured Asteroid, Deimos, Phobos and Jupiter. Each space center is enabled if the 'Land on "planet name" and return safely' challenge has been accomplished. It should work at every difficulty. 

```
[
    {
      "enabled": 1,
      "difficulty":"all",
      "address": "Earth",
      "location": "Alternate",
      "angle": 91.0,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 26.2
      }
    }
    ,{
      "enabled": 2,
      "difficulty":"normal",
      "address": "Moon",
      "angle": 91.0,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 168.0
      }
    }
    ,{
      "enabled": 2,
      "difficulty":"hard",
      "address": "Moon",
      "angle": 91,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 395.0
      }
    }
    ,{
      "enabled": 2,
      "difficulty":"realistic",
      "address": "Moon",
      "angle": 91,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 2835.0
      }
    }
    ,{
      "enabled": 2,
      "difficulty":"normal",
      "address": "Mars",
      "angle": 90.0,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 208.0
      }
    }
    ,{
      "enabled": 2,
      "difficulty":"hard",
      "address": "Mars",
      "angle": 90.0,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 229.0
      }
    }
    ,{
      "enabled": 2,
      "difficulty":"realistic",
      "address": "Mars",
      "angle": 90.0,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 254.0
      }
    }
    ,{
      "enabled": 2,
      "address": "Venus",
      "angle": 90.0,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 20.0
      }
    }
    ,{
      "enabled": 2,
      "difficulty":"normal",
      "address": "Mercury",
      "angle": 89.4,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 235.0
      }
    }
    ,{
      "enabled": 2,
      "difficulty":"hard",
      "address": "Mercury",
      "angle": 89.42,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 464.0
      }
    }
    ,{
      "enabled": 2,
      "difficulty":"realistic",
      "address": "Mercury",
      "angle": 89.4,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 278.5
      }
    }
    ,{
      "enabled": 2,
      "difficulty":"normal",
      "address": "Callisto",
      "angle": 90.9,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 250.0
      }
    }
    ,{
      "enabled": 2,
      "difficulty":"hard",
      "address": "Callisto",
      "angle": 90.9,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 234.0
      }
    }
    ,{
      "enabled": 2,
      "difficulty":"realistic",
      "address": "Callisto",
      "angle": 90.9,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 236.0
      }
    }
    ,{
      "enabled": 2,
      "address": "Ganymede",
      "angle": 90.1,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 20.0
      }
    }
    ,{
      "enabled": 2,
      "difficulty":"normal",
      "address": "Europa",
      "angle": 90.5,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 16.0
      }
    }
    ,{
      "enabled": 2,
      "difficulty":"hard",
      "address": "Europa",
      "angle": 90.5,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 13.0
      }
    }
    ,{
      "enabled": 2,
      "difficulty":"realistic",
      "address": "Europa",
      "angle": 90.5,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 17.0
      }
    }
    ,{
      "enabled": 2,
      "difficulty":"normal",
      "address": "Io",
      "angle": 91.6,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 8.0
      }
    }
    ,{
      "enabled": 2,
      "difficulty":"hard",
      "address": "Io",
      "angle": 91.56,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 8.0
      }
    }
    ,{
      "enabled": 2,
      "difficulty":"realistic",
      "address": "Io",
      "angle": 91.6,
      "position_LaunchPad": {
        "horizontalPosition": 365.0,
        "height": 8.0
      }
    }
]
```

