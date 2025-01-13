using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using static MyQuizzesScriptableObject;
using System.Net;
public class MainMenuController : MonoBehaviour
{
    //reference to quiz manager script which contains the logic for handling quiz data
    private QuizManager quizManager;

    //reference to active quiz
    private Quiz activeQuiz = null;
    private Quiz previousQuiz = null;

    //reference to audio manager
    private AudioManager audioManager;

    public Animator runnerAnimator;

    //for storing animation state names
    private static Dictionary<int, string> animStateHashToName;

    //store all active instances of road prefabs for pausing movement
    private GameObject[] RoadPrefabs;

    //THE REMAINING DECLARATIONS ARE FOR THE GAME UI

    //root UI Element
    private VisualElement Root;
    
    //common elements
    private List<Button> homeButtons;
    private List<Button> backButtons;

    //button 2D textures, assign in inspector
    [SerializeField] private Texture2D YellowButtonTexture;
    [SerializeField] private Texture2D GreenButtonTexture;
    [SerializeField] private Texture2D RedButtonTexture;

    //main menu elements
    private Button LetsGoBtn;
    private Button SettingsBtn;
    private Button QuitBtn;

    //settings page elements
    private Button ResetHighScoresBtn;
    private Button ResetHighScoresYesBtn;
    private Button ResetHighScoresNoBtn;
    private VisualElement ResetHighScoresConfirmation;
    private Label ResetCompleteTextLabel;

    //quiz selection elements
    private Button CreateQuizBtn;
    private Button DeleteAllQuizzesBtn;
    private Button MentalSumsBtn;
    private ScrollView ScrollViewMyQuizzes;

    //quiz options elements
    private Label TitleQuizNameLabel;
    private Button PlayQuizBtn;
    private Button EditQuizBtn;
    private Button DeleteQuizBtn;

    //delete quiz confirmation elements
    private Button DeleteQuizConfirmBtn;
    private Button DeleteQuizGoBackBtn;

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

    //delete all quizzes confirmation page elements
    private Button ConfirmDeleteAllQuizzesBtn;
    private Button GoBackBtn;

    //play mode page elements
    private Button PauseBtn;
    private Label QuestionTextLabel;
    private Button Option1Btn;
    private Button Option2Btn;
    private Button Option3Btn;
    private Button Option4Btn;
    private List<Button> OptionButtons;
    private Question currentQuestion;
    private Label QuizNameTextLabel;
    private Label HighScoreTextLabel;
    private bool isAnswering;
    
    //Question Number UI
    private VisualElement QuestionNumber;
    private VisualElement QuestionTotal;
    [SerializeField] private Texture2D _Q1;
    [SerializeField] private Texture2D _Q2;
    [SerializeField] private Texture2D _Q3;
    [SerializeField] private Texture2D _Q4;
    [SerializeField] private Texture2D _Q5;
    [SerializeField] private Texture2D _Q6;
    [SerializeField] private Texture2D _Q7;
    [SerializeField] private Texture2D _Q8;
    [SerializeField] private Texture2D _Q9;
    [SerializeField] private Texture2D _Q10;
    [SerializeField] private Texture2D _Total1;
    [SerializeField] private Texture2D _Total2;
    [SerializeField] private Texture2D _Total3;
    [SerializeField] private Texture2D _Total4;
    [SerializeField] private Texture2D _Total5;
    [SerializeField] private Texture2D _Total6;
    [SerializeField] private Texture2D _Total7;
    [SerializeField] private Texture2D _Total8;
    [SerializeField] private Texture2D _Total9;
    [SerializeField] private Texture2D _Total10;

    //Timer UI
    private VisualElement Timer;
    [SerializeField] private Texture2D _1seconds;
    [SerializeField] private Texture2D _2seconds;
    [SerializeField] private Texture2D _3seconds;
    [SerializeField] private Texture2D _4seconds;
    [SerializeField] private Texture2D _5seconds;
    [SerializeField] private Texture2D _6seconds;
    [SerializeField] private Texture2D _7seconds;
    [SerializeField] private Texture2D _8seconds;
    [SerializeField] private Texture2D _9seconds;
    [SerializeField] private Texture2D _10seconds;

    //Score Indicator UI
    private VisualElement scoreIndicator;
    [SerializeField] private Texture2D _0Percent;
    [SerializeField] private Texture2D _10Percent;
    [SerializeField] private Texture2D _20Percent;
    [SerializeField] private Texture2D _30Percent;
    [SerializeField] private Texture2D _40Percent;
    [SerializeField] private Texture2D _50Percent;
    [SerializeField] private Texture2D _60Percent;
    [SerializeField] private Texture2D _70Percent;
    [SerializeField] private Texture2D _80Percent;
    [SerializeField] private Texture2D _90Percent;
    [SerializeField] private Texture2D _100Percent;

    //pause mode page elements
    private Button ResumeBtn;
    private Button RestartBtn;
    //private Button ReturnToMainMenuBtn; //THIS IS ASSIGNED TO HAVE SAME BEHAVIOUR AS HOME BUTTON, via UI BUILDER.

    //Quiz Completed Page
    /// <summary>
    /// Total Score, High Score, Final Score Indicator, Buttons to try again or return to quiz selection
    /// </summary>
    private Label TotalScoreLabel;
    private Label HighScoreLabel;
    private VisualElement FinalScoreIndicator;
    private Button TryAgainBtn;
    private Button DoAnotherQuizBtn;
    private int TotalScore;

    //PAGES
    private VisualElement activePage; //the page that is currently active

