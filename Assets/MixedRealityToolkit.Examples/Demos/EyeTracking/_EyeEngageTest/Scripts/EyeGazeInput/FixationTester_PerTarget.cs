using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// TestFixation01 only allowed to get the targets which raised interest right now. 
/// TestFixation02 allows for issuing a date and to get back the targets of interest at that time. 
/// </summary>
public class FixationTester_PerTarget : MonoBehaviour
{
    #region Singleton
    private static FixationTester_PerTarget instance;
    public static FixationTester_PerTarget Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<FixationTester_PerTarget>();
            }
            return instance;
        }
    }
    #endregion

    [SerializeField]
    private float maxMemoryInSeconds = 10f;

    [SerializeField]
    private float minFixationInSeconds = 0.15f;

    [SerializeField]
    private float maxFixationInSeconds = 1.0f; // TODO: Is this even necessary?

    [SerializeField]
    private GameObject fixationIndicatorTemplate = null;

    [SerializeField]
    private int maxAmountOfFixationIndicators = 100;

    private Dictionary<DateTime, FixatedGameObject> fixationHistory = new Dictionary<DateTime, FixatedGameObject>();
    private FixatedGameObject currInterestList = null; // A collection of "interesting" game objects with their "interest scores".
    private List<GameObject> fixationIndicators;

    public bool clearIndicators = false;

    public void Start()
    {
        ResetHistory();
        EyeGazeHistory.Instance.OnHistoryUpdated.AddListener(Remember);
    }

    public void Update()
    {
        ForgetOverTime();

        if (clearIndicators)
        {
            clearIndicators = false;
            ClearAllFixationIndicators();
        }
        ClearFixationIndicators();
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

    private void ClearFixationIndicators()
    {
        if ((fixationIndicators != null) && (fixationIndicators.Count > maxAmountOfFixationIndicators))
        {
            while (fixationIndicators.Count > maxAmountOfFixationIndicators)
            {
                GameObject tmpObj = fixationIndicators[0];
                fixationIndicators.RemoveAt(0);
                GameObject.Destroy(tmpObj);
            }
        }
    }

    private void ClearAllFixationIndicators()
    {
        if ((fixationIndicators != null) && (fixationIndicators.Count > 0))
        {
            for (int i = fixationIndicators.Count - 1; i >= 0; i--)
            {
                Destroy(fixationIndicators[i]);
                fixationIndicators.RemoveAt(i);
            }
        }
    }

    public void ResetHistory()
    {
        ClearFixationIndicators();
        fixationIndicators = new List<GameObject>();
        fixationHistory = new Dictionary<DateTime, FixatedGameObject>();
        currInterestList = new FixatedGameObject();
    }

    public void Remember()
    {
        // Handle fixations
        // The currently looked at target increases its interest, whereas all other targets decrease it
        // Problem with this approach: Biased towards larger targets. Does not take into account if I was quickly looking around a target
        EyeGazeHistoryEntry gazeHistEntry = EyeGazeHistory.Instance.GetMostRecentEntry();
        if (gazeHistEntry != null)
        {
            currInterestList.timestamp = gazeHistEntry.timestamp;
            //IncreaseInterest(gazeHistEntry.lookedAtTarget);
            //DecreaseInterest(gazeHistEntry.lookedAtTarget);

            FixatedGameObject newEntry = new FixatedGameObject();
            newEntry.UpdateDictionary(gazeHistEntry.timestamp, currInterestList.FixatedTargetsDic);
            fixationHistory.Add(newEntry.timestamp, newEntry);

            // --------------------------------------------------------------------------
            // Identify fixation at certain time        
            //  GetFixatedTarget(currInterestList, true);
            
            DateTime dt1 = DateTime.UtcNow;
            DateTime dt = dt1.AddSeconds(-delayInSeconds);
            GetFixatedTarget(dt);

            GetFixatedTarget(dt1);

            previousLookedAtTarget = gazeHistEntry.lookedAtTarget;
            previousLookedAtTargetTimestamp = gazeHistEntry.timestamp;
        }
    }

    private double Increase_DeltaFixationTimeInSeconds(GameObject currTarget)
    {
        // If there is not previously looked at target or the previously looked at target is different to our current one,
        // let's not count that towards a new fixation.
        if ((previousLookedAtTarget == null) || (currTarget != previousLookedAtTarget))
        {
            return 0; 
        }
        return (DateTime.UtcNow - previousLookedAtTargetTimestamp).TotalSeconds;
    }

    private double Decrease_DeltaFixationTimeInSeconds(GameObject currTarget)
    {
        // If there is not previously looked at target or the previously looked at target is different to our current one,
        // let's not count that towards a new fixation.
        if ((previousLookedAtTarget == null) || (currTarget != previousLookedAtTarget))
        {
            return 0;
        }
        return (DateTime.UtcNow - previousLookedAtTargetTimestamp).TotalSeconds;
    }

    public float delayInSeconds = 2f;
    public GameObject previousLookedAtTarget = null;
    public DateTime previousLookedAtTargetTimestamp = DateTime.MinValue;
   
    /// <summary>
    /// Increase interest for looked at target.
    /// </summary>
    private void IncreaseInterest(GameObject currTarget)
    {
        if (currTarget != null)
        {
            if (currInterestList.FixatedTargetsDic.ContainsKey(currTarget))
            {
                if (currTarget != previousLookedAtTarget)
                {
                    currInterestList.FixatedTargetsDic[currTarget] = 0;
                }
                else
                {
                    currInterestList.FixatedTargetsDic[currTarget] += (float)Increase_DeltaFixationTimeInSeconds(currTarget);
                    if (currInterestList.FixatedTargetsDic[currTarget] > maxFixationInSeconds)
                    {
                        currInterestList.FixatedTargetsDic[currTarget] = maxFixationInSeconds;
                    }

                    if (currInterestList.FixatedTargetsDic[currTarget] > minFixationInSeconds)
                    {
                        Debug.Log($">> INCR >> [{currTarget.name}]: {currInterestList.FixatedTargetsDic[currTarget]} [Fixation!]");
                    }
                    else
                    {
                        Debug.Log($">> INCR >> [{currTarget.name}]: {currInterestList.FixatedTargetsDic[currTarget]} [?]");
                    }
                }
            }
            else
            {
                currInterestList.FixatedTargetsDic.Add(currTarget, 0);                
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
                float oldScore = currInterestList.FixatedTargetsDic[targetsOfInterest[i]];
                currInterestList.FixatedTargetsDic[targetsOfInterest[i]] -= (float)Decrease_DeltaFixationTimeInSeconds(currTarget);
                float newScore = currInterestList.FixatedTargetsDic[targetsOfInterest[i]];

                Debug.Log($">> [{targetsOfInterest[i].name}]: {oldScore} --> {newScore}");
                if (currInterestList.FixatedTargetsDic[targetsOfInterest[i]] <= 0)
                {
                    currInterestList.FixatedTargetsDic.Remove(targetsOfInterest[i]);
                }             
            }
        }
        Debug.Log($">>---------");
    }

    public GameObject GetFixatedTarget(FixatedGameObject fixTargets)
    {
        if ((fixTargets != null) && (fixTargets.FixatedTargetsDic != null))
        {
            KeyValuePair<GameObject, float> valuePair = fixTargets.FixatedTargetsDic.Aggregate((x, y) => x.Value > y.Value ? x : y);
            GameObject keyOfMaxValue = valuePair.Key;

            if (keyOfMaxValue != null)
            {
                if (fixationIndicatorTemplate != null)
                {
                    GameObject newInstance = GameObject.Instantiate(fixationIndicatorTemplate);
                    fixationIndicators.Add(newInstance);
                    newInstance.SetActive(true);
                    newInstance.transform.position = keyOfMaxValue.transform.position;                    
                }

                float deltaTimeInSec = Mathf.Round((float)(DateTime.UtcNow - fixTargets.timestamp).TotalSeconds * 1000) / 1000f;

                Debug.Log($">> [FiX-2] AND THE WINNER IS: {keyOfMaxValue.name} (Score: {fixTargets.FixatedTargetsDic.Values.Max()}) -- DeltaTime: {deltaTimeInSec}");
            }

            return keyOfMaxValue;
        }
        return null;
    }

    public GameObject GetFixatedTarget(DateTime date)
    {
        if ((fixationHistory != null) && (fixationHistory.Count > 0) && (fixationHistory.Keys.Count > 0))
        {
            IEnumerable<DateTime> results = fixationHistory.Keys.Where(x => x <= date); // Warning: Not the most efficient!?
            if ((results != null) && (results.Count() > 0))
            {
                DateTime result = results.Max();
                FixatedGameObject historicFixations;
                bool check = fixationHistory.TryGetValue(result, out historicFixations);

                if (check)
                {
                    string sdatePast = date.ToString("HH:mm.ss:FFF");
                    string sdateQuery = result.ToString("HH:mm.ss:FFF");
                    string sdateNow = DateTime.UtcNow.ToString("HH:mm.ss:FFF");
                    float deltaTimeInSec = Mathf.Round((float)(DateTime.UtcNow - date).TotalSeconds * 1000f) / 1000f;
                    return GetFixatedTarget(historicFixations);
                }
            }
        }
        return null;
    }
}
