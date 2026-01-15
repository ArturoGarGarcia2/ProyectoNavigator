using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class WinManager : MonoBehaviour
{
    [Header("Panel de fundido")]
    [SerializeField] private Image fadePanel;
    [SerializeField] private float fadeDuration = 2f;

    private bool hasFaded = false;

    void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player") && !hasFaded)
        {
            other.GetComponent<PlayerController>().KillFlashlight();
            StartCoroutine(FadeToBlack());
            hasFaded = true;
        }
    }

    private IEnumerator FadeToBlack()
    {
        if (fadePanel == null)
        {
            Debug.LogWarning("Fade panel no asignado");
            yield break;
        }

        fadePanel.gameObject.SetActive(true);

        Color color = fadePanel.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadePanel.color = color;
            yield return null;
        }

        color.a = 1f;
        fadePanel.color = color;

        SceneManager.LoadScene("WinScene");
    }
}
