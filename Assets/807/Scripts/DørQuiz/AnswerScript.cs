using UnityEngine;

public class AnswerScript : MonoBehaviour
{
    public bool isCorrect = false; // Flag to determine if the answer is correct
    public QuizManager quizManager; // Reference to the QuizManager

    public void Answer()
    {
        if (isCorrect)
        {
            Debug.Log("Correct Answer");
        }
        else
        {
            Debug.Log("Wrong Answer");
        }

        // Call the 'correct' method in QuizManager after selecting an answer
        quizManager.correct();
    }
}
