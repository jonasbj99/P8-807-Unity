using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuizManager : MonoBehaviour
{
    public List<QuestionsAndAnswers> QnA;
    public GameObject[] options;
    public int currentQuestion;
    public TextMeshProUGUI QuestionTxt;

    private void Start()
    {
        generateQuestion();
    }

    void SetAnswers()
    {
        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponentInChildren<TextMeshProUGUI>().text = QnA[currentQuestion].Answers[i];
        }
    }

    void generateQuestion()
    {
        currentQuestion = Random.Range(0, QnA.Count); 
        QuestionTxt.text = QnA[currentQuestion].Question;
        SetAnswers();
    }
}
