using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using static MyQuizzesScriptableObject;
public class MainMenuController : MonoBehaviour
{
    //reference to quiz manager script which contains the logic for handling quiz data
    private QuizManager quizManager;

    //reference to active quiz
    private Quiz activeQuiz;
    
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
    private Label QuestionTextLabel;
    private Button Option1Btn;
    private Button Option2Btn;
    private Button Option3Btn;
    private Button Option4Btn;
    private List<Button> OptionButtons;
    private Question currentQuestion;
    private Label QuizNameTextLabel;
    private Label HighScoreTextLabel;

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

    //Quiz Completed Page Total Score, High Score and Final Score Indicator
    private Label TotalScoreLabel;
    private Label HighScoreLabel;
    private VisualElement FinalScoreIndicator;
    private int TotalScore;

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
    private VisualElement quizCompletedPage;
    private VisualElement timesUpPage;

    private void Awake()
    {
        quizManager = GetComponent<QuizManager>();
        audioManager = AudioManager.instance;

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
        quizCompletedPage = Root.Q<VisualElement>("QuizCompletedPage");
        timesUpPage = Root.Q<VisualElement>("TimesUpPage");

        // Other Visual Elements
        Timer = Root.Q<VisualElement>("Timer");
        scoreIndicator = Root.Q<VisualElement>("ScoreIndicator");
        FinalScoreIndicator = Root.Q<VisualElement>("FinalScoreIndicator");

        //initialise activePage as mainMenuPage
        activePage = mainMenuPage;

        //init total score to zero
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
                    //switch bgm
                    audioManager.PlayMusic("Main Menu bgm");

                    playModePage.style.display = DisplayStyle.None;
                    //unpause the game time
                    Time.timeScale = 1;
                }
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

        //Elements on play mode page
        //QuizNameText
        QuizNameTextLabel = Root.Q<Label>("QuizNameText");
        //HighScoreText
        HighScoreTextLabel = Root.Q<Label>("HighScoreText");
        //pause button 
        PauseBtn = Root.Q<Button>("Pause");
        PauseBtn.clicked += OnPauseButtonClicked;
        //question text label
        QuestionTextLabel = Root.Q<Label>("QuestionTextPlayMode");
        //option buttons
        Option1Btn = Root.Q<Button>("Option1");
        Option2Btn = Root.Q<Button>("Option2");
        Option3Btn = Root.Q<Button>("Option3");
        Option4Btn = Root.Q<Button>("Option4");
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
                    Debug.LogWarning("Correct Answer Selected!");
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
                    Debug.LogWarning("Wrong Answer Selected!");
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
        ResumeBtn = Root.Q<Button>("Resume");
        ResumeBtn.clicked += OnResumeButtonClicked;
        RestartBtn = Root.Q<Button>("Restart");
        RestartBtn.clicked += OnRestartButtonClicked;
        //ReturnToMainMenuBtn = Root.Q<Button>("ReturnToMainMenu");
        //ReturnToMainMenuBtn.clicked += OnReturnToMainMenuButtonClicked;

