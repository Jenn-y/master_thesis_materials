using UnityEngine;
using TMPro;
using System.Collections;

public class NumberChanger : MonoBehaviour
{
    [Header("Math Display")]
    public TMP_Text number1Text;
    public TMP_Text operationText;
    public TMP_Text number2Text;
    public TMP_Text rightAnswersCounterText;
    public TMP_Text wrongAnswersCounterText;

    [Header("Answer Buttons")]
    public AnswerButton[] answerButtons;

    [Header("Timer Settings")]
    public TMP_Text timerText;
    public float baseTimePerQuestion = 10f;
    public float minTimePerQuestion = 4f;
    private float timeRemaining;
    private bool timerActive = true;

    [Header("Game Settings")]
    public int totalQuestions = 30;
    private int currentQuestion = 1;
    private int correctAnswer;
    private int correctAnswerIndex;

    private int number1;
    private int number2;
    private string currentOperation;
    private string[] currentStageOperations;
    private Coroutine gameLoopCoroutine;

    private int correctAnsNum = 0;
    [SerializeField] private GameObject XRRig;
     public AudioSource audioSource;
    public AudioClip wrongAnswerSound;


    void Start()
    {
        XRRig.transform.position = new Vector3(-3.8f, -5.5f, 8.3f);
        currentStageOperations = new[] { "+", "-" };
        timeRemaining = baseTimePerQuestion;
        GenerateNewProblem();
        UpdateQuestionCounter();
    }

    void Update()
    {
        if (currentQuestion > totalQuestions)
        {
            timerActive = false;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadNextScene();
            }

            return;
        }

        if (timerActive)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay();
            
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                timerActive = false;
                OnAnswerSelected(false); // Auto-fail if time runs out
            }
        }
    }

    void GenerateNewProblem()
    {
        float progress = currentQuestion / (float)totalQuestions;
        
        if (currentQuestion <= 5)
        {
            currentStageOperations = new[] { "+", "-" };
            currentOperation = currentStageOperations[Random.Range(0, currentStageOperations.Length)];
            GenerateNumbers(5, 25);
            timeRemaining = 5f;
        }
        else if (currentQuestion <= 10)
        {
            currentStageOperations = new[] { "+", "-" };
            currentOperation = currentStageOperations[Random.Range(0, currentStageOperations.Length)];
            GenerateNumbers(10, 50);
            timeRemaining = 3f;
        }
        else if (currentQuestion <= 15)
        {
            currentStageOperations = new[] { "×" };
            currentOperation = currentStageOperations[Random.Range(0, currentStageOperations.Length)];
            GenerateNumbers(3, 10);
            timeRemaining = 2f;
        }
        else if (currentQuestion <= 20)
        {
            currentStageOperations = new[] { "+", "-", "×" };
            currentOperation = currentStageOperations[Random.Range(0, currentStageOperations.Length)];
            if (currentOperation == "×")
            {
                GenerateNumbers(3, 12);
            }
            else
            {
                GenerateNumbers(15, 60);
            }
            timeRemaining = 4;
        }
        else if (currentQuestion <= 25)
        {
            currentStageOperations = new[] { "+", "-", "×" };
            currentOperation = currentStageOperations[Random.Range(0, currentStageOperations.Length)];
            if (currentOperation == "×")
            {
                GenerateNumbers(3, 15);
            }
            else
            {
                GenerateNumbers(10, 70);
            }
            timeRemaining = 3f;
        }
        else
        {
            currentStageOperations = new[] { "+", "-", "×", "÷" };
            currentOperation = currentStageOperations[Random.Range(0, currentStageOperations.Length)];
            if (currentOperation == "×" || currentOperation == "÷")
            {
                GenerateNumbers(3, 10);
            }
            else
            {
                GenerateNumbers(10, 100);
            }
            timeRemaining = 3f;
        }

        CalculateAnswer();
        SetupAnswerButtons();
        timerActive = true;
    }

    void GenerateNumbers(int min, int max)
    {    
        if (currentOperation == "÷")
        {
            number2 = Random.Range(1, 15);
            number1 = number2 * Random.Range(1, Mathf.FloorToInt(max / (float)number2));
        }
        else if (currentOperation == "-")
        {
            // Subtraction - ensure number1 > number2
            number2 = Random.Range(min, max - 1);
            number1 = Random.Range(number2 + 1, max + 1); // +1 to include max
        }
        else
        {
            number1 = Random.Range(min, max);
            number2 = Random.Range(min, max);
        }
        
        UpdateDisplay();
    }

    void CalculateAnswer()
    {
        correctAnswer = currentOperation switch
        {
            "+" => number1 + number2,
            "-" => number1 - number2,
            "×" => number1 * number2,
            "÷" => number1 / number2,
            _ => 0
        };
    }

    void UpdateDisplay()
    {
        number1Text.text = number1.ToString();
        operationText.text = currentOperation;
        number2Text.text = number2.ToString();
    }

    void UpdateTimerDisplay()
    {
        int displaySeconds = Mathf.CeilToInt(timeRemaining);
        displaySeconds = Mathf.Max(1, displaySeconds);
        timerText.text = $"{displaySeconds}s";
        timerText.color = displaySeconds <= 3 ? Color.red : Color.white;
    }

    void UpdateQuestionCounter()
    {
        wrongAnswersCounterText.text = $"\u00D7 {currentQuestion - 1 - correctAnsNum}";
        rightAnswersCounterText.text = $"\u2713 {correctAnsNum}";
    }

    void SetupAnswerButtons()
    {
        int[] wrongAnswers = GenerateWrongAnswers();
        correctAnswerIndex = Random.Range(0, 3);
        
        for (int i = 0, wrongIndex = 0; i < 3; i++) {
            if (i == correctAnswerIndex) {
                answerButtons[i].SetAnswer(correctAnswer, true);
            } else {
                answerButtons[i].SetAnswer(wrongAnswers[wrongIndex++], false);
            }
        }
    }

    int[] GenerateWrongAnswers()
    {
        int[] wrongAnswers = new int[2];
        
        // First wrong answer (always different from correct)
        wrongAnswers[0] = correctAnswer + Random.Range(1, 6);
        
        // Second wrong answer (keep generating until unique)
        do {
            wrongAnswers[1] = correctAnswer - Random.Range(1, 6);
        } while (wrongAnswers[1] == wrongAnswers[0]);
        
        return wrongAnswers;
    }
    
    public void OnAnswerSelected(bool isCorrect)
    {
        // Pause timer when answer is selected
        timerActive = false;
        SetAllButtonsInteractable(false);
        
        StartCoroutine(TransitionToNextQuestion(isCorrect));
        
        if (isCorrect)
        {
            correctAnsNum++;
            Debug.Log("Correct answer!");
            // Add any correct answer effects here
        }
        else
        {
            if (audioSource) audioSource.PlayOneShot(wrongAnswerSound);
            Debug.Log("Wrong answer!");
            // Add any wrong answer effects here
        }
    }

    IEnumerator TransitionToNextQuestion(bool wasCorrect)
    {
        // Wait for 1 second to show feedback
        yield return new WaitForSeconds(2f);
        
        currentQuestion++;
        if (currentQuestion <= totalQuestions)
        {
            GenerateNewProblem();
            SetAllButtonsInteractable(true);
            UpdateQuestionCounter();
        }
        else
        {
            Debug.Log("Challenge completed!");
        }
        
    }

    void SetAllButtonsInteractable(bool state)
    {
        foreach (AnswerButton button in answerButtons)
        {
            button.SetInteractable(state);
        }
    }
}