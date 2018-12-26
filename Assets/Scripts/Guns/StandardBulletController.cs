using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardBulletController : MonoBehaviour {
    public float speed = 10f;
    public float deathTime = 3;

    private Vector3 constantVelocity;
    private Rigidbody rb;
    private float liveTime;

	private void Start() {
        this.rb = GetComponent<Rigidbody>();
        this.constantVelocity = this.transform.forward * speed;
        this.gameObject.tag = "Bullet";
	}

    private void Update() {
        this.liveTime += Time.deltaTime;
        if (this.liveTime > this.deathTime) {
            this.Die();
        }
    }

    private void Die() {
        this.gameObject.SetActive(false);
        Object.Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
            Player player = collision.collider.gameObject.GetComponent<Player>();
            player.TakeDamage(10);
        }

        this.Die();
    }

    private void LateUpdate() {
        this.rb.velocity = constantVelocity;
    }
}
