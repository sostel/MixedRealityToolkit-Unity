using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    public class KeywordHandler : MonoBehaviour, IMixedRealitySpeechHandler
    {
        private void Start()
        {

        }

        void IMixedRealitySpeechHandler.OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            Debug.Log($"Now: {DateTime.UtcNow} ");
            Debug.Log($"Start: {eventData.PhraseStartTime} ");
            Debug.Log($"Keyword recognized: {eventData.Command.Keyword} [StartTime: {eventData.PhraseStartTime.ToShortTimeString()}; \t duration: {(DateTime.UtcNow - eventData.PhraseStartTime).TotalSeconds} [{eventData.PhraseDuration.TotalSeconds}]]");
          //  if (eventData.Command.Keyword == "smaller")
            
        }

        private void OnEnable()
        {
            // Instruct Input System that we would like to receive all input events of type 
            CoreServices.InputSystem?.RegisterHandler<IMixedRealitySpeechHandler>(this);
        }

        private void OnDisable()
        {
            // This component is being destroyed
            // Instruct the Input System to disregard us for input event handling
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealitySpeechHandler>(this);            
        }
    }
}