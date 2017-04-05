# FBX converter {#fbx-converter}

General limitations:

- Only the active scalar is exported as vertex colors, so mapping of colors can not be modified later on

## ParaView

A [ParaView exporter plugin](https://github.com/ufz-vislab/VtkFbxConverter) allows to export the current visible data sets to an FBX file (*File / Export Scene*).

Either copy the DLL from the [plugins release page](https://github.com/ufz-vislab/VtkFbxConverter/releases) into your ParaView installation or use the ParaView on `Y:\gruppen\vislab\software\paraview\[version]\bin\paraview.exe`.

# Use converted data sets in Unity

After adding a data set to your scene you have to setup its material properties by selecting the data set it in the hierarchy and then click on *Menu / UFZ / Consolidate Material Properties*. This adds a new script *Material Properties* to the data set where set up its appearance. The settings are pretty self-explanatory. Often you have to switch *Side* to *Two Sided* as mesh faces are often not consistent.
