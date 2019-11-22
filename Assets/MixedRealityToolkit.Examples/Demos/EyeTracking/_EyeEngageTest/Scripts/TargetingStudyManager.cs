using System;
using UnityEngine;
using UnityEngine.Rendering;

public class TargetingStudyManager : MonoBehaviour
{
    #region Singleton
    private static TargetingStudyManager instance;
    public static TargetingStudyManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TargetingStudyManager>();
            }
            return instance;
        }
    }
    #endregion

    public DelayedInputTrigger[] testInputs;
    
    public string[] GetListOfTestInputNames()
    {
        string[] result = new string[testInputs.Length];
        for (int i = 0; i < testInputs.Length; i++)
        {
            result[i] = testInputs[i].name;
        }
        return result;
    }

    public string[] GetListOfPredictedTargetNames()
    {
        string[] result = new string[testInputs.Length];
        for (int i = 0; i < testInputs.Length; i++)
        {
            if (testInputs[i].PredictedTarget != null)
            {
                result[i] = testInputs[i].PredictedTarget.name;
            }
            else
            {
                result[i] = "None";
            }
        }
        return result;
    }

    public GameObject IntendedTarget { get; internal set; }

    public void UpdateTarget(GameObject newTarget)
    {
        IntendedTarget = newTarget;
    }
}
