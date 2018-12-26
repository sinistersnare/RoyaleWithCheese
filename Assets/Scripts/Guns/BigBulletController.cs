using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBulletController : MonoBehaviour {
    private float speed = 10f;
    private float deathTime = 10f;

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
            // Debug.Log("TOOMUCHTIME");
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
            player.TakeDamage(25);
        } else {
            Collider[] hits = Physics.OverlapSphere(collision.contacts[0].point, 4f, LayerMask.GetMask("Player"));
            foreach (Collider hit in hits) {
                Player player = hit.gameObject.GetComponent<Player>();
                player.TakeDamage(14);
            }
        }

        this.Die();
    }

    private void LateUpdate() {
        this.rb.velocity = constantVelocity;
    }
}
