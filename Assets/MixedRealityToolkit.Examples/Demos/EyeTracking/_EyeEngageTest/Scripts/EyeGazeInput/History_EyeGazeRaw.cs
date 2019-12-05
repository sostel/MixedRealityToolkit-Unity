using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;
using UnityEngine.Events;

public class History_EyeGazeRaw : HistoryBase
{
    #region Singleton
    private static History_EyeGazeRaw instance;
    public static History_EyeGazeRaw Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<History_EyeGazeRaw>();
            }
            return instance;
        }
    }
    #endregion

    private IMixedRealityEyeGazeProvider eyeProvider
    {
        get
        {
            return CoreServices.InputSystem?.EyeGazeProvider; // Warning: It may be better to load the reference once in the beginning for performance? 
        }
    }

    public UnityEvent 

    public new void Update()
    {
        Remember(); // ToDo: Would be nice to have an actual event that is triggered when the eye gaze ray has changed.
    }

    public override void Remember()
    {
        InputMemory_EyeGazeRaw memory = new InputMemory_EyeGazeRaw();
        memory.lookedAtTarget = EyeTrackingTarget.LookedAtTarget;
        memory.eyeGaze = new Ray(eyeProvider.GazeOrigin, eyeProvider.GazeDirection);

        Remember(eyeProvider.Timestamp, memory);
    }

    public GameObject GetLookedAtTargetAt(DateTime date)
    {
        InputMemory_EyeGazeRaw gazeHistoryEntry = GetMemoryAt(date);
        return ((gazeHistoryEntry != null) ? gazeHistoryEntry.lookedAtTarget : null);
    }

    public GameObject GetMostRecentLookedAtTarget()
    {
        return GetLookedAtTargetAt(DateTime.UtcNow);
    }
}