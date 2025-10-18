using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{ 

    [SerializeField] private GameObject instructionPanel;

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }


    public void QuitGame()
    {
#if UNITY_EDITOR
        // Stop play mode inside the editor
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
        
        Application.Quit();
#elif UNITY_ANDROID || UNITY_IOS
        
        Application.Quit();

       
        Debug.Log("Quit command sent on mobile.");
#else
       
        Application.Quit();
#endif
    }


    public void ShowInstructionPanel()
    {
        instructionPanel.SetActive(true);
    }


    public void HideInstructionPanel()
    {
        instructionPanel.SetActive(false);
    }

}
