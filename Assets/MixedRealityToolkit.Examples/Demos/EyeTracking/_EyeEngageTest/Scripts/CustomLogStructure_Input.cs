// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Boo.Lang;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityScript.Steps;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.Logging
{
    public class CustomLogStructure_Input : LogStructure
    {
        private IMixedRealityEyeGazeProvider EyeTrackingProvider => eyeTrackingProvider ?? (eyeTrackingProvider = CoreServices.InputSystem?.EyeGazeProvider);
        private IMixedRealityEyeGazeProvider eyeTrackingProvider = null;

        public override string[] GetHeaderColumns()
        {
            string[] part1 = new string[]
            {
                // UserId
                "UserId",
                // SessionType
                "SessionType",
                // Timestamp
                "dt in ms",
                // Cam / Head tracking
                "HeadOrigin.x",
                "HeadOrigin.y",
                "HeadOrigin.z",
                "HeadDir.x",
                "HeadDir.y",
                "HeadDir.z",
                // Raw eye gaze tracking 
                "EyeOrigin.x",
                "EyeOrigin.y",
                "EyeOrigin.z",
                "EyeDir.x",
                "EyeDir.y",
                "EyeDir.z",
                "EyeHitPos.x",
                "EyeHitPos.y",
                "EyeHitPos.z",
                "LookedAtTarget",
                // Study
                "IntendedTarget",
                // Fixation testers
                "FixatedTarget",

                // Voice input
                "Voice.LastUpdated",
                "Voice.Text",
                "Voice.Status",
                // Different techniques
                //
            };

            string[] part2 = TargetingStudyManager.Instance.GetListOfTestInputNames();
            return MergeStringArrays(part1, part2);
        }

        public string[] MergeStringArrays(string[] part1, string[] part2)
        {
            string[] result = new string[part1.Length + part2.Length];
            part1.CopyTo(result, 0);
            part2.CopyTo(result, part1.Length);
            return result;
        }

        public override object[] GetData(string inputType, string inputStatus, EyeTrackingTarget intTarget)
        {
            // Let's prepare all the data we wanna log
            // Eye gaze hit position
            Vector3? eyeHitPos = null;
            string lookedAtTarget = "NaN";
            if (EyeTrackingProvider?.GazeTarget != null && EyeTrackingProvider.IsEyeGazeValid)
            {
                eyeHitPos = EyeTrackingProvider.HitPosition;
                lookedAtTarget = eyeTrackingProvider.HitInfo.collider.name;
            }

            // Voice input
            VoiceHistoryEntry voiceMemory = VoiceHistory.Instance.GetMostRecentEntry();

            // Put it all together
            object[] data = new object[]
            { 
                //-------------------------------
                // Cam / Head tracking
                CameraCache.Main.transform.position.x,
                CameraCache.Main.transform.position.y,
                CameraCache.Main.transform.position.z,
                CameraCache.Main.transform.forward.x,
                CameraCache.Main.transform.forward.y,
                CameraCache.Main.transform.forward.z,

                // Raw eye gaze tracking 
                EyeTrackingProvider.IsEyeGazeValid ? EyeTrackingProvider.GazeOrigin.x : 0,
                EyeTrackingProvider.IsEyeGazeValid ? EyeTrackingProvider.GazeOrigin.y : 0,
                EyeTrackingProvider.IsEyeGazeValid ? EyeTrackingProvider.GazeOrigin.z : 0,

                EyeTrackingProvider.IsEyeGazeValid ? EyeTrackingProvider.GazeDirection.x : 0,
                EyeTrackingProvider.IsEyeGazeValid ? EyeTrackingProvider.GazeDirection.y : 0,
                EyeTrackingProvider.IsEyeGazeValid ? EyeTrackingProvider.GazeDirection.z : 0,

                (eyeHitPos != null) ? eyeHitPos.Value.x : float.NaN,
                (eyeHitPos != null) ? eyeHitPos.Value.y : float.NaN,
                (eyeHitPos != null) ? eyeHitPos.Value.z : float.NaN,

                lookedAtTarget,
                TargetingStudyManager.Instance.IntendedTarget.name,

                // Fixation testers
                FixationTester_PerTarget2.Instance?.FixationTargetName,

                // Voice input
                voiceMemory.timestamp,
                voiceMemory.dictationText,
                voiceMemory.dictationStatus,
            };

            // Targeting prediction approaches
            //List<object> data2 = new List<object>();
            //data2.AddRange(data);
            //data2.AddRange(TargetingStudyManager.Instance.GetListOfPredictedTargetNames());
            //return data2.ToArray();

            return data;
        }
    }
}