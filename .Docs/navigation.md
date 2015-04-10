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

- Create a new emtpy GameObject and name it **Viewpoints**
- Attach new emtpy child GameObjects to it
- Add the script UFZ.Interaction.Viewpoint to the child objects
- In the inspector you can now see a render window of viewpoint
- Simply translate / rotate the GameObjects transform to the desired location

If you start the application you a new submenu in the VR menu with a list of all viewpoints. Select one to fly to it. Additionally you can *Reset Rotation*here. See UFZ.Menu.VrMenuViewpoint.

To fade in / out objects when flying to a viewpoint you can attach the script UFZ.Interaction.ViewpointObjectVisibility.

