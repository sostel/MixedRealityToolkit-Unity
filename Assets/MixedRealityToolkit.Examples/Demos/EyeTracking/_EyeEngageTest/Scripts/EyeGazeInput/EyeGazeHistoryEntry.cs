using System;
using UnityEngine;


public class EyeGazeHistoryEntry
{
    public DateTime timestamp;
    public Ray eyeGaze;
    public RaycastHit hitInfo;
    public GameObject lookedAtTarget;
}