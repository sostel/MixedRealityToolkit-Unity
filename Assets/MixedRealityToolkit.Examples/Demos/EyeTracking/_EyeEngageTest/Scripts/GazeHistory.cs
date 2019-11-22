using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GazeHistoryEntry
{
    public DateTime timestamp;
    public Ray eyeGaze;
    public RaycastHit hitInfo;
    public GameObject lookedAtTarget;
}

public class GazeHistory : MonoBehaviour
{
    #region Singleton
    private static GazeHistory instance;
    public static GazeHistory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GazeHistory>();
            }
            return instance;
        }
    }
    #endregion

    internal Dictionary<DateTime, GazeHistoryEntry> History { get; private set; }

    [SerializeField]
    private float maxMemoryInSeconds = 3f;

    internal UnityEvent HistoryUpdated = new UnityEvent();
    
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
        HistoryUpdated.Invoke();
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
        History = new Dictionary<DateTime, GazeHistoryEntry>();
    }
        
    public void Remember()
    {
        DateTime date = eyeProvider.Timestamp;
        int result = History.Keys.Where(x => x > date).Count();

        if (result == 0)
        {
            GazeHistoryEntry ghe = new GazeHistoryEntry();
            ghe.lookedAtTarget = EyeTrackingTarget.LookedAtTarget;
            ghe.eyeGaze = new Ray(eyeProvider.GazeOrigin, eyeProvider.GazeDirection);
            History.Add(eyeProvider.Timestamp, ghe);
        }
    }

    public GameObject GetLookedAtTarget(DateTime date)
    {
        GazeHistoryEntry gazeHistoryEntry = GetEntry(date);
        return ((gazeHistoryEntry != null) ? gazeHistoryEntry.lookedAtTarget : null);
    }

    public GazeHistoryEntry GetEntry(DateTime date)
    {
        DateTime result = History.Keys.Where(x => x <= date).Max(); // Warning: Not the most efficient!?
        GazeHistoryEntry gazeHistoryEntry;
        bool check = History.TryGetValue(result, out gazeHistoryEntry);

        gazeHistoryEntry.timestamp = result;

        Debug.Log($"GazeHist: [{check}] -- {gazeHistoryEntry.timestamp}");
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

    public GazeHistoryEntry GetMostRecentEntry()
    {
        return GetEntry(DateTime.UtcNow);
    }
    
    public GameObject GetFixatedTarget (DateTime date)
    {
        GameObject lookedAtTarget = null;

        DateTime[] results = (DateTime[])History.Keys.Where(x => x <= date); // Warning: Not the most efficient!?
 ///??
        return lookedAtTarget;
    }
}
