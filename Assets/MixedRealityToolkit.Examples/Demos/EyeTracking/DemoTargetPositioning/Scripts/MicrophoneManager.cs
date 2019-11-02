using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class MicrophoneManager : MonoBehaviour
{
    public TextMesh dictationText; // Text game object to provide feedback on the dictation result

    private DictationRecognizer dictationRecognizer;  // Component converting speech to text

    #region Singleton
    private static MicrophoneManager instance;
    public static MicrophoneManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MicrophoneManager>();
            }
            return instance;
        }
    }
    #endregion


    bool isSupposedToCapture = false;
    void Start()
    {
        TryStartCapturingAudio();
    }

    private void Update()
    {
        if (isSupposedToCapture && (dictationRecognizer.Status != SpeechSystemStatus.Running))
        {
            Debug.Log("Hm... something WEIRD happened! " + dictationRecognizer.Status);
            TryStartCapturingAudio();
        }
    }


    IEnumerator JustATest()
    {
        if (isSupposedToCapture && (dictationRecognizer.Status != SpeechSystemStatus.Running))
        {
            Debug.Log("Hm... something WEIRD happened! " + dictationRecognizer.Status);
            TryStartCapturingAudio();
        }
       // Debug.Log("justatest " + dictationRecognizer.Status );
        yield return new WaitForSeconds(1);
    }

    private void OnDestroy()
    {
        StopCapturingAudio();
    }

    /// <summary>
    /// Start microphone capture by providing the microphone as a continual audio source (looping),
    /// then initialize the DictationRecognizer, which will capture spoken words.
    /// </summary>
    public void TryStartCapturingAudio()
    {
        if((dictationRecognizer == null) || (dictationRecognizer.Status != SpeechSystemStatus.Running))
        {
            if (Microphone.devices.Length > 0)
            {
                // Debug.Log("Mic Detected! #"+Microphone.devices.Length + "; " + Microphone.devices[0]);

                if (dictationRecognizer == null)
                {
                    dictationRecognizer = new DictationRecognizer
                    {
                        InitialSilenceTimeoutSeconds = 60,
                        AutoSilenceTimeoutSeconds = 5
                    };

                    dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;
                    dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
                    dictationRecognizer.DictationError += DictationRecognizer_DictationError;
                }

                StartCoroutine(TryStartDictation());
            }
        }
    }


    IEnumerator TryStartDictation()
    {
        if (PhraseRecognitionSystem.Status == SpeechSystemStatus.Running)
        {
            Debug.Log($">> PhraseRecognitionSystem.Status: {PhraseRecognitionSystem.Status}!!!");
            PhraseRecognitionSystem.Shutdown();
            yield return null;
        }

        dictationRecognizer.Start();
        isSupposedToCapture = true;
        dictationText.text = "...";
        yield return null;
    }

    private void DictationRecognizer_DictationHypothesis(string dictationCaptured)
    {
        string msg = ">> [DictationHypothesis]: " + dictationCaptured;
        Debug.Log(msg);
        dictationText.text = msg;
    }

    /// <summary>
    /// Stop microphone capture
    /// </summary>
    public void StopCapturingAudio()
    {
        if (dictationRecognizer != null)
        {
            dictationRecognizer.Stop();
            isSupposedToCapture = false;
            Debug.Log("Stop capturing audio...");
        }
        else
        {
            Debug.Log("No dictation recognizer could be found.");
        }
    }

    /// <summary>
    /// This handler is called every time the Dictation detects a pause in the speech. 
    /// This method will stop listening for audio, send a request to the LUIS service 
    /// and then start listening again.
    /// </summary>
    private void DictationRecognizer_DictationResult(string dictationCaptured, ConfidenceLevel confidence)
    {
        StopCapturingAudio();
        StartCoroutine(LuisManager.Instance.SubmitRequestToLUIS(dictationCaptured, TryStartCapturingAudio));

        string msg = ">> [DictationResult]: " + dictationCaptured;
        Debug.Log(msg);
        dictationText.text = msg;
    }

    private void DictationRecognizer_DictationError(string error, int hresult)
    {
        Debug.Log("Dictation exception: " + error);
    }
}
