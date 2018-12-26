using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public abstract class Player : MonoBehaviour {

    public GameObject cameraPrefab;
    public abstract void AttachCamera();

    public abstract XboxController WhichXboxController { get; }

    public XboxAxis GunButton { get { return XboxAxis.RightTrigger; } }
    public XboxButton BombButton { get { return XboxButton.RightBumper; } }
    public XboxButton PodButton { get { return XboxButton.LeftBumper; } }
    public XboxButton PickUpButton { get { return XboxButton.A; } }
    public XboxButton JumpButton { get { return XboxButton.X; } }
    public XboxAxis MovePlayerHorizontalAxis { get { return XboxAxis.LeftStickX; } }
    public XboxAxis MovePlayerVerticalAxis { get { return XboxAxis.LeftStickY; } }
    public XboxAxis MoveCameraHorizontalAxis { get { return XboxAxis.RightStickX; } }
    public XboxAxis MoveCameraVerticalAxis { get { return XboxAxis.RightStickY; } }
    public abstract Transform Target { get; }

    public int Score { get { return this.score; } }

    public float Health { get { return ((float) currentHealth) / ((float)startingHealth); } }

    public WeaponState GunState { get { if (this.currentGun) return this.currentGun.GunState; else return WeaponState.UnEquipped; } }
    public float GunValue { get { if (this.currentGun) return this.currentGun.GunValue; else return 0f; } }

    public WeaponState PodState { get { if (this.currentPodLauncher) return this.currentPodLauncher.PodState; else return WeaponState.UnEquipped; } }
    public float PodValue { get { if (this.currentPodLauncher) return this.currentPodLauncher.PodValue; else return 0f; } }

    public WeaponState BombState { get { if (this.currentBombLauncher) return this.currentBombLauncher.BombState; else return WeaponState.UnEquipped; } }
    public float BombValue { get { if (this.currentBombLauncher) return this.currentBombLauncher.BombValue; else return 0f; } }
    
    public Gun CurrentGun { get { return this.currentGun; } }

    public GameObject podLauncherPrefab;
    public GameObject bombLauncherPrefab;

    // public Transform target;
    public float baseSpeed = 20;
    public float startingHealth = 100f;

    private Gun currentGun;
    private PodLauncher currentPodLauncher;
    private BombLauncher currentBombLauncher;

    private float currentHealth;
    
    private bool isFiringBomb;
    private Rigidbody rb;
    private int maxJumps = 2;
    private int jumps;

    private int score;
    private float speed;

    Animator animator;
    // FIXME: get speed calculation actually correct...
    private Vector3 delta;

    public void TakeDamage(float damage) {
        this.currentHealth -= damage;
    }

    private void Start() {
        this.rb = this.GetComponent<Rigidbody>();
        this.animator = this.GetComponentInChildren<Animator>();
        this.delta = Vector3.zero;
        this.currentHealth = this.startingHealth;
        this.gameObject.layer = LayerMask.NameToLayer("Player");

        if (PlayerManager.instance.twoPlayerMode) {
            this.AttachPodLauncher();
            this.AttachBombLauncher();
        }
        this.AttachCamera();
    }
    
    private void AttachPodLauncher() {
        GameObject podLauncherInstance = Object.Instantiate(this.podLauncherPrefab, this.transform.position, Quaternion.identity, this.transform);
        this.currentPodLauncher = podLauncherInstance.GetComponent<PodLauncher>();
        this.currentPodLauncher.target = this.Target;
    }

    private void AttachBombLauncher() {
        GameObject bombLauncherInstance = Object.Instantiate(this.bombLauncherPrefab, this.transform.position, Quaternion.identity, this.transform);
        this.currentBombLauncher = bombLauncherInstance.GetComponent<BombLauncher>();
        this.currentBombLauncher.target = this.Target;
    }

    private void FixedUpdate() {
        RuntimePlatform plat = Application.platform;
        float moveCamHor, movePlayerVert, movePlayerHor;
        moveCamHor = XCI.GetAxis(MoveCameraHorizontalAxis, WhichXboxController);
        movePlayerHor = XCI.GetAxis(MovePlayerHorizontalAxis, WhichXboxController);
        movePlayerVert = -XCI.GetAxis(MovePlayerVerticalAxis, WhichXboxController);

        if (XCI.GetNumPluggedCtrlrs() == 0) {
            moveCamHor = Input.GetAxis("Mouse X");
            movePlayerHor = Input.GetAxis("Horizontal");
            movePlayerVert = -Input.GetAxis("Vertical");
        }

        if (!PlayerManager.instance.twoPlayerMode) {
            this.transform.Rotate(0, moveCamHor * 10, 0);
        }
        delta.x = this.speed * Time.deltaTime * movePlayerHor;
        delta.z = this.speed * Time.deltaTime * -movePlayerVert;
        this.rb.MovePosition(this.rb.position + this.transform.TransformDirection(delta));
    }

    private void OnCollisionEnter(Collision hit) {
        this.rb.velocity = Vector3.zero;
        if (hit.gameObject.tag == "Ground") {
            this.jumps = 0;
            // TODO would set grounded variable here :)
        }
        if (hit.gameObject.layer == LayerMask.NameToLayer("Fall Plane")) {
            this.currentHealth = 0;
        }
    }

    // Update is called once per frame
    private void Update() {
        if (this.Target == null && PlayerManager.instance.twoPlayerMode) {
            Debug.Log("NOT YET FOR " + this.name);
        }
        if (PlayerManager.instance.twoPlayerMode) {
            this.FaceTarget();
            float divisor = 4f * ((float)this.score / PlayerManager.instance.scoreWinThreshold);
            this.speed = divisor==0 ? this.baseSpeed : this.baseSpeed / divisor;
        } else {
            this.speed = this.baseSpeed;
        }
        
        float gun = XCI.GetAxis(GunButton, WhichXboxController);
        bool jump = XCI.GetButtonDown(JumpButton, WhichXboxController);
        bool bomb = XCI.GetButtonDown(BombButton, WhichXboxController);
        bool pod = XCI.GetButtonDown(PodButton, WhichXboxController);
        bool pickup = XCI.GetButtonDown(PickUpButton, WhichXboxController);
        // Triggers > 0 is right trigger
        // 'Fire Button' becuase it feels weird to say 'Fire Axis', when this is buttonlike... dont mind...
        if (gun > 0) {
            if (this.currentGun) this.currentGun.FireGun();
        }
        if (jump) {
            this.Jump();
        }
        if (bomb) {
            this.currentBombLauncher.FireBomb();
        }
        if (pod) {
            this.currentPodLauncher.FirePod();
        }
        if (pickup) {
            this.PickUp();
        }
        this.animator.SetFloat("speedPercent", this.delta.magnitude / Time.deltaTime / this.speed  / 3, .1f, Time.deltaTime);
    }

    private void PickUp() {
        Collider[] collisions = Physics.OverlapSphere(this.transform.position + Vector3.up * 2, 4f, LayerMask.GetMask("Pickup"));
        if (collisions.Length == 0) return;
        PickupController pickupController = collisions[0].gameObject.GetComponent<PickupController>();
        if (pickupController.type == PickupType.Gun) {
            GameObject pickupInstance = Object.Instantiate(pickupController.pickupPrefab, this.transform.position, Quaternion.identity, this.transform);
            this.currentGun = pickupInstance.GetComponent<Gun>();
            this.currentGun.target = this.Target;
        } else if (pickupController.type == PickupType.Score) {
            this.score++;
        } else {
            throw new System.Exception("NO PICK UP CODE FOR PICKUP_TYPE: " + pickupController.type);
        }
        Object.Destroy(collisions[0].gameObject);
    }

    private void Jump() {
        // TODO use state pattern :)
        // Also, we need a 'grounded' to check if player like fell off something and jumped
        // we want a dash then, not a regular jump.
        if (this.jumps == 0) {
            this.jumps++;
            this.rb.AddForce(new Vector3(0, 500f, 0), ForceMode.Impulse);
        } else if (jumps < maxJumps) {
            this.jumps++;
            float hori = XCI.GetAxis(MovePlayerHorizontalAxis, WhichXboxController);
            float vert = -XCI.GetAxis(MovePlayerVerticalAxis, WhichXboxController);
            float sideForce = 1000f;
            if (!Mathf.Approximately(hori, 0) || !Mathf.Approximately(vert, 0)) {
                // If we have to move, move!
                this.rb.AddRelativeForce(new Vector3(hori * sideForce, 0, -vert * sideForce), ForceMode.Impulse);
            } else {
                // only go forward if horizontal is 0 or if nothing is set.
                this.rb.AddRelativeForce(new Vector3(0, 0, sideForce), ForceMode.Impulse);
            }
        }
    }

    public void OnDied() {
        this.score = 0;
        this.currentHealth = this.startingHealth;
    }
    
    private void FaceTarget() {
        if (this.Target == null) return;
        Vector3 direction = (this.Target.position - this.transform.position).normalized;
        if (Mathf.Abs(direction.x) > float.Epsilon || Mathf.Abs(direction.z) > float.Epsilon) {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
}
