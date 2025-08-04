using UnityEngine;
using System.Collections;

public class EndingScript : MonoBehaviour
{
    [SerializeField] private GameObject XRRig;

    void Start()
    {
        XRRig.transform.position = new Vector3(-5.609f, 1.65f, 5.3f);
        StartCoroutine(ExitAfterDelay());
    }

    IEnumerator ExitAfterDelay()
    {
        yield return new WaitForSeconds(4f);
        
        // Close the application
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}