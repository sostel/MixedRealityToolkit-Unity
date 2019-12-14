using Microsoft.MixedReality.Toolkit.Examples;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InputMemory_EyeGazeProcessed : InputMemory
{
    public List<InputMemory_EyeGazeRaw> rawEyeGazes;
    
    public DateTime tStartFixation { get; private set; }
    public DateTime tEndFixation { get; private set; }
    
    public InputMemory_EyeGazeProcessed(DateTime startFixationTime)
    {
        rawEyeGazes = new List<InputMemory_EyeGazeRaw>();
        tStartFixation = startFixationTime;
    }

    public void Update(InputMemory_EyeGazeRaw rawEyeGaze, VoiceDictationStatus voiceStatus)
    {
        tEndFixation = rawEyeGaze.timestamp;
        rawEyeGaze.voiceInputStatus = voiceStatus;
        rawEyeGazes.Add(rawEyeGaze);
    }

    public int Count
    {
        get { return rawEyeGazes.Count; }
    }
    public float FixationRadius
    {
        //ToDo: better to use angular radius 
        get
        {
            if ((rawEyeGazes != null) && (rawEyeGazes.Count > 0))
            {
                Vector3 meanDir = FixationMeanDirection;
                float maxDist = float.MinValue;

                for (int i = 0; i < rawEyeGazes.Count; i++)
                {
                    float dist = Vector3.Distance(FixationMeanDirection, rawEyeGazes[i].eyeGaze.direction);
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

    public Ray FixationMeanRay
    {
        get
        {
            return new Ray(FixationMeanOrigin, FixationMeanDirection); 
        }
    }

    public Vector3 FixationMeanDirection
    {
        get
        {
            if ((rawEyeGazes != null) && (rawEyeGazes.Count > 0))
            {
                Vector3 meanDir = Vector3.zero;
                for (int i = 0; i < rawEyeGazes.Count; i++)
                {
                    meanDir += rawEyeGazes[i].eyeGaze.direction;
                }
                meanDir /= rawEyeGazes.Count;
                return meanDir;
            }
            return Vector3.zero;
        }
    }

    public Vector3 FixationMeanOrigin
    {
        get
        {
            if ((rawEyeGazes != null) && (rawEyeGazes.Count > 0))
            {
                Vector3 meanOrigin = Vector3.zero;
                for (int i = 0; i < rawEyeGazes.Count; i++)
                {
                    meanOrigin += rawEyeGazes[i].eyeGaze.origin;
                }
                meanOrigin /= rawEyeGazes.Count;
                return meanOrigin;
            }
            return Vector3.zero;
        }
    }
}