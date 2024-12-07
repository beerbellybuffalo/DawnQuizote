using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using static MyQuizzesScriptableObject;

public class MainMenuController : MonoBehaviour
{
    //reference to quiz manager script which contains the logic for handling quiz data
    private QuizManager quizManager;
    
    //root UI Element
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
    private Button DeleteAllQuizzesBtn;
    private Button MentalSumsBtn;

    //creating quiz NAME elements
    private Button SaveQuizNameBtn;
    private TextField InputQuizNameField;
    //creating quiz QUESTIONS elements
    private TextField AddQuestionField;
    private TextField AddCorrectAnswerField;
    private TextField AddAltOption1Field;
    private TextField AddAltOption2Field;
    private TextField AddAltOption3Field;
    private Button SaveQuestionBtn;
    private Button SaveWholeQuizBtn;
    private Label NotificationText;
    //delete all?
    private Button ConfirmDeleteAllQuizzesBtn;
    private Button GoBackBtn;
    //play mode page elements
    private Button PauseBtn;
    //pause mode page elements
    private Button ResumeBtn;
    private Button RestartBtn;
    //private Button ReturnToMainMenuBtn;

    //PAGES
    private VisualElement activePage; //the page that is currently active

    private VisualElement mainMenuPage;
    private VisualElement quizSelectionPage;
    private VisualElement creatingQuizNamePage;
    private VisualElement creatingQuizQuestionsPage;
    private VisualElement deleteAllQuizzesConfirmationPage;
    private VisualElement allQuizzesDeletedPage;
    private VisualElement playModePage;
    private VisualElement pauseModePage;

    private void Awake()
    {
        quizManager = GetComponent<QuizManager>();
        
        Root = GetComponent<UIDocument>().rootVisualElement;
        // Query for the pages by their names
        mainMenuPage = Root.Q<VisualElement>("MainMenuPage");
        quizSelectionPage = Root.Q<VisualElement>("QuizSelectionPage");
        creatingQuizNamePage = Root.Q<VisualElement>("CreatingQuizNamePage");
        creatingQuizQuestionsPage = Root.Q<VisualElement>("CreatingQuizQuestionsPage");
        deleteAllQuizzesConfirmationPage = Root.Q<VisualElement>("DeleteAllQuizzesConfirmationPage");
        allQuizzesDeletedPage = Root.Q<VisualElement>("AllQuizzesDeletedPage");
        playModePage = Root.Q<VisualElement>("PlayModePage");
        pauseModePage = Root.Q<VisualElement>("PauseModePage");
        //initialise activePage as mainMenuPage
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
                //this is for the case where returning from pause to main menu
                pauseModePage.style.display = DisplayStyle.None;
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
        DeleteAllQuizzesBtn = Root.Q<Button>("DeleteAllQuizzes");
        DeleteAllQuizzesBtn.clicked += OnDeleteAllQuizzesButtonClicked;
        MentalSumsBtn = Root.Q<Button>("MentalSums");
        MentalSumsBtn.clicked += OnMentalSumsButtonClicked;

        //Buttons on Creating Quiz Page
        SaveQuizNameBtn = Root.Q<Button>("SaveQuizName");
        SaveQuizNameBtn.clicked += OnSaveQuizNameButtonClicked;
        //Text Field on Creating Quiz NAME Page
        InputQuizNameField = Root.Q<TextField>("InputQuizNameField");
        // Force label wrapping
        InputQuizNameField.labelElement.style.whiteSpace = new StyleEnum<WhiteSpace>(WhiteSpace.Normal);
        InputQuizNameField.labelElement.style.maxHeight = new StyleLength(Length.Percent(100)); // Allow full height
        InputQuizNameField.labelElement.style.overflow = Overflow.Visible; // Ensure text is not clipped

        //Text Fields on Creating Quiz QUESTIONS Page
        AddQuestionField = Root.Q<TextField>("AddQuestionField");
        AddCorrectAnswerField = Root.Q<TextField>("AddCorrectAnswerField");
        AddAltOption1Field = Root.Q<TextField>("AddAltOption1Field");
        AddAltOption2Field = Root.Q<TextField>("AddAltOption2Field");
        AddAltOption3Field = Root.Q<TextField>("AddAltOption3Field");
        //Buttons on Creating Quiz QUESTIONS Page
        SaveQuestionBtn = Root.Q<Button>("SaveQuestion");
        SaveQuestionBtn.clicked += OnSaveQuestionButtonClicked;
        SaveWholeQuizBtn = Root.Q<Button>("SaveWholeQuiz");
        SaveWholeQuizBtn.clicked += OnSaveWholeQuizButtonClicked;
        //Notif text
        NotificationText = Root.Q<Label>("NotificationText");

        //Buttons on delete all confirmation page
        ConfirmDeleteAllQuizzesBtn = Root.Q<Button>("DeleteAll");
        ConfirmDeleteAllQuizzesBtn.clicked += OnConfirmDeleteAllQuizzesButtonClicked;
        GoBackBtn = Root.Q<Button>("GoBack");
        GoBackBtn.clicked += OnGoBackButtonButtonClicked;

        //pause button on play mode page
        PauseBtn = Root.Q<Button>("Pause");
        PauseBtn.clicked += OnPauseButtonClicked;

        //Buttons on pause menu
        ResumeBtn = Root.Q<Button>("Resume");
        ResumeBtn.clicked += OnResumeButtonClicked;
        RestartBtn = Root.Q<Button>("Restart");
        RestartBtn.clicked += OnRestartButtonClicked;
        //ReturnToMainMenuBtn = Root.Q<Button>("ReturnToMainMenu");
        //ReturnToMainMenuBtn.clicked += OnReturnToMainMenuButtonClicked;
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
        if (activePage == creatingQuizNamePage || activePage == creatingQuizQuestionsPage)
        {
            if (activePage == creatingQuizQuestionsPage)
            { 
                //remove it from the scroll view
                ScrollView ScrollViewMyQuizzes = Root.Q<ScrollView>("ScrollViewMyQuizzes");
                string quizNameToRemove = quizManager.myQuizzesData.quizzes.Last().quizName;
                Button buttonToRemove = ScrollViewMyQuizzes.Q<Button>(quizNameToRemove);
                if (buttonToRemove != null)
                { 
                    ScrollViewMyQuizzes.Remove(buttonToRemove);
                    Debug.Log($"Removed {buttonToRemove}");
                }

                //delete the latest quiz
                quizManager.DeleteQuizByIndex(quizManager.myQuizzesData.quizzes.Count-1);
            }
            activePage.style.display = DisplayStyle.None;
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
        creatingQuizNamePage.style.display = DisplayStyle.Flex;
        activePage = creatingQuizNamePage;
    }
    private void OnDeleteAllQuizzesButtonClicked()
    {
        quizSelectionPage.style.display = DisplayStyle.None;
        deleteAllQuizzesConfirmationPage.style.display = DisplayStyle.Flex;
        activePage = deleteAllQuizzesConfirmationPage;
    }
    private void OnMentalSumsButtonClicked()
    {
        //change to play mode page
        activePage.style.display = DisplayStyle.None;
        playModePage.style.display = DisplayStyle.Flex;
        activePage = playModePage;
    }

    //CREATING QUIZ NAME PAGE
    private void OnSaveQuizNameButtonClicked()
    {
        //if (InputQuizNameField.value != null) // if something was typed in
        //{
        if (quizManager.AddQuiz(InputQuizNameField, InputQuizNameField.value))
        {
            // Add a new button under "My Quizzes"
            CreateButtonInMyQuizzes(InputQuizNameField.value);

            //GO TO THE NEXT PAGE TO CREATE QUIZ QUESTIONS
            creatingQuizNamePage.style.display = DisplayStyle.None;
            creatingQuizQuestionsPage.style.display = DisplayStyle.Flex;
            activePage = creatingQuizQuestionsPage;
        };

        //}
        //else
        //{
        //    Debug.LogWarning("Quiz Name Field is Empty, please provide a valid Name");
        //}
    }

    public void ClearMyQuizzesScrollView()
    {
        ScrollView ScrollViewMyQuizzes = Root.Q<ScrollView>("ScrollViewMyQuizzes");
        //Find all buttons in ScrollView
        var Buttons = ScrollViewMyQuizzes.Query<Button>().ToList();
        foreach (var button in Buttons)
        {
            ScrollViewMyQuizzes.Remove(button);
            Debug.LogWarning($"button removed: {button.text}");
        }
        //// Find all buttons with the .yellow-button class inside the ScrollView
        //var yellowButtons = ScrollViewMyQuizzes.Query<Button>(".yellow-button").ToList();

        //// Remove each yellow button from the ScrollView
        //foreach (var button in yellowButtons)
        //{
        //    ScrollViewMyQuizzes.Remove(button);
        //    Debug.LogWarning($"button removed: {button.text}");
        //}
        //Debug.Log("Cleared all yellow buttons from scroll view!");
        ////force redraw of UI
        //ScrollViewMyQuizzes.MarkDirtyRepaint();

    }

    public void CreateButtonInMyQuizzes(string name)
    {
        ScrollView ScrollViewMyQuizzes = Root.Q<ScrollView>("ScrollViewMyQuizzes");
        Button newQuizButton = new Button();
        newQuizButton.text = name;
        newQuizButton.AddToClassList("yellow-button");
        ScrollViewMyQuizzes.Add(newQuizButton);
    }

    //CREATING QUIZ QUESTIONS PAGE
    private void OnSaveQuestionButtonClicked()
    {
        //Show warning if any of the required fields have missing inputs
        bool allFieldsFilled = true;
        List<TextField> QuestionInputs = new();
        QuestionInputs.Add(AddQuestionField);
        QuestionInputs.Add(AddCorrectAnswerField);
        QuestionInputs.Add(AddAltOption1Field);
        QuestionInputs.Add(AddAltOption2Field);
        QuestionInputs.Add(AddAltOption3Field);
        foreach (var field in QuestionInputs)
        {
            if (string.IsNullOrEmpty(field.value))
            {
                field.value = "Field cannot be empty.";
                field.style.color = new StyleColor(Color.red);
                allFieldsFilled = false;
            }
            else 
            {
                field.style.color = new StyleColor(Color.black);
            }
        }
        if (allFieldsFilled)
        {
            //get the name of the latest quiz added
            List<MyQuizzesScriptableObject.Quiz> quizzes = quizManager.myQuizzesData.quizzes;
            string quizName = quizzes[quizzes.Count - 1].quizName;
            quizManager.AddQuestionToQuiz(QuestionInputs, quizName);

            //Clear the fields to fill for next question
            foreach (var field in QuestionInputs)
            {
                field.value = string.Empty;
            }
        }
    }
    public void ShowNotificationText(string notification)
    {
        NotificationText.text = notification;
    }

    private void OnSaveWholeQuizButtonClicked()
    {
        Quiz latestQuiz = quizManager.myQuizzesData.quizzes.Last();
        if (latestQuiz.questions.Count > 0) //must have at least 1 question to save
        {
            StartCoroutine(SaveAndWaitBeforeGoingToMyQuizzes());
        }
        else
        {
            NotificationText.style.color = new StyleColor(Color.red);
            ShowNotificationText("Must have at least 1 question");
        }
    }
    private IEnumerator SaveAndWaitBeforeGoingToMyQuizzes()
    { 
        quizManager.SaveQuizzes();
        NotificationText.style.color = new StyleColor(Color.green);
        ShowNotificationText("Quiz Saved.");
        quizManager.LoadQuizzes();
        yield return new WaitForSeconds(2f);
        //navigate to quiz selection page
        creatingQuizQuestionsPage.style.display = DisplayStyle.None;
        quizSelectionPage.style.display = DisplayStyle.Flex;
        activePage = quizSelectionPage;
    }

    //CONFIRM DELETE ALL PAGE
    private void OnConfirmDeleteAllQuizzesButtonClicked()
    { 
        ClearMyQuizzesScrollView();
        quizManager.myQuizzesData.quizzes.Clear();
        //delete from persistent storage as well
        quizManager.SaveQuizzes();
        //navigate to the confirmation message page and display for 2 seconds before going back to quiz selection
        StartCoroutine(DisplayAllDeleted());
    }

    private IEnumerator DisplayAllDeleted()
    { 
        activePage.style.display = DisplayStyle.None;
        allQuizzesDeletedPage.style.display = DisplayStyle.Flex;
        activePage = allQuizzesDeletedPage;
        yield return new WaitForSeconds(2f);
        activePage.style.display = DisplayStyle.None;
        quizSelectionPage.style.display = DisplayStyle.Flex;
        activePage = quizSelectionPage;
    }

    private void OnGoBackButtonButtonClicked()
    {
        activePage.style.display = DisplayStyle.None;
        quizSelectionPage.style.display = DisplayStyle.Flex;
        activePage = quizSelectionPage;
    }

    //PLAY MODE PAGE
    private void OnPauseButtonClicked()
    {
        pauseModePage.style.display = DisplayStyle.Flex;
        activePage = pauseModePage;
        //pause the game time
        Time.timeScale = 0;
    }

    //PAUSE MODE PAGE
    private void OnResumeButtonClicked()
    {
        pauseModePage.style.display = DisplayStyle.None;
        activePage = playModePage;
        //unpause the game time
        Time.timeScale = 1;
    }
    private void OnRestartButtonClicked()
    {
        pauseModePage.style.display = DisplayStyle.None;
        activePage = playModePage;
        //unpause the game time
        Time.timeScale = 1;

        //ADD LOGIC HERE FOR STARTING FROM BEGINNING OF THE QUIZ
    }
    //private void OnReturnToMainMenuButtonClicked()
    //{ 
        //same as home buttons
    //}
}

