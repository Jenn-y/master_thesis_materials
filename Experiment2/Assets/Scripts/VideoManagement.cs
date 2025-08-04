using UnityEngine;
using UnityEngine.Video;

public class VideoManagement : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayVideo(string videoName)
    {
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoName);
        videoPlayer.url = videoPath;
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.Prepare();
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        vp.Play();
    }
}
