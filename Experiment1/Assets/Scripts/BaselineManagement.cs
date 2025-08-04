using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class BaselineSceneLogic : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private float sceneDuration = 90f;
    [SerializeField] private float messageDuration = 5f;
    [SerializeField] private TMP_Text centerMessageText;
    [SerializeField] private TMP_Text endingMessageText;
    [SerializeField] private GameObject XRRig;
    
    private bool timerStarted = false;

    void Start(){
        XRRig.transform.position = new Vector3(-5.609f, 1.55f, 5.3f);
    }

    public void OnStartButtonClicked()
    {
        if (!timerStarted)
        {
            StartCoroutine(SceneTimer());
            timerStarted = true;
            GameManager.Instance.StartRecording();
        }
    }

    private IEnumerator SceneTimer()
    {
        yield return new WaitForSeconds(sceneDuration);
        
        if (endingMessageText != null && centerMessageText != null)
        {
            centerMessageText.gameObject.SetActive(false);
            endingMessageText.gameObject.SetActive(true);
        }
        
        yield return new WaitForSeconds(messageDuration);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadNextScene();
        }
    }
}