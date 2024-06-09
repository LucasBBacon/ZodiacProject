using System.Collections;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Player _player;
    private Animator _anim;
    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private float _maxTilt;
    [Range(0, 1)]
    [SerializeField] private float _tiltSpeed;

    private void Start()
    {
        _player = GetComponent<Player>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _anim = _spriteRenderer.GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        float tiltProgress;
        tiltProgress = Mathf.InverseLerp
            (
                -_player.data.runMaxSpeed,
                _player.data.runMaxSpeed,
                _player.body.velocity.x
            );

        int mult = _player.IsFacingRight ? 1 : -1;
        float newRot = (tiltProgress * _maxTilt * 2) - _maxTilt;
        float rot                               = Mathf.LerpAngle(_spriteRenderer.transform.localRotation.eulerAngles.z * mult, newRot, _tiltSpeed);
        _spriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, rot * mult);
    }
}