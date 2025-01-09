using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomSlider : MonoBehaviour
{
    //reference to mainmenucontroller
    private MainMenuController mainMenuController;

    //reference to audiomanager
    private AudioManager audioManager;
    
    //UI elements
    private VisualElement Root;
    private Slider SFXVolumeSlider;
    private Slider MusicVolumeSlider;
    private VisualElement SFXDragContainer;
    private VisualElement SFXTracker;
    private VisualElement SFXDragger;
    private VisualElement SFXBarFill;
    private VisualElement MusicDragger;
    private VisualElement MusicBarFill;

    //for triggering callback when done interacting with slider
    //private bool isDragging = false;
    private Coroutine stopDraggingCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainMenuController = GetComponent<MainMenuController>();
        audioManager = AudioManager.instance;

        Root = GetComponent<UIDocument>().rootVisualElement;
        SFXVolumeSlider = Root.Q<Slider>("SFXVolumeSlider");
        MusicVolumeSlider = Root.Q<Slider>("MusicVolumeSlider");
        SFXDragger = SFXVolumeSlider.Q<VisualElement>("unity-dragger");
        SFXDragContainer = SFXVolumeSlider.Q<VisualElement>("unity-drag-container");
        SFXTracker = SFXVolumeSlider.Q<VisualElement>("unity-tracker");
        MusicDragger = MusicVolumeSlider.Q<VisualElement>("unity-dragger");

        //link slider to audiomanager
        SFXVolumeSlider.value = PlayerPrefs.GetFloat("sfxVolume"); //init volume
        audioManager.sfxSource.volume = PlayerPrefs.GetFloat("sfxVolume");
        MusicVolumeSlider.value = PlayerPrefs.GetFloat("musicVolume"); //init volume
        audioManager.musicSource.volume = PlayerPrefs.GetFloat("musicVolume");

        // Register a callback to update the AudioManager when each slider value changes
        MusicVolumeSlider.RegisterValueChangedCallback(evt =>
        {
            audioManager.SetMusicVolume(evt.newValue); // Update volume based on slider value
            //Debug.Log($"Music Volume set: {evt.newValue}");
            //save to playerprefs for persistence
            PlayerPrefs.SetFloat("musicVolume", evt.newValue);
        });
        SFXVolumeSlider.RegisterValueChangedCallback(evt =>
        {
            audioManager.SetSFXVolume(evt.newValue); // Update volume based on slider value
            //Debug.Log($"SFX Volume set: {evt.newValue}");
            PlayerPrefs.SetFloat("sfxVolume", evt.newValue);

            // Restart the coroutine every time the value changes
            if (stopDraggingCoroutine != null)
            {
                StopCoroutine(stopDraggingCoroutine);
            }
            stopDraggingCoroutine = StartCoroutine(HandleStopDragging());
        });

        //For SFX, play flip sfx when the pointer is released so user can hear the difference
        //NOTE: PointerUpEvents are consumed internally by the slider and not propagated to children, therefore
        //...can't directly register callback to pointer up events on SFXDragger.

        //ATTEMPT 1
        //SFXVolumeSlider.RegisterCallback<PointerUpEvent>(evt =>
        //{
        //    Debug.Log("Pointer Up Event occurred on SFXVolumeSlider!");
        //    audioManager.PlaySFX("Correct Answer");
        //});

        //ATTEMPT 2
        // Register value changed callback
        //SFXVolumeSlider.RegisterValueChangedCallback(evt =>
        //{
        //    isDragging = true; // User is dragging
        //    if (stopDraggingCoroutine != null)
        //        StopCoroutine(stopDraggingCoroutine);
        //});

        //// Monitor drag release
        //SFXVolumeSlider.RegisterCallback<PointerLeaveEvent>(evt =>
        //{
        //    if (isDragging)
        //    {
        //        stopDraggingCoroutine = StartCoroutine(HandleStopDragging());
        //    }
        //});

        //ATTEMPT 3
        //refer to SFXVolumeSlider.RegisterValueChangedCallback from isDragging = true onwards

        //Add bar to the dragger
        AddElements();
    }

    private IEnumerator HandleStopDragging()
    {
        yield return new WaitForSeconds(0.25f); // Wait briefly
        //Debug.Log("Pointer release detected after dragging!");
        audioManager.PlaySFX("Correct Answer");
    }

    void AddElements()
    {
        ////create a wrapper for hiding the bar
        //VisualElement SFXBarWrapper = new VisualElement();
        //SFXDragContainer.Add(SFXBarWrapper);
        //// Style the wrapper to clip its children
        //SFXBarWrapper.style.overflow = Overflow.Hidden;

        SFXBarFill = new VisualElement();
        SFXBarFill.name = "SFXBar";
        SFXBarFill.AddToClassList("bar");
        SFXDragger.Add(SFXBarFill);

        MusicBarFill = new VisualElement();
        MusicBarFill.name = "MusicBar";
        MusicBarFill.AddToClassList("bar");
        MusicDragger.Add(MusicBarFill);

        //SFXBarWrapper.Add(SFXBarFill);
        //SFXDragger.Add(SFXBarWrapper);

        // Use absolute positioning to prevent affecting parent's layout
        SFXBarFill.style.position = Position.Absolute;
        MusicBarFill.style.position = Position.Absolute;

        //SFXBarFill.style.width = SFXTracker.resolvedStyle.width; //resolvedStyle gives the computed layout values
        //Debug.Log($"SFX Bar Width: {SFXBarFill.style.width}");
    }
}
