using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO; 
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum SceneType
    {
        BaselineScene,
        NeutralScene,
        AnxietyScene,
        AvatarScene,
        EndingScene
    }

    public enum AvatarType
    {
        Neutral,
        Happy
    }

    [System.Serializable]
    public struct Trial
    {
        public SceneType stimulus;
        public AvatarType avatar;

        public Trial(SceneType stim, AvatarType av)
        {
            stimulus = stim;
            avatar = av;
        }
    }

    public static AvatarType CurrentAvatarType; // This gets read by the AvatarScene
    private int trialIndex = 0;        // Which trial we're on (0 to 3)
    private int phaseInTrial = 0;      // 0 = Baseline, 1 = Stimulus, 2 = Avatar

    private List<Trial> trials = new List<Trial>();

    private string csvFilePath;
    private DateTime sceneStartTime;
    public static string currentSceneName;
    public static string participantID;

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
            File.WriteAllText(csvFilePath, "ParticipantID,TimestampStart,TimestampEnd,SceneType,SceneDuration(seconds),TrialNumber,Phase\n");
        }
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        PrepareTrials();
        LoadNextScene(); // Start experiment
    }

    void PrepareTrials()
    {
        trials.Add(new Trial(SceneType.NeutralScene, AvatarType.Happy));
        trials.Add(new Trial(SceneType.NeutralScene, AvatarType.Neutral));
        trials.Add(new Trial(SceneType.AnxietyScene, AvatarType.Neutral));
        trials.Add(new Trial(SceneType.AnxietyScene, AvatarType.Happy));

        Shuffle(trials);
    }

    public void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(currentSceneName))
        {
            RecordSceneEnd();
        }

        if (trialIndex >= trials.Count)
        {
            // All trials finished → load final baseline + ending
            SceneManager.LoadScene(SceneType.EndingScene.ToString());
            return;
        }

        Trial current = trials[trialIndex];

        switch (phaseInTrial)
        {
            case 0: // Baseline
                currentSceneName = SceneType.BaselineScene.ToString();
                Debug.Log($"[Trial {trialIndex + 1}] Phase: Baseline");
                SceneManager.LoadScene(SceneType.BaselineScene.ToString());
                phaseInTrial = 1;
                break;

            case 1: // Stimulus
                currentSceneName = current.stimulus.ToString();
                Debug.Log($"[Trial {trialIndex + 1}] Phase: Stimulus - {current.stimulus}");
                SceneManager.LoadScene(current.stimulus.ToString());
                phaseInTrial = 2;
                break;

            case 2: // Avatar
                currentSceneName = SceneType.AvatarScene.ToString() + current.avatar.ToString();
                Debug.Log($"[Trial {trialIndex + 1}] Phase: Avatar - {current.avatar}");
                CurrentAvatarType = current.avatar;
                SceneManager.LoadScene(SceneType.AvatarScene.ToString());
                phaseInTrial = 0;
                trialIndex++;
                StartRecording();
                break;
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
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
        string avatarType = (currentSceneName == "AvatarScene") ? CurrentAvatarType.ToString() : "N/A";
        
        string logLine = string.Format("{0},{1},{2:F2},{3},{4},{5}\n",
            participantID,
            sceneStartTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            currentSceneName,
            duration.TotalSeconds,
            trialIndex + 1,
            phaseInTrial);

        File.AppendAllText(csvFilePath, logLine);
    }
}