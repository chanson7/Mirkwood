using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Collections;

public class PregameMenuHandler : MonoBehaviour
{

    //so we can test locally within the editor
#if UNITY_EDITOR
    public PlayerSession playerSession;
#endif

    [SerializeField] UIDocument uiDocument;
    VisualElement menu;
    VisualElement[] mainMenuOptions;
    Button playButton;
    Button settingsButton;
    Button quitButton;
    Label matchmakingStatusLabel;
    [SerializeField] GameObject matchmakingClient;
    private int _mainPopupIndex = -1;

    private const string POPUP_ANIMATION = "pop-animation-hide";

    private void Awake()
    {
        VisualElement rootVisualElement = uiDocument.rootVisualElement;

        //assign the UI elements
        matchmakingStatusLabel = rootVisualElement.Q<Label>("MatchmakingStatusLabel");
        menu = rootVisualElement.Q<VisualElement>("Menu");
        mainMenuOptions = rootVisualElement.Q<VisualElement>("MainNav").Children().ToArray<VisualElement>();

        playButton = rootVisualElement.Q<Button>("Play");
        settingsButton = rootVisualElement.Q<Button>("Settings");
        quitButton = rootVisualElement.Q<Button>("Quit");

        //register transition event callbacks
        menu.RegisterCallback<TransitionEndEvent>(OnMenuTransitionEnd);

        //register click events
        playButton.clicked += PlayButtonPressed;
        settingsButton.clicked += SettingsButtonPressed;
        quitButton.clicked += QuitButtonPressed;
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        menu.ToggleInClassList(POPUP_ANIMATION);
    }

    void PlayButtonPressed()
    {
        matchmakingClient.SetActive(true);
    }

    void SettingsButtonPressed()
    {
        Debug.Log("..Settings Button Pressed");
    }

    void QuitButtonPressed()
    {
        Application.Quit();
    }
    public void UpdateMatchmakingStatusLabel(string text)
    {
        matchmakingStatusLabel.visible = true;
        matchmakingStatusLabel.text = text;
    }

    private void OnMenuTransitionEnd(TransitionEndEvent endEvent)
    {
        if (!endEvent.stylePropertyNames.Contains("opacity")) { return; }

        if (_mainPopupIndex < mainMenuOptions.Length - 1)
        {
            _mainPopupIndex++;
            mainMenuOptions[_mainPopupIndex].ToggleInClassList(POPUP_ANIMATION);
        }
    }
}
