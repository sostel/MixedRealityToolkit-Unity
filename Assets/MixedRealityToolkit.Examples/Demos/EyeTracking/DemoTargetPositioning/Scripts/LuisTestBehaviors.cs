using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class LuisTestBehaviors : MonoBehaviour
{
    #region Singleton
    private static LuisTestBehaviors instance;
    public static LuisTestBehaviors Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LuisTestBehaviors>();
                if (instance == null)
                {
                    Debug.LogError("No 'Behaviors' instance found! Please ensure to add one to your scene or remove the LuisManager script.");
                }
            }
            return instance;
        }
    }
    #endregion
    
    // the following variables are references to possible targets
    public GameObject sphere;
    public GameObject cylinder;
    public GameObject cube;

    private IMixedRealityInputSystem inputSystem = null;

    /// <summary>
    /// The active instance of the input system.
    /// </summary>
    private IMixedRealityInputSystem InputSystem
    {
        get
        {
            if (inputSystem == null)
            {
                MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
            }
            return inputSystem;
        }
    }

    /// <summary>
    /// Changes the color of the target GameObject by providing the name of the object
    /// and the name of the color
    /// </summary>
    public void ChangeTargetColor(string targetName, string colorName)
    {
        Debug.Log($">> ChangeTargetColor -- [{targetName}]; [{colorName}]");
        GameObject foundTarget = FindTarget(targetName);
        if (foundTarget != null)
        {
            Debug.Log("Changing color " + colorName + " to target: " + foundTarget.name);

            switch (colorName)
            {
                case "blue":
                    foundTarget.GetComponent<Renderer>().material.color = Color.blue;
                    break;

                case "red":
                    foundTarget.GetComponent<Renderer>().material.color = Color.red;
                    break;

                case "yellow":
                    foundTarget.GetComponent<Renderer>().material.color = Color.yellow;
                    break;

                case "green":
                    foundTarget.GetComponent<Renderer>().material.color = Color.green;
                    break;

                case "white":
                    foundTarget.GetComponent<Renderer>().material.color = Color.white;
                    break;

                case "black":
                    foundTarget.GetComponent<Renderer>().material.color = Color.black;
                    break;
                default:
                    foundTarget.GetComponent<Renderer>().material.color = Color.cyan;
                    Debug.Log("Sorry, we don't know that color: "+colorName);
                    break;
            }
        }
    }

    /// <summary>
    /// Reduces the size of the target GameObject by providing its name
    /// </summary>
    public void DownSizeTarget(string targetName)
    {
        GameObject foundTarget = FindTarget(targetName);
        foundTarget.transform.localScale -= new Vector3(0.5F, 0.5F, 0.5F);
    }

    /// <summary>
    /// Increases the size of the target GameObject by providing its name
    /// </summary>
    public void UpSizeTarget(string targetName)
    {
        GameObject foundTarget = FindTarget(targetName);
        foundTarget.transform.localScale += new Vector3(0.5F, 0.5F, 0.5F);
    }

    /// <summary>
    /// Determines which obejct reference is the target GameObject by providing its name
    /// </summary>
    private GameObject FindTarget(string name)
    {
        GameObject targetAsGO = null;

        switch (name)
        {
            case "sphere":
                targetAsGO = sphere;
                break;

            case "cylinder":
                targetAsGO = cylinder;
                break;

            case "cube":
                targetAsGO = cube;
                break;

            case "this": // as an example of target words that the user may use when looking at an object
            case "it":  // as this is the default, these are not actually needed in this example
            case "that":
            default: // if the target name is none of those above, check if the user is looking at something
                targetAsGO = (GazedTarget != null) ? GazedTarget : null;
                break;
        }
        return targetAsGO;
    }

    private GameObject GazedTarget
    {
        get
        {
            if ((InputSystem != null) && (InputSystem.EyeGazeProvider != null))
            {
                return inputSystem.EyeGazeProvider.HitInfo.target;
            }

            return null;
        }
    }
}
