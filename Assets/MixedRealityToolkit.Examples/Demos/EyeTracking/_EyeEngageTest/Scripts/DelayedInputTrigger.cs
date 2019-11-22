﻿using Microsoft.MixedReality.Toolkit.Examples;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DelayedInputTrigger : MonoBehaviour
{       
    private DateTime dtInputArrived;
    
    internal UnityEvent OnTrigger = new UnityEvent();

    public GameObject PredictedTarget { get; private set; } = null;
    public GameObject CurrentTarget { get; private set; } = null;

    public void TriggerDelayedInput(string sourceName, float delayInSec)
    {
        dtInputArrived = DateTime.UtcNow;
        StartCoroutine(DelayedButtonRecall(delayInSec));
        
        string sdate = dtInputArrived.ToString("HH:mm.ss:FFF");
        Debug.Log($"gfh29 [{sdate} -- [{sourceName}] triggered with [{delayInSec}] seconds delay.");
    }

    public void TriggerVoiceInput(string sourceName)
    {
        Debug.Log($"gfh30 TriggerVoiceInput.");
        DateTime? dt = VoiceHistory.Instance.HypothesisStart();

        if (dt != null)
        {
            double delayInSec = (DateTime.UtcNow - dt.Value).TotalSeconds;
            UpdatePredictedTarget_UsingGazePoints(dt.Value);
            string sdate = dt.Value.ToString("HH:mm.ss:FFF");
            Debug.Log($"gfh30 [{sdate} -- [{sourceName}] triggered with [{delayInSec}] seconds delay.");
        }        
    }


    IEnumerator DelayedButtonRecall(float delayInSec)
    {
        yield return new WaitForSeconds(delayInSec);

        // Code to execute after the delay
        if (GazeHistory.Instance != null)
        {
            CurrentTarget = GazeHistory.Instance.GetLookedAtTarget(DateTime.UtcNow);
            PredictedTarget = GazeHistory.Instance.GetLookedAtTarget(dtInputArrived);
            OnTrigger.Invoke();
        }
    }

    public void UpdatePredictedTarget_UsingGazePoints(DateTime dateTime)
    {
        if (GazeHistory.Instance != null)
        {
            CurrentTarget = GazeHistory.Instance.GetLookedAtTarget(DateTime.UtcNow);
            PredictedTarget = GazeHistory.Instance.GetLookedAtTarget(dateTime);
            OnTrigger.Invoke();
        }
    }

    public void UpdatePredictedTarget_UsingFixations(DateTime dateTime)
    {
        if (GazeHistory.Instance != null)
        {
            CurrentTarget = GazeHistory.Instance.GetFixatedTarget(DateTime.UtcNow);
            PredictedTarget = GazeHistory.Instance.GetFixatedTarget(dateTime);
            OnTrigger.Invoke();
        }
    }
}
