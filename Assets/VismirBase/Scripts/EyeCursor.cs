using UnityEngine;

public class EyeCursor : MonoBehaviour
{
    [SerializeField]
    EyeCursorType eyeCursorType = EyeCursorType.None;

    public EyeCursorType CursorType { get { return eyeCursorType; } }
}