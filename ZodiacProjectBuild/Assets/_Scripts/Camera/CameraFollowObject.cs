using System.Collections;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform _playerTransform;

    [Header("Flip Rotation Stats")]
    [SerializeField] float flipYRotationTime = 0.5f;

    Player _player;
    bool _isFacingRight;

    Vector2 velocity = Vector2.zero;

    float _panTimer;

    [SerializeField] Movement movement;

    private void Awake()
    {
        _player = _playerTransform.gameObject.GetComponent<Player>();
        _isFacingRight = _player.Movement.IsFacingRight;
    }

    private void Start()
    {
        movement.TurnEvent.AddListener(CallTurn);
    }

    private void Update()
    {
        if (
            _panTimer >= 0
            )
            _panTimer -= Time.deltaTime;
            
        transform.position = Vector3.Lerp(transform.position, _playerTransform.position, Time.deltaTime * 100);
    }

    private void FixedUpdate() 
    {
        //transform.position = _playerTransform.position;
    }

    public void CallTurn()
    {
        StartCoroutine(FlipYLerp());
    }

    private IEnumerator FlipYLerp()
    {
        float startRotation = transform.localEulerAngles.y;
        float endRotationAmount = DetermineEndRotation();
        float yRotation = 0f;

        float elapsedTime = 0f;
        while (
            elapsedTime < flipYRotationTime
            )
        {
            elapsedTime += Time.deltaTime;

            yRotation = Mathf.Lerp
                (
                    startRotation,
                    endRotationAmount,
                    elapsedTime/flipYRotationTime
                );
            
            transform.rotation = Quaternion.Euler
                (
                    0f,
                    yRotation,
                    0f
                );
            
            yield return null;
        }
    }

    float DetermineEndRotation()
    {
        _isFacingRight = !_isFacingRight;
        // Debug.Log(_player.movement.IsFacingRight + " + " + _isFacingRight);

        if (
            _isFacingRight
            )
            return 180f;
        else
            return 0f;
    }
}