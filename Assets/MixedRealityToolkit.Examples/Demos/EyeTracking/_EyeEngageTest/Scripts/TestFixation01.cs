using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestFixation01 : MonoBehaviour
{

    public float scoreIncr = 2;
    public float scoreDecr = 1;
    public float maxScore = 30 * 5f; // FPS * timewindow in seconds

    public GameObject FixationIndicator;

    private Dictionary<GameObject, float> lossyInterest = new Dictionary<GameObject, float>();


    public void Start()
    {
        lossyInterest = new Dictionary<GameObject, float>();
        GazeHistory.Instance.HistoryUpdated.AddListener(HandleNewGaze);
    }

    private void HandleNewGaze()
    {
        // Handle fixations
        // The currently looked at target increases its interest, whereas all other targets decrease it
        // Problem with this approach: Biased towards larger targets. Does not take into account if I was quickly looking around a target

        // --------------------------------------------------------------------------
        // Increase interest for looked at target
        GameObject gobj = GazeHistory.Instance.GetMostRecentLookedAtTarget();
        if (gobj != null)
        {
            if (lossyInterest.ContainsKey(gobj))
            {
                //    int value = 0;
                //    lossyInterest.TryGetValue(ghe.lookedAtTarget.name, out value);
                lossyInterest[gobj] += scoreIncr;
                if (lossyInterest[gobj] > maxScore)
                {
                    lossyInterest[gobj] = maxScore;
                }

                Debug.Log($">> ADD 2 -- {gobj.name}: {lossyInterest[gobj]}]");
            }
            else
            {
                lossyInterest.Add(gobj, scoreIncr);
                Debug.Log($">> ADD NEW -- {gobj.name}: {lossyInterest[gobj]}]");
            }
        }

        // --------------------------------------------------------------------------
        // Decrease interest for everything else
        GameObject[] names = lossyInterest.Keys.ToArray<GameObject>();
        for (int i = 0; i < names.Length; i++)
        {
            if (names[i] != gobj)
            {
                lossyInterest[names[i]] -= scoreDecr;
                if (lossyInterest[names[i]] <= 0)
                {
                    lossyInterest.Remove(names[i]);
                    Debug.Log($">> REMOVED -- {names[i]}!!");
                }
                else
                {
                    Debug.Log($">> REMOVE -- {names[i]}: {lossyInterest[names[i]]}]");
                }
            }
        }

        // --------------------------------------------------------------------------
        // Identify fixation at certain time        
        GetFixatedTarget();
    }

    public GameObject GetFixatedTarget()
    {
        try
        {
            GameObject keyOfMaxValue = lossyInterest.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

            if (keyOfMaxValue != null)
            {
                if (FixationIndicator != null)
                {
                    FixationIndicator.transform.position = keyOfMaxValue.transform.position;
                }

                Debug.Log($">> ------------");
                Debug.Log($">> AND THE WINNER IS: {keyOfMaxValue.name} (Score: {lossyInterest.Values.Max()})");
                Debug.Log($">> ------------");
                Debug.Log($">> ------------");
            }

            return keyOfMaxValue;
        }
        catch (InvalidOperationException) { }

        return null;
    }
}
