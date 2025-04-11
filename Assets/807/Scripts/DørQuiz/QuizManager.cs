using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class QuizManager : MonoBehaviour
{
    public List<QuestionsAndAnswers> QnA; // This will hold all your questions and answers
    public GameObject[] options; // Buttons for the answers
    public int currentQuestion; // Index of the current question
    public TextMeshProUGUI QuestionTxt; // TextMeshPro component for the question display
    public int score = 0; // Track score

    private void Start()
    {
        // Ensure QnA list is populated
        if (QnA.Count > 0)
        {
            generateQuestion(); // Start the quiz by generating the first question
        }
        else
        {
            Debug.LogError("No questions in the quiz! Please populate the QnA list.");
        }
    }

    public void correct()
    {
        // Check if current question index is valid
        if (currentQuestion < QnA.Count)
        {
            // Increase the score if the answer is correct
            if (options[currentQuestion].GetComponent<AnswerScript>().isCorrect)
            {
                score++;
            }

            // Check if there are more questions
            if (currentQuestion < QnA.Count - 1)
            {
                currentQuestion++; // Move to the next question
                generateQuestion(); // Generate the next question
            }
            else
            {
                Debug.Log("Quiz Complete! Final Score: " + score + "/" + QnA.Count);
            }
        }
        else
        {
            Debug.LogError("currentQuestion index is out of range: " + currentQuestion);
        }
    }

    void SetAnswers()
    {
        // Ensure answers are set only if valid options exist
        if (QnA.Count > 0 && options.Length > 0)
        {
            for (int i = 0; i < options.Length; i++)
            {
                if (i < QnA[currentQuestion].Answers.Length)
                {
                    options[i].GetComponent<AnswerScript>().isCorrect = false;
                    options[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = QnA[currentQuestion].Answers[i];

                    // Mark the correct answer button
                    if (QnA[currentQuestion].CorrectAnswer == i + 1)
                    {
                        options[i].GetComponent<AnswerScript>().isCorrect = true;
                    }
                }
            }
        }
    }

    void generateQuestion()
    {
        // Ensure there are still questions available
        if (currentQuestion < QnA.Count)
        {
            // Update the displayed question and set answers
            QuestionTxt.text = QnA[currentQuestion].Question;
            SetAnswers();
        }
        else
        {
            Debug.Log("No more questions left!");
        }
    }
}
