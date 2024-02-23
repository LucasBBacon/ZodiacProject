using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollowObjects : MonoBehaviour
{
    
    [Header("References")]
    [SerializeField] private Transform _playerTransform;

    [Header("Flip Rotation Stats")]
    [SerializeField] private float _flipYRotationTime = 0.5f;
    private Coroutine _turnCoroutine;
    private Player _player;
    private bool _isFacingRight;

    private Vector2 _worldPosition;
    private Vector2 _direction;
    private Vector3 _lastMouseCoordinate = Vector3.zero;
    private float _panTimer;
    private Coroutine _panCoroutine;
    

    private void Awake()
    {
        _player = _playerTransform.gameObject.GetComponent<Player>();
        _isFacingRight = _player.IsFacingRight;

       
    }

    private void Update() 
    {
        
        if(_panTimer >= 0) _panTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        transform.position = _playerTransform.position;  
    }
    //make the cameraFollowObject follow the player's position transform.position= _playerTransform.position;

    public void CallTurn()
    {
        _turnCoroutine = StartCoroutine(FlipYLerp());
    }

    private IEnumerator FlipYLerp()
    {
        float startRotation = transform.localEulerAngles.y;
        float endRotationAmount = DetermineEndRotation();
        float yRotation = 0f;

        float elapsedTime = 0f;
        while(elapsedTime < _flipYRotationTime)
        {
            elapsedTime += Time.deltaTime;
            //Lerp the y rotation
            yRotation = Mathf.Lerp(startRotation, endRotationAmount, (elapsedTime / _flipYRotationTime)); 
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            yield return null;
        }
    }

    private float DetermineEndRotation()
    {
        _isFacingRight = !_isFacingRight;
        if (_isFacingRight)
        {
            return 180f;
        }
        else
        {
            return 0f;
        }
    }
}
