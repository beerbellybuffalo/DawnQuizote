using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static MyQuizzesScriptableObject;

[CreateAssetMenu(fileName = "MyQuizzesScriptableObject", menuName = "Scriptable Objects/MyQuizzesScriptableObject")]
public class MyQuizzesScriptableObject : ScriptableObject
{
    [System.Serializable]
    public class Option
    {
        public string optionText;
        [Tooltip("Check to indicate that this is the correct answer")]
        public bool isCorrect = false; //false by default
    }

    [System.Serializable]
    public class Question
    {
        public string questionText;

        [Tooltip("Questions can only have a maximum of 4 options.")]
        public List<Option> options = new();

        // Enforce size in code
        public void EnforceOptionSize()
        {
            if (options.Count > 4)
            {
                options = options.Take(4).ToList(); // Resize to 4 elements
            }
        }
    }

    [System.Serializable]
    public class Quiz
    {
        public string quizName;
        [Tooltip("Quizzes can only have a maximum of 10 questions.")]
        public List<Question> questions = new List<Question>();
        public int HighScore = -1; //-1 as the default value to check for

        // Enforce size of options in each question
        public void EnforceQuestionOptionsSize()
        {
            foreach (var question in questions)
            {
                question.EnforceOptionSize();
            }
        }
    }

    //This is for My Quizzes
    public List<Quiz> quizzes = new List<Quiz>();
    //This is for Popular Quizzes
    public List<Quiz> popularQuizzes = new List<Quiz>();

    public Quiz GetQuizByName(string name, List<Quiz> quizList)
    {
        return quizList.Find(q => q.quizName == name);
    }

    private void OnValidate()
    {
        foreach (var quiz in quizzes)
        {
            quiz.EnforceQuestionOptionsSize();
            if (quiz.questions.Count > 10)
            {
                Debug.LogWarning($"Quiz '{quiz.quizName}' cannot have more than 10 questions. Extra questions will be removed.");
                quiz.questions = quiz.questions.Take(10).ToList(); // Trim the list to 10
            }
        }

        foreach (var quiz in popularQuizzes)
        {
            quiz.EnforceQuestionOptionsSize();
            if (quiz.questions.Count > 10)
            {
                Debug.LogWarning($"Quiz '{quiz.quizName}' cannot have more than 10 questions. Extra questions will be removed.");
                quiz.questions = quiz.questions.Take(10).ToList(); // Trim the list to 10
            }
        }
    }
}
