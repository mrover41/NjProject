using UnityEngine;

[System.Serializable]
public class PlayerMovment : ModuleBase {
    private float slashT = 0;
    private Vector3 slashDirection;

    [SerializeField] private float deadZone = 0.1f; //only for joysticks
    [SerializeField] private string groundTag = "Ground";

    [SerializeField] public float force = 15f;
    [SerializeField] public float airMult = 0.3f;
    [SerializeField] public float defaultDamping = 5;
    [SerializeField] public float damping = 0.5f;
    [SerializeField] public float jumpSpeed = 6;
    [SerializeField] public float jumpForce = 2.5f;
    [SerializeField] public float slashSpeed = 35;
    [SerializeField] public float slashTime = 0.35f;
    [SerializeField] public float slashCooldown = 5;

    [SerializeField] public Vector3 Gravity = new Vector3(0, -9.8f, 0);
    [SerializeField] public float maxSpeed = 10f;

    public Rigidbody rb;
    public int groundCounter = 0;
    public Vector3 direction {get; private set;}
    public Vector3 localDirection {get; private set;}
    public Vector3 input {get; private set;}
    public bool Grounded => groundCounter > 0;
    public bool isEnabled = true;
    public bool isDashEnabled = true;
    public bool isWalking {get; private set;} = false;
    public bool isDashing {get; private set;} = false;
    public bool isSlaming {get; private set;} = false;

	private Vector3 savedLinear = Vector3.zero;

    public float DeadZone {
        get => deadZone;
        private set => deadZone = value;
    }

    public override void OnEnable(Player pl) {
        rb = pl.gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    public override void OnUpdate() {
        if (!isEnabled) return;
        UpdateInput();        
    }

    public override void OnFixedUpdate() {
        if (!isEnabled) return;
        UpdateMoving();
    }


    private void UpdateInput() {
        if (Input.GetKeyDown(KeyCode.Space) && Grounded) rb.linearVelocity += player.gameObject.transform.up * jumpSpeed + direction.normalized * jumpForce;

		if (Input.GetKeyDown(KeyCode.LeftShift) && slashT + slashTime <= Time.time && isDashEnabled) {
            slashT = Time.time;
            slashDirection = direction.normalized;
            if (slashDirection.magnitude == 0) slashDirection = player.gameObject.transform.forward;
        } 

		if (Input.GetKeyDown(KeyCode.LeftControl)) {
			isSlaming = true;

			CapsuleCollider collider = player.gameObject.GetComponent<CapsuleCollider>();

			PhysicsMaterial noFriction = new PhysicsMaterial("ZeroFriction") {
			    dynamicFriction = 0f,
			    staticFriction = 0f,
			    frictionCombine = PhysicsMaterialCombine.Minimum,
			};
			collider.sharedMaterial = noFriction;
			collider.height *= 0.5f;

			savedLinear = rb.linearVelocity;
		} else if (Input.GetKeyUp(KeyCode.LeftControl)) {
			isSlaming = false;

			CapsuleCollider collider = player.gameObject.GetComponent<CapsuleCollider>();
			collider.height *= 2;
			collider.sharedMaterial = null;

			savedLinear = Vector3.zero;
		}
    }

    private void UpdateMoving() {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 localDir = input.normalized * force * Time.fixedDeltaTime;
        localDirection = localDir;
        direction = player.gameObject.transform.TransformDirection(localDir);

        if (slashT + slashTime >= Time.time) {
            isDashing = true;
            rb.linearVelocity = slashDirection * slashSpeed;
            return;
        } else if (isDashing) {
            rb.linearVelocity = Vector3.zero;
            isDashing = false;
        } else if (isSlaming) {
			/*Vector3 local = player.gameObject.transform.InverseTransformDirection(savedLinear);
			local = new Vector3(local.x, 0, local.z);
			local = player.gameObject.transform.TransformDirection(local);

			Vector3 localVel = player.gameObject.transform.InverseTransformDirection(rb.linearVelocity);
			localVel = new Vector3(0, localVel.y, 0);
			localVel = player.gameObject.transform.TransformDirection(localVel);

			Vector3 newSavedLinear = local += localVel;
			rb.linearVelocity = newSavedLinear;*/
			rb.AddForce(Gravity, ForceMode.Acceleration);
			return;
		}

        if (input.magnitude > deadZone && Grounded) {
            rb.linearDamping = damping;
            Vector3 localVel = player.gameObject.transform.InverseTransformDirection(rb.linearVelocity);
            Vector3 newLocalXZ = Vector3.ClampMagnitude(new Vector3(localVel.x + localDir.x, 0, localVel.z + localDir.z), maxSpeed);
            rb.linearVelocity = player.gameObject.transform.TransformDirection(new Vector3(newLocalXZ.x, localVel.y, newLocalXZ.z));
            isWalking = true;
        } else if (Grounded) {
            rb.linearDamping = defaultDamping;
            isWalking = false;
        } else {
            rb.linearDamping = 0;
            rb.linearVelocity += player.gameObject.transform.TransformDirection(new Vector3(localDir.x * airMult, 0, localDir.z * airMult));
            isWalking = false;
        }

        rb.AddForce(Gravity, ForceMode.Acceleration);
    }

    public override void OnCollisionEnter(Collision collision) {
        if (collision.collider.gameObject.CompareTag(groundTag)) groundCounter ++;
    }

    public override void OnCollisionExit(Collision collision) {
        if (collision.collider.gameObject.CompareTag(groundTag)) groundCounter --;
    }
}
