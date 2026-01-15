using UnityEngine;
using TMPro;
using System.Collections;

public class ChestChecker : MonoBehaviour
{
    [SerializeField] private float spawnWaitTime = 0.2f;

    private ChestHandler[] chests;
    private EnemyBehaviour[] enemies;
    private bool alreadyTriggered = false;

    [Header("Puertas")]
    [SerializeField] private GameObject closedDoor;
    [SerializeField] private GameObject openedDoor;

    [Header("UI Misión")]
    [SerializeField] private TMP_Text missionText;
    [SerializeField] private float typeSpeed = 0.05f;
    [SerializeField] private float textDisplayTime = 2f;

    void Start()
    {
        openedDoor.SetActive(false);
        StartCoroutine(InitAfterSpawn());
    }

    IEnumerator InitAfterSpawn()
    {
        yield return new WaitForSeconds(spawnWaitTime);

        chests = GetComponentsInChildren<ChestHandler>(true);
        enemies = GetComponentsInChildren<EnemyBehaviour>(true);

        if (chests.Length == 0)
            Debug.LogWarning("No se encontraron cofres hijos", this);

        if (enemies.Length == 0)
            Debug.LogWarning("No se encontraron enemigos hijos", this);

        if (missionText != null)
            StartCoroutine(ShowMission("Misión actual:\nEncontrar los 5 cofres perdidos."));
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.P))
            OpenAllChests();

        if (alreadyTriggered || chests == null || chests.Length == 0)
            return;

        foreach (ChestHandler chest in chests)
        {
            if (!chest.IsOpen())
            {
                return;
            }
        }

        alreadyTriggered = true;
        closedDoor.SetActive(false);
        openedDoor.SetActive(true);

        if (missionText != null)
            StartCoroutine(UpdateMission());
    }

    private void OpenAllChests()
    {
        foreach (ChestHandler chest in chests)
        {
            chest.OpenChest();
        }
    }

    private IEnumerator TypewriterText(string fullText)
    {
        missionText.text = "";
        for (int i = 0; i <= fullText.Length; i++)
        {
            missionText.text = fullText.Substring(0, i);
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    private IEnumerator EraseText()
    {
        string currentText = missionText.text;
        for (int i = currentText.Length; i >= 0; i--)
        {
            missionText.text = currentText.Substring(0, i);
            yield return new WaitForSeconds(typeSpeed);
        }
        missionText.text = "";
    }

    private IEnumerator ShowMission(string text)
    {
        yield return StartCoroutine(TypewriterText(text));
        yield return new WaitForSeconds(textDisplayTime);
        yield return StartCoroutine(EraseText());
    }

    private IEnumerator UpdateMission()
    {
        yield return StartCoroutine(ShowMission("Objetivo actualizado."));

        yield return StartCoroutine(ShowMission("Nuevo objetivo:\nSalir de la cripta."));
    }
}
