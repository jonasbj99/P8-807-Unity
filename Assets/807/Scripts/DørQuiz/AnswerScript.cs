using UnityEngine;

public class AnswerScript : MonoBehaviour
{
    public bool isCorrect = false;
    public QuizManager quizManager;

    public void Answer()
    {
        quizManager.AnswerSelected(isCorrect); // send whether the selected answer was correct
    }
}
