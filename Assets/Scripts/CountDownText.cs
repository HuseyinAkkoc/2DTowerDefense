using UnityEngine;
using TMPro;
using System.Collections;
public class CountDownText : MonoBehaviour
{
    public TextMeshProUGUI countdownText;

    private void Start()
    {
        // PAUSE THE GAME AT THE BEGINNING
        GameManager.Instance.SetGameSpeed(0f);

        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        countdownText.gameObject.SetActive(true);

        countdownText.text = "3";
        yield return new WaitForSecondsRealtime(1f);

        countdownText.text = "2";
        yield return new WaitForSecondsRealtime(1f);

        countdownText.text = "1";
        yield return new WaitForSecondsRealtime(1f);

        countdownText.text = "START!";
        yield return new WaitForSecondsRealtime(1f);

        countdownText.gameObject.SetActive(false);

        // UNPAUSE THE GAME WHEN COUNTDOWN FINISHES
        GameManager.Instance.SetGameSpeed(1f);
    }
}
