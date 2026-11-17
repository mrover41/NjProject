using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
    private enum AnimationStatus {
        None,
        Idle,
        Walk,
        Fall,
    }

    private AnimationStatus animStat;

    private Animator _anim = null;
    private PlayerMovment _playerMov = null;

    private void Start() {
        _anim = GetComponent<Animator>();
        _playerMov = GetComponent<PlayerMovment>();
    }

    private void Update() {
		if (_anim == null || _playerMov == null) return;

        if (!_playerMov.Grounded && animStat != AnimationStatus.Fall) {
            _anim.SetTrigger("ToFall");
            animStat = AnimationStatus.Fall;
        } else if (_playerMov.Grounded && animStat == AnimationStatus.Fall) {
            //
        }

        if (animStat != AnimationStatus.Walk && _playerMov.isWalking && _playerMov.Grounded) {
            _anim.SetTrigger("ToWalking");
            animStat = AnimationStatus.Walk;
        } else if (animStat != AnimationStatus.Idle && !_playerMov.isWalking && _playerMov.Grounded) {
            _anim.SetTrigger("ToIdle");
            animStat = AnimationStatus.Idle;
        }

    }

    private void OnDisable() {
        _anim = null;
        _playerMov = null;        
    }
}