    private VisualElement mainMenuPage;
    private VisualElement settingsPage;
    private VisualElement quizSelectionPage;
    private VisualElement quizOptionsPage;
    private VisualElement deleteQuizConfirmationPage;
    private VisualElement quizDeletedPage;
    private VisualElement creatingQuizNamePage;
    private VisualElement creatingQuizQuestionsPage;
    private VisualElement deleteAllQuizzesConfirmationPage;
    private VisualElement allQuizzesDeletedPage;
    private VisualElement playModePage;
    private VisualElement pauseModePage;
    private VisualElement quizCompletedPage;
    private VisualElement timesUpPage;

    private void Awake()
    {
        quizManager = GetComponent<QuizManager>();
        audioManager = AudioManager.instance;

        Root = GetComponent<UIDocument>().rootVisualElement;
        // Query for the pages by their names
        mainMenuPage = Root.Q<VisualElement>("MainMenuPage");
        settingsPage = Root.Q<VisualElement>("SettingsPage");
        quizSelectionPage = Root.Q<VisualElement>("QuizSelectionPage");
        quizOptionsPage = Root.Q<VisualElement>("QuizOptionsPage");
        deleteQuizConfirmationPage = Root.Q<VisualElement>("DeleteQuizConfirmationPage");
        quizDeletedPage = Root.Q<VisualElement>("QuizDeletedPage");
        creatingQuizNamePage = Root.Q<VisualElement>("CreatingQuizNamePage");
        creatingQuizQuestionsPage = Root.Q<VisualElement>("CreatingQuizQuestionsPage");
        deleteAllQuizzesConfirmationPage = Root.Q<VisualElement>("DeleteAllQuizzesConfirmationPage");
        allQuizzesDeletedPage = Root.Q<VisualElement>("AllQuizzesDeletedPage");
        playModePage = Root.Q<VisualElement>("PlayModePage");
        pauseModePage = Root.Q<VisualElement>("PauseModePage");
        quizCompletedPage = Root.Q<VisualElement>("QuizCompletedPage");
        timesUpPage = Root.Q<VisualElement>("TimesUpPage");

        //initialise activePage as mainMenuPage
        activePage = mainMenuPage;

        //init variables for Play mode
        isAnswering = false;
        TotalScore = 0;
    }
    private void OnEnable()
    {
        //save references to all animation states for later use
        //create a dictionary to store these state names
        animStateHashToName = new Dictionary<int, string>();
        //var controller = runnerAnimator.runtimeAnimatorController as AnimatorController;
        //if (controller == null)
        //{
        //    Debug.LogError("Animator Controller not found!");
        //    return;
        //}

        //foreach (var layer in controller.layers)
        //{
        //    //Debug.Log($"Layer: {layer.name}");
        //    foreach (var state in layer.stateMachine.states)
        //    {
        //        //Debug.Log($"  State: {state.state.name}");
        //        //store state name in dictionary
        //        var name = state.state.name;
        //        var nameAsHash = runnerAnimator.StringToHash(name);
        //        animStateHashToName.Add(nameAsHash,name);
        //    }
        //}

        //Common Buttons on Multiple Pages, e.g. home
        homeButtons = Root.Query<Button>("Home").ToList();
        foreach (var homeButton in homeButtons)
        {
            homeButton.clicked += () =>
            {
                if (activePage == pauseModePage) //this is for the case where returning from pause to main menu
                {
                    playModePage.style.display = DisplayStyle.None;
                    //unpause the game time
                    Time.timeScale = 1;
                    StartCoroutine(EndQuizAndNavigateToPage(mainMenuPage));
                }
                else
                {
                    activeQuiz = null;
                    NavigateFromTo(activePage, mainMenuPage);
                }
            };
        }
        backButtons = Root.Query<Button>("Back").ToList();
        foreach (var backButton in backButtons)
        {
            backButton.clicked += OnBackButtonClicked; //handle different cases below
        }

        //Buttons on Main Menu Page
        LetsGoBtn = mainMenuPage.Q<Button>("LetsGo");
        LetsGoBtn.clicked += OnLetsGoButtonClicked;
        SettingsBtn = mainMenuPage.Q<Button>("Settings");
        SettingsBtn.clicked += OnSettingsButtonClicked;
        QuitBtn = mainMenuPage.Q<Button>("Quit");
        QuitBtn.clicked += OnQuitButtonClicked;

        //Elements on Settings Page
        ResetHighScoresConfirmation = settingsPage.Q<VisualElement>("ResetHighScoresConfirmation");
        ResetCompleteTextLabel = settingsPage.Q<Label>("ResetCompleteText");

        //Buttons on Settings Page
        ResetHighScoresBtn = settingsPage.Q<Button>("ResetHighScores");
        ResetHighScoresBtn.clicked += OnResetHighScoresButtonClicked;
        ResetHighScoresYesBtn = settingsPage.Q<Button>("ResetHighScoresYes");
        ResetHighScoresYesBtn.clicked += OnResetHighScoresYesButtonClicked;
        ResetHighScoresNoBtn = settingsPage.Q<Button>("ResetHighScoresNo");
        ResetHighScoresNoBtn.clicked += OnResetHighScoresNoButtonClicked;

        //Elements on Quiz Selection Page
        ScrollViewMyQuizzes = quizSelectionPage.Q<ScrollView>("ScrollViewMyQuizzes");
        //Buttons on Quiz Selection Page
        CreateQuizBtn = quizSelectionPage.Q<Button>("CreateQuiz");
        CreateQuizBtn.clicked += OnCreateQuizButtonClicked;
        DeleteAllQuizzesBtn = quizSelectionPage.Q<Button>("DeleteAllQuizzes");
        DeleteAllQuizzesBtn.clicked += OnDeleteAllQuizzesButtonClicked;
        MentalSumsBtn = quizSelectionPage.Q<Button>("MentalSums");
        MentalSumsBtn.clicked += OnMentalSumsButtonClicked;

        //Elements on Quiz Options Page
        TitleQuizNameLabel = quizOptionsPage.Q<Label>("TitleQuizName");
        //Buttons on Quiz Options Page
        PlayQuizBtn = quizOptionsPage.Q<Button>("PlayQuiz");
        PlayQuizBtn.clicked += OnPlayQuizButtonClicked;
        EditQuizBtn = quizOptionsPage.Q<Button>("EditQuiz");
        EditQuizBtn.clicked += OnEditQuizButtonClicked;
        DeleteQuizBtn = quizOptionsPage.Q<Button>("DeleteQuiz");
        DeleteQuizBtn.clicked += OnDeleteQuizButtonClicked;

        //Buttons on Delete Quiz Confirmation Page
        DeleteQuizConfirmBtn = deleteQuizConfirmationPage.Q<Button>("DeleteQuizConfirm");
        DeleteQuizConfirmBtn.clicked += OnConfirmDeleteQuizButtonClicked;
        DeleteQuizGoBackBtn = deleteQuizConfirmationPage.Q<Button>("DeleteQuizGoBack");
        DeleteQuizGoBackBtn.clicked += OnDeleteQuizGoBackButtonClicked;

        //Buttons on Creating Quiz NAME Page
        SaveQuizNameBtn = creatingQuizNamePage.Q<Button>("SaveQuizName");
        SaveQuizNameBtn.clicked += OnSaveQuizNameButtonClicked;
        //Text Field on Creating Quiz NAME Page
        InputQuizNameField = creatingQuizNamePage.Q<TextField>("InputQuizNameField");
        // Force label wrapping
        InputQuizNameField.labelElement.style.whiteSpace = new StyleEnum<WhiteSpace>(WhiteSpace.Normal);
        InputQuizNameField.labelElement.style.maxHeight = new StyleLength(Length.Percent(100)); // Allow full height
        InputQuizNameField.labelElement.style.overflow = Overflow.Visible; // Ensure text is not clipped

        //Text Fields on Creating Quiz QUESTIONS Page
        AddQuestionField = creatingQuizQuestionsPage.Q<TextField>("AddQuestionField");
        AddCorrectAnswerField = creatingQuizQuestionsPage.Q<TextField>("AddCorrectAnswerField");
        AddAltOption1Field = creatingQuizQuestionsPage.Q<TextField>("AddAltOption1Field");
        AddAltOption2Field = creatingQuizQuestionsPage.Q<TextField>("AddAltOption2Field");
        AddAltOption3Field = creatingQuizQuestionsPage.Q<TextField>("AddAltOption3Field");

        //register callbacks to clear these fields when the user clicks on them
        //this is so that the "Field cannot be empty" message will be cleared.
        // Register a FocusInEvent callback to clear the text when the TextField is focused
        AddQuestionField.RegisterCallback<FocusInEvent>(evt =>
        {
            AddQuestionField.value = string.Empty; // Clear the text
        });
        AddCorrectAnswerField.RegisterCallback<FocusInEvent>(evt =>
        {
            AddCorrectAnswerField.value = string.Empty; // Clear the text
        });
        AddAltOption1Field.RegisterCallback<FocusInEvent>(evt =>
        {
            AddAltOption1Field.value = string.Empty; // Clear the text
        });
        AddAltOption2Field.RegisterCallback<FocusInEvent>(evt =>
        {
            AddAltOption2Field.value = string.Empty; // Clear the text
        });
        AddAltOption3Field.RegisterCallback<FocusInEvent>(evt =>
        {
            AddAltOption3Field.value = string.Empty; // Clear the text
        });

        //Buttons on Creating Quiz QUESTIONS Page
        SaveQuestionBtn = creatingQuizQuestionsPage.Q<Button>("SaveQuestion");
        SaveQuestionBtn.clicked += OnSaveQuestionButtonClicked;
        SaveWholeQuizBtn = creatingQuizQuestionsPage.Q<Button>("SaveWholeQuiz");
        SaveWholeQuizBtn.clicked += OnSaveWholeQuizButtonClicked;
        //Notif text
        NotificationText = creatingQuizQuestionsPage.Q<Label>("NotificationText");

        //Buttons on delete all confirmation page
        ConfirmDeleteAllQuizzesBtn = deleteAllQuizzesConfirmationPage.Q<Button>("DeleteAll");
        ConfirmDeleteAllQuizzesBtn.clicked += OnConfirmDeleteAllQuizzesButtonClicked;
        GoBackBtn = deleteAllQuizzesConfirmationPage.Q<Button>("GoBack");
        GoBackBtn.clicked += OnGoBackButtonButtonClicked;

        //Elements on play mode page
        QuizNameTextLabel = playModePage.Q<Label>("QuizNameText");
        HighScoreTextLabel = playModePage.Q<Label>("HighScoreText");
        Timer = playModePage.Q<VisualElement>("Timer");
        QuestionNumber = playModePage.Q<VisualElement>("QuestionNumber");
        QuestionTotal = playModePage.Q<VisualElement>("QuestionTotal");
        scoreIndicator = playModePage.Q<VisualElement>("ScoreIndicator");
        //pause button 
        PauseBtn = playModePage.Q<Button>("Pause");
        PauseBtn.clicked += OnPauseButtonClicked;
        //question text label
        QuestionTextLabel = playModePage.Q<Label>("QuestionTextPlayMode");
        //option buttons
        Option1Btn = playModePage.Q<Button>("Option1");
        Option2Btn = playModePage.Q<Button>("Option2");
        Option3Btn = playModePage.Q<Button>("Option3");
        Option4Btn = playModePage.Q<Button>("Option4");
        OptionButtons = new() { Option1Btn , Option2Btn , Option3Btn , Option4Btn };

        //DEFINES LOGIC FOR ALL OF THE OPTION BUTTONS i.e. what happens when they are selected while the quiz is being played
        foreach (var optionButton in OptionButtons)
        {
            optionButton.clicked += () =>
            {
                if (!isAnswering) return;
                isAnswering = false; // Prevent multiple selections
                //Check whether the selected option is correct
                string correctOption = currentQuestion.options.Find(o => o.isCorrect).optionText;

                //verify button textures are not null at runtime
                Debug.LogWarning($"GreenButtonTexture: {GreenButtonTexture}, RedButtonTexture: {RedButtonTexture}");

                if (optionButton.text == correctOption)
                {
                    //Debug.LogWarning("Correct Answer Selected!");
                    // play correct answer sound
                    audioManager.PlaySFX("Correct Answer");
                    // play flip animation and flip sound
                    runnerAnimator.SetTrigger("Flip");
                    
                    TotalScore++; //increment total score
                    //update the score indicator i.e. the silhouette of Dawn becomes more green
                    UpdateScoreIndicator(TotalScore);

                    //set the option colour to GREEN
                    try
                    {
                        //optionButton.style.backgroundImage = null;
                        //optionButton.style.backgroundImage = new StyleBackground(GreenButtonTexture);
                        //Debug.LogWarning($"Style set to: {optionButton.style.backgroundImage}");

                        //TEMPORARILY CHANGE THE TEXT COLOUR INSTEAD
                        optionButton.style.color = Color.green;
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning("Could not update button to green");
                        Debug.LogWarning($"Exception: {e}");
                    }
                }
                else
                {
                    //Debug.LogWarning("Wrong Answer Selected!");
                    // play wrong answer sound
                    audioManager.PlaySFX("Wrong Answer");
                    //play stumble animation and stumble sound
                    runnerAnimator.SetTrigger("Stumble");

                    //set the option colour to RED
                    try
                    {
                        //optionButton.style.backgroundImage = null;
                        //optionButton.style.backgroundImage = new StyleBackground(RedButtonTexture);
                        //Debug.LogWarning($"Style set to: {optionButton.style.backgroundImage}");

                        //TEMPORARILY CHANGE THE TEXT COLOUR INSTEAD
                        optionButton.style.color = Color.red;
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning("Could not update button to red");
                        Debug.LogWarning($"Exception: {e}");
                    }
                }
            };
        }

        //Buttons on pause menu
        ResumeBtn = pauseModePage.Q<Button>("Resume");
        ResumeBtn.clicked += OnResumeButtonClicked;
        RestartBtn = pauseModePage.Q<Button>("Restart");
        RestartBtn.clicked += OnRestartButtonClicked;
        //ReturnToMainMenuBtn = Root.Q<Button>("ReturnToMainMenu");
        //ReturnToMainMenuBtn.clicked += OnReturnToMainMenuButtonClicked;

        //quiz completed page
        FinalScoreIndicator = quizCompletedPage.Q<VisualElement>("FinalScoreIndicator");
        TotalScoreLabel = quizCompletedPage.Q<Label>("TotalScoreMessage");
        HighScoreLabel = quizCompletedPage.Q<Label>("HighScoreMessage");
        TryAgainBtn = quizCompletedPage.Q<Button>("TryAgain");
        TryAgainBtn.clicked += OnTryAgainButtonClicked;
        DoAnotherQuizBtn = quizCompletedPage.Q<Button>("DoAnotherQuiz");
        DoAnotherQuizBtn.clicked += OnDoAnotherQuizButtonClicked;
    }

