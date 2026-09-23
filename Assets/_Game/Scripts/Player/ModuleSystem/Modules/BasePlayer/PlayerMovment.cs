using UnityEngine;

[System.Serializable]
public class PlayerMovment : ModuleBase {
    private Rigidbody _rb;

    private float slashT = 0;
    private Vector3 slashDirection;
    private int groundCounter = 0;

    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float force = 15f;
    [SerializeField] private float airMult = 0.3f;
    [SerializeField] private float deadZone = 0.1f; //only for joysticks
    [SerializeField] private float defaultDamping = 5;
    [SerializeField] private float damping = 0.5f;
    [SerializeField] private float jumpSpeed = 6;
    [SerializeField] private float jumpForce = 2.5f;
    [SerializeField] private float slashSpeed = 35;
    [SerializeField] private float slashTime = 0.2f;
    [SerializeField] private float slashCooldown = 5;
    [SerializeField] private string groundTag = "Ground";

    [SerializeField] public Vector3 Gravity = new Vector3(0, -9.81f, 0);

    public Vector3 direction {get; private set;}
    public Vector3 input {get; private set;}
    public bool Grounded => groundCounter > 0;
    public bool isEnabled = true;
    public bool isWalking {get; private set;} = false;
    public bool isDashing {get; private set;} = false;

    public float DeadZone {
        get => deadZone;
        private set => deadZone = value;
    }

    public override void OnEnable(Player pl) {
        _rb = pl.gameObject.GetComponent<Rigidbody>();
        _rb.useGravity = false;
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
        if (Input.GetKeyDown(KeyCode.Space) && Grounded) { 
            //_rb.linearVelocity += new Vector3(0, jumpSpeed, 0) + direction.normalized * jumpForce;
            _rb.linearVelocity += player.gameObject.transform.up * jumpSpeed + direction.normalized * jumpForce;
        } if (Input.GetKeyDown(KeyCode.LeftShift) && slashT + slashTime <= Time.time) {
            slashT = Time.time;
            slashDirection = direction.normalized;
            if (slashDirection.magnitude == 0) slashDirection = player.gameObject.transform.forward;
        }
    }

    private void UpdateMoving() {
        input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        direction = (player.gameObject.transform.right * input.x + player.gameObject.transform.forward * input.z).normalized * force * Time.fixedDeltaTime;
        Vector3 localDir = input.normalized * force * Time.fixedDeltaTime;
        direction = player.gameObject.transform.TransformDirection(localDir);

        if (slashT + slashTime >= Time.time) {
            isDashing = true;
            _rb.linearVelocity = slashDirection * slashSpeed;
            return;
        } else if (isDashing) {
            _rb.linearVelocity = Vector3.zero;
            isDashing = false;
        }

        if (input.magnitude > deadZone && Grounded) {
            _rb.linearDamping = damping;
            Vector3 localVel = player.gameObject.transform.InverseTransformDirection(_rb.linearVelocity);
            Vector3 newLocalXZ = Vector3.ClampMagnitude(new Vector3(localVel.x + localDir.x, 0, localVel.z + localDir.z), maxSpeed);
            _rb.linearVelocity = player.gameObject.transform.TransformDirection(new Vector3(newLocalXZ.x, localVel.y, newLocalXZ.z));
            isWalking = true;
        } else if (Grounded) {
            _rb.linearDamping = defaultDamping;
            isWalking = false;
        } else {
            _rb.linearDamping = 0;
            _rb.linearVelocity += player.gameObject.transform.TransformDirection(new Vector3(localDir.x * airMult, 0, localDir.z * airMult));
            isWalking = false;
        }

        _rb.AddForce(Gravity, ForceMode.Acceleration);
    }

    public override void OnCollisionEnter(Collision collision) {
        if (collision.collider.gameObject.CompareTag(groundTag)) groundCounter ++;
    }

    public override void OnCollisionExit(Collision collision) {
        if (collision.collider.gameObject.CompareTag(groundTag)) groundCounter --;
    }
}
