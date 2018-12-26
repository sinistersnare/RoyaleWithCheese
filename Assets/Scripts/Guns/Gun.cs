using UnityEngine;

public abstract class Gun : MonoBehaviour {   
	public GameObject bulletPrefab;
	public Transform target;

    public abstract WeaponState GunState { get; }
    public abstract float GunValue { get;  }

	public abstract string GetGunType();
	public abstract void FireGun();

}
