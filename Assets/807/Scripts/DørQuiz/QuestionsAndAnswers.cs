using UnityEngine;

[System.Serializable]
public class QuestionsAndAnswers
{
    public string Question; // The question text
    public string[] Answers; // Array of possible answers
    public int CorrectAnswer; // The correct answer index (1-based)
}

