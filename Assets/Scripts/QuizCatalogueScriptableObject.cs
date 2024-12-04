using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static QuizCatalogueScriptableObject;

[CreateAssetMenu(fileName = "QuizCatalogueScriptableObject", menuName = "Scriptable Objects/QuizCatalogueScriptableObject")]
public class QuizCatalogueScriptableObject : ScriptableObject
{
    [System.Serializable]
    public class Option
    {
        public string optionText;
        [Tooltip("Check to indicate that this is the correct answer")]
        public bool isCorrect;
    }

    [System.Serializable]
    public class Question
    {
        public string questionText;

        [Tooltip("Questions can only have a maximum of 4 options.")]
        public Option[] options = new Option[4];

        // Enforce size in code
        public void EnforceOptionSize()
        {
            if (options.Length != 4)
            {
                Array.Resize(ref options, 4); // Resize to 4 elements
            }
        }
    }

    [System.Serializable]
    public class Quiz
    {
        public string quizName;
        [Tooltip("Quizzes can only have a maximum of 10 questions.")]
        public List<Question> questions = new List<Question>();

        // Enforce size of options in each question
        public void EnforceQuestionOptionsSize()
        {
            foreach (var question in questions)
            {
                question.EnforceOptionSize();
            }
        }
    }

    public List<Quiz> quizzes = new List<Quiz>();

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
    }
}
