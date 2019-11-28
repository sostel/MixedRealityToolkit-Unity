using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EyeGazeHistory : MonoBehaviour
{
    #region Singleton
    private static EyeGazeHistory instance;
    public static EyeGazeHistory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EyeGazeHistory>();
            }
            return instance;
        }
    }
    #endregion

    internal Dictionary<DateTime, EyeGazeHistoryEntry> History { get; private set; }

    [SerializeField]
    private float maxMemoryInSeconds = 3f;

    internal UnityEvent OnHistoryUpdated = new UnityEvent();


    private IMixedRealityEyeGazeProvider eyeProvider
    {
        get
        {
            return CoreServices.InputSystem?.EyeGazeProvider; // Warning: It may be better to load the reference once in the beginning for performance? 
        }
    }

    public void Start()
    {
        ResetHistory();
    }

    public void Update()
    {
        Remember();
        ForgetOverTime();

        if (OnHistoryUpdated != null)
        {
            OnHistoryUpdated.Invoke();
        }
    }

    private void ForgetOverTime()
    {
        DateTime date = DateTime.UtcNow.AddSeconds(-maxMemoryInSeconds);
        List<DateTime> results = History.Keys.Where(x => x <= date).ToList();

        for (int i = 0; i < results.Count; i++)
        {
            History.Remove(results[i]);
        }
    }

    public void ResetHistory()
    {
        History = new Dictionary<DateTime, EyeGazeHistoryEntry>();
    }

    private void Remember()
    {
        DateTime date = eyeProvider.Timestamp;
        int result = History.Keys.Where(x => x > date).Count();

        if (result == 0)
        {
            EyeGazeHistoryEntry ghe = new EyeGazeHistoryEntry();
            ghe.lookedAtTarget = EyeTrackingTarget.LookedAtTarget;
            ghe.eyeGaze = new Ray(eyeProvider.GazeOrigin, eyeProvider.GazeDirection);
            History.Add(eyeProvider.Timestamp, ghe);
        }
    }

    public GameObject GetLookedAtTarget(DateTime date)
    {
        EyeGazeHistoryEntry gazeHistoryEntry = GetEntry(date);
        return ((gazeHistoryEntry != null) ? gazeHistoryEntry.lookedAtTarget : null);
    }

    public EyeGazeHistoryEntry GetEntry(DateTime date)
    {
        DateTime result = History.Keys.Where(x => x <= date).Max(); // Warning: Not the most efficient!?
        EyeGazeHistoryEntry gazeHistoryEntry;
        bool check = History.TryGetValue(result, out gazeHistoryEntry);
        gazeHistoryEntry.timestamp = result;

        if (check)
        {
            return gazeHistoryEntry;
        }

        return null;
    }

    public GameObject GetMostRecentLookedAtTarget()
    {
        return GetLookedAtTarget(DateTime.UtcNow);
    }

    public EyeGazeHistoryEntry GetMostRecentEntry()
    {
        return GetEntry(DateTime.UtcNow);
    }

    public GameObject GetFixatedTarget(DateTime date)
    {
        GameObject lookedAtTarget = null;

        DateTime[] results = (DateTime[])History.Keys.Where(x => x <= date); // Warning: Not the most efficient!?
                                                                             ///??
        return lookedAtTarget;
    }
}