# General project setup {#setup-project}

- Create a new Unity project
- Under *Menu / Edit / Project Settings / Player Settings* in the *Other Settings*-tab under *Scripting Define Symbols* enter `MVR` and hit ENTER
- Under *Menu / Assets / Import Package / Custom Package* import the package *assets.unitypackage*

## Setting up a new application

### Create the scene

- Create a new scene
- Delete the *Main Camera*-object in the scene
- Drag'n'drop the scene **VRBase** found in *Assets/UFZ/Scenes* into the Hierarchy window
- Click on *Menu / UFZ / Create scene setup*
- Save your scene into *Assets/_project/Scenes*

### Add a build

Open build settings *Menu / Windows / PygmyMonkey / Advanced Builder*. Click on *Add Release Type* and fill in information. Under *Project Configurations* add the release type (from the dropdown select your created scene with *Windows x86_64*-ending). Activate the newly created configuration (is highlighted in green). Under *Custom Defines* add `MVR`. Under *Scenes* add the scene *VRBase* and your created scene. Make sure you have your scene at the beginning of the list! Click *Apply this config*.

### Organize your project

Please create a subfolder under *Assets / _project* and put your assets into it for better organization. Please save your scene under *Assets / _project / Scenes*.

----
Previous page: @ref dev-env
