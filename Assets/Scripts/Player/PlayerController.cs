using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private Vector3 destination;
    [SerializeField] private Vector3 min, max;
    [SerializeField] private Camera camera;
    [SerializeField] private Difficulty difficulty;
    
    void Start()
    {
        
    }

    public void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            if(Physics.Raycast(ray, out hit, 1000))
            {
                GetComponent<NavMeshAgent>().SetDestination(hit.point);
            }
        }
        camera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    public Difficulty GetDifficulty() => difficulty;
}
