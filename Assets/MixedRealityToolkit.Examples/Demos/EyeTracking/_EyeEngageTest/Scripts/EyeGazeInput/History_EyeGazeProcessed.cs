using System;
using UnityEngine;

[RequireComponent(typeof(History_EyeGazeRaw))]
public class History_EyeGazeProcessed : HistoryBase
{
    [SerializeField]
    private float maxAngleWhileStill = 1f;

    [SerializeField]
    private float maxAngleWhileMoving = 3f;

    [SerializeField]
    private float minFixationDurationInMs = 150f;

    private History_EyeGazeRaw rawEyeGazeHistory = null;
    private InputMemory_EyeGazeRaw previousEyeGaze;
    private bool fixationStarted = false;
    private bool maybeInFixation = false;
    private DateTime dtFixationStarted = DateTime.MaxValue;
    
    InputMemory_EyeGazeProcessed currentMemory = null;

    #region Singleton
    private static History_EyeGazeProcessed instance;
    public static History_EyeGazeProcessed Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<History_EyeGazeProcessed>();
            }
            return instance;
        }
    }
    #endregion

    public new void Start()
    {
        base.Start();
        previousEyeGaze = null;
        rawEyeGazeHistory = this.GetComponent<History_EyeGazeRaw>();
        rawEyeGazeHistory.OnHistoryUpdated.AddListener(ProcessGazeData);
    }

    public void ProcessGazeData()
    {
        InputMemory_EyeGazeRaw rawEyeGaze = (InputMemory_EyeGazeRaw)(rawEyeGazeHistory.GetMostRecentMemory());

        if (previousEyeGaze != null)
        {
            // Compute angular distance of current eye gaze to the average fixation ray. If the angle is too large, 
            // the new eye gaze is not considered to be part of the fixation anymore. 
            float angle = 0;
            float maxAngle = 0;
            if ((currentMemory != null) && (currentMemory.FixationMeanDirection != Vector3.zero))
            {
                Debug.DrawRay(Camera.main.transform.position, rawEyeGaze.eyeGaze.direction, Color.green);
                Debug.DrawRay(Camera.main.transform.position, currentMemory.FixationMeanDirection, Color.red);
                angle = Vector3.Angle(rawEyeGaze.eyeGaze.direction, currentMemory.FixationMeanDirection);

                // Compare the angle of the current eye gaze origint + direction to the original ones
                for (int i = 0; i < currentMemory.rawEyeGazes.Count; i++)
                {
                    Vector3 lookatpoint = currentMemory.rawEyeGazes[i].eyeGaze.origin + currentMemory.rawEyeGazes[i].eyeGaze.direction.normalized;
                    Vector3 dirFromCurrOrigin = lookatpoint - rawEyeGaze.eyeGaze.origin;
                    float angle2 = Vector3.Angle(rawEyeGaze.eyeGaze.direction, dirFromCurrOrigin);
                    if (maxAngle < angle2)
                    {
                        maxAngle = angle2;
                    }
                }
            }
            else
            {
                angle = Vector3.Angle(rawEyeGaze.eyeGaze.direction, previousEyeGaze.eyeGaze.direction);
            }
                        
            // After computing the angle, let's compare it now:
            if ((angle > maxAngleWhileStill) || (maxAngle > maxAngleWhileMoving)) // If larger than the threshold, it's not part of the existing fixation
            {
                maybeInFixation = false;

                if (fixationStarted)
                {
                    Debug.Log("Saccade!");
                    fixationStarted = false;    
                    currentMemory = null;
                }
            }
            else // Otherwise it is part of the fixation and we need to update the data
            {
                if (!maybeInFixation)
                {
                    maybeInFixation = true;
                    dtFixationStarted = previousEyeGaze.timestamp;                    
                }
                else if((rawEyeGaze.timestamp - dtFixationStarted).TotalMilliseconds > minFixationDurationInMs)
                {
                    // Todo: If head is moving too much the eye gaze should not count as a fixation
                    // Option 1: We already created a fixation holder and need to update the values
                    if ((fixationStarted) && (currentMemory != null)) 
                    {
                        currentMemory.Update(rawEyeGaze, MicrophoneManager.Instance.dictationStatus);
                    }
                    else
                    {
                        fixationStarted = true;
                        currentMemory = new InputMemory_EyeGazeProcessed(dtFixationStarted);
                        // Todo: Actually we need to add not only the previous but all of the gaze points that were within the proximity
                        currentMemory.Update(previousEyeGaze, MicrophoneManager.Instance.dictationStatus);
                        currentMemory.Update(rawEyeGaze, MicrophoneManager.Instance.dictationStatus);
                        Remember(DateTime.UtcNow, currentMemory);
                    }
                }
            }
        }
        previousEyeGaze = rawEyeGaze;
        
        //Debug.Log($"New eye gaze data to process: {((InputMemory_EyeGazeProcessed)GetMostRecentMemory()).timestamp} \t--> {((InputMemory_EyeGazeProcessed)GetMostRecentMemory()).lookedAtTarget} ");
        // TODO: Handle new eye gaze data -> Saccades, Fixations... If uncertain, mark as "PotentialFixation" and update later.
    }

    public InputMemory_EyeGazeProcessed GetFixationAt(DateTime date)
    {
        InputMemory_EyeGazeProcessed memory = (InputMemory_EyeGazeProcessed)GetMostRecentMemory();
        if (memory != null)
        {
            if (memory.tEndFixation < date)
            {
                // return current
            }
            else
            {
                return memory;
            }
        }
        return null;
    }

    public override void Remember()
    {

    }
}