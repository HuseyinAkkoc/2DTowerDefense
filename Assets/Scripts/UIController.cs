using UnityEngine;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{

    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text resourcesText;


    [SerializeField] private GameObject towerPanel;
    [SerializeField] private GameObject towerCard;
    [SerializeField] private Transform cardContainer;


    [SerializeField] private TowerData[] towers;
    private List<GameObject> activeCards =new List<GameObject>();
    private Platform _currentPlatform;


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


   
    private void UpdateWaveText(int currentWave)
    {
        waveText.text = $"Wave: {currentWave + 1}";
    }


    private void UpdateLiveText(int currentLives)
    {
        livesText.text = $"Lives: {currentLives}";
    }

    private void UpdateResourcesText(int currentResources)
    {
        resourcesText.text = $"Resources: {currentResources}";
    }


    public void ShowTowerPanel()
    {
        towerPanel.SetActive(true);
        GameManager.Instance.SetTimeScale(0f);
        PopulateTowerCards();
    }

    public void HideTowerPanel()
    {
        towerPanel.SetActive(false);
        GameManager.Instance.SetTimeScale(1f);
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
        _currentPlatform.PlaceTower(towerData);
        HideTowerPanel();   
    }
}
