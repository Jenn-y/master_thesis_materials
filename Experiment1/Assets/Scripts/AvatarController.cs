using UnityEngine;
using System.Collections;

public class AvatarManagement : MonoBehaviour
{
    [Header("Avatar References")]
    [SerializeField] private GameObject neutralAvatar;
    [SerializeField] private GameObject XRRig;
    [SerializeField] private GameObject happyAvatar;
    [SerializeField] private float avatarDuration = 5f;

    private IEnumerator Start()
    {
        // Default both to inactive
        SetBothInactive();
        XRRig.transform.position = new Vector3(-1.13f, 2.55f, -1.9f);
    
        // Activate the selected one
        if (GameManager.CurrentAvatarType == GameManager.AvatarType.Happy)
        {
            happyAvatar.SetActive(true);
        }
        else
        {
            neutralAvatar.SetActive(true);
        }

        yield return new WaitForSeconds(avatarDuration);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadNextScene();
        }
    }

    private void SetBothInactive()
    {
        if (neutralAvatar != null) neutralAvatar.SetActive(false);
        if (happyAvatar != null) happyAvatar.SetActive(false);
    }
}
