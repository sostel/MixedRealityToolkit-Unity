using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// <summary>
/// TestFixation01 only allowed to get the targets which raised interest right now. 
/// TestFixation02 allows for issuing a date and to get back the targets of interest at that time. 
/// </summary>
public class FixationTester_PerTarget2 : MonoBehaviour
{
    // TODO: 
    // - Log and see how well it works
    // - Handle brief outliers
    // - Extend to history to allow for looking back in history

    #region Singleton
    private static FixationTester_PerTarget2 instance;
    public static FixationTester_PerTarget2 Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<FixationTester_PerTarget2>();
            }
            return instance;
        }
    }
    #endregion

    public float minFixationTimeInMs = 150;
    public GameObject indicator;

    private GameObject prevLookedAtTarget = null;
    private GameObject currFixationTarget = null;
    private DateTime lookAtStartTime = DateTime.MaxValue;
    private bool isFixating = false;

    public void Start()
    {
        
    }

    public void Update()
    {
        // User is looking at new target
        if (prevLookedAtTarget != EyeTrackingTarget.LookedAtTarget) 
        {
            if(currFixationTarget != null)
            {
                InvokeFixationEnd((float)(DateTime.UtcNow - lookAtStartTime).TotalMilliseconds);
            }

            currFixationTarget = null;
            isFixating = false;
            lookAtStartTime = DateTime.UtcNow;            
        }
        // User is looking at the same target 
        else
        {
            TimeSpan tmpFixTime = DateTime.UtcNow - lookAtStartTime;
            if ((!isFixating)&&(tmpFixTime.TotalMilliseconds > minFixationTimeInMs))
            {
                isFixating = true;
                currFixationTarget = EyeTrackingTarget.LookedAtTarget;
                InvokeFixationStart();
            }
        }

        prevLookedAtTarget = EyeTrackingTarget.LookedAtTarget;
    }

    private void InvokeFixationStart()
    {
        Debug.Log($"Fixation START: {currFixationTarget.name}");
        if (indicator != null)
        {
            indicator.transform.position = currFixationTarget.transform.position;
            indicator.SetActive(true);
        }
    }

    private void InvokeFixationEnd(float fixationTimeInMs)
    {
        Debug.Log($"Fixation STOP: {currFixationTarget.name} -> {fixationTimeInMs} ms");
        if (indicator != null)
        {
            indicator.SetActive(false);
        }
    }

    public string FixationTargetName
    {
        get
        {
            if (currFixationTarget == null)
            {
                return "None";
            }

            return currFixationTarget.name;
        }
    }
}
