using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// TestFixation01 only allowed to get the targets which raised interest right now. 
/// TestFixation02 allows for issuing a date and to get back the targets of interest at that time. 
/// </summary>
public class TestFixation02 : MonoBehaviour
{
    /// <summary>
    /// A collection of "interesting" game objects with their "interest scores".
    /// </summary>
    public class FixatedTargets
    {
        public DateTime timestamp;
        public Dictionary<GameObject, float> FixatedTargetsDic { get; private set; }

        public FixatedTargets()
        {
            FixatedTargetsDic = new Dictionary<GameObject, float>();
        }

        public void AddFixatedTarget(GameObject gobj, float score)
        {
            FixatedTargetsDic.Add(gobj, score);
        }

        public void UpdateDictionary(DateTime date, Dictionary<GameObject, float> tmpFixatedTargets)
        {
            timestamp = date;
            FixatedTargetsDic = new Dictionary<GameObject, float>();
            GameObject[] list = tmpFixatedTargets.Keys.ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                AddFixatedTarget(list[i], tmpFixatedTargets[list[i]]);
            }
        }
    }

    [SerializeField]
    private float maxMemoryInSeconds = 10f;

    public GameObject FixationIndicator_Now;
    public GameObject FixationIndicator_2sAgo;

    public float scoreIncr = 2;
    public float scoreDecr = 1;
    public float maxScore = 30 * 5f; // FPS * timewindow in seconds

    private Dictionary<DateTime, FixatedTargets> fixationHistory = new Dictionary<DateTime, FixatedTargets>();
    private FixatedTargets currInterestList = null; // A collection of "interesting" game objects with their "interest scores".

    public void Start()
    {
        ResetHistory();
        GazeHistory.Instance.HistoryUpdated.AddListener(Remember);
    }

    public void Update()
    {
        ForgetOverTime();
    }

    private void ForgetOverTime()
    {
        DateTime date = DateTime.UtcNow.AddSeconds(-maxMemoryInSeconds);
        List<DateTime> results = fixationHistory.Keys.Where(x => x <= date).ToList();

        for (int i = 0; i < results.Count; i++)
        {
            fixationHistory.Remove(results[i]);
        }
    }

    public void ResetHistory()
    {
        fixationHistory = new Dictionary<DateTime, FixatedTargets>();
        currInterestList = new FixatedTargets();
    }

    public void Remember()
    {
        // Handle fixations
        // The currently looked at target increases its interest, whereas all other targets decrease it
        // Problem with this approach: Biased towards larger targets. Does not take into account if I was quickly looking around a target
        GazeHistoryEntry gazeHistEntry = GazeHistory.Instance.GetMostRecentEntry();
        if (gazeHistEntry != null)
        {
            currInterestList.timestamp = gazeHistEntry.timestamp;
            IncreaseInterest(gazeHistEntry.lookedAtTarget);
            DecreaseInterest(gazeHistEntry.lookedAtTarget);

            FixatedTargets newEntry = new FixatedTargets();
            newEntry.UpdateDictionary(gazeHistEntry.timestamp, currInterestList.FixatedTargetsDic);
            fixationHistory.Add(newEntry.timestamp, newEntry);

            Debug.Log($">> [FiX-2] 11-gazeHistEntry.timestamp: {gazeHistEntry.timestamp} >>");
            Debug.Log($">> [FiX-2] 11-AddTimeStamp: {currInterestList.timestamp} >>");
            

            // --------------------------------------------------------------------------
            // Identify fixation at certain time        
            // Test
            //  GetFixatedTarget(currInterestList, true);

            DateTime dt1 = DateTime.UtcNow;
            DateTime dt = dt1.AddSeconds(-delayInSeconds);
            GetFixatedTarget(dt);
            
        }
    }

    public float delayInSeconds = 2f;
    /// <summary>
    /// Increase interest for looked at target.
    /// </summary>
    private void IncreaseInterest(GameObject currTarget)
    {
        if (currTarget != null)
        {
            if (currInterestList.FixatedTargetsDic.ContainsKey(currTarget))
            {
                currInterestList.FixatedTargetsDic[currTarget] += scoreIncr;
                if (currInterestList.FixatedTargetsDic[currTarget] > maxScore)
                {
                    currInterestList.FixatedTargetsDic[currTarget] = maxScore;
                }

                Debug.Log($">> ADD 2 -- {currTarget.name}: {currInterestList.FixatedTargetsDic[currTarget]}]");
            }
            else
            {
                currInterestList.FixatedTargetsDic.Add(currTarget, scoreIncr);
                Debug.Log($">> ADD NEW -- {currTarget.name}: {currInterestList.FixatedTargetsDic[currTarget]}]");
            }
        }
    }

    /// <summary>
    /// Increase interest for looked at target.
    /// </summary>
    private void DecreaseInterest(GameObject currTarget)
    {
        GameObject[] targetsOfInterest = currInterestList.FixatedTargetsDic.Keys.ToArray<GameObject>();
        for (int i = 0; i < targetsOfInterest.Length; i++)
        {
            if ((currTarget == null) || (targetsOfInterest[i] != currTarget))
            {
                currInterestList.FixatedTargetsDic[targetsOfInterest[i]] -= scoreDecr;
                if (currInterestList.FixatedTargetsDic[targetsOfInterest[i]] <= 0)
                {
                    currInterestList.FixatedTargetsDic.Remove(targetsOfInterest[i]);
                    Debug.Log($">> REMOVED -- {targetsOfInterest[i]}!!");
                }
                else
                {
                    Debug.Log($">> REMOVE -- {targetsOfInterest[i]}: {currInterestList.FixatedTargetsDic[targetsOfInterest[i]]}]");
                }
            }
        }
    }

    public GameObject GetFixatedTarget(FixatedTargets fixTargets, bool now)
    {
        GameObject keyOfMaxValue = fixTargets.FixatedTargetsDic.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

        if (keyOfMaxValue != null)
        {
            if (now && (FixationIndicator_Now != null))
            {
                FixationIndicator_Now.transform.position = keyOfMaxValue.transform.position;
            }
            else if (FixationIndicator_2sAgo != null)
            {
                FixationIndicator_2sAgo.transform.position = keyOfMaxValue.transform.position;
            }

            float deltaTimeInSec = Mathf.Round((float)(DateTime.UtcNow - fixTargets.timestamp).TotalSeconds * 1000) / 1000f;

            Debug.Log($">> ------------");
            Debug.Log($">> [FiX-2] AND THE WINNER IS: NOW?? {now}>> {keyOfMaxValue.name} (Score: {fixTargets.FixatedTargetsDic.Values.Max()}) -- DeltaTime: {deltaTimeInSec}");
            Debug.Log($">> ------------");
        }

        return keyOfMaxValue;
    }

    public GameObject GetFixatedTarget(DateTime date)
    {
        if ((fixationHistory != null) && (fixationHistory.Count > 0) && (fixationHistory.Keys.Count > 0))
        {
            IEnumerable<DateTime> results = fixationHistory.Keys.Where(x => x <= date); // Warning: Not the most efficient!?
            if ((results != null) && (results.Count() > 0))
            {
                DateTime result = results.Max();

                Debug.Log($">> 2");
                FixatedTargets historicFixations;
                bool check = fixationHistory.TryGetValue(result, out historicFixations);

                if (check)
                {
                    string sdatePast = date.ToString("HH:mm.ss:FFF");
                    string sdateQuery = result.ToString("HH:mm.ss:FFF");
                    string sdateNow = DateTime.UtcNow.ToString("HH:mm.ss:FFF");

                    //historicFixations.timestamp = date;
                    float deltaTimeInSec = Mathf.Round((float)(DateTime.UtcNow - date).TotalSeconds * 1000f) / 1000f;
                    Debug.Log($">> [FiX-2] QUERY: {sdatePast} \t RESULT: {sdateQuery} \t NOW: {sdateNow} >> -- DeltaTime: {deltaTimeInSec}");
                    return GetFixatedTarget(historicFixations, false);
                }
            }
        }
        return null;
    }
}
