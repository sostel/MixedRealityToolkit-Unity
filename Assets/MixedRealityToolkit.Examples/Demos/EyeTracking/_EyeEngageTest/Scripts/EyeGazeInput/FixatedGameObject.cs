using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A collection of "interesting" game objects with their "interest scores".
/// </summary>
public class FixatedGameObject
{

    public DateTime timestamp;
    public Dictionary<GameObject, float> FixatedTargetsDic { get; private set; }
    
    public FixatedGameObject()
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
