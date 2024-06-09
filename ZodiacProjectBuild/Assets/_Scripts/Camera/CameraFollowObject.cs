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

    float _panTimer;

    private void Awake()
    {
        _player = _playerTransform.gameObject.GetComponent<Player>();
        _isFacingRight = _player.IsFacingRight;
    }

    private void Update()
    {
        if (
            _panTimer >= 0
            )
            _panTimer -= Time.deltaTime;
    }

    private void FixedUpdate() 
    {
        transform.position = _playerTransform.position;
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

        if (
            _isFacingRight
            )
            return 180f;
        else
            return 0f;
    }
}