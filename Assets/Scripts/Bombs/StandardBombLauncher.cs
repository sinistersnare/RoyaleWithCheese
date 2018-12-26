using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardBombLauncher : BombLauncher {

    public override WeaponState BombState { get { return bombState; } }

    public override float BombValue { get { return CalculateBombValue(); } }

    public override string BombType { get { return "Standard Bomb"; } }
    
    public override void FireBomb() {
        if (this.bombState == WeaponState.Ready) this.bombState = WeaponState.Firing;
    }

    private const float bombHeight = 15;
    private const float bombTravelTime = 2.5f;
    private const float bombCoolDown = 2f;

    private WeaponState bombState;
    private bool releasedBomb;
    private float timeWaited = 0f;

    private void Start() {
        this.bombState = WeaponState.Ready;
    }

    private void Update() {
        switch (this.bombState) {
        case WeaponState.Firing:
            if (!releasedBomb) {
                Vector3 startPos = this.transform.position + Vector3.up * 5;
                Vector3 endPos = this.target.position;
                GameObject bomb = Instantiate(this.bombPrefab, startPos, Quaternion.identity);
                StandardBombController controller = bomb.GetComponent<StandardBombController>();
                controller.startPos = startPos;
                controller.endPos = endPos;
                controller.height = bombHeight;
                controller.timeToDestination = bombTravelTime;
                controller.callbackObject = this;
                controller.explosionPrefab = this.explosionPrefab;
                this.releasedBomb = true;
            }
            break;
        case WeaponState.CoolDown:
            if (this.timeWaited >= bombCoolDown) {
                this.timeWaited = 0f;
                this.bombState = WeaponState.Ready;
            } else {
                this.timeWaited += Time.deltaTime;
            }
            break;
        case WeaponState.Ready:
        case WeaponState.UnEquipped:
            break;
        }

    }
    
    public override void BombDestroyedCallBack() {
        this.releasedBomb = false;
        this.bombState = WeaponState.CoolDown;
    }

    private float CalculateBombValue() {
        switch (this.bombState) {
        case WeaponState.Firing:
            return 0f;
        case WeaponState.CoolDown:
            return this.timeWaited / bombCoolDown;
        case WeaponState.Ready:
            return 1f;
        case WeaponState.UnEquipped:
            return 0f;
        default:
            throw new System.Exception("Illegal State Calculation StandardBombLauncher value: " + this.bombState);
        }
    }
}
