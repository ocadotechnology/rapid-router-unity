## UPGRADE GUIDE ##

To upgrade to a new ProCamera2D version you should:
- Open a new empty scene
- Delete ProCamera2D folders (Assets/ProCamera2D and Assets/Gizmos/ProCamera2D)
- Import the new package




## QUICK START ##

Select your game camera (called Main Camera by default) and drag the ProCamera2D component file to the Main Camera GameObject.
You should now see the ProCamera2D component editor on the Inspector window when you select your Main Camera GameObject.




## USER GUIDE ##

For more information about all ProCamera2D features and how to use them please visit our User Guide at: 
http://www.procamera2d.com/user-guide




## SUPPORT ##

Do you have an issue you want to see resolved, a suggestion or you simply want to know if this is the right plugin for your game? Get in touch using any of the links below and we’ll do our best to get back to you ASAP.

Contact Form - http://www.procamera2d.com/support
Unity forums - http://goo.gl/n80dMb
Twitter - http://www.twitter.com/lpfonseca




## CHANGELOG ##

2.2.4
- TriggerBoundaries - Fixed a bug that prevent them from working if no trigger was set as starting boundaries
- Cinematics - Fixed a bug on the editor that prevented it from showing the CinematicTargetReached UnityEvent

2.2.3
- Rooms - Removed the dependency from having the NumericBoundaries extension on the same GameObject
- Rooms - Added an event that fires when the camera target has exited all rooms
- Cinematics - Replaced standard C# events with Unity events for easier integration with the editor

2.2.2
- Core - Added a Dolly Zoom (Hitchcock effect) method. Created an example scene for it
- Rooms - Added a method to set the default numeric boundaries settings for when leaving all rooms
- PanAndZoom - Prevented unintentional pans when outside of the GameView on the editor

2.2.1
- Core - Greatly improved rendering performance of the editor
- PanAndZoom - Fixed an issue that prevented zooming after toggling the component
- Rooms - Added an ID property that can be used to identify each room
- Rooms - OnStartedTransition and OnFinishedTransition now send the previous room too
- Rooms - Editor tweaks

2.2.0
- New extension - Rooms! Easily create and manage multiple rooms on your scene
- Parallax extension - Added a new method (CalculateParallaxObjectsOffset) to manually recalculate parallax objects offset if needed
- PanAndZoom extension - Added a boolean (ResetPrevPanPoint) to prevent movement jumps after toggling the component
- Added links to documentation on core, extensions and triggers' editor windows
- Fixed an issue with the SpeedBasedZoom extension that caused an hard stop when reaching the maximum and minimum zoom values

2.1.5
- NumericBoundaries extension editor tweaks
- Fixed PanAndZoom extension bug when on the XZ or YZ axis
- Fixed shake not stopping to correct position when using the PixelPerfect extension
- Support for Unity 5.5

2.1.4
- The values on the EaseFromTo method are now automatically clamped to prevent overshooting
- Fixed missing static instance of the TransitionsFX extension on scene change
- Fixed typo on the AdjustCameraTargetInfluence core method

2.1.3
- Fixed PanAndZoom bug on touch devices that slowed down pan speed

2.1.2
- Added additional methods to the TransitionsFX extension to easily update the properties during runtime
- Fixed PanAndZoom bug that prevented it from working properly on the XZ axis

2.1.1
- Improved TransitionsFX performance
- Fixed memory allocation on the Repeater extension
- Fixed extremely quick transition routines not being cleared on certain circumstances
- Fixed bug in PanAndZoom extension that could prevent zoom when using negative speed values

2.1.0
- TransitionsFX extension now supports the use of textures for completely customizable transitions
- The drag speed of the PanAndZoom extension is now based on "real" world coordinates, giving it a much more natural feel on all platforms

2.0.5
- Support for removal and destruction of extensions during runtime
- Fixed a bug on the PanAndZoom extension where the camera could get stuck on its max/min zoom
- Improved TransitionsFX shaders compatibility with older devices
- Fire only one transition start/end event even if multiple occur during one complex animation
- Fixed a bug on the Cinematics extension when EaseInDuration is set to 0
- Fixed a bug on the Cinematics extension where it wouldn't return to origin if there was no camera target

2.0.4
- Added Pause/Unpause methods to the Cinematics extension
- Cinematics extension now supports parented cameras
- Fixed camera stutter if trying to zoom paste the limits with the PanAndZoom extension
- Fixed regression bug - TriggerBoundaries wouldn't override previous one if transition still occurring 

2.0.3
- Added an AutoScaleMode option to the PixelPerfect extension for better control over how the camera scales
- Allow the ClearFlags of the last camera on the Parallax extension to be manually set if needed
- Fixed regression - bug calculating camera size when using a 2DToolkit Pixels Per Meter camera

2.0.2
- Improved the LetterBox of the Cinematics extension so it doesn't render when it's set to 0
- Improved TriggerInfluence algorithm to provide smoother results when using the ExclusivePercentage parameter
- Improved LetterBox and TransitionsFX shaders
- Fixed camera start position when parented
- Fixed null reference bug on PixelPerfectSprite under certain circumstances  
- Fixed a bug where a PixelPerfectSprite could get misplaced on certain occasions
- Added the action "OnShakeCompleted" to the Shake extension to know when a shake has completed
- Reset Shake presets and Rails nodes from the editor after scene load

2.0.1
- Support for Unity 5.4.0
- Fixed skipping certain TriggerBoundaries transitions on some edges cases
- Fixed bug calculating camera size when using a 2DToolkit Pixels Per Meter camera
- Fixed a null reference when destroying a Pixel Perfect sprite

2.0.0
- New extension - TransitionsFX! Transition between scenes or camera positions with beautiful effects
- Major refactor focusing on code architecture and performance
- Moved LimitSpeed out of the core into a separate extension
- Moved LimitDistance out of the core into a separate extension
- Allow the camera to be parented to any kind of hierarchy
- Added the option to not snap the camera to the pixel grid when using SnapMovementToGrid on the PixelPerfect extension
- Added the option for the Shake extension to ignore the timeScale, allowing it to work even if the game is paused
- Added a "ApplyInfluenceIgnoringBoundaries" method to the Shake extension
- Fixed StopShaking method on the Shake extension
- More code comments

1.9.2
- Added a parameter to the PanAndZoom extension that allows to define how fast the camera inertia should stop once the user starts dragging after a previous pan movement
- NumericBoundaries extension editor tweaks

1.9.1
- Added an indication of the current camera velocity to the SpeedBasedZoom extension editor for reference
- Added Load/Save buttons to the presets list on the Shake extension for an easier control
- Fixed a bug that could cause a few extensions to start before the core is initialized
- Added a property to know if a TriggerBoundaries is the currently active one
- Use the original camera parent when parenting the camera for the Shake extension

1.9.0
- New extension - PanAndZoom! Move and/or zoom the camera with touch (on mobile) or with the mouse (on desktop)
- Re-enable triggers calculations after their GameObjects have been disabled and enabled again
- Added a Zoom method to the core
- Allow TriggerBoundaries to be defined as starting boundaries at run-time

1.8.1
- Added a Rect property to ProCamera2D core that will change its rect and the rect of parallax cameras if existent
- Fixed build error related to EditorPrefsX class on Windows Phone 8.1

1.8.0
- New extension - Repeater!
- The ProCamera2D editor list of available extensions and triggers is now dynamically populated.
- Improved Trigger Rails editor
- Name parallax cameras according to their speeds for easier identification
- Gizmos drawing optimizations
- Implemented ISerializationCallbackReceiver so ProCamera2D works even during runtime code reloads
- Changed the OnReset handler of BasePC2D to public
- Elastic numeric boundaries
- Added a method (RemoveCinematicTarget) to manually remove a target from the Cinematics plugin
- Added support for the Letterbox effect when using the Parallax extension

1.7.3
- Added a method (TestTrigger) to the BaseTrigger class that allows to manually force the trigger collision test
- Added a method (AddCinematicTarget) to the Cinematics extension that allows to manually add a new cinematic target at runtime
- Added a method (AddRailsTarget) to the Rails extension that allows to manually add a new rails target at runtime
- Fixed a bug that could cause a null reference after destroying an object that had a ProCamera2D extension

1.7.2
- Allow Rails extension to be added to a different GameObject than the one with ProCamera2D
- Support for Unity 5.3.0

1.7.1
- Triggers now dispatch public OnEnteredTrigger and OnExitedTrigger events
- Tweaked Rails triggers icons
- Added a Rails snapping parameter to ProCamera2D options panel
- Added a left and right handle to the Rails so it's easier to create new nodes

1.7.0
- New extension - Rails!
- New trigger - TriggerRails!
- Improved extensions architecture and performance
- Added a ManualUpdate option to the UpdateType  
- Fixed zoom size when not using Auto-Scale on the Pixel Perfect extension

1.6.3
- Added 3 new methods to the Core API - AddCameraTargets, RemoveAllCameraTargets and Reset
- Highlight correspondent GameObject when selecting a target from the camera targets list
- Allow the Cinematics extension to be called at the start of a scene
- Added the option to zoom by changing the FOV instead of moving the camera
- Fixed a bug where the Trigger Influence exclusive area wouldn't point to the correct position on the XZ and YZ axes
- Fixed a bug on the Zoom Trigger that could cause the camera to zoom weirdly when not using "SetSizeAsMultiplier" 

1.6.2
- Fixed a bug that would show the incorrect triggers gizmos position if using a Circle
- Only show the Zoom trigger preview size when selected
- On the Zoom and Influence triggers, when not using the targets mid point, use the provided target to calculate the distance to the center percentage
- Zoom trigger "Reset Size On Exit" is now "Reset Size On Leave" so it resets progressively as you leave the trigger instead of only once you exit
- Fixed a bug where after deleting boundaries related extensions / triggers the camera could get stuck at (0,0)

1.6.1
- The camera is only parented if using the Shake extension
- Fixed an issue where deleting Cinematic camera targets could throw an (harmless) error to the console
- When using the Cinematics extension, if a target has a Hold Duration less than 0, the camera will follow it indefinitely
- Added a method (GoToNextTarget) to the Cinematics extension that allows you to skip the current target
- Prevent the Game Viewport size on the Pixel Perfect extension to go below (1, 1)
- Fixed a bug where the TriggerBoundaries would have a missing reference if instantiated manually

1.6.0
- Moved Camera Window, Numeric Boundaries and Geometry Boundaries from the core into their own extensions, leaving the core as light as possible
- Added a new powerful extension (Cinematics) that replaces the CinematicFocusTarget
- Added a new demo (TopDownShooter) that shows how to use multiple ProCamera2D features in one simple game
- Tweaked Zoom-To-Fit extension to support camera boundaries
- Tweaked Speed-Based-Zoom extension to support camera boundaries
- Forward focus auto-adjusts to the camera size when zooming
- Gizmos for triggers are now shown as circles instead of spheres
- Added the option for triggers to be triggered by a specific GameObject instead of always using the camera targets mid-point
- Renamed Plugins to Extensions and Helpers to Triggers

1.5.3
- Upgraded Shake extension - Presets support, rotation and overall tweaks
- Zooming with perspective cameras is now made by moving the camera instead of changing the field of view to avoid distortion
- Fix TriggerBoundaries left and right gizmos incorrect size on XZ and YZ axis
- Maintain pixel-perfect during shakes

1.5.2
- Fixed bug when applying influences on XZ and YZ axis

v1.5.1
- Added option to reset size when exiting a zoom trigger
- Added option to disable Zoom-To-Fit extension when there's only one target
- Disable Parallax extension toggle button when in perspective mode
- Fixed a bug when adding targets progressively
- Tweaked the CinematicFocusTrigger to take all influences in consideration when returning from the cinematic position
- Added an optional "duration" parameter to the RemoveCameraTarget method that allows to remove a camera target progressively by reducing it's influence over time
- Added an optional “duration” parameter to the UpdateScreenSize method that allows to manually update the screen size progressively

v1.5.0
- Added a new extension, Speed Based Zoom that adjusts the zoom based on how fast the camera is moving.

v1.4.1
- Fixed a bug where if the camera is a prefab it would loose its targets on scene load, if the changes weren't saved

v1.4
- Pixel perfect support!! :)
- Fixed a few more bugs related to setting the Time.timeScale to 0 (Thanks to the users who reported this issue and helped solving it!)
- Added a user-guide link to the top of ProCamera2D editor for easier access to the documentation

v1.3.1
- Added a ParallaxObject MonoBehaviour that makes a GameObject position on the scene view to match the same relative position to the main parallax layer during runtime.
- Fixed slight camera movement on start when no targets are added
- Fixed a bug where if Time.timeScale was put to 0 the camera would stop following its targets afterwards
- Fixed a few Playmaker actions descriptions
- Fixed target vertical offset when on XZ and YZ axis
- Fixed PointerInfluence extension on XZ and YZ axes

v1.3
- Full compatibility with 2DToolkit!
- Added namespace to UpdateType and MovementAxis enums to avoid conflicts with other packages
- Added the option to set the TriggerZoom helper size as a direct value instead of as a multiplier
- Added a new method to stop shaking and a flag to check if the camera is currently shaking
- Fixed bug with InfluenceTrigger not smoothing correctly the value on first entrance if ExclusiveInfluencePercentage is 1

v1.2
- Added support for perspective cameras
- Fixed bug with camera getting stuck when using Camera Window and Numeric Boundaries
- Fixed bug that made camera float away when using an offset on a specific axis and turned off following on that same axis

v1.1
- Custom PlayMaker actions with full API support
- Fix for AdjustCameraTargetInfluence method when starting at values different than zero

v1.0
- Public release