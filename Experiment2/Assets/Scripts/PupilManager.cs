using UnityEngine;
using System.IO;
using VIVE.OpenXR;
using VIVE.OpenXR.EyeTracker;

public class PupilLogger : MonoBehaviour
{
    public static PupilLogger Instance { get; private set; }

    private StreamWriter _writer;
    private string _filePath;
    private float _lastLogTime;
    private const float LOG_INTERVAL = 0.1f; // 10Hz sampling
    private bool _isTracking = false;
    private bool _fileInitialized = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void StartTracking()
    {
        if (!_fileInitialized)
        {
            InitializeLogFile();
            _fileInitialized = true;
        }
        _isTracking = true;
    }

    public void StopTracking()
    {
        _isTracking = false;
    }

    private void InitializeLogFile()
    {
        _filePath = Path.Combine(
            Application.persistentDataPath,
            $"PupilData_{GameManager.participantID}.csv"
        );

        _writer = new StreamWriter(_filePath, false);
        _writer.WriteLine("Timestamp,Scene,LeftPupil(mm),RightPupil(mm),LeftValid,RightValid");
        _writer.Flush();
        Debug.Log($"Initialized pupil logging to {_filePath}");
    }

    void Update()
    {
        if (!_isTracking || _writer == null) return;
        if (Time.time - _lastLogTime < LOG_INTERVAL) return;

        if (XR_HTC_eye_tracker.Interop.GetEyePupilData(out XrSingleEyePupilDataHTC[] pupilData))
        {
            var left = pupilData[(int)XrEyePositionHTC.XR_EYE_POSITION_LEFT_HTC];
            var right = pupilData[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];

            bool IsPhysiologicalPupilSize(float diameter) => diameter >= 2.0f && diameter <= 7.0f;

            bool leftValid = left.isDiameterValid && IsPhysiologicalPupilSize(left.pupilDiameter);
            bool rightValid = right.isDiameterValid && IsPhysiologicalPupilSize(right.pupilDiameter);

            _writer.WriteLine($"{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}," +
                            $"{GameManager.currentSceneName}," +
                            $"{(leftValid ? left.pupilDiameter.ToString("F2") : "NaN")}," +
                            $"{(rightValid ? right.pupilDiameter.ToString("F2") : "NaN")}," +
                            $"{leftValid}," +
                            $"{rightValid}");

            _writer.Flush();
            _lastLogTime = Time.time;
        }
    }

    void OnApplicationQuit()
    {
        if (_writer != null)
        {
            _writer.Close();
            Debug.Log($"Pupil data saved to: {_filePath}");
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            OnApplicationQuit();
        }
    }
}