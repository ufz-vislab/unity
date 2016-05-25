# Navigation {#navigation}

## Core navigation in VRBase

Navigations are based on UFZ.Interaction.NavigationBase. In the *VRBase*-scene the following navigations are included:

### Keyboard navigation

- The keys *WASD* move forward / sideward
- Hold down *Left Shift* to move faster

See UFZ.Interaction.KeyboardNavigation.

### Gamepad navigation

- Left analog stick moves forward / sideward
- Right analog stick rotates horizontally and moves faster

See UFZ.Interaction.GamepadNavigation.

### Flystick navigation

- Small yellow stick moves forward and rotates horizontally
- The direction of movement is defined by the orientation of the Flystick

See UFZ.Interaction.WandNavigation.

### SpaceMouse navigation

- Push an axis moves along that axis
- TODO: rotation
- Holding the left button moves faster

See UFZ.Interaction.SpaceMouseNavigation.


## Viewpoints

You can define a number of viewpoints and then use the VR menu to fly to them:

- Create a new viewpoint by clicking on *Menu / UFZ / Add viewpoint*
- This adds the script UFZ.Interaction.Viewpoint to a new child object under *Scene Setup / Viewpoints*
- In the inspector you can now see a render window of viewpoint
- Simply translate / rotate the GameObjects transform to the desired location

You can check one viewpoints *Start Application Here* checkbox for the initial starting point of the application.

To fade in / out objects when flying to a viewpoint you can attach the script UFZ.Interaction.ViewpointObjectVisibility.
