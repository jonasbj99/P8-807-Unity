using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class QuizManager : MonoBehaviour
{
    public List<QuestionsAndAnswers> QnA; // This will hold all your questions and answers
    public GameObject[] options; // Buttons for the answers
    public int currentQuestion;
    public TextMeshProUGUI QuestionTxt; // TextMeshPro component for the question display

    private void Start()
    {
        generateQuestion();
    }

    public void correct()
    {
        QnA.RemoveAt(currentQuestion); // Remove the current question after answering
        generateQuestion(); // Generate the next question
    }

    void SetAnswers()
    {
        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<AnswerScript>().isCorrect = false;
            options[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = QnA[currentQuestion].Answers[i];
            
            // Mark the correct answer button
            if (QnA[currentQuestion].CorrectAnswer == i + 1) // CorrectAnswer is 1-based (1, 2, 3, 4)
            {
                options[i].GetComponent<AnswerScript>().isCorrect = true;
            }
        }
    }

    void generateQuestion()
    {
        currentQuestion = Random.Range(0, QnA.Count); // Randomly select a question
        QuestionTxt.text = QnA[currentQuestion].Question; // Display the question
        SetAnswers(); // Set the answers on the buttons
    }
}