    /// <summary>
    /// Methods for handling button click events.
    /// </summary>

    private void OnBackButtonClicked()
    {
        if (activePage == creatingQuizNamePage || activePage == creatingQuizQuestionsPage)
        {
            if (activePage == creatingQuizQuestionsPage)
            { 
                //remove it from the scroll view
                string quizNameToRemove = quizManager.myQuizzesData.quizzes.Last().quizName;
                Button buttonToRemove = ScrollViewMyQuizzes.Q<Button>(quizNameToRemove);
                if (buttonToRemove != null)
                { 
                    ScrollViewMyQuizzes.Remove(buttonToRemove);
                    Debug.LogWarning($"Removed {buttonToRemove}");
                }

                //delete the latest quiz
                quizManager.DeleteQuizByIndex(quizManager.myQuizzesData.quizzes.Count-1);
            }
            //clear the quiz name label and value on creatingQuizNamePage
            InputQuizNameField.label = string.Empty;
            InputQuizNameField.value = string.Empty;
            //clear the fields and notification text on creatingQuizQuestionsPage
            AddQuestionField.value = string.Empty;
            AddCorrectAnswerField.value = string.Empty;
            AddAltOption1Field.value = string.Empty;
            AddAltOption2Field.value = string.Empty;
            AddAltOption3Field.value = string.Empty;
            ShowNotificationText(new StyleColor(Color.black), string.Empty);
        }
        activeQuiz = null;
        NavigateFromTo(activePage,quizSelectionPage);
    }

