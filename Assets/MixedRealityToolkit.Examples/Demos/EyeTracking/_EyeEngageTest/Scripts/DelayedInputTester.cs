using UnityEngine;

public enum InputType
{
    NotSpecified,
    Manual,
    Voice
}

[RequireComponent(typeof(DelayedInputTrigger))]
public class DelayedInputTester : MonoBehaviour
{
    [SerializeField]
    private string friendlyName = "";

    [SerializeField]
    private InputType inputType = InputType.NotSpecified;
    
    [SerializeField]
    private float delayInSeconds = 0f;
       
    [SerializeField]
    private GameObject currentTargetIndicator = null;

    [SerializeField]
    private GameObject predictedTargetIndicator = null;

    private DelayedInputTrigger delayedTrigger;

    private void Start()
    {
        delayedTrigger = GetComponent<DelayedInputTrigger>();
        delayedTrigger.name = friendlyName;
        delayedTrigger.OnTrigger.AddListener(OnTriggered);
    }

    /// <summary>
    /// Trigger the input based on the given delay. This may also be called from other components in Unity.
    /// </summary>
    public void TriggerIt()
    {
        switch (inputType)
        {
            case InputType.Manual:
                delayedTrigger.TriggerDelayedInput(friendlyName, delayInSeconds); break;
            case InputType.Voice:
                delayedTrigger.TriggerVoiceInput(friendlyName); break;
        }
    }

    private void OnTriggered()
    {
        Debug.Log($"sd OnTriggered -> {friendlyName} [{inputType}]");
        predictedTargetIndicator.transform.position = delayedTrigger.PredictedTarget.transform.position;
        currentTargetIndicator.transform.position = delayedTrigger.CurrentTarget.transform.position;
    }
}
