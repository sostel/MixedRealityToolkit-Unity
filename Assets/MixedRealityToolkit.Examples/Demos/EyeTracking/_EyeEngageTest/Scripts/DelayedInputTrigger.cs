using Microsoft.MixedReality.Toolkit.Examples;
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
        if (EyeGazeHistory.Instance != null)
        {
            CurrentTarget = EyeGazeHistory.Instance.GetLookedAtTarget(DateTime.UtcNow);
            PredictedTarget = EyeGazeHistory.Instance.GetLookedAtTarget(dtInputArrived);
            OnTrigger.Invoke();
        }
    }

    public void UpdatePredictedTarget_UsingGazePoints(DateTime dateTime)
    {
        if (EyeGazeHistory.Instance != null)
        {
            CurrentTarget = EyeGazeHistory.Instance.GetLookedAtTarget(DateTime.UtcNow);
            PredictedTarget = EyeGazeHistory.Instance.GetLookedAtTarget(dateTime);
            OnTrigger.Invoke();
        }
    }

    public void UpdatePredictedTarget_UsingFixations(DateTime dateTime)
    {
        if (EyeGazeHistory.Instance != null)
        {
            CurrentTarget = EyeGazeHistory.Instance.GetFixatedTarget(DateTime.UtcNow);
            PredictedTarget = EyeGazeHistory.Instance.GetFixatedTarget(dateTime);
            OnTrigger.Invoke();
        }
    }
}
