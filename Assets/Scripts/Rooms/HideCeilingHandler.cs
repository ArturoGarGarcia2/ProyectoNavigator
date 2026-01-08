using UnityEngine;

public class HideCeilingHandler : MonoBehaviour
{
    [SerializeField] private GameObject ceiling;

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
        {
            ceiling.SetActive(false);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player"))
        {
            ceiling.SetActive(true);
        }
    }
}
