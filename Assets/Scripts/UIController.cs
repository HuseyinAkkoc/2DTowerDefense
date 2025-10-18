using UnityEngine;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{

    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text resourcesText;
    [SerializeField] private GameObject noResourcesText;


    [SerializeField] private GameObject towerPanel;
    [SerializeField] private GameObject towerCard;
    [SerializeField] private Transform cardContainer;


    [SerializeField] private TowerData[] towers;
    private List<GameObject> activeCards =new List<GameObject>();
    private Platform _currentPlatform;


    [SerializeField] private Button speed1Button;
    [SerializeField] private Button speed2Button;
    [SerializeField] private Button speed3Button;

    [SerializeField] private Color normalButtonColor =Color.white;
    [SerializeField]private Color  selectedButtonColor= Color.red;
    [SerializeField] private Color normalTextColor = Color.black;
    [SerializeField] private Color selectedTextColor = Color.white;


    [SerializeField] private GameObject pausePanel;
    private bool _isGamePaused = false;
    [SerializeField] private GameObject gameOverPanel;

    private void OnEnable()
    {
        Spawner.OnWaveChanged += UpdateWaveText;
        GameManager.OnLivesChanged += UpdateLiveText;
        GameManager.OnResourcesChanged += UpdateResourcesText;
        Platform.OnPlatformClicked += HandlePlatformClicked;
        TowerCard.OnTowerSelected += HandleTowerSelected;
    }

    private void OnDisable()
    {
        Spawner.OnWaveChanged -= UpdateWaveText;
        GameManager.OnLivesChanged -= UpdateLiveText;
        GameManager.OnResourcesChanged -= UpdateResourcesText;
        Platform.OnPlatformClicked -= HandlePlatformClicked;
        TowerCard.OnTowerSelected -= HandleTowerSelected;


    }

    private void Start()
    {
        speed1Button.onClick.AddListener(() => SetGameSpeed(0.5f));
        speed2Button.onClick.AddListener(() => SetGameSpeed(1f));
        speed3Button.onClick.AddListener(() => SetGameSpeed(4f));

        HighlightSelectedSpeedButton(GameManager.Instance.GameSpeed);
    }



    private void Update()
    {                  ///////// only for esc to pasue/////////////////////
#if UNITY_EDITOR || UNITY_STANDALONE
        // PC or Editor – use ESC key
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();
#elif UNITY_ANDROID || UNITY_IOS
    // Mobile – use back button (Android) or a custom pause button
    if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
    {
        // Optional: detect top-corner tap as a pause gesture
        Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
        if (touchPos.y > Screen.height * 0.9f && touchPos.x > Screen.width * 0.9f)
            TogglePause();
    }

    // Android hardware back button
    if (Keyboard.current != null && Keyboard.current.backspaceKey.wasPressedThisFrame)
        TogglePause();
#endif
    }

    private void UpdateWaveText(int currentWave)
    {
        waveText.text = $"Wave: {currentWave + 1}";
    }


    private void UpdateLiveText(int currentLives)
    {
        livesText.text = $"Lives: {currentLives}";

        if(currentLives <= 0)
        {
            ShowGameOver();
        }
    }

    private void UpdateResourcesText(int currentResources)
    {
        resourcesText.text = $"Gold: {currentResources}";
    }


    public void ShowTowerPanel()
    {

        towerPanel.SetActive(true);
        Platform.towerPanelOpen = true;
        GameManager.Instance.SetTimeScale(0f);
        PopulateTowerCards();
    }

    public void HideTowerPanel()
    {
        UIButtonSound.Instance.PlayClick();
        towerPanel.SetActive(false);
        Platform.towerPanelOpen = false;
        GameManager.Instance.SetTimeScale(GameManager.Instance.GameSpeed);
    }

    private void HandlePlatformClicked(Platform platform)
    {
        _currentPlatform = platform;
        ShowTowerPanel();
    }


    private void  PopulateTowerCards()
    {
        foreach (var card in activeCards)
        {
            Destroy(card);

        }
            activeCards.Clear();

            foreach (var data in towers)
            {
                GameObject cardGameObject = Instantiate(towerCard, cardContainer);
               TowerCard card = cardGameObject.GetComponent<TowerCard>(); 
            card.Initialize(data);
            activeCards.Add(cardGameObject);
            }
        
    }



    public void HandleTowerSelected(TowerData towerData)
    {
        if(GameManager.Instance.Resources >= towerData.cost)
        {
           GameManager.Instance.SpendResources(towerData.cost);
          _currentPlatform.PlaceTower(towerData);
             
        }
        else
        {
            StartCoroutine(ShowNoResourcesMessage());
        }
       HideTowerPanel();
    }


    private IEnumerator ShowNoResourcesMessage()
    {
        noResourcesText.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        noResourcesText.SetActive(false);

    }

    private void SetGameSpeed(float timeScale)
    {
        HighlightSelectedSpeedButton(timeScale);
        GameManager.Instance.SetGameSpeed(timeScale);
    }


    private void UpdateButtonVisual(Button button, bool isSelected)
    {
        button.image.color = isSelected ? selectedButtonColor : normalButtonColor;
        TMP_Text text = button.GetComponentInChildren<TMP_Text>();
        text.color= isSelected ? selectedTextColor : normalTextColor;
    }


    private void HighlightSelectedSpeedButton( float selectedSpeed)
    {
        UpdateButtonVisual(speed1Button, selectedSpeed == 0.5f);
        UpdateButtonVisual(speed2Button, selectedSpeed == 1f);
        UpdateButtonVisual(speed3Button, selectedSpeed == 4f);
    }

    public void TogglePause()
    {

        if(towerPanel.activeSelf)
        {
            return;
        }
        if(_isGamePaused)
        {
            
            pausePanel.SetActive(false);
            _isGamePaused = false;
            GameManager.Instance.SetTimeScale(GameManager.Instance.GameSpeed);
        }
        else
        {
            UIButtonSound.Instance.PlayClick();
            pausePanel.SetActive(true);
            _isGamePaused = true;
            GameManager.Instance.SetTimeScale(0f);
        }
    }

    public void RestartLevel()
    {
        UIButtonSound.Instance.PlayClick();
        GameManager.Instance.SetTimeScale(1f);
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }


    public void QuitGame()
    {
        UIButtonSound.Instance.PlayClick();
#if UNITY_EDITOR
        // Stop play mode inside Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
    
    Application.Quit();
#elif UNITY_ANDROID || UNITY_IOS
   
    Application.Quit();
    Debug.Log("Quit command sent (Android/iOS)");
#else
    
    Application.Quit();
#endif
    }



    public void MainMenuButton()
    {
        UIButtonSound.Instance.PlayClick();
        GameManager.Instance.SetTimeScale(1f);
        SceneManager.LoadScene("MainMenu");
    }

    private void ShowGameOver()
    {
        GameManager.Instance.SetTimeScale(0f);
        gameOverPanel.SetActive(true);
    }

}
