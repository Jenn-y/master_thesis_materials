using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO; 
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Scene names
    private const string BASELINE_SCENE = "BaselineScene";
    private readonly string[] ANXIETY_SCENES = { "AnxietyScene1", "AnxietyScene2", "AnxietyScene3" };
    private readonly string[] AVATAR_SCENES = { "AvatarSceneAnime", "AvatarSceneHuman", "AvatarSceneMix" };
    private readonly float[] VIDEO_DURATIONS = { 207f, 220f, 200f };

    // Trial tracking
    private List<int> remainingAnxietyIndices = new List<int> { 0, 1, 2 };
    private List<int> remainingAvatarIndices = new List<int> { 0, 1, 2 };
    private int currentTrial = 0;
    private const int TOTAL_TRIALS = 3;

    private string csvFilePath;
    private DateTime sceneStartTime;
    public static string currentSceneName;
    public static string participantID = "223322";

    private void Awake()
    {
        // Singleton check
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject); // destroy duplicates
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        csvFilePath = Path.Combine(Application.persistentDataPath, "ExperimentLog.csv");
        if (!File.Exists(csvFilePath))
        {
            File.WriteAllText(csvFilePath, "ParticipantID,TimestampStart,TimestampEnd,SceneType,SceneDuration(seconds),TrialNumber\n");
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        StartNewTrial();
    }

    private void StartNewTrial()
    {
        if (currentTrial >= TOTAL_TRIALS)
        {
            Debug.Log("Experiment complete!");
            SceneManager.LoadScene("EndingScene");
            return;
        }

        currentTrial++;
        Debug.Log($"Starting Trial {currentTrial}");
        currentSceneName = "BaselineScene";
        SceneManager.LoadScene(BASELINE_SCENE);
    }

    private void LoadRandomAnxietyScene()
    {
        if (remainingAnxietyIndices.Count == 0)
        {
            Debug.LogError("No more anxiety scenes available!");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, remainingAnxietyIndices.Count);
        int sceneIndex = remainingAnxietyIndices[randomIndex];
        remainingAnxietyIndices.RemoveAt(randomIndex);
        if (sceneIndex == 0) currentSceneName = "AnxietySceneElevator";
        else if (sceneIndex == 1) currentSceneName = "AnxietySceneRoom";
        else currentSceneName = "AnxietyScenePayphone";

        LoadScene(ANXIETY_SCENES[sceneIndex]);
        SetupVideoDetection(VIDEO_DURATIONS[sceneIndex]);
    }

    private void SetupVideoDetection(float seconds)
    {
        Invoke("LoadRandomAvatarScene", seconds);
    }

    private void LoadRandomAvatarScene()
    {
        RecordSceneEnd();
        if (remainingAvatarIndices.Count == 0)
        {
            Debug.LogError("No more avatar scenes available!");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, remainingAvatarIndices.Count);
        int sceneIndex = remainingAvatarIndices[randomIndex];
        remainingAvatarIndices.RemoveAt(randomIndex);
        currentSceneName = AVATAR_SCENES[sceneIndex];

        LoadScene(AVATAR_SCENES[sceneIndex]);
    }

    public void AvatarSceneCompleted()
    {
        RecordSceneEnd();
        Debug.Log($"Trial {currentTrial} completed");
        StartNewTrial();
    }

    public void BaselineSceneCompleted()
    {
        RecordSceneEnd();
        Debug.Log($"Trial {currentTrial} completed");
        LoadRandomAnxietyScene();
    }

    private void LoadScene(string sceneName)
    {
        Debug.Log($"Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
        StartRecording();
    }
    
    public void StartRecording()
    {
        sceneStartTime = DateTime.Now;
        PupilLogger.Instance.StartTracking();
    }

    private void RecordSceneEnd()
    {
        PupilLogger.Instance.StopTracking();
        TimeSpan duration = DateTime.Now - sceneStartTime;
        
        string logLine = string.Format("{0},{1},{2:F2},{3},{4},{5}\n",
            participantID,
            sceneStartTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            currentSceneName,
            duration.TotalSeconds,
            currentTrial + 1);

        File.AppendAllText(csvFilePath, logLine);
    }
}