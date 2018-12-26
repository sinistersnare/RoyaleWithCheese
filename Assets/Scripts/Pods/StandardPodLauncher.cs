using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardPodLauncher : PodLauncher {

    public override string PodType { get { return "Standard Pod"; } }
    public override WeaponState PodState { get { return this.podState; } }

    private WeaponState podState;
    private bool releasedPod;
    private float timeToWait;
    private float timeWaited;

    private const float podCoolDownTime = 2f;

    public override float PodValue { get {
        switch (this.podState) {
            case WeaponState.Ready:
                return 1f;
            case WeaponState.CoolDown:
                if (this.timeToWait == -1) return 1f;
                return this.timeWaited / this.timeToWait;
            case WeaponState.Firing:
                return 0f;
            case WeaponState.UnEquipped:
                return 0f;
            default:
                throw new System.Exception("ILLEGAL STATE @ (Standard)PodValue: " + this.podState);
        }
    } }
    
    private void Start() {
        this.podState = WeaponState.Ready;
        this.releasedPod = false;
    }

    public override void PodDestroyedCallBack(bool worked) {
        this.timeToWait = worked ? podCoolDownTime : -1f;
        this.releasedPod = false;
        this.podState = WeaponState.CoolDown;
    }

    public override void FirePod() {
        if (this.podState == WeaponState.Ready) this.podState = WeaponState.Firing;
    }

    private void Update() {
        switch (this.podState) {
        case WeaponState.Firing:
            if (!this.releasedPod) {
                Vector3 startPos = this.transform.position + this.transform.right * 3 + Vector3.up;
                GameObject pod = Instantiate(this.podPrefab, startPos, Quaternion.identity);
                StandardPodController controller = pod.GetComponent<StandardPodController>();
                controller.startPos = startPos;
                controller.target = this.target;
                controller.callbackObject = this;
                controller.explosionPrefab = this.explosionPrefab;
                this.timeWaited = 0f;
                this.releasedPod = true;
            }
            break;
        case WeaponState.CoolDown:
            if (this.timeWaited >= this.timeToWait) {
                this.podState = WeaponState.Ready;
                this.timeWaited = 0f;
            } else {
                this.timeWaited += Time.deltaTime;
            }
            break;
        case WeaponState.UnEquipped:
        case WeaponState.Ready:
            break;
        default:
            throw new System.Exception("UNHANDLED UPDATE STATE @ (Standard)PodValue: " + this.podState);
        }
    }

}
