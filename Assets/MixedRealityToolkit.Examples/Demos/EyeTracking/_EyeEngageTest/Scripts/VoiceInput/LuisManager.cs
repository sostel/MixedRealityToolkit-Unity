using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class LuisManager : MonoBehaviour
{
    [Serializable]
    public class LUISEvent : UnityEvent<ResponseFromLUIS> { }

    /// <summary>
    /// This class represents the LUIS response.
    /// </summary>
    [Serializable] 
    public class ResponseFromLUIS
    {
        public TopScoringIntentData topScoringIntent;
        public EntityData[] entities;
        public string query;
    }

    /// <summary>
    /// This class contains the *Intent* LUIS determines 
    /// to be the most likely
    /// </summary>
    [Serializable]
    public class TopScoringIntentData
    {
        public string intent;
        public float score;
    }

    /// <summary>
    /// This class contains data for an *Entity*.
    /// </summary>
    [Serializable]
    public class EntityData
    {
        public string entity;
        public string type;
        public int startIndex;
        public int endIndex;
        public float score;
    }

    #region Singleton
    private static LuisManager instance;
    public static LuisManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LuisManager>();
                if (instance == null)
                {
                    Debug.LogError("No LuisManager instance found! Please ensure to add one to your scene or remove the LuisManager script.");
                }
            }
            return instance;
        }
    }
    #endregion

    //Substitute the value of luis Endpoint with your own End Point
    [SerializeField]
    private string luisEndpoint = "Add your endpoint from the LUIS Portal: https://westus.api.cognitive...";

    public LUISEvent OnResponseReceived;

    /// <summary>
    /// Call LUIS to submit a dictation result.
    /// The done Action is called at the completion of the method.
    /// </summary>
    public IEnumerator SubmitRequestToLUIS(string dictationResult, Action done)
    {
        string queryString = string.Concat(Uri.EscapeDataString(dictationResult));

        using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(luisEndpoint + queryString))
        {
            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
            {
                Debug.Log(unityWebRequest.error);
            }
            else
            {
                try
                {
                    ResponseFromLUIS analyzedQuery = JsonUtility.FromJson<ResponseFromLUIS>(unityWebRequest.downloadHandler.text);

                    // Invoke an event to let subscribers know that a new query was received
                    OnResponseReceived.Invoke(analyzedQuery);
                }
                catch (Exception exception)
                {
                    Debug.Log("Luis Request Exception Message: " + exception.Message);
                }
            }

            done();
            yield return null;
        }
    }
}
