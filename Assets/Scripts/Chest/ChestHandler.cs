using UnityEngine;
using System.Collections;

public class ChestHandler : MonoBehaviour
{
    private bool canOpen = false;
    private bool isOpen = false;

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;

    public static System.Action OnAnyChestOpened;

    void Update()
    {
        if (canOpen && Input.GetKeyDown(KeyCode.E))
            OpenChest();
    }

    public void OpenChest()
    {
        if (isOpen) return;

        GetComponent<Animator>().SetBool("Open",true);
        sfxSource.volume = .2f;
        sfxSource.Play();
        StartCoroutine(FreezeChest());
    }

    private IEnumerator FreezeChest()
    {
        yield return new WaitForSeconds(3.1f);
        GetComponent<Animator>().speed = 0f;
        isOpen = true;

        OnAnyChestOpened?.Invoke();
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
            canOpen = true;
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player"))
            canOpen = false;
    }

    public bool IsOpen() => isOpen;
}
