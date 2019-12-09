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

    private IMixedRealityEyeGazeProvider eyeGazeProvider
    {
        get
        {
            return CoreServices.InputSystem?.EyeGazeProvider; // Warning: It may be better to load the reference once in the beginning for performance? 
        }
    }

    public new void Update()
    {
        Remember(); // ToDo: Would be nice to have an actual event that is triggered when the eye gaze ray has changed.
        base.Update();
    }

    public override void Remember()
    {
        Remember(eyeGazeProvider.Timestamp, new InputMemory_EyeGazeRaw(
            new Ray(eyeGazeProvider.GazeOrigin, eyeGazeProvider.GazeDirection), 
            EyeTrackingTarget.LookedAtTarget, EyeTrackingTarget.LookedAtPoint));
    }

    public GameObject GetLookedAtTargetAt(DateTime date)
    {
        return ((InputMemory_EyeGazeRaw)this.GetMemoryAt(date))?.lookedAtTarget;
    }

    public GameObject GetMostRecentLookedAtTarget()
    {
        return GetLookedAtTargetAt(DateTime.UtcNow);
    }
}