using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Player          _player;
    private Animator        _anim;
    private SpriteRenderer  _spriteRenderer;

    private GameManager     gameManager;

    [Header("Movement Tilt")]
    [SerializeField] private float _maxTilt;
    [Range(0, 1)]
    [SerializeField] private float _tiltSpeed;

    [Header("Particle Effects")]
    [SerializeField] private GameObject _jumpEffects;
    [SerializeField] private GameObject _landEffects;
    private ParticleSystem _jumpParticle;
    private ParticleSystem _landParticle;

    private Coroutine _resetTriggerCoroutine;

    public bool StartedJumping  { private get; set; }
    public bool JustLanded      { private get; set; }

    public float currentVelocityY;

    private void Start() 
    {
        _player         = GetComponent<Player>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _anim           = _spriteRenderer.GetComponent<Animator>();

        gameManager     = FindObjectOfType<GameManager>();

        _jumpParticle   = _jumpEffects.GetComponent<ParticleSystem>();
        _landParticle   = _landEffects.GetComponent<ParticleSystem>();
    }

    private void LateUpdate() 
    {
        #region Tilt

        float tiltProgress;

        int mult = -1;

        if(_player.IsSliding) tiltProgress = 0.25f;
        
        else
        {
            tiltProgress = Mathf.InverseLerp(-_player.playerData.runMaxSpeed, _player.playerData.runMaxSpeed, _player.RB.velocity.x);
            mult = (_player.IsFacingRight) ? 1 : -1;
        }

        float newRot                            = ((tiltProgress * _maxTilt * 2) - _maxTilt);
        float rot                               = Mathf.LerpAngle(_spriteRenderer.transform.localRotation.eulerAngles.z * mult, newRot, _tiltSpeed);
        _spriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, rot * mult);
        
        #endregion

        CheckAnimationState();
    
        // ParticleSystem.MainModule jumpPSettings = _jumpParticle.main;
        // jumpPSettings.startColor = new ParticleSystem.MinMaxGradient(gameManager.SceneData.foregroundColor);
        // ParticleSystem.MainModule landPSettings = _landParticle.main;
        // landPSettings.startColor = new ParticleSystem.MinMaxGradient(gameManager.SceneData.foregroundColor);
    }

    private void CheckAnimationState()
    {
        if(StartedJumping)
        {
            _anim.SetTrigger("Jump");
            
            GameObject obj = Instantiate(_jumpEffects, transform.position - (Vector3.up * transform.localScale.y / 2), Quaternion.Euler(-90, 0, 0));
            Destroy(obj, 1);
            //Debug.Log("Spawned Jump Particles");
            

            StartedJumping = false;

            return;
        }

        if(JustLanded)
        {
            _anim.SetTrigger("Land");

            GameObject obj = Instantiate(_landEffects, transform.position - (Vector3.up * transform.localScale.y / 1.5f), Quaternion.Euler(-90, 0, 0));
            Destroy(obj, 1);
            RumbleManager.instance.RumblePulse(0.2f, 0.5f, 0.15f);


            JustLanded = false;
            _resetTriggerCoroutine = StartCoroutine(Reset());

            return;
        }

        _anim.SetFloat("Velocity Y", _player.RB.velocity.y);
    }

    private IEnumerator Reset()
    {
        // allows for things to be done in a certain amount of time, waits for sing frame
        yield return null;

        // reset animation trigger for landing
        _anim.SetTrigger("Land");
    }
}
