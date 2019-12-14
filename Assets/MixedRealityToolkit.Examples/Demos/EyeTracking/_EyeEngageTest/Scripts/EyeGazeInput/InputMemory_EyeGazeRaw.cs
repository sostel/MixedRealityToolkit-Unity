using Microsoft.MixedReality.Toolkit.Examples;
using UnityEngine;

public class InputMemory_EyeGazeRaw : InputMemory
{
    public Ray eyeGaze { get; private set; }
    public GameObject lookedAtTarget { get; private set; }
    public Vector3 lookedAtPoint { get; private set; }
    public VoiceDictationStatus voiceInputStatus { get; set; }

    public InputMemory_EyeGazeRaw(Ray eyeGazeRay) 
    {
        new InputMemory_EyeGazeRaw(eyeGazeRay, null, Vector3.zero, VoiceDictationStatus.Unknown);
    }

    public InputMemory_EyeGazeRaw(Ray eyeGazeRay, GameObject lookedAtGameObj, Vector3 lookedAtPos, VoiceDictationStatus voiceStatus)
    {
        eyeGaze = eyeGazeRay;
        lookedAtTarget = lookedAtGameObj;
        lookedAtPoint = lookedAtPos;
        voiceInputStatus = voiceStatus;
    }
}