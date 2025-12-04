using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "MapScene";

    private UIController uiController;

    private void Awake()
    {
        uiController = FindObjectOfType<UIController>();
    }

    private void OnEnable()
    {
        Spawner.OnLevelCompleted += HandleLevelCompleted;
    }

    private void OnDisable()
    {
        Spawner.OnLevelCompleted -= HandleLevelCompleted;
    }

    private void HandleLevelCompleted()
    {
        StartCoroutine(LevelCompleteSequence());
    }

    private IEnumerator LevelCompleteSequence()
    {
        // Show "Level Completed" UI
        uiController.ShowLevelText();

        // Wait 5 real seconds
        yield return new WaitForSecondsRealtime(5f);

        // NOW load next scene
        SceneManager.LoadScene(nextSceneName);
    }
}
