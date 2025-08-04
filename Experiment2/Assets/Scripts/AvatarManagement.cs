using UnityEngine;
using System.Collections;

public class AvatarManagement : MonoBehaviour
{
    [SerializeField] private float avatarDuration = 5f;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(avatarDuration);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AvatarSceneCompleted();
        }
    }
}