        //quiz completed page
        TotalScoreLabel = Root.Q<Label>("TotalScoreMessage");
        HighScoreLabel = Root.Q<Label>("HighScoreMessage");
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
                    Debug.LogWarning($"Removed {buttonToRemove}");
                }

                //delete the latest quiz
                quizManager.DeleteQuizByIndex(quizManager.myQuizzesData.quizzes.Count-1);
            }
            //clear the quiz name label and value
            InputQuizNameField.label = string.Empty;
            InputQuizNameField.value = string.Empty;
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
        //save all quiz data before quitting
        quizManager.SaveQuizzes();
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
        StartCoroutine(PlayQuiz(MentalSumsBtn.text));
    }

    //CREATING QUIZ NAME PAGE
    private void OnSaveQuizNameButtonClicked()
    {
        //if (InputQuizNameField.value != null) // if something was typed in
        //{
        if (quizManager.AddQuiz(InputQuizNameField, InputQuizNameField.value))
        {
            // Add a new button under "My Quizzes"
            //CreateButtonInMyQuizzes(InputQuizNameField.value);

            //clear the quiz name label and value
            InputQuizNameField.label = string.Empty;
            InputQuizNameField.value = string.Empty;
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
        //TEMPORARILY DISABLE THIS FIRST UNTIL LOGIC FOR RESTARTING HAS BEEN IMPLEMENTED
        //pauseModePage.style.display = DisplayStyle.None;
        //activePage = playModePage;
        ////unpause the game time
        //Time.timeScale = 1;

        //ADD LOGIC HERE FOR STARTING FROM BEGINNING OF THE QUIZ
    }
    //private void OnReturnToMainMenuButtonClicked()
    //{ 
    //same as home buttons
    //}

    //These variables are needed for Mental Sums Quiz
    private bool isAnswering = false;

    private IEnumerator PlayQuiz(string _quizName)
    {
        //switch from main menu bgm to quiz bgm
        audioManager.PlayMusic("Quiz bgm");
        //set active quiz
        if (_quizName == "Mental Sums")
        {
            //get reference to mental sums quiz
            List<Quiz> popularQuizzes = quizManager.myQuizzesData.popularQuizzes;
            Quiz MentalSumsQuiz = quizManager.myQuizzesData.GetQuizByName("Mental Sums", popularQuizzes);
            activeQuiz = MentalSumsQuiz;
        }
        else
        {
            //search for quiz from My Quizzes and set the active quiz
            List<Quiz> myQuizzes = quizManager.myQuizzesData.quizzes;
            activeQuiz = quizManager.myQuizzesData.GetQuizByName(_quizName, myQuizzes);
        } 
        //update QuizNameText and HighScoreText
        QuizNameTextLabel.text = $"{activeQuiz.quizName}";
        HighScoreTextLabel.text = (activeQuiz.HighScore == -1) ? "-" :$"{activeQuiz.HighScore}";
        foreach (Question qn in activeQuiz.questions)
        {
            currentQuestion = qn;
            
            // Update UI with question and options
            UpdateUIWithQuestion(qn);

            // Wait for player's answer or timeout, and update UI countdown timer each second
            isAnswering = true;
            yield return StartCoroutine(HandleTimerAndAnswerTimeout(10));

            // Reset button colors for the next question
            foreach (var optionButton in OptionButtons)
            {
                //optionButton.style.backgroundImage = new StyleBackground(YellowButtonTexture);

                //FOR NOW, RESET THE BUTTON TEXT TO BLACK
                optionButton.style.color = Color.black;
                //RESET TIMER
                UpdateTimerUI(10);
            }
        }

        // Quiz complete
        Debug.Log($"{activeQuiz.quizName} Quiz Completed!");
        // Update High Score if needed
        if (TotalScore > activeQuiz.HighScore)
        {
            activeQuiz.HighScore = TotalScore;
        }

        StartCoroutine(ShowQuizCompleteThenBackToSelection(activeQuiz));
    }
    private IEnumerator ShowQuizCompleteThenBackToSelection(Quiz completedQuiz)
    {
        //Update the text label in quiz completed page to show total score and high score
        TotalScoreLabel.text = $"You scored {TotalScore}/{completedQuiz.questions.Count} on '{completedQuiz.quizName}'.";
        HighScoreLabel.text = $"High Score: {completedQuiz.HighScore}";
        //Update the final score indicator to reflect the score
        FinalScoreIndicator.style.backgroundImage = new StyleBackground(scoreIndicator.style.backgroundImage.value);

        //navigate to quiz completed page
        activePage.style.display = DisplayStyle.None; //hide play mode screen
        quizCompletedPage.style.display = DisplayStyle.Flex;
        activePage = quizCompletedPage;
        //pause for 2 seconds, then go back to quiz selection page
        yield return new WaitForSeconds(2f);
        //switch bgm back to main menu bgm
        audioManager.PlayMusic("Main Menu bgm");
        //navigate to quiz selection page
        activePage.style.display = DisplayStyle.None;
        quizSelectionPage.style.display = DisplayStyle.Flex;
        activePage = quizSelectionPage;
        //reset labels, total score, score indicators
        TotalScoreLabel.text = string.Empty;
        HighScoreLabel.text = string.Empty;
        TotalScore = 0;
        scoreIndicator.style.backgroundImage = new StyleBackground(_0Percent);
        FinalScoreIndicator.style.backgroundImage = new StyleBackground(_0Percent);
        //save quizzes so that highscores are persistent
        quizManager.SaveQuizzes();
    }

    void UpdateUIWithQuestion(Question qn)
    {
        int nextIndex;
        //Update Question Text
        QuestionTextLabel.text = qn.questionText;
        //Randomise order of options so that it's different each time
        System.Random rnd = new();
        List<string>myOptions = new(){qn.options[0].optionText, qn.options[1].optionText, qn.options[2].optionText, qn.options[3].optionText};
        //Update Button Texts
        foreach (var button in OptionButtons)
        {
            nextIndex = rnd.Next(myOptions.Count);
            button.text = myOptions[nextIndex];
            myOptions.RemoveAt(nextIndex);
        }
    }

    private void UpdateScoreIndicator(int score)
    {
        int percentToIndicate = (int)(100*score / activeQuiz.questions.Count); //truncated number
        Debug.Log($"score: {score}");
        Debug.Log($"total questions: {activeQuiz.questions.Count}");
        Debug.Log($"{percentToIndicate} percent on indicator");
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

        while (elapsed < timeout && isAnswering)
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

        if (isAnswering)
        {
            // Timeout: No answer selected
            Debug.Log("Time's up! No answer selected.");
            isAnswering = false;
            yield return StartCoroutine(ShowTimesUpScreen());
        }
        else
        {
            Debug.LogWarning("Question answered, waiting for animation");
            yield return StartCoroutine(WaitForAnimationToComplete());
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
        activePage.style.display = DisplayStyle.None;
        timesUpPage.style.display = DisplayStyle.Flex;
        activePage = timesUpPage;
        yield return StartCoroutine(PlayTimesUpSound());
        activePage.style.display = DisplayStyle.None;
        playModePage.style.display = DisplayStyle.Flex;
        activePage = playModePage;
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
        int runningStateHash = Animator.StringToHash("Running");
        int stumbleStateHash = Animator.StringToHash("Stumble Shortened");
        AnimatorStateInfo animationStateInfo;

        // Wait for the animation to transition out of the "Running" state, since it may not be instantaneous
        do
        {
            animationStateInfo = runnerAnimator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        } while (animationStateInfo.shortNameHash == runningStateHash);

        //check if the animation state is "Stumble Shortened", and if so then pause the env movement
        animationStateInfo = runnerAnimator.GetCurrentAnimatorStateInfo(0);
        if (animationStateInfo.shortNameHash == stumbleStateHash)
        {
            PauseEnvMovement();
        }

        // Wait until the animation finishes and transitions back to "Running"
        do
        {
            animationStateInfo = runnerAnimator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        } while (animationStateInfo.shortNameHash != runningStateHash);

        ResumeEnvMovement();
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
}

