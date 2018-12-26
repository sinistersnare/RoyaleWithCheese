using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BombLauncher : MonoBehaviour {
    public GameObject bombPrefab;
    public GameObject explosionPrefab;
    public Transform target;

    public abstract WeaponState BombState { get; }
    public abstract float BombValue { get; }
    public abstract string BombType { get; }

    public abstract void FireBomb();

    public abstract void BombDestroyedCallBack();
}
