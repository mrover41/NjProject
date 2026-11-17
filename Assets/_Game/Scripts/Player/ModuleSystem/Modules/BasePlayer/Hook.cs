using UnityEngine;

public class Hook : MonoBehaviour {
    private bool hooking = false;
    private bool isFinded = false;

    private Vector3 offset;
    private Transform targetTransform;
    private Rigidbody _rb;
    private Rigidbody _objRb;
    private LineRenderer lineRenderer;

    [SerializeField] private float maxLenght = 200;
    [SerializeField] private float speed = 2500;
    [SerializeField] private LayerMask hookLayer = 1 << 6;
    [SerializeField] private float rqMass = 5;
    [SerializeField] private float otherSpeed = 2500;

    private void Start() {
        _rb = GetComponent<Rigidbody>();
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;

        Material lineMaterial = new Material(Shader.Find("Sprites/Default"));
        lineMaterial.color = Color.black;
        lineRenderer.material = lineMaterial;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F)) {
            lineRenderer.enabled = true;
            hooking = true;
        }


        if (Input.GetKeyUp(KeyCode.F)) {
            hooking = false;
            isFinded = false;

            lineRenderer.enabled = false;
        }
    }

    private void FixedUpdate() {
        if (hooking) {
            Transform c = Camera.main.gameObject.transform;
            if (!isFinded && Physics.Raycast(c.position, c.forward, out RaycastHit hit, maxLenght) && hit.collider.gameObject.CompareTag("Hook")) {
                targetTransform = hit.collider.gameObject.transform;
                offset = targetTransform.InverseTransformPoint(hit.point);

                _objRb = hit.collider.gameObject.GetComponent<Rigidbody>();

                isFinded = true;
            } else if (isFinded) {
                Vector3 pos = targetTransform.TransformPoint(offset);

                lineRenderer.SetPosition(0, c.position + Vector3.down);
                lineRenderer.SetPosition(1, pos);

                if (_objRb != null && _objRb.mass <= rqMass) {
                    _objRb.AddForce((c.position - pos).normalized * otherSpeed * Time.fixedDeltaTime);
                    return;
                } 
                _rb.AddForce((pos - c.position).normalized * speed * Time.fixedDeltaTime);
            }
        }
    }
}
