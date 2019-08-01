using UnityEngine;

public class EyeCursorManager : MonoBehaviour
{
    [SerializeField]
    private EyeCursor[] CursorVariants = new EyeCursor[] { };

    public void SetCursor(string cursorType)
    {
        if (CursorVariants != null)
        {
            foreach (EyeCursor cursor in CursorVariants)
            {
                if (cursor.CursorType.ToString() == cursorType)
                {
                    cursor.gameObject.SetActive(true);
                }
                else
                {
                    cursor.gameObject.SetActive(false);
                }
            }
        }
    }
}
