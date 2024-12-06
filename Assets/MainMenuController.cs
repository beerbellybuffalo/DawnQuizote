using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class MainMenuController : MonoBehaviour
{
    private VisualElement Root;
    private Button LetsGoBtn;
    private Button SettingsBtn;
    private Button QuitBtn;
    private Button BackToMainMenuBtn;

    //pages
    private VisualElement mainMenuPage;
    private VisualElement quizSelectionPage;

    private void Awake()
    {
        Root = GetComponent<UIDocument>().rootVisualElement;
        // Query for the pages by their names
        mainMenuPage = Root.Q<VisualElement>("MainMenuPage");
        quizSelectionPage = Root.Q<VisualElement>("QuizSelectionPage");
    }
    private void OnEnable()
    {
        //Buttons on Main Menu Page
        LetsGoBtn = Root.Q<Button>("LetsGo");
        LetsGoBtn.clicked += OnLetsGoButtonClicked;

        SettingsBtn = Root.Q<Button>("Settings");
        SettingsBtn.clicked += OnSettingsButtonClicked;

        QuitBtn = Root.Q<Button>("Quit");
        QuitBtn.clicked += OnQuitButtonClicked;

        //Buttons on Quiz Selection Page
        BackToMainMenuBtn = Root.Q<Button>("BackToMainMenu");
        BackToMainMenuBtn.clicked += OnBackToMainMenuButtonClicked;
    }
    private void OnLetsGoButtonClicked()
    {
        mainMenuPage.style.display = DisplayStyle.None;
        quizSelectionPage.style.display = DisplayStyle.Flex;

        //temporarily make this deactivate the Root
        //gameObject.SetActive(false);
    }
    private void OnSettingsButtonClicked()
    {
        
    }
    private void OnQuitButtonClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

    private void OnBackToMainMenuButtonClicked()
    {
        quizSelectionPage.style.display = DisplayStyle.None;
        mainMenuPage.style.display = DisplayStyle.Flex;
    }
}
