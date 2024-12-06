using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MyQuizzesScriptableObject))]
public class MyQuizzesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Get the target ScriptableObject
        MyQuizzesScriptableObject catalogue = (MyQuizzesScriptableObject)target;

        // Iterate through quizzes and questions
        foreach (var quiz in catalogue.quizzes)
        {
            foreach (var question in quiz.questions)
            {
                // Check if at least one option is marked as correct
                bool hasCorrectAnswer = false;
                foreach (var option in question.options)
                {
                    if (option != null && option.isCorrect)
                    {
                        hasCorrectAnswer = true;
                        break;
                    }
                }

                // Highlight options section if no correct answer is set
                if (!hasCorrectAnswer)
                {
                    GUIStyle redBoxStyle = new GUIStyle(EditorStyles.helpBox);
                    redBoxStyle.normal.background = MakeTex(2, 2, new Color(1f, 0.5f, 0.5f, 1f)); // Light red background
                    GUILayout.BeginVertical(redBoxStyle);
                    EditorGUILayout.HelpBox($"Question '{question.questionText}' must have at least one correct option.", MessageType.Error);
                    GUILayout.EndVertical();
                }
            }
        }
    }

    // Helper method to create colored textures
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
