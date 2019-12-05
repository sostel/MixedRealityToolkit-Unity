using UnityEngine;

public class InputMemory_EyeGazeRaw : InputMemory
{
    public Ray eyeGaze;
    public RaycastHit hitInfo;
    public GameObject lookedAtTarget;
}