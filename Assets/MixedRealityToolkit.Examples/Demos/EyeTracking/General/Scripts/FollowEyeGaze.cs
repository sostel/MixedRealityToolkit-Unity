﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// Sample for allowing a GameObject to follow the user's eye gaze
    /// at a given distance of "DefaultDistanceInMeters".
    /// </summary>
    public class FollowEyeGaze : MonoBehaviour
    {
        [Tooltip("Display the game object along the eye gaze ray at a default distance (in meters).")]
        [SerializeField]
        private float defaultDistanceInMeters = 2f;

        [SerializeField]
        private bool keepFacingUser = true;

        private IMixedRealityInputSystem inputSystem = null;
  
        /// <summary>
        /// The active instance of the input system.
        /// </summary>
        private IMixedRealityInputSystem InputSystem
        {
            get
            {
                if (inputSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
                }
                return inputSystem;
            }
        }

        private void Update()
        {
            //gameObject.transform.position = InputSystem.EyeGazeProvider.GazeOrigin + InputSystem.EyeGazeProvider.GazeDirection.normalized * defaultDistanceInMeters;
            EyeTrackingTarget eyeTarget = EyeTrackingTarget.LookedAtEyeTarget;

            if (keepFacingUser)
            {
                transform.LookAt(Camera.main.transform);
                Vector3 turnAround = new Vector3(0, 180, 0);
                Vector3 turnedTransform = (transform.rotation.eulerAngles + turnAround) ;
                turnedTransform = new Vector3(turnedTransform.x * (-1), turnedTransform.y, turnedTransform.z);
                transform.rotation =  Quaternion.Euler(turnedTransform);
            }

            // Update game object to the current eye gaze hit position at a given distance
            if (eyeTarget == null)
            {
                Ray rayToObj = new Ray(CameraCache.Main.transform.position, InputSystem.EyeGazeProvider.GazeDirection);
                RaycastHit hitInfo;
                UnityEngine.Physics.Raycast(rayToObj, out hitInfo);
                gameObject.transform.position = hitInfo.point;

                if (hitInfo.collider == null)
                {
                    // If no target is hit, show the object at a default distance along the gaze ray.
                    gameObject.transform.position = InputSystem.EyeGazeProvider.GazeOrigin + InputSystem.EyeGazeProvider.GazeDirection.normalized * defaultDistanceInMeters;
                }
            }
            // If this is an EyeTrackinTarget, we may want to show the game object at the center of the currently looked at target.
            else
            {
                if (eyeTarget.EyeCursorSnapToTargetCenter)
                {
                    Ray rayToCenter = new Ray(CameraCache.Main.transform.position, eyeTarget.transform.position - CameraCache.Main.transform.position);
                    RaycastHit hitInfo;
                    UnityEngine.Physics.Raycast(rayToCenter, out hitInfo);
                    gameObject.transform.position = hitInfo.point;
                }
                else
                {
                    // Show the object at the hit position of the user's eye gaze ray with the target.    
                    gameObject.transform.position = EyeTrackingTarget.LookedAtPoint;
                    //gameObject.transform.position = InputSystem.EyeGazeProvider.GazeOrigin + InputSystem.EyeGazeProvider.GazeDirection.normalized * defaultDistanceInMeters;
                }
            }
        }
    }
}
