using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 160f;
    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 offset;

    private bool isOn = true;

    [SerializeField] private GameObject lightVisual;

    void Update()
    {
        float input = 0f;
        if (Input.GetKey(KeyCode.A))
            input = -1f;
        else if (Input.GetKey(KeyCode.D))
            input = 1f;

        transform.Rotate(Vector3.up, input * rotationSpeed * Time.deltaTime, Space.World);
        transform.position = player.transform.position + offset;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isOn = !isOn;

            if (lightVisual != null)
                lightVisual.SetActive(isOn);
        }
    }

    public bool IsOn()
    {
        return isOn;
    }
}
