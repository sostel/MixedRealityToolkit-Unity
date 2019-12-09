using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;

public class History_EyeGazeProcessed : History_EyeGazeRaw
{
    #region Singleton
    private static History_EyeGazeProcessed instance;
    public new static History_EyeGazeProcessed Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<History_EyeGazeProcessed>();
            }
            return instance;
        }
    }
    #endregion

    public new void Start()
    {
        OnHistoryUpdated.AddListener(ProcessGazeData);
    }

    public void ProcessGazeData()
    {
        //Debug.Log($"New eye gaze data to process: {((InputMemory_EyeGazeRaw)GetMostRecentMemory()).timestamp.ToShortTimeString()} \t--> " +
        //    $"{((InputMemory_EyeGazeRaw)GetMostRecentMemory()).lookedAtTarget} ");

        // TODO: Handle new eye gaze data -> Saccades, Fixations... If uncertain, mark as "PotentialFixation" and update later.
    }
}