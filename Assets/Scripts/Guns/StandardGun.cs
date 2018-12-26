using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardGun : Gun {

	private int amountOfBullets = 3;
	private float bulletGapTime = 0.1f;
    private float gunCooldownTime = 1f;
    private float timeSinceLastShot = float.PositiveInfinity;
    private int amountFired = 0;
    private float totalTimeCooled = 0f;

    private WeaponState gunState;
    public override WeaponState GunState { get { return this.gunState; } }

    public override float GunValue { get {
            switch (this.gunState) {
            case WeaponState.Firing:
                return 1 - ((float)amountFired / amountOfBullets);
            case WeaponState.CoolDown:
                return totalTimeCooled / gunCooldownTime;
            case WeaponState.Ready:
                return 1f;
            default:
                throw new System.Exception("ILLEGAL STATE @ (Standard)GunValue: " + this.GunState);
            }
    } }

    public override string GetGunType() {
		return "Standard Gun";
    }

    public override void FireGun() {
        if (this.gunState == WeaponState.Ready) {
            this.gunState = WeaponState.Firing;
        }
    }

    private void Start() {
        this.gunState = WeaponState.Ready;
    }

    private void Update() {
        switch (this.GunState) {
        case WeaponState.Firing:
            if (this.amountFired >= this.amountOfBullets) {
                this.gunState = WeaponState.CoolDown;
                this.totalTimeCooled += Time.deltaTime;
            } else if (this.timeSinceLastShot >= bulletGapTime) {
                Vector3 pos = this.transform.parent.position;
                GameObject o = Object.Instantiate(this.bulletPrefab, pos + this.transform.parent.forward * 2 + Vector3.up, Quaternion.identity);
                if (this.target != null) {
                    o.transform.rotation = Quaternion.LookRotation(this.target.position - pos + Vector3.up);
                } else {
                    o.transform.rotation = Quaternion.LookRotation(this.transform.parent.forward);
                }
                this.timeSinceLastShot = 0;
                this.amountFired++;
            } else {
                this.timeSinceLastShot += Time.deltaTime;
            }
            break;
        case WeaponState.CoolDown:
            this.totalTimeCooled += Time.deltaTime;
            if (this.totalTimeCooled >= this.gunCooldownTime) {
                this.totalTimeCooled = 0;
                this.amountFired = 0;
                this.timeSinceLastShot = float.PositiveInfinity;
                this.gunState = WeaponState.Ready;
            }
            break;
        case WeaponState.Ready:
            break;
        }
	}
}
