using UnityEngine;

public class AnxietyManager : MonoBehaviour
{
    public string videoName;
    public VideoManagement videoManagement;

    void Start()
    {
        videoManagement.PlayVideo(videoName);
    }
}
