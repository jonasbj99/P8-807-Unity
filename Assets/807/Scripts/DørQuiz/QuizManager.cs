using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Added for Button
using System.Collections; // Added for IEnumerator

public class QuizManager : MonoBehaviour
{
    [Header("Quiz Content")]
    public List<QuestionsAndAnswers> QnA;
    public GameObject[] options;
    public TextMeshProUGUI QuestionTxt;

    [Header("UI Panels")]
    public GameObject QuizPanel;
    public GameObject EndScreen;

    [Header("Score Display")]
    // public TextMeshProUGUI ScoreText;

    private int currentQuestion;
    private int score = 0;

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;
    public float feedbackDisplayTime = 3f; // How long to show the feedback
    private bool isShowingFeedback = false;

    private List<QuestionsAndAnswers> originalQuestions = new List<QuestionsAndAnswers>();

    private void Awake()
    {
        // Store the original questions when the component initializes
        if (originalQuestions.Count == 0 && QnA.Count > 0)
        {
            foreach (QuestionsAndAnswers qa in QnA)
            {
                originalQuestions.Add(qa);
            }
        }
    }

    private void Start()
    {
        // Ensure end screen is hidden and quiz panel is shown
        EndScreen.SetActive(false);
        QuizPanel.SetActive(true);

        if (QnA.Count > 0)
        {
            generateQuestion();
        }
        else
        {
            Debug.LogWarning("QnA list is empty. Please add questions.");
        }
    }

    public void AnswerSelected(bool wasCorrect)
    {
        if (wasCorrect)
        {

            // Hide any feedback that might be showing
            // if (feedbackText != null)
                // feedbackText.gameObject.SetActive(false);
            score++;

            // Only remove and proceed to next question if answer was correct
            QnA.RemoveAt(currentQuestion);

            if (QnA.Count > 0)
            {
                generateQuestion();
            }
            else
            {
                EndQuiz();
            }
        }
        else
        {
            // For wrong answers, show feedback specific to this question
            Debug.Log("Wrong answer! Try again.");

            if (feedbackText != null && !isShowingFeedback)
            {
                // Use the custom feedback message for this question
                feedbackText.text = QnA[currentQuestion].FeedbackMessage;
                feedbackText.gameObject.SetActive(true);
                isShowingFeedback = true;

                // Hide the feedback after a delay
                StartCoroutine(HideFeedbackAfterDelay());
            }

            // Reset the answer buttons
            SetAnswers();
        }
    }

    private System.Collections.IEnumerator HideFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(feedbackDisplayTime);
        //feedbackText.gameObject.SetActive(false);
        QuestionTxt.text = QnA[currentQuestion].Question;
        isShowingFeedback = false;
    }

    void generateQuestion()
    {
        currentQuestion = Random.Range(0, QnA.Count);
        QuestionTxt.text = QnA[currentQuestion].Question;
        SetAnswers();
    }

    void SetAnswers()
    {
        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<AnswerScript>().isCorrect = false;

            if (i < QnA[currentQuestion].Answers.Length)
            {
                options[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = QnA[currentQuestion].Answers[i];

                if (QnA[currentQuestion].CorrectAnswer == i + 1)
                {
                    options[i].GetComponent<AnswerScript>().isCorrect = true;
                }
            }
            else
            {
                options[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            }
        }
    }

    void EndQuiz()
    {
        QuizPanel.SetActive(false);
        EndScreen.SetActive(true);
        // ScoreText.text = "Your Score: " + score + " / 6";
    }
    public void Retry()
    {
        // Reset the quiz state instead of reloading the scene
        score = 0;

        // Reset the QnA list (you'll need to store the original questions)
        ResetQuestions();

        // Hide end screen and show quiz panel again
        EndScreen.SetActive(false);
        QuizPanel.SetActive(true);

        // Generate first question
        if (QnA.Count > 0)
        {
            generateQuestion();
        }
    }

    private void ResetQuestions()
    {
        // Clear current questions
        QnA.Clear();

        // Restore from the original questions
        foreach (QuestionsAndAnswers qa in originalQuestions)
        {
            QnA.Add(qa);
        }
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene("Scenario2_CoffeeShop");
    }
}
