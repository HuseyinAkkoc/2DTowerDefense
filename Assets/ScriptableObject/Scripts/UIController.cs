using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{

    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text livesText;


    

    private void OnEnable()
    {
        Spawner.OnWaveChanged += UpdateWaveText;
        GameManager.OnLivesChanged += UpdateLiveText;
    }

    private void OnDisable()
    {
        Spawner.OnWaveChanged -= UpdateWaveText;
        GameManager.OnLivesChanged -= UpdateLiveText;
    }


   
    private void UpdateWaveText(int currentWave)
    {
        waveText.text = $"Wave: {currentWave + 1}";
    }


    private void UpdateLiveText(int currentLives)
    {
        livesText.text = $"Lives: {currentLives}";
    }

}
