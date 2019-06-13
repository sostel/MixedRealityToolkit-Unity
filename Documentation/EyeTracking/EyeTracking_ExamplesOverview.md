# Eye tracking examples in MRTK
This page covers how to get quickly started with using eye tracking in MRTK by building on our provided [MRTK eye tracking example package](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/mrtk_release/Assets/MixedRealityToolkit.Examples/Demos/EyeTracking).
The samples let you experience one of our new magical input capabilities: **Eye tracking**! 

The demo includes a number of different use cases ranging from implicit eye-based activations, to how to seamlessly combine information about what you are looking at with **voice** and **hand** input. 
This enables users to quickly and effortlessly select and move holographic content across their view simply by looking at a target and saying _'Select'_ or performing a hand gesture. 
The demos also include an example for eye-gaze-directed scroll, pan and zoom of text and images on a slate. 
Finally, an example is provided for recording and visualizing the user's visual attention on a 2D slate.

In the following, we'll go into more detail what each of the different samples in the [eye tracking example package](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/mrtk_release/Assets/MixedRealityToolkit.Examples/Demos/EyeTracking) includes:

![List of eye tracking scenes](../Images/EyeTracking/mrtk_et_list_et_scenes.jpg)

The individual sample scenes will be [loaded additively](https://docs.unity3d.com/ScriptReference/SceneManagement.LoadSceneMode.Additive.html).
In case, you wonder what that entails, we will start with a quick overview of how to set up and test the MRTK eye tracking demos.

## Setting up the MRTK eye tracking samples

### 1. Load EyeTrackingDemo-00-RootScene.unity
The *EyeTrackingDemo-00-RootScene* is the base (_root_) scene that has all the core MRTK components included.
This is the scene that you need to load first and from which you will run the eye tracking demos. 
It comes with a graphical scene menu that allows you to easily switch between the different eye tracking samples which will be [loaded additively](https://docs.unity3d.com/ScriptReference/SceneManagement.LoadSceneMode.Additive.html).

![Scene menu in eye tracking sample](../Images/EyeTracking/mrtk_et_scenemenu.jpg)

The root scene includes a few core components that will persist across the additively loaded scenes. 
The _MixedRealityBasicSceneSetup_ includes a script that will automatically load the referenced scene on startup. 
By default this is _EyeTrackingDemo-02-TargetSelection_.  

![Example for the OnLoadStartScene script](../Images/EyeTracking/mrtk_et_onloadstartscene.jpg)


### 2. Adding scenes to the Build menu
To load additive scenes during runtime, you must add these scenes to your _Build Settings -> Scenes in Build_ menu first.
It is important the the root scene is shown as the first scene in the list:

![Build Settings scene menu for eye tracking samples](../Images/EyeTracking/mrtk_et_build_settings.jpg)


### 3. Play the eye tracking samples in the Unity Editor
After adding the eye tracking scenes to the Build Settings and loading the _EyeTrackingDemo-00-RootScene_, there is one last thing you may want to check: Is the _'OnLoadStartScene'_ script that is attached to the _MixedRealityBasicSceneSetup_ GameObject enabled? This is to let the root scene know which demo scene to load first.

![Example for the OnLoad_StartScene script](../Images/EyeTracking/mrtk_et_rootscene_onload.png)

Let's roll! Hit _"Play"_!
You should see several gems appear and should see the scene menu at the top.

![Sample screenshot from the ET target select scene](../Images/EyeTracking/mrtk_et_targetselect.png)

You should notice a small semitransparent circle at the center of your Game view. 
This acts as an indicator of your _simulated eye gaze_.
You can move it by pressing down the _right mouse button_ and move the mouse.
When the cursor is hovering over a gem, you will notice that the cursor will snap to the center of it. 
This is a great way to test if events are triggered as expected when _"looking"_ at a target. 
Please be aware that the _simulated eye gaze_ via mouse control is a rather poor supplement to our rapid and unintentional eye movements. 
However, it is great for testing the basic functionality before iterating on the design by deploying it to the HoloLens 2 device.
Coming back to our sample scene: The gem rotates as long as it is being looked at, and can be destroyed by "looking" at it and ...
- Pressing _Enter_ (which simulates saying "select")
- Actually saying _"select"_ into your microphone
- While pressing _Space_ to show the simulated hand input, click the left mouse button to perform a simulated pinch

We describe in more detail how you can achieve these interactions in our [**Eye-Supported Target Selection**](EyeTracking_TargetSelection.md) tutorial.

When moving the cursor up to the top menu bar in the scene, you will notice that the currently hovered item will highlight subtly. 
You can select the currently highlighted item by using one of the above described commit methods (e.g., pressing _Enter_).
This way you can switch between the different eye tracking sample scenes.

### 4. How to test specific sub scenes
When working on a specific scenario, you may not want to go through the scene menu every time. 
No problem!
- Load the _root_ scene
- Disable the _'OnLoadStartScene'_ script
- _Drag and drop_ one of the eye tracking test scenes that are described below (or any other scene) into your _Hierarchy_ view.

![Example for additive scene](../Images/EyeTracking/mrtk_et_additivescene.jpg)


## Overview of the eye tracking demo samples
[**Eye-Supported Target Selection**](EyeTracking_TargetSelection.md)

This tutorial showcases the ease of accessing eye gaze data to select targets. 
It includes an example for subtle yet powerful feedback to provide confidence to the user that a target is focused while not being overwhelming.
In addition, there is a simple example of smart notifications that automatically disappear after being read. 

**Summary**: Fast and effortless target selections using a combination of eyes, voice and hand input.

<br>


[**Eye-Supported Navigation**](EyeTracking_Navigation.md)

Imagine you are reading some information on a distant display or your e-reader, and when you reach the end of the displayed text, the text automatically scrolls up to reveal more content. 
Or how about magically zooming directly towards where you are looking? 
These are some of the examples showcased in this tutorial about eye-supported navigation.
In addition, there is an example for hands-free rotation of 3D holograms by making them automatically rotate based on your current focus. 

**Summary**: Scroll, pan, zoom, 3D rotation using a combination of eyes, voice and hand input.

<br>


[**Eye-Supported Positioning**](EyeTracking_Positioning.md)

This tutorial shows an input scenario called [Put-That-There](https://youtu.be/CbIn8p4_4CQ) dating back to research from the MIT Media Lab in the early 1980's with eye, hand and voice input.
The idea is simple: Benefit from your eyes for fast target selection and positioning. 
Simply look at a hologram and say _'put this'_, look over where you want to place it and say _'there!'_. 
For more precisely positioning your hologram, you can use additional input from your hands, voice or controllers. 

**Summary**: Positioning holograms using eyes, voice and hand input (*drag-and-drop*). Eye-supported sliders using eyes + hands. 

<br>


[**Visualization of Visual Attention**](EyeTracking_Visualization.md)

Information about where users look at is an immensely powerful tool to assess usability of a design and to identify problems in efficient work streams. 
This tutorial discusses different eye tracking visualizations and how they fit different needs. 
We provide basic examples for logging and loading eye tracking data and examples for how to visualize them. 

**Summary**: Two-dimensional attention map (heatmaps) on slates. Recording & replaying eye tracking data.

---
[Back to "Eye tracking in the MixedRealityToolkit"](EyeTracking_Main.md)
