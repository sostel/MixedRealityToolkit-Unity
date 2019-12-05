using System;
using UnityEngine;

public class InputMemory_EyeGazeProcessed : InputMemory
{
    public bool isFixation;
    public GameObject fixatedTarget;
    public DateTime tStartFixation;
    public DateTime tEndFixation;
}