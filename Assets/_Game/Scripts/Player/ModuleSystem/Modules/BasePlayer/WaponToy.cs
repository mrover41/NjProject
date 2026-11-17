using UnityEngine;

public class WaponToy : MonoBehaviour {
    private Transform _cam;

    private void Start() {
        _cam = Camera.main.transform;
    }

    private void Update() {
        if (!Input.GetKey(KeyCode.Q)) return;
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = _cam.position;
        sphere.AddComponent<Rigidbody>().AddForce(_cam.forward);
    }
}
