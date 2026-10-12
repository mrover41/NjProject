using UnityEngine;
using System;
using System.Linq;

public class Player : MonoBehaviour {
	private enum mode {
		Player,
		Enemy,
	}

	[SerializeField] private mode _mode;

	[SerializeReference] public ModuleBase[] modules;
	
	private delegate void UpdateDel();
	private delegate void FixedUpdateDel();
	private delegate void CollisionEnter(Collision _);
	private delegate void CollisionExit(Collision _);
	private delegate void TriggerEnter(Collider _);
	private delegate void TriggerExit(Collider _);

	private UpdateDel updateDel;
	private FixedUpdateDel fixedUpdateDel;
	private CollisionEnter collisionEnterDel;
	private CollisionExit collisionExitDel;
	private TriggerEnter triggerEnterDel;
	private TriggerExit triggerExitDel;

	public ModuleBase GetModule(string name) {
		return modules.FirstOrDefault(x => x.Name == name);
	}

	public T GetModule<T>() where T : ModuleBase {
		return modules.OfType<T>().FirstOrDefault();
	}

	void Awake() {
		switch(_mode) {
			case mode.Player:
				modules = new ModuleBase[] { 
					new PlayerMovment(),
					new PlaerCamera(),
					new Hook(),
					new PlayerHealth(),
					new KillPlayer(),
					//new PlayerAnimation(),
				};
			break;
			case mode.Enemy:
				modules = new ModuleBase[] {
					new AiMovment(),
				};
			break;
		}

	}

	void Start() {
		foreach (ModuleBase module in modules) {
			module.EnableModule(this);
			updateDel += module.OnUpdate;
			fixedUpdateDel += module.OnFixedUpdate;
			collisionEnterDel += module.OnCollisionEnter;
			collisionExitDel += module.OnCollisionExit;
			triggerEnterDel += module.OnTriggerEnter;
			triggerExitDel += module.OnTriggerExit;
		}	
	}

	void Update() {
		updateDel();
	}

	void FixedUpdate() {
		fixedUpdateDel();
	}

	void OnDisable() {
		foreach (ModuleBase module in modules) {
			module.DisableModule();
		}
	}

	private void OnCollisionEnter(Collision collision) {
		collisionEnterDel(collision);
	}

	private void OnCollisionExit(Collision collision) {
		collisionExitDel(collision);
	}

	private void OnTriggerEnter(Collider other) {
		triggerEnterDel(other);
	}

	private void OnTriggerExit(Collider other) {
		triggerExitDel(other);
	}
}
