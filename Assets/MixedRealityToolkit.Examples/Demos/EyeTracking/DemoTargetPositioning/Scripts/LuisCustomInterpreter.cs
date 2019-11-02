using System;
using System.Collections.Generic;
using UnityEngine;
using static LuisManager;

public class LuisCustomInterpreter : MonoBehaviour
{
    private void Start()
    {
        if (LuisManager.Instance != null)
        {
            // Add a listener when we receive a response from the LUIS service
            LuisManager.Instance.OnResponseReceived.AddListener(ProcessLUISResponse);
        }
    }

    public void ProcessLUISResponse(ResponseFromLUIS aQuery)
    {
        Debug.Log(">> ProcessLUISResponse triggered!");

        Debug.Log($">> -- Intent: {aQuery.topScoringIntent.intent}");
        Debug.Log($">> -- # of entities: {aQuery.entities.Length}");
        

        string topIntent = aQuery.topScoringIntent.intent;

        // Create a dictionary of entities associated with their type
        Dictionary<string, string> entityDic = new Dictionary<string, string>();

        int i = 1;
        foreach (EntityData ed in aQuery.entities)
        {
            Debug.Log($">> -- #{i} Entity: {ed.type} and {ed.entity}");
            entityDic.Add(ed.type, ed.entity);
            i++;
        }

        // Depending on the topmost recognised intent, read the entities name
        switch (aQuery.topScoringIntent.intent)
        {
            case "ChangeTargetColor":
                string targetForColor = null;
                string color = null;

                foreach (var pair in entityDic)
                {
                    if (pair.Key == "target")
                    {
                        targetForColor = pair.Value;
                    }
                    else if (pair.Key == "color")
                    {
                        color = pair.Value;
                    }
                }

                LuisTestBehaviors.Instance.ChangeTargetColor(targetForColor, color);
                break;

            case "ChangeTargetSize":
                string targetForSize = null;
                foreach (var pair in entityDic)
                {
                    if (pair.Key == "target")
                    {
                        targetForSize = pair.Value;
                    }
                }

                if (entityDic.ContainsKey("upsize") == true)
                {
                    LuisTestBehaviors.Instance.UpSizeTarget(targetForSize);
                }
                else if (entityDic.ContainsKey("downsize") == true)
                {
                    LuisTestBehaviors.Instance.DownSizeTarget(targetForSize);
                }
                break;
        }
    }
}
