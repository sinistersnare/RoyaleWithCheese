using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StandardPodController : MonoBehaviour {
    
	public Vector3 startPos;
    public Transform target;
    public PodLauncher callbackObject;
	public float maxLiveTime = 20f;
    public GameObject explosionPrefab;

	private float timeAlive;
	private NavMeshAgent agent;

    private void Start () {
		this.agent = this.GetComponent<NavMeshAgent>();
		if (!this.agent.isOnNavMesh) {
			// TODO: error handling, try to find land manually!
            // Placed very poorly, couldnt find land.
			this.Kill(false);
		}
	}

	private void Update () {
		// use this.agent.CalculatePath(Vector3, NavMeshPathStatus) to 
        // check if a path is possible!!!!
		if (this.agent.pathStatus == NavMeshPathStatus.PathPartial) {
			// No available path
			this.Kill(false);
			return;
		}
        
		this.agent.SetDestination(target.position);
		if (!this.agent.pathPending && this.agent.remainingDistance <= this.agent.stoppingDistance) {
            this.Explode();
			this.Kill(true);
			return;
		}
		this.timeAlive += Time.deltaTime;
		if (timeAlive > this.maxLiveTime) {
            this.Explode();
			this.Kill(true);
			return;
		}
	}

    private void Explode() {
        Collider[] hits = Physics.OverlapSphere(this.transform.position, 6f, LayerMask.GetMask("Player"));
        foreach (Collider hit in hits) {
            Player player = hit.gameObject.GetComponent<Player>();
            player.TakeDamage(20);
        }
        GameObject obj = Instantiate(this.explosionPrefab, this.transform.position, Quaternion.identity);
        obj.name = "Standard Pod Explosion";
        ParticleSystem explosion = obj.GetComponent<ParticleSystem>();
        explosion.Play();
        GameObject.Destroy(obj, explosion.main.duration);
    }

	private void Kill(bool worked) {
		Object.Destroy(this.gameObject);
		this.callbackObject.PodDestroyedCallBack(worked);
	}
}
