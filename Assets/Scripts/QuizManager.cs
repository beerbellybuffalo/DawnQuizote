using System.IO;
using UnityEngine;
using System.Linq;
using static MyQuizzesScriptableObject;
using Unity.Core;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class QuizManager : MonoBehaviour
{
    // Reference to the ScriptableObject instance in the inspector
    public MyQuizzesScriptableObject myQuizzesData;
    private MainMenuController mainMenuController;

    private string filePath => Application.persistentDataPath + "/quizzes.json";

    public void Awake()
    {
        mainMenuController = GetComponent<MainMenuController>();
        LoadQuizzes();
    }

    // Load quizzes from persistent storage
    public void LoadQuizzes()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);

            // Create a temporary wrapper class to deserialize the data
            MyQuizzesScriptableObject tempData = ScriptableObject.CreateInstance<MyQuizzesScriptableObject>();
            JsonUtility.FromJsonOverwrite(json, tempData);

            // Copy the deserialized data into the existing ScriptableObject
            myQuizzesData.quizzes = tempData.quizzes;
            Debug.Log("Quiz data successfully loaded from persistent storage.");

            //mainMenuController.ClearMyQuizzesScrollView();
            foreach (var quiz in myQuizzesData.quizzes)
            {
                mainMenuController.CreateButtonInMyQuizzes(quiz.quizName);
            }
            Debug.Log("Updated My Quizzes UI");
        }
        else
        {
            Debug.Log("No persistent quiz data found.");
        }
    }


    // Save quizzes to persistent storage
    public void SaveQuizzes()
    {
        string json = JsonUtility.ToJson(myQuizzesData, true);
        File.WriteAllText(filePath, json);
    }

    public bool DoesQuizExist(string quizName)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Quiz data file does not exist.");
            return false;
        }

        //// Load JSON data from file
        //string json = File.ReadAllText(filePath);

        //// Deserialize JSON to a temporary MyQuizzesScriptableObject instance
        //MyQuizzesScriptableObject tempData = JsonUtility.FromJson<MyQuizzesScriptableObject>(json);

        //// Check if the quiz name exists in the list
        //bool exists = tempData.quizzes.Any(q => q.quizName == quizName);

        // Check if the quiz name exists in the list
        bool exists = myQuizzesData.quizzes.Any(q => q.quizName == quizName);
        return exists;
    }

    // Add a new quiz
    public bool AddQuiz(TextField InputQuizNameField, string name)
    {
        //if no valid input was given, show a warning message
        if (string.IsNullOrEmpty(name))
        {
            InputQuizNameField.label = "Quiz name cannot be empty.";
            InputQuizNameField.labelElement.style.color = new StyleColor(Color.white);
            return false;
        }
        //if quiz does not already exist, add it
        else if (!DoesQuizExist(name))
        {
            // Create a new quiz and add it to the catalogue
            Quiz newQuiz = new Quiz { quizName = name };
            myQuizzesData.quizzes.Add(newQuiz);
            Debug.Log($"New Quiz Added: {name}");
            InputQuizNameField.label = $"Created new quiz: {name}";
            InputQuizNameField.labelElement.style.color = new StyleColor(Color.green);
            // Save changes to persistent storage
            SaveQuizzes();
            return true;
        }
        else // quiz already exists, show a warning message
        {
            Debug.LogWarning($"Quiz named '{name}' already exists, try something else!");
            InputQuizNameField.label = $"Quiz named '{name}' already exists, try something else!";
            InputQuizNameField.labelElement.style.color = new StyleColor(Color.white);
            return false;
        }
    }

    //Retrieve a quiz by name
    public Quiz GetQuizByName(string name, List<Quiz> quizList)
    {
        return quizList.Find(q => q.quizName == name);
    }

    // Delete a quiz by name
    public void DeleteQuizByName(string quizName)
    {
        var quizToRemove = myQuizzesData.quizzes.FirstOrDefault(q => q.quizName == quizName);
        if (quizToRemove != null)
        {
            myQuizzesData.quizzes.Remove(quizToRemove);
            SaveQuizzes();  // Save changes to file
        }
        else
        {
            Debug.LogWarning($"Quiz '{quizName}' not found.");
        }
    }

    // Delete a quiz by index (as in original code)
    public void DeleteQuizByIndex(int index)
    {
        if (index >= 0 && index < myQuizzesData.quizzes.Count)
        {
            myQuizzesData.quizzes.RemoveAt(index);
            SaveQuizzes();
        }
    }

    public void AddQuestionToQuiz(List<TextField> QuestionInputs, string quizName)
    { 

        Quiz QuizToAddQuestion= myQuizzesData.quizzes.Find(q => q.quizName == quizName);
        if (QuizToAddQuestion != null)
        {
            if (QuizToAddQuestion.questions.Count < 10)
            {
                //CONSTRUCT THE NEW QUESTION
                Question question = new Question();
                question.questionText = QuestionInputs[0].value; //AddQuestionField
                Option correctOption = new Option() { optionText = QuestionInputs[1].value, isCorrect = true };
                question.options.Add(correctOption);
                //Add all other options, by default they have isCorrect set as false as per the Scriptable Object definition
                question.options.Add(new Option() { optionText = QuestionInputs[2].value, isCorrect = false });
                question.options.Add(new Option() { optionText = QuestionInputs[3].value, isCorrect = false });
                question.options.Add(new Option() { optionText = QuestionInputs[4].value, isCorrect = false });
                //ADD THE QUESTION TO THE SPECIFIED QUIZ
                QuizToAddQuestion.questions.Add(question);

                //UPDATE IN PERSISTENT STORAGE
                SaveQuizzes();

                //SHOW FEEDBACK NOTIFICATION ON UI
                mainMenuController.ShowNotificationText(new StyleColor(Color.green),$"Question Added! The Quiz '{quizName}' has {QuizToAddQuestion.questions.Count}/10 questions");
            }
            else
            {
                Debug.LogWarning("Cannot add anymore questions to quiz.");
            }
        }
        else
        {
            Debug.LogWarning($"Quiz '{quizName}' could not be found. Question not added.");
        }

    }
}
