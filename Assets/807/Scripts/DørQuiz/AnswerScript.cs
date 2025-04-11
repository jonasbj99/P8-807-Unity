using UnityEngine;
using UnityEngine.UI; // Importing to work with colors

public class AnswerScript : MonoBehaviour
{
    public bool isCorrect = false; // Flag to determine if the answer is correct
    public QuizManager quizManager; // Reference to the QuizManager
    private Image buttonImage; // The image component of the button

    private void Start()
    {
        // Get the button's Image component to change the color
        buttonImage = GetComponent<Image>();
    }

    public void Answer()
    {
        // If the answer is correct, highlight in green, otherwise red
        if (isCorrect)
        {
            buttonImage.color = Color.green; // Correct answer highlighted in green
            Debug.Log("Correct Answer");
        }
        else
        {
            buttonImage.color = Color.red; // Incorrect answer highlighted in red
            Debug.Log("Wrong Answer");
        }

        // Call the 'correct' method in QuizManager after selecting an answer
        quizManager.correct();
    }
}
