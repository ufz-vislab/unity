# General project setup {#setup-project}

## Setting up a new application

### Create the scene

Create a new scene. Drag'n'drop the scene **VRBase** found in *Assets/UFZ/Scenes* into the Hierarchy window. Click on *Menu / UFZ / Create scene setup*. Save your scene into *Assets/_project/Scenes*.

### Add a build

Open build settings *Menu / Windows / PygmyMonkey / Advanced Builder*. Click on *Add Release Type* and fill in information. Under *Project Configurations* add the release type. Activate the newly created configuration (is highlighted in green). Under *Custom Defines* add `MVR`. Under *Scenes* add the scene *VRBase* and your created scene. Make sure you have your scene at the beginning of the list!

----
Previous page: @ref dev-env
