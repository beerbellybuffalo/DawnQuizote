using System.IO;
using UnityEngine;
using System.Linq;
using static QuizCatalogueScriptableObject;

public class QuizManager : MonoBehaviour
{
    // Reference to the ScriptableObject instance in the inspector
    public QuizCatalogueScriptableObject quizCatalogueData;

    private string filePath => Application.persistentDataPath + "/quizzes.json";

    // Load quizzes from file (optional: if you want to load data from a file)
    public void LoadQuizzes()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            // Manually load from JSON if needed
            quizCatalogueData = JsonUtility.FromJson<QuizCatalogueScriptableObject>(json);
        }
        else
        {
            Debug.Log("No quizzes found.");
        }
    }

    // Save quizzes to file (optional: if you want to save data)
    public void SaveQuizzes()
    {
        string json = JsonUtility.ToJson(quizCatalogueData, true);
        File.WriteAllText(filePath, json);
    }

    // Add a new quiz
    public void AddQuiz(string name)
    {
        // Create a new quiz and add it to the catalogue
        Quiz newQuiz = new Quiz { quizName = name };
        quizCatalogueData.quizzes.Add(newQuiz);

        // Optionally save changes to file
        SaveQuizzes();
    }

    // Delete a quiz by name
    public void DeleteQuizByName(string quizName)
    {
        var quizToRemove = quizCatalogueData.quizzes.FirstOrDefault(q => q.quizName == quizName);
        if (quizToRemove != null)
        {
            quizCatalogueData.quizzes.Remove(quizToRemove);
            SaveQuizzes();  // Optionally save changes to file
        }
        else
        {
            Debug.LogWarning($"Quiz '{quizName}' not found.");
        }
    }

    // Delete a quiz by index (as in original code)
    public void DeleteQuizByIndex(int index)
    {
        if (index >= 0 && index < quizCatalogueData.quizzes.Count)
        {
            quizCatalogueData.quizzes.RemoveAt(index);
            SaveQuizzes();  // Optionally save changes to file
        }
    }
}
