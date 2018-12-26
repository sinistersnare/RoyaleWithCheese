using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardBombController : MonoBehaviour {

	public Vector3 startPos;
	public Vector3 endPos;
	public float height = -1;
	public float timeToDestination = -1;
	public BombLauncher callbackObject;
    public GameObject explosionPrefab;

	private float timeSinceStart = 0;
 	
	void Update() {
		this.transform.position = SampleParabola(this.timeSinceStart);
		this.timeSinceStart += Time.deltaTime / timeToDestination;
        if (timeSinceStart > 1) {
			this.Kill();
        }
	}

	private Vector3 SampleParabola(float t) {
        float parabolicT = t * 2 - 1;
      
		Vector3 travelDirection = this.endPos - this.startPos;
		Vector3 levelDirecteion = this.endPos - new Vector3(this.startPos.x, this.endPos.y, this.startPos.z);
        Vector3 right = Vector3.Cross(travelDirection, levelDirecteion);
        Vector3 up = Vector3.Cross(right, travelDirection);
		if (this.endPos.y > this.startPos.y) up = -up;
		Vector3 result = this.startPos + t * travelDirection;
		result += ((-parabolicT * parabolicT + 1) * this.height) * up.normalized;
        return result;   
    }

	private void OnCollisionEnter(Collision collision) {
        // TODO: cool explosion effect.
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
            Player player = collision.collider.gameObject.GetComponent<Player>();
            player.TakeDamage(40);
        } else {
            Collider[] hits = Physics.OverlapSphere(collision.contacts[0].point, 6f, LayerMask.GetMask("Player"));
            foreach (Collider hit in hits) {
                Player player = hit.gameObject.GetComponent<Player>();
                player.TakeDamage(20);
            }
        }
        Vector3 pos = collision.collider.transform.position;
        GameObject obj = Instantiate(this.explosionPrefab, pos, Quaternion.identity);
        obj.name = "Standard Bomb Explosion";
        ParticleSystem explosion = obj.GetComponent<ParticleSystem>();
        explosion.Play();
        GameObject.Destroy(obj, explosion.main.duration);
        
        this.Kill();
	}

	private void Kill() {
		this.callbackObject.BombDestroyedCallBack();
        Object.Destroy(this.gameObject);
	}
}
