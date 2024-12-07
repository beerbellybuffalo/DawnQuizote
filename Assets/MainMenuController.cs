using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System.Collections.Generic;

public class MainMenuController : MonoBehaviour
{
    private VisualElement Root;
    //common elements
    private List<Button> homeButtons;
    private List<Button> backButtons;
    //main menu elements
    private Button LetsGoBtn;
    private Button SettingsBtn;
    private Button QuitBtn;
    //quiz selection elements
    private Button CreateQuizBtn;
    //creating quiz elements
    private Button SaveQuizNameBtn;

    //pages
    private VisualElement activePage; //the page that is currently active
    private VisualElement mainMenuPage;
    private VisualElement quizSelectionPage;
    private VisualElement creatingQuizPage;

    private void Awake()
    {
        Root = GetComponent<UIDocument>().rootVisualElement;
        // Query for the pages by their names
        mainMenuPage = Root.Q<VisualElement>("MainMenuPage");
        quizSelectionPage = Root.Q<VisualElement>("QuizSelectionPage");
        creatingQuizPage = Root.Q<VisualElement>("CreatingQuizPage");
        activePage = mainMenuPage;
    }
    private void OnEnable()
    {
        //Common Buttons on Multiple Pages, e.g. home
        homeButtons = Root.Query<Button>("Home").ToList();
        foreach (var homeButton in homeButtons)
        {
            homeButton.clicked += () =>
            {
                activePage.style.display = DisplayStyle.None;
                mainMenuPage.style.display = DisplayStyle.Flex;
                activePage = mainMenuPage;
            };
        }
        backButtons = Root.Query<Button>("Back").ToList();
        foreach (var backButton in backButtons)
        {
            backButton.clicked += OnBackButtonClicked; //handle different cases below
        }

        //Buttons on Main Menu Page
        LetsGoBtn = Root.Q<Button>("LetsGo");
        LetsGoBtn.clicked += OnLetsGoButtonClicked;

        SettingsBtn = Root.Q<Button>("Settings");
        SettingsBtn.clicked += OnSettingsButtonClicked;

        QuitBtn = Root.Q<Button>("Quit");
        QuitBtn.clicked += OnQuitButtonClicked;

        //Buttons on Quiz Selection Page
        CreateQuizBtn = Root.Q<Button>("CreateQuiz");
        CreateQuizBtn.clicked += OnCreateQuizButtonClicked;

        //Buttons on Creating Quiz Page
        SaveQuizNameBtn = Root.Q<Button>("SaveQuizName");
        SaveQuizNameBtn.clicked += OnSaveQuizNameButtonClicked;
    }

    /// <summary>
    /// Methods for handling button click events.
    /// </summary>
    
    //COMMON
    //private void OnHomeButtonClicked()
    //{
    //    activePage.style.display = DisplayStyle.None;
    //    mainMenuPage.style.display = DisplayStyle.Flex;
    //    activePage = mainMenuPage;
    //}
    private void OnBackButtonClicked()
    {
        if (activePage == creatingQuizPage)
        {
            creatingQuizPage.style.display = DisplayStyle.None;
            quizSelectionPage.style.display = DisplayStyle.Flex;
            activePage = quizSelectionPage;
        }
    }

    //MAIN MENU PAGE
    private void OnLetsGoButtonClicked()
    {
        mainMenuPage.style.display = DisplayStyle.None;
        quizSelectionPage.style.display = DisplayStyle.Flex;
        activePage = quizSelectionPage;

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

    //QUIZ selection PAGE
    private void OnCreateQuizButtonClicked()
    {
        quizSelectionPage.style.display = DisplayStyle.None;
        creatingQuizPage.style.display = DisplayStyle.Flex;
        activePage = creatingQuizPage;
    }

    //CREATING QUIZ PAGE
    private void OnSaveQuizNameButtonClicked()
    { 
    
    }
}
