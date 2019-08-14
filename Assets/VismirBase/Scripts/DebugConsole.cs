using TMPro;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI statusTextMesh = new TextMeshProUGUI();

    private static DebugConsole singletonConsole;
    public static DebugConsole Console
    {
        get
        {
            if (singletonConsole == null)
            {
                singletonConsole = FindObjectOfType<DebugConsole>();
            }
            return singletonConsole;
        }
    }


    private void Start()
    {
        if (singletonConsole == null)
        {
            singletonConsole = this;
        }
    }

    public void Text(string message)
    {
        if (statusTextMesh != null)
        {
            statusTextMesh.text = message;
        }
    }
}
