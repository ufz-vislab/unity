# FBX converter {#fbx-converter}

General limitations:

- Only the active scalar is exported as vertex colors, so mapping of colors can not be modified later on

## ParaView

A [ParaView exporter plugin](https://github.com/ufz-vislab/VtkFbxConverter) allows to export the current visible data sets to an FBX file (*File / Export Scene*).

Either copy the DLL from the [plugins release page](https://github.com/ufz-vislab/VtkFbxConverter/releases) into your ParaView installation or use the ParaView on `Y:\gruppen\vislab\software\paraview\[version]\bin\paraview.exe`.
