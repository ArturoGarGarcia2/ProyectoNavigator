using UnityEngine;

public class HideCeilingHandler : MonoBehaviour
{
    [SerializeField] private GameObject ceiling;
    [SerializeField] private bool showAgain;

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
        {
            ceiling.SetActive(false);
        }
    }

    void OnTriggerExit(Collider other) {
        if (showAgain && other.CompareTag("Player"))
        {
            ceiling.SetActive(true);
        }
    }
}
