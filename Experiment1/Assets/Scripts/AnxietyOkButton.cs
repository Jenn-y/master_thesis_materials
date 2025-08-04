using UnityEngine;

public class AnxietyOkButton : MonoBehaviour
{
    public void OnOkButtonClicked()
    {
        GameManager.Instance.StartRecording();
    }
}
