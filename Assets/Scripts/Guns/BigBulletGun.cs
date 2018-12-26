using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBulletGun : Gun {

	private float totalTimeCooled;
	private int amountFired = 0;
	private int amountOfBullets = 4;
	private float bulletGapTime = 0.5f;
	private float timeSinceLastShot = float.PositiveInfinity;
	private float cooldownTime = 2f;

    private WeaponState gunState;
    public override WeaponState GunState { get { return this.gunState; } }

    private void Start() {
        this.gunState = WeaponState.Ready;
    }

    public override float GunValue {
        get {
            switch (this.gunState) {
            case WeaponState.Firing:
                return 1 - ((float)amountFired / amountOfBullets);
            case WeaponState.CoolDown:
                return totalTimeCooled / cooldownTime;
            case WeaponState.Ready:
                return 1f;
            default:
                throw new System.Exception("ILLEGAL STATE @ (BigBullet)GunValue: " + this.GunState);
            }
        }
    }

    public override void FireGun() {
        if (this.gunState == WeaponState.Ready) {
            this.gunState = WeaponState.Firing;
        }
	}
    

	public override string GetGunType() {
		return "Big Bullet Gun";
	}

	void Update () {
        switch (this.gunState) {
        case WeaponState.Firing:
            if (this.amountFired >= this.amountOfBullets) { // done firing, time to cool down.
                this.gunState = WeaponState.CoolDown;
                this.totalTimeCooled += Time.deltaTime;
                return;
            } else if (this.timeSinceLastShot >= bulletGapTime) { // time to fire another bullet!
                Vector3 pos = this.transform.parent.position;
                GameObject o = Object.Instantiate(this.bulletPrefab, pos + this.transform.parent.forward * 2 + Vector3.up * 2, Quaternion.identity);
                if (this.target != null)
                    o.transform.rotation = Quaternion.LookRotation(this.target.position - pos + Vector3.up);
                else
                    o.transform.rotation = Quaternion.LookRotation(this.transform.forward);
                this.timeSinceLastShot = 0;
                this.amountFired++;
            } else { // in between shots
                this.timeSinceLastShot += Time.deltaTime;
            }
            break;
        case WeaponState.CoolDown:
            this.totalTimeCooled += Time.deltaTime;
            if (this.totalTimeCooled >= this.cooldownTime) {
                this.totalTimeCooled = 0;
                this.amountFired = 0;
                this.gunState = WeaponState.Ready;
            }
            break;
        case WeaponState.Ready:
            break;
        }
        

	}
}