    //COMMON METHOD FOR PAGE NAVIGATION
    private void NavigateFromTo(VisualElement fromPage, VisualElement toPage)
    {
        fromPage.style.display = DisplayStyle.None;
        toPage.style.display = DisplayStyle.Flex;
        activePage = toPage;
    }

    //MAIN MENU PAGE
    private void OnLetsGoButtonClicked()
    {
        NavigateFromTo(mainMenuPage, quizSelectionPage);

        //temporarily make this deactivate the Root
        //gameObject.SetActive(false);
    }
    private void OnSettingsButtonClicked()
    {
        NavigateFromTo(mainMenuPage, settingsPage);
    }
    private void OnQuitButtonClicked()
    {
        //save all quiz data before quitting
        quizManager.SaveQuizzes();
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

    //Settings Page Functions
    private void OnResetHighScoresButtonClicked()
    {
        ResetHighScoresConfirmation.style.display = DisplayStyle.Flex;
    }

    private void OnResetHighScoresNoButtonClicked()
    {
        ResetHighScoresConfirmation.style.display = DisplayStyle.None;
    }

    private void OnResetHighScoresYesButtonClicked()
    {
        //popular quizzes
        foreach (var quiz in quizManager.myQuizzesData.popularQuizzes)
        {
            quiz.HighScore = -1; //this is the default to indicate no high score
        }
        //my quizzes
        foreach (var quiz in quizManager.myQuizzesData.quizzes)
        {
            quiz.HighScore = -1; //this is the default to indicate no high score
        }
        quizManager.SaveQuizzes();

        ResetHighScoresConfirmation.style.display = DisplayStyle.None;
        StartCoroutine(ShowHighScoreResetCompleteMessage());
    }

    private IEnumerator ShowHighScoreResetCompleteMessage()
    {
        //show confirmation message for 2 seconds
        ResetCompleteTextLabel.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(2f);
        ResetCompleteTextLabel.style.display = DisplayStyle.None;
    }

    //QUIZ SELECTION Page Functions
    private void OnCreateQuizButtonClicked()
    {
        NavigateFromTo(quizSelectionPage, creatingQuizNamePage);
    }
    private void OnDeleteAllQuizzesButtonClicked()
    {
        NavigateFromTo(quizSelectionPage, deleteAllQuizzesConfirmationPage);
    }
    private void OnMentalSumsButtonClicked()
    {
        //set active quiz to Mental Sums
        List<Quiz> popularQuizzes = quizManager.myQuizzesData.popularQuizzes;
        activeQuiz = quizManager.GetQuizByName("Mental Sums", popularQuizzes);

        //disable the edit and delete buttons on the quiz options page since mental sums quiz is for demo and should be immutable
        EditQuizBtn.SetEnabled(false);
        DeleteQuizBtn.SetEnabled(false);

        //set quiz name label to mental sums
        TitleQuizNameLabel.text = activeQuiz.quizName;

        NavigateFromTo(activePage, quizOptionsPage);
    }

    //QUIZ OPTIONS Page Functions
    private void OnPlayQuizButtonClicked()
    {
        StartCoroutine(PlayQuiz(activeQuiz.quizName));

        //to account for the case where it's mental sums
        EditQuizBtn.SetEnabled(true);
        DeleteQuizBtn.SetEnabled(true);
    }
    private void OnEditQuizButtonClicked()
    {
        
    }
    private void OnDeleteQuizButtonClicked()
    {
        //navigate to Delete Quiz Confirmation Page
        NavigateFromTo(activePage, deleteQuizConfirmationPage);
    }

    private void OnConfirmDeleteQuizButtonClicked()
    {
        string nameOfButtonToDelete = activeQuiz.quizName.Replace(" ", "");
        //remove that quiz's button from the quiz selection page
        Button buttonToRemove = ScrollViewMyQuizzes.Q<Button>(nameOfButtonToDelete);
        ScrollViewMyQuizzes.Remove(buttonToRemove);
        //delete the quiz
        quizManager.DeleteQuizByName(activeQuiz.quizName);
        activeQuiz = null;
        quizManager.SaveQuizzes();
        //reset UI quiz name on quiz options page
        TitleQuizNameLabel.text = "Quiz Name Here";
        StartCoroutine(ShowQuizDeletedMessage());
    }

    private void OnDeleteQuizGoBackButtonClicked()
    {
        //hide the Delete Quiz Confirmation Page, show quiz options page
        NavigateFromTo(activePage, quizOptionsPage);
    }

    private IEnumerator ShowQuizDeletedMessage()
    {
        NavigateFromTo(activePage, quizDeletedPage);
        yield return new WaitForSeconds(2f);
        //navigate back to the quiz selection page
        NavigateFromTo(activePage, quizSelectionPage);
    }

    //CREATING QUIZ NAME Page Functions
    private void OnSaveQuizNameButtonClicked()
    {
        if (quizManager.AddQuiz(InputQuizNameField, InputQuizNameField.value))
        {
            //clear the quiz name label and value
            InputQuizNameField.label = string.Empty;
            InputQuizNameField.value = string.Empty;
            //GO TO THE NEXT PAGE TO CREATE QUIZ QUESTIONS
            NavigateFromTo(creatingQuizNamePage, creatingQuizQuestionsPage);
        };
    }

    public void CreateButtonInMyQuizzes(string name)
    {
        Button newQuizButton = new Button();
        newQuizButton.text = name;
        //newQuizButton.name = name;
        newQuizButton.name = name.Replace(" ", ""); // Removes spaces
        //newQuizButton.AddToClassList("yellow-button");
        newQuizButton.AddToClassList("myquizzes-button"); //sets max width
        newQuizButton.clicked += () => {
            //set active quiz to the one whose button was pressed
            List<Quiz> myQuizzes = quizManager.myQuizzesData.quizzes;
            activeQuiz = quizManager.GetQuizByName(newQuizButton.text, myQuizzes);
            //update the quiz name title on quiz options page
            TitleQuizNameLabel.text = name;
            //enable edit and delete buttons
            EditQuizBtn.SetEnabled(true);
            DeleteQuizBtn.SetEnabled(true);
            //navigate to quiz options page
            NavigateFromTo(activePage, quizOptionsPage);
        };
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
            if (string.IsNullOrEmpty(field.value) || field.value == "Field cannot be empty.")
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
    public void ShowNotificationText(StyleColor style, string notification)
    {
        NotificationText.style.color = style;
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
            ShowNotificationText(new StyleColor(Color.red),"Must have at least 1 question");
        }
    }
    private IEnumerator SaveAndWaitBeforeGoingToMyQuizzes()
    { 
        quizManager.SaveQuizzes();
        ShowNotificationText(new StyleColor(Color.green),"Quiz Saved. Returning to quiz selection");
        Quiz latestQuiz = quizManager.myQuizzesData.quizzes.Last();
        CreateButtonInMyQuizzes(latestQuiz.quizName);
        //quizManager.LoadQuizzes();
        yield return new WaitForSeconds(2f);
        //navigate to quiz selection page
        NavigateFromTo(creatingQuizQuestionsPage, quizSelectionPage);
    }

    //CONFIRM DELETE ALL PAGE
    private void OnConfirmDeleteAllQuizzesButtonClicked()
    { 
        ClearMyQuizzesScrollView();
        quizManager.myQuizzesData.quizzes.Clear();
        //delete from persistent storage as well
        quizManager.SaveQuizzes();
        //navigate to the confirmation message page and display for 2 seconds before going back to quiz selection
        StartCoroutine(ShowAllDeletedMessage());
    }

    public void ClearMyQuizzesScrollView()
    {
        //Find all buttons in ScrollView and remove them
        var Buttons = ScrollViewMyQuizzes.Query<Button>().ToList();
        foreach (var button in Buttons)
        {
            ScrollViewMyQuizzes.Remove(button);
            Debug.LogWarning($"button removed: {button.text}");
        }
    }

    private IEnumerator ShowAllDeletedMessage()
    {
        NavigateFromTo(activePage, allQuizzesDeletedPage);
        yield return new WaitForSeconds(2f);
        NavigateFromTo(activePage, quizSelectionPage);
    }

    private void OnGoBackButtonButtonClicked()
    {
        NavigateFromTo(activePage, quizSelectionPage);
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
        StartCoroutine(EndQuizAndNavigateToPage(playModePage));
    }
    //private void OnReturnToMainMenuButtonClicked()
    //{ 
    //same as home buttons
    //}

    private IEnumerator PlayQuiz(string _quizName)
    {
        //change to play mode page
        NavigateFromTo(activePage, playModePage);
        //switch from main menu bgm to quiz bgm
        audioManager.PlayMusic("Quiz bgm");
        //set active quiz
        if (_quizName == "Mental Sums")
        {
            //get reference to mental sums quiz
            List<Quiz> popularQuizzes = quizManager.myQuizzesData.popularQuizzes;
            Quiz MentalSumsQuiz = quizManager.GetQuizByName("Mental Sums", popularQuizzes);
            activeQuiz = MentalSumsQuiz;
        }
        else
        {
            //search for quiz from My Quizzes and set the active quiz
            List<Quiz> myQuizzes = quizManager.myQuizzesData.quizzes;
            activeQuiz = quizManager.GetQuizByName(_quizName, myQuizzes);
        }
        Debug.Log("Updating UI and starting quiz!");
        //update QuizNameText and HighScoreText
        QuizNameTextLabel.text = $"{activeQuiz.quizName}";
        HighScoreTextLabel.text = (activeQuiz.HighScore == -1) ? "-" :$"{activeQuiz.HighScore}";
        //set quiz total number of questions
        UpdateQuestionTotal(activeQuiz.questions.Count);
        for(int i = 0;i<activeQuiz.questions.Count;i++)
        {
            currentQuestion = activeQuiz.questions[i];
            
            // Update UI with question and options
            UpdateQuestionUI(currentQuestion, i+1);

            // Wait for player's answer or timeout, and update UI countdown timer each second
            isAnswering = true;
            yield return StartCoroutine(HandleTimerAndAnswerTimeout(10));
            if (activeQuiz == null) break;
        }

        if (activeQuiz != null)
        { 
            StartCoroutine(ShowQuizComplete(activeQuiz));
        } 
    }

    private IEnumerator EndQuizAndNavigateToPage(VisualElement pageToNavigateTo)
    {
        yield return StartCoroutine(EndQuizAndResetUI());
        //navigate to specified page
        NavigateFromTo(activePage, pageToNavigateTo);

        if (pageToNavigateTo == playModePage)
        {
            yield return StartCoroutine(PlayQuiz(previousQuiz.quizName));
        }
        else
        {
            //switch bgm back to main menu bgm
            audioManager.PlayMusic("Main Menu bgm");
        }
    }

    private IEnumerator EndQuizAndResetUI()
    {
        previousQuiz = activeQuiz;
        activeQuiz = null;
        //RESET UI
        //Reset Question
        UpdateQuestionUI(null,-1);
        //Reset button colors for the next question
        foreach (var optionButton in OptionButtons)
        {
            //optionButton.style.backgroundImage = new StyleBackground(YellowButtonTexture);

            //FOR NOW, RESET THE BUTTON TEXT TO BLACK AND EMPTY
            optionButton.style.color = Color.black;
            optionButton.text = string.Empty;
        }
        //Reset Timer
        UpdateTimerUI(10);
        //Reset labels, total score, score indicators
        QuizNameTextLabel.text = string.Empty;
        HighScoreLabel.text = string.Empty;
        TotalScoreLabel.text = string.Empty;
        HighScoreLabel.text = string.Empty;
        TotalScore = 0;
        scoreIndicator.style.backgroundImage = new StyleBackground(_0Percent);
        FinalScoreIndicator.style.backgroundImage = new StyleBackground(_0Percent);

        Debug.Log("Finished Resetting Quiz UI!");
        yield break;
    }

    private IEnumerator ShowQuizComplete(Quiz completedQuiz)
    {
        // Quiz complete
        Debug.Log($"{activeQuiz.quizName} Quiz Completed!");
        // Update High Score if needed
        if (TotalScore > activeQuiz.HighScore)
        {
            activeQuiz.HighScore = TotalScore;
        }

        //Update the text label in quiz completed page to show total score and high score
        TotalScoreLabel.text = $"You scored {TotalScore}/{completedQuiz.questions.Count} on '{completedQuiz.quizName}'.";
        HighScoreLabel.text = $"High Score: {completedQuiz.HighScore}";
        //Update the final score indicator to reflect the score
        FinalScoreIndicator.style.backgroundImage = new StyleBackground(scoreIndicator.style.backgroundImage.value);

        //navigate to quiz completed page
        NavigateFromTo(activePage, quizCompletedPage);

        //save quizzes so that highscores are persistent
        quizManager.SaveQuizzes();
        yield break;
    }

    void UpdateQuestionUI(Question qn, int currentNumber)
    {
        if (qn == null)
        {
            QuestionTextLabel.text = string.Empty; //Reset qn
        }
        else
        {
            //Update Question Number and Question Total
            UpdateQuestionNumber(currentNumber);

            int nextIndex;
            //Update Question Text
            QuestionTextLabel.text = qn.questionText;
            //Randomise order of options so that it's different each time
            System.Random rnd = new();
            List<string>myOptions = new(){qn.options[0].optionText, qn.options[1].optionText, qn.options[2].optionText, qn.options[3].optionText};
            //Update Button Texts
            foreach (var button in OptionButtons)
            {
                button.style.color = Color.black;
                nextIndex = rnd.Next(myOptions.Count);
                button.text = myOptions[nextIndex];
                myOptions.RemoveAt(nextIndex);
            }
        }
    }

    private void UpdateQuestionNumber(int currentNumber)
    {
        switch (currentNumber)
        {
            case 1:
                QuestionNumber.style.backgroundImage = new StyleBackground(_Q1);
                break;
            case 2:
                QuestionNumber.style.backgroundImage = new StyleBackground(_Q2);
                break;
            case 3:
                QuestionNumber.style.backgroundImage = new StyleBackground(_Q3);
                break;
            case 4:
                QuestionNumber.style.backgroundImage = new StyleBackground(_Q4);
                break;
            case 5:
                QuestionNumber.style.backgroundImage = new StyleBackground(_Q5);
                break;
            case 6:
                QuestionNumber.style.backgroundImage = new StyleBackground(_Q6);
                break;
            case 7:
                QuestionNumber.style.backgroundImage = new StyleBackground(_Q7);
                break;
            case 8:
                QuestionNumber.style.backgroundImage = new StyleBackground(_Q8);
                break;
            case 9:
                QuestionNumber.style.backgroundImage = new StyleBackground(_Q9);
                break;
            case 10:
                QuestionNumber.style.backgroundImage = new StyleBackground(_Q10);
                break;
        }
    }

    private void UpdateQuestionTotal(int total)
    {
        switch (total)
        { 
            case 1:
                QuestionTotal.style.backgroundImage = new StyleBackground(_Total1);
                break;
            case 2:
                QuestionTotal.style.backgroundImage = new StyleBackground(_Total2);
                break;
            case 3:
                QuestionTotal.style.backgroundImage = new StyleBackground(_Total3);
                break;
            case 4:
                QuestionTotal.style.backgroundImage = new StyleBackground(_Total4);
                break;
            case 5:
                QuestionTotal.style.backgroundImage = new StyleBackground(_Total5);
                break;
            case 6:
                QuestionTotal.style.backgroundImage = new StyleBackground(_Total6);
                break;
            case 7:
                QuestionTotal.style.backgroundImage = new StyleBackground(_Total7);
                break;
            case 8:
                QuestionTotal.style.backgroundImage = new StyleBackground(_Total8);
                break;
            case 9:
                QuestionTotal.style.backgroundImage = new StyleBackground(_Total9);
                break;
            case 10:
                QuestionTotal.style.backgroundImage = new StyleBackground(_Total10);
                break;
        }
    }

        private void UpdateScoreIndicator(int score)
    {
        int percentToIndicate = (int)(100*score / activeQuiz.questions.Count); //truncated number
        //Debug.Log($"score: {score}");
        //Debug.Log($"total questions: {activeQuiz.questions.Count}");
        //Debug.Log($"{percentToIndicate} percent on indicator");
        switch (percentToIndicate)
        {
            case int i when i>=0 && i<10:
                scoreIndicator.style.backgroundImage = new StyleBackground(_0Percent);
                break;
            case int i when i >= 10 && i < 20:
                scoreIndicator.style.backgroundImage = new StyleBackground(_10Percent);
                break;
            case int i when i >= 20 && i < 30:
                scoreIndicator.style.backgroundImage = new StyleBackground(_20Percent);
                break;
            case int i when i >= 30 && i < 40:
                scoreIndicator.style.backgroundImage = new StyleBackground(_30Percent);
                break;
            case int i when i >= 40 && i < 50:
                scoreIndicator.style.backgroundImage = new StyleBackground(_40Percent);
                break;
            case int i when i >= 50 && i < 60:
                scoreIndicator.style.backgroundImage = new StyleBackground(_50Percent);
                break;
            case int i when i >= 60 && i < 70:
                scoreIndicator.style.backgroundImage = new StyleBackground(_60Percent);
                break;
            case int i when i >= 70 && i < 80:
                scoreIndicator.style.backgroundImage = new StyleBackground(_70Percent);
                break;
            case int i when i >= 80 && i < 90:
                scoreIndicator.style.backgroundImage = new StyleBackground(_80Percent);
                break;
            case int i when i >= 90 && i < 100:
                scoreIndicator.style.backgroundImage = new StyleBackground(_90Percent);
                break;
            case 100:
                scoreIndicator.style.backgroundImage = new StyleBackground(_100Percent);
                break;
        }
    }


    IEnumerator HandleTimerAndAnswerTimeout(float timeout)
    {
        int lastSecond = (int) Math.Ceiling(timeout);
        float elapsed = 0;
        UpdateTimerUI(10); //always start with 10
        while (elapsed < timeout && isAnswering && activeQuiz!=null)
        {
            //Calculate remaining seconds and update UI if needed
            int remainingSeconds = (int) Math.Ceiling(timeout - elapsed);
            if (remainingSeconds < lastSecond)
            {
                //update UI with remainingSeconds
                UpdateTimerUI(remainingSeconds);
                lastSecond = remainingSeconds;
            }
            elapsed += Time.deltaTime;
            yield return null; // Wait for next frame
        }
        if (activeQuiz == null)
        {
            yield break;
        }
        else if (isAnswering)
        {
            // Timeout: No answer selected
            Debug.Log("Time's up! No answer selected.");
            isAnswering = false;
            yield return StartCoroutine(ShowTimesUpScreen());
        }
        else
        {
            //Debug.LogWarning("Question answered, waiting for animation");
            yield return StartCoroutine(WaitForAnimationToComplete());
            if (activeQuiz == null) //e.g. if restart button was clicked mid-animation
            {
                //set the animation state to "Running" immediately
                runnerAnimator.Play("Running");
                ResumeEnvMovement();
            }
        }
    }

    private void UpdateTimerUI(int seconds)
    {
        //Debug.LogWarning($"{seconds} seconds remaining, updating timer UI...");
        switch(seconds)
        {
            case 1:
                audioManager.PlaySFX("Clock Tick");
                Timer.style.backgroundImage = new StyleBackground(_1seconds);
                break;
            case 2:
                audioManager.PlaySFX("Clock Tick");
                Timer.style.backgroundImage = new StyleBackground(_2seconds);
                break;
            case 3:
                audioManager.PlaySFX("Clock Tick");
                Timer.style.backgroundImage = new StyleBackground(_3seconds);
                break;
            case 4:
                Timer.style.backgroundImage = new StyleBackground(_4seconds);
                break;
            case 5:
                Timer.style.backgroundImage = new StyleBackground(_5seconds);
                break;
            case 6:
                Timer.style.backgroundImage = new StyleBackground(_6seconds);
                break;
            case 7:
                Timer.style.backgroundImage = new StyleBackground(_7seconds);
                break;
            case 8:
                Timer.style.backgroundImage = new StyleBackground(_8seconds);
                break;
            case 9:
                Timer.style.backgroundImage = new StyleBackground(_9seconds);
                break;
            case 10:
                Timer.style.backgroundImage = new StyleBackground(_10seconds);
                break;
        }
    }

    IEnumerator ShowTimesUpScreen()
    {
        NavigateFromTo(activePage, timesUpPage);
        yield return StartCoroutine(PlayTimesUpSound());
        NavigateFromTo(activePage, playModePage);
    }

    IEnumerator PlayTimesUpSound()
    {
        audioManager.PlaySFX("Any Slower");
        while (audioManager.sfxSource.isPlaying)
        { 
            yield return null;
        }
    }

    IEnumerator WaitForAnimationToComplete()
    {
        //all the checks for "activeQuiz" are to account for when the restart button is pressed mid-animation
        
        int runningStateHash = Animator.StringToHash("Running");
        int stumbleStateHash = Animator.StringToHash("Stumble Shortened");
        AnimatorStateInfo animationStateInfo;

        // Wait for the animation to transition out of the "Running" state, since it may not be instantaneous
        do
        {
            if (activeQuiz == null) yield break;
            animationStateInfo = runnerAnimator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        } while (animationStateInfo.shortNameHash == runningStateHash);

        //check if the animation state is "Stumble Shortened", and if so then pause the env movement
        animationStateInfo = runnerAnimator.GetCurrentAnimatorStateInfo(0);
        if (activeQuiz != null && animationStateInfo.shortNameHash == stumbleStateHash)
        {
            PauseEnvMovement();
        }

        // Wait until the animation finishes and transitions back to "Running"
        do
        {
            if (activeQuiz == null) yield break;
            animationStateInfo = runnerAnimator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        } while (animationStateInfo.shortNameHash != runningStateHash);

        if (activeQuiz != null)
        { 
            ResumeEnvMovement();
        }
    }

    private void PauseEnvMovement()
    {
        //Stop Environment From Moving Backwards (i.e. Road Prefabs)
        RoadPrefabs = GameObject.FindGameObjectsWithTag("RoadPrefabs");
        foreach (var prefab in RoadPrefabs)
        {
            //set the speed to 0
            prefab.GetComponent<RoadHandler>().Speed = 0;
        }
    }

    private void ResumeEnvMovement()
    {
        RoadPrefabs = GameObject.FindGameObjectsWithTag("RoadPrefabs");
        foreach (var prefab in RoadPrefabs)
        {
            //set the speed back to 4
            prefab.GetComponent<RoadHandler>().Speed = 4;
        }
    }

    //QUIZ COMPLETED PAGE
    private void OnTryAgainButtonClicked()
    {
        StartCoroutine(EndQuizAndNavigateToPage(playModePage));
    }
    private void OnDoAnotherQuizButtonClicked()
    {
        StartCoroutine(EndQuizAndNavigateToPage(quizSelectionPage));
    }
}