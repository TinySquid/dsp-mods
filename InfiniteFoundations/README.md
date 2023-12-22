# Infinite Foundations Mod

Overrides the soil and foundations that the player has so that it never depletes and is set to a static number.

### Configurable Options

Run the game with the mod installed at least once so that the configuration file can be properly generated if you want to modify any settings.

```
[General]

## toggle for infinite soil
# Setting type: Boolean
# Default value: true
shouldOverrideSoil = true

## toggle for infinite foundations
# Setting type: Boolean
# Default value: true
shouldOverrideFoundations = true

[Other]

## exposed item id for foundations if the game ever changes it for any reason. See https://dsp-wiki.com/Modding:Items_IDs
# Setting type: Int32
# Default value: 1131
foundationItemId = 1131
```
