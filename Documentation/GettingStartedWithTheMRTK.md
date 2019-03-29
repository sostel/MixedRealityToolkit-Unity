# Getting started with the Mixed Reality Toolkit

![](/External/ReadMeImages/MRTK_Logo_Rev.png)

## Prerequisites

To get started with the Mixed Reality Toolkit you will need:

* [Unity 2018.3.9f1 +](https://unity3d.com/get-unity/download/archive)
* [Latest MRTK release (Beta)](https://github.com/Microsoft/MixedRealityToolkit-Unity/releases)

## Starting your new project
To get your first project up and running, the steps are as follows:

### 1. Create your new project (or start a new scene in your current project)

> *Note* when creating a new project with Unity 2018, Unity gives you several templates to choose from.  Currently the **MRTK does not yet support the Scriptable Render Pipeline**, so the LWSRP, HDSRP and VRSRP projects are not compatible with MRTK projects.  Please stay tuned to the MRTK GitHub site for future announcements on SRP support.

### 2. [Import the Mixed Reality Toolkit asset](https://github.com/Microsoft/MixedRealityToolkit-Unity/releases)

> The Mixed Reality Toolkit is available via [multiple delivery mechanisms](/Documentation/DownloadingTheMRTK.md) and in the future will also be available via the Unity package manager once Unity makes that option available.

Get the asset and import it in to your Unity project using  "Asset -> Import Package -> Custom Package" from the Unity Editor menu.

![](/External/ReadMeImages/Unity_ImportAssetOption.png)

Accept all the content and continue.

![](/External/ReadMeImages/MRTK_AssetImportDialog.png)

> The SDK, Example, Providers, Services folders are optional but highly recommended for new users.  Once you have a feel for how the toolkit works, you can remove these safely if you are not using them.

### 3. Configure your first Mixed Reality Toolkit scene
The toolkit has been designed so that there is just one object that is mandatory in your scene.  This is there to provide the core configuration and runtime for the Mixed Reality Toolkit (one of the key advantages in the new framework).

1. Select "Mixed Reality Toolkit/Configure" from the Editor menu:
> Mixed Reality Toolkit -> Configure

![](/External/ReadMeImages/MRTK_ConfigureScene.png)

2. Select "Ok" in the dialog that says 'you must choose a configuration'.

3. Select "DefaultMixedRealityToolkitConfigurationProfile"

Once this completes, you will see the following in your Scene hierarchy:

![](/External/ReadMeImages/MRTK_SceneSetup.png)

> The MRTK will also select the configured profile, [click here for more details on this configuration screens](#configuring)

Which contains the following:

* Mixed Reality Toolkit - The toolkit itself, providing the central configuration entry point for the entire framework.
* MixedRealityPlayspace - The parent object for the headset, which ensures the headset / controllers and other required systems are managed correctly in the scene.
* The Main Camera is moved as a child to the Playspace - Which allows the playspace to manage the camera in conjunction with the SDK's
* UIRaycastCamera added as a child to the Main Camera - To enable seamless UI interactions through the toolkit

> If you get errors like "XR: OpenVR Error! OpenVR failed initialization with error code VRInitError_Init_PathRegistryNotFound: "Installation path could not be located (110)!" it is safe to ignore this. You can change your build settings (File->Build Settings) and change your build target to "Universal Windows Platform" to remove these errors.

> **Note** While working in your scene, **DON'T move the Main Camera** (or the playspace) from the scene origin (0,0,0).  This is controlled by the MRTK and the active SDK.
> If you need to move the players start point, then **move the scene content and NOT the camera**!

### 4. Hit play

You are now ready to start building your Mixed Reality Solution, just start adding content and get building.
Switch to other platforms (ensure they have XR enabled in their player settings) and your project will still run as expected without change.

To preview your change in a mixed reality headset, do the following:

1. Open build settings (File->Build Settings) and change your build target to "Universal Windows Platform".
2. Close Unity
3. Connect your WMR headset, wait for Windows Mixed Reality to load and verify you are in cliff house.
4. Open Unity.
5. Press play. You will see an animation to go into the Unity app, and should see controllers.

To preview changes in editor, press play, and move a hand aroud by pressing spacebar.

### 5. Check out example scene
1. Open the scene *MixedRealityToolkit.Examples\Demos\HandTracking\Scenes\HandInteractionExamples.unity* to see a scene with lots of UI controls you can use.

2. When you see 'TMP Importer' dialog, select 'Import TMP Essentials'. You will then see large text in the screen, so you'll need to close and re-open Unity.

3. Close and re-open Unity

4. Re-open the scene, and press play to see the example.

<a name="configuring"/>

## Configuring your project

The Mixed Reality Toolkit configuration is all centralized on one place and attached to the MixedRealityToolkit object in your active scene.

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_ActiveConfiguration.png)

Clicking on this profile will show the configuration screens for the Mixed Reality Toolkit:

![](/External/HowTo/MixedRealityToolkitConfigurationProfileScreens/MRTK_MixedRealityToolkitConfigurationScreen.png)

From here you can navigate to all the configuration profiles for the MRTK, including:

> The "Default" profiles provided by the Mixed Reality Toolkit are locked by default, so when you view these in the inspector they will appear greyed out.  This is to ensure you always have a common default for any project.  We recommend you create your own profiles (see below) when you need to customize the configuration for your project.

* Main Mixed Reality Toolkit Configuration
* Camera Settings
* Input System Settings
* Boundary Visualization Settings
* Teleporting Settings
* Spatial Awareness Settings
* Diagnostics Settings
* Additional Services Settings
* Input Actions Settings
* Input Actions Rules
* Pointer Configuration
* Gestures Configuration
* Speech Commands
* Controller Mapping Configuration
* Controller Visualization Settings

As you can see there are lots of options available and more will come available as we progress through the beta.

When you start a new project, we provide a default set of configurations with every option turned on, styled for a fully cross platform project.  These defaults are "Locked" to ensure you always have a common start point for your project and we encourage you to start defining your own settings as your project evolves.  For this we provide options to either:

* Copy the defaults in to a new profile for you to start customizing it for your project
* Start afresh with a brand-new profile.

![](/External/ReadMeImages/MRTK_CopyCreateConfigurationOptions.png)

When profiles are created by the MRTK, they are then placed in the following folder:

> "Assets\MixedRealityToolkit-Generated\CustomProfiles"

At each step in the configuration, you can choose to remove and create a new profile, or simply copy the existing settings and continue to customize:

![](/External/ReadMeImages/MRTK_CopyProfileOptions.png)

### **[For more information on customizing the Configuration Profiles](/Documentation/MixedRealityConfigurationGuide.md)**
Please check out the [Mixed Reality Configuration Guide](/Documentation/MixedRealityConfigurationGuide.md)

## Get building your project

Now your project is up and running, you can start building your Mixed Reality project.  

For more information on the rest of the toolkit, please check the following guides:

* [Mixed Reality Configuration Guide](/Documentation/MixedRealityConfigurationGuide.md)
* [Getting to know the Mixed Reality Toolkit Input System]() (Coming Soon)
* [Customizing your controllers in the MRTK]() (Coming Soon)
* [A walkthrough the UX components of the MRTK SDK]() (Coming Soon)
* [Using Solvers to bind your objects together]() (Coming Soon)
* [Creating interactions between the player and your project]() (Coming Soon)
* [Configuration Profile Usage Guide]() (Coming Soon)
* [Guide to building Registered Services]() (Coming Soon)
* [Guide to Pointers documentation]() (Coming Soon)

## Upgrading from the HoloToolkit (HTK)

There is not a direct upgrade path from the HoloToolkit to the new Mixed Reality Toolkit due to the rebuilt framework.  However, it is possible to import the MRTK into your HoloToolkit project and start work migrating your implementation if you wish.

> The Mixed Reality Team will release a guide in the future on the key differences between HTK and MRTK project implementations.

Our recommendation is that if you have an existing HTK project, then it is best to continue with the HTK as it is still a robust and feature rich platform for building HoloLens projects.

If you are building a new Mixed Reality solution, then we encourage you to join us on the MRTK journey. Some of our key contributors are already building production-ready solutions already utilizing the MRTK and using their experiences to enrich the toolkit moving forward.
