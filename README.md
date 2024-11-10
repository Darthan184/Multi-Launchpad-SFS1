# Multi-Launchpad-SFS1

The mod adds support for alternate space centers to custom solar systems. In general, the planets that are expected to have an alternate space center should have a flat area at the top with the correct terrain altitude. If it doesn't (like the default solar system) it is possible to shift the location and height of the space center to find a flat-ish location but these tend to differ for different difficulties. As a result you will need to provide a space center for each difficulty unless you are lucky enough to have a flat area in the correct place.

To specify them you need to add an Alternate_Space_Center_Data.txt file:

![File Directory](Images/Directory.png)

The file is essentially a json array of space center definitions with a couple of optional extra fields:

```
[
    {
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

**"enabled":**

0 - never enabled

1 - always enabled (default value if this field is omitted)

2 - enabled if the 'land on' challenge has been completed for this planet

**"difficulty":**

"all" - used for all difficulties (default value  if this field is omitted)

"normal" - only used in the 'normal' difficulty

"hard" - only used in the 'hard' difficulty

"realistic" - only used in 'realistic' difficulty

## UI

If the mod is installed a window will appear in the space center view:

![UI](Images/UI.png)

The '\<' and '>' buttons will switch the space center between the original (as specified in Space_Center_Data.txt) and the enabled alternates.

## Known Limitations

Only the selected space center will exist in the game. Switching to a different one effectively 'teleports' the space center to a different location. As a result, if you launch a rocket, leave it on the launchpad, change the space center and resume game, the rocket will fall to the ground.

The space center that is selected when entering a world is always the original (Space_Center_Data.txt) one.

Challenges that specify '... and return safely' always expect you to return to the original space center planet - wherever the rocket is launched from.

You can only recover at the *currently selected* space center's  planet. So to recover the rocket after completing a challenge you need to ensure that the currently selected space center is the original space center. Note, you do get a 'Recover' button if you land at the currently selected space center's planet when it differs from the original, but it does not seem to affect the challenges.

The "angle" field works as well as the "angle" field in Space_Center_Data.txt . It specifies the position of the space center but not its orientation, so if moved away from 90 the launch pad will appear tilted. It is best to try and move it no more that a couple of degrees.

The space center screen sometimes looks a little odd, expecially for planets/moons without an atmosphere.

## Example

This is an Alternate_Space_Center_Data.txt for the default solar system. It defines alternate space centers for every planet and moon apart from Captured Asteroid, Deimos, Phobos and Jupiter. Each space center is enabled if the 'Land on <<planet>> and return safely' challenge has been accomplished. It should work at every difficultly. 

```
[
    {
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

