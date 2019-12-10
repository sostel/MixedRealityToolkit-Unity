using System;
using System.Collections.Generic;
using UnityEngine;

public class InputMemory_EyeGazeProcessed : InputMemory
{
    public List<Vector3> gazeDirections;
    public DateTime tStartFixation { get; private set; }
    public DateTime tEndFixation { get; private set; }
    
    public InputMemory_EyeGazeProcessed(DateTime startFixationTime)
    {
        gazeDirections = new List<Vector3>();
        tStartFixation = startFixationTime;
    }

    public void Update(DateTime newEndFixationTime, Vector3 additionalDirection)
    {
        tEndFixation = newEndFixationTime;
        gazeDirections.Add(additionalDirection);
    }

    public float FixationRadius
    {
        get
        {
            if ((gazeDirections != null) && (gazeDirections.Count > 0))
            {
                Vector3 meanDir = FixationMeanDirection;
                float maxDist = float.MinValue;

                for (int i = 0; i < gazeDirections.Count; i++)
                {
                    float dist = Vector3.Distance(FixationMeanDirection, gazeDirections[i]);
                    if (dist > maxDist)
                    {
                        maxDist = dist;
                    }
                }
                return maxDist;
            }
            return 0;
        }
    }

    public Vector3 FixationMeanDirection
    {
        get
        {
            if ((gazeDirections != null) && (gazeDirections.Count > 0))
            {
                Vector3 meanDir = Vector3.zero;
                for (int i = 0; i < gazeDirections.Count; i++)
                {
                    meanDir += gazeDirections[i];
                }
                meanDir /= gazeDirections.Count;
                return meanDir;
            }
            return Vector3.zero;
        }
    }
}