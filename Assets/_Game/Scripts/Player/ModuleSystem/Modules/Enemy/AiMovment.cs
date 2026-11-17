using UnityEngine.AI;
using UnityEngine;

public class AiMovment : MonoBehaviour {
    private NavMeshAgent agent;
    //private bool isFound = false;

    [SerializeField] private Transform target;
    

    private void Start() {
        target = ScheneManager.Instance.PlayerInstance.transform; //player.transform it is enemy
        agent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate() {
        /*if (target != null && Physics.Linecast(transform.position, target.position, out RaycastHit hitInfo)) {
            if (hitInfo.transform.CompareTag("Player")) {
                //if (isFound) isFound = false;
                agent.destination = target.position;
                return;
            }
        }*/
        agent.destination = target.position;
    }
}
