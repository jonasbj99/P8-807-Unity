using System.Collections;
using System.Collection.Generic;
using UnityEngine;

public class QuizManager : MonoBehaviour
{
    public List<QuestionsAndAnswers> QnA;
    public GameObject[] options;
    public int curentQuestion
    public Text QuestionTxt;
    private void Start()
    {
        generateQuestion();
    }
    
    void generateQuestion()
    {
        currentQuestion = Random.Range(0, QnA.Count); 
        QuestionTxt.text = QnA[currentQuestion].Question;

    }


}
