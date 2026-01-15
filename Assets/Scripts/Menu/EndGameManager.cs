using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndGameManager : MonoBehaviour
{
    [Header("Referencia al texto")]
    [SerializeField] private TMP_Text uiText;

    [Header("Referencia al botón")]
    [SerializeField] private GameObject buttonToShow;

    [Header("Ajustes de shake y typewriter")]
    [SerializeField] private float shakeMagnitude = 5f;
    [SerializeField] private float shakeStepDuration = 0.1f;
    [SerializeField] private float typeSpeed = 0.1f;
    [SerializeField] private float buttonDelay = 1f;

    private Vector3 originalPos;
    private string fullText;

    private void OnEnable()
    {
        if (uiText == null)
        {
            Debug.LogError("No se ha asignado el TMP_Text en EndGameManager");
            return;
        }

        originalPos = uiText.rectTransform.localPosition;
        fullText = uiText.text;
        uiText.text = "";

        if (buttonToShow != null)
            buttonToShow.SetActive(false);

        StartCoroutine(ContinuousShake());
        StartCoroutine(TypeTextAndShowButton());
    }

    private IEnumerator TypeTextAndShowButton()
    {
        for (int i = 0; i <= fullText.Length; i++)
        {
            uiText.text = fullText.Substring(0, i);
            yield return new WaitForSeconds(typeSpeed);
        }

        // Espera antes de mostrar el botón
        yield return new WaitForSeconds(buttonDelay);

        if (buttonToShow != null)
            buttonToShow.SetActive(true);

        yield return new WaitForSeconds(3f);
        
        SceneManager.LoadScene("MenuScene");
    }

    private IEnumerator ContinuousShake()
    {
        while (gameObject.activeInHierarchy)
        {
            Vector2 randomOffset = Random.insideUnitCircle * shakeMagnitude;

            yield return MoveText(originalPos, originalPos + new Vector3(randomOffset.x, randomOffset.y, 0f), shakeStepDuration);
            yield return MoveText(originalPos + new Vector3(randomOffset.x, randomOffset.y, 0f), originalPos, shakeStepDuration);
        }
    }

    private IEnumerator MoveText(Vector3 from, Vector3 to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;
            uiText.rectTransform.localPosition = Vector3.Lerp(from, to, lerp);
            yield return null;
        }
        uiText.rectTransform.localPosition = to;
    }
}
