using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    public enum VoiceDictationStatus
    {
        Unknown,
        Dictation_HypothesisStart,
        Dictation_HypothesisCont,
        Dictation_Result,
        Dictation_Result_High,
        Dictation_Result_Medium,
        Dictation_Result_Low,
        Dictation_Result_Rejected,
        LUIS_IntentRecognized,
        LUIS_QueryReceived
    }

    public class VoiceHistoryEntry
    {
        public DateTime timestamp;
        public string dictationText = "";
        public VoiceDictationStatus dictationStatus = VoiceDictationStatus.Unknown;

        public VoiceHistoryEntry()
        {
            timestamp = DateTime.MinValue;
            dictationText = " ";
            dictationStatus = VoiceDictationStatus.Unknown;
        }
    }

    public class VoiceHistory : MonoBehaviour
    {

        #region Singleton
        private static VoiceHistory instance;
        public static VoiceHistory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<VoiceHistory>();
                }
                return instance;
            }
        }
        #endregion

        private Dictionary<DateTime, VoiceHistoryEntry> history;

        [SerializeField]
        private float maxMemoryInSeconds = 30f;

        public UnityEvent OnNewDataReceived;

        private DateTime? hypothesisStart;
        private DateTime? hypothesisStop;
        private DateTime? resultRecognized;

        public void Start()
        {
            ResetHistory();
        }

        public void Update()
        {
            ForgetOverTime();
        }

        public void ResetHistory()
        {
            history = new Dictionary<DateTime, VoiceHistoryEntry>();
        }

        public void Remember(DateTime date, string text, VoiceDictationStatus status)
        {
            int result = history.Keys.Where(x => x > date).Count();

            if (result == 0)
            {
                VoiceHistoryEntry memory = new VoiceHistoryEntry();
                memory.timestamp = date;
                memory.dictationText = text;
                memory.dictationStatus = status;

                string sdate = date.ToString("HH:mm.ss:FFF");
                Debug.Log($"VoiceHist -> Add: [{sdate}] - {text}");
                history.Add(date, memory);

                OnNewDataReceived.Invoke();

                if (status == VoiceDictationStatus.Dictation_HypothesisStart)
                {
                    Debug.Log($"VoiceHist -> Add: [{sdate}] - HypothesisStart!! ***************** ");
                    hypothesisStart = memory.timestamp;
                }
                else if (status == VoiceDictationStatus.Dictation_HypothesisCont)
                {
                    hypothesisStop = memory.timestamp;
                }
            }
        }

        private void ForgetOverTime()
        {
            DateTime date = DateTime.UtcNow.AddSeconds(-maxMemoryInSeconds);
            List<DateTime> results = history.Keys.Where(x => x <= date).ToList();

            for (int i = 0; i < results.Count; i++)
            {
                history.Remove(results[i]);
            }
        }

        public VoiceHistoryEntry GetMostRecentEntry()
        {
            return GetEntryAt(DateTime.UtcNow);
        }

        public VoiceHistoryEntry GetEntryAt(DateTime date)
        {
            VoiceHistoryEntry lastEntry = new VoiceHistoryEntry();
            if ((history != null) && (history.Keys.Count > 0))
            {
                DateTime result = history.Keys.Where(x => x <= date).Max(); // Warning: Not the most efficient!?
                bool check = history.TryGetValue(result, out lastEntry);
            }
            return lastEntry;
        }
        
        public DateTime? HypothesisStart()
        {
            return hypothesisStart;
        }

        public DateTime? HypothesisStop()
        {
            return hypothesisStop;
        }
    }
}