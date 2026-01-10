using UnityEngine;
using System.Collections;

public class ChestHandler : MonoBehaviour
{
    private bool canOpen = false;

    void Update()
    {
        if (canOpen && Input.GetKey(KeyCode.E)){
            Debug.Log("Me abro");
            OpenChest();
        }
    }

    public void OpenChest()
    {
        GetComponent<Animator>().SetBool("Open",true);
        Debug.Log("Muestro la apertura");
        StartCoroutine(FreezeChest());
    }

    private IEnumerator FreezeChest()
    {
        yield return new WaitForSeconds(3.2f);
        GetComponent<Animator>().speed = 0f;
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jugador delante");
            canOpen = true;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")){
            Debug.Log("El jugador se ha ido");
            canOpen = false;
        }
    }
}
