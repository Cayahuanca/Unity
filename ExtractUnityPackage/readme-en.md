
## About ExtractUnityPackage,py

**ExtractUnityPackage,py** is a Python script for extracting the contents of a UnityPackage.

This script can be useful in the following situations:
- When you want to retrieve files from a UnityPackage without going through the time-consuming import process in Unity.
- When you already have files with the same path or GUID in your Unity project, and importing the UnityPackage would overwrite them.

**Note:**
- Python 3 is required to run this script. In most environments where Python 3 is installed, you should not need to install any additional libraries using tools like pip.
- This script has been tested only on Windows and may not work correctly in Linux or macOS environments.

## How to Run

Execute the script using the following command:

```bash
python ExtractUnityPackage.py [path to UnityPackage] [options]
```

## Command Line Options

The following are the command line options for this script:

- `-m`, `--meta`: Output .meta files.
- `-f`, `--force`: When a folder with the same name as the UnityPackage already exists in the folder containing the UnityPackage, confirm whether to overwrite it.
(You cannot decide on individual file overwrites.)

## About Meta Files Option

By outputting files with .meta files attached, the exported files, when placed in Unity's Assets folder, will have the same GUID as the ones in the UnityPackage. This makes it possible to use them for shaders, scripts, or other files that might be updated by importing another UnityPackage into the project or when multiple copies of the same file might exist within the project, potentially causing bugs.

On the other hand, for materials or other items where it's okay to have multiple instances within the Unity project, you can output them without .meta files. When placed in Unity's Assets folder, these files will be assigned different GUIDs from those in the UnityPackage. This can be useful when you want to create multiple variants in your Unity project or simply need specific files from the UnityPackage.