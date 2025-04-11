using UnityEngine;
[System.Serializable]
public class QuestionsAndAnswers
{
    public string Question;
    public string[] Answers = new string[4]; // 4 answers per question
    public int CorrectAnswer; // 1 = first answer, 2 = second, etc.
}

