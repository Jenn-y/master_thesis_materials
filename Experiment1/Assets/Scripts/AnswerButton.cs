using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerButton : MonoBehaviour
{
    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;

    [Header("Components")]
    public Image buttonImage;
    public TMP_Text answerText;

    private bool isCorrectAnswer;
    private NumberChanger numberChanger;

    void Start()
    {
        buttonImage.color = normalColor;
        numberChanger = FindObjectOfType<NumberChanger>();
        GetComponent<Button>().onClick.AddListener(OnButtonClicked);
    }

    public void SetAnswer(int answer, bool isCorrect)
    {
        answerText.text = answer.ToString();
        isCorrectAnswer = isCorrect;
        buttonImage.color = normalColor;
    }

    public void OnButtonClicked()
    {
        buttonImage.color = isCorrectAnswer ? correctColor : wrongColor;
        numberChanger.OnAnswerSelected(isCorrectAnswer);
        
    }

    public void SetInteractable(bool state)
    {
        GetComponent<Button>().interactable = state;
    }
}