using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PodLauncher : MonoBehaviour {
    public GameObject podPrefab;
    public GameObject explosionPrefab;
    public Transform target;

    public abstract WeaponState PodState { get; }
    public abstract float PodValue { get; }
    public abstract string PodType { get; }

    public abstract void FirePod();

    public abstract void PodDestroyedCallBack(bool worked);
}
