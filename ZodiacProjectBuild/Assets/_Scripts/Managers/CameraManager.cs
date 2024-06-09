using System.Collections;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField] CinemachineVirtualCamera[] _allVirtualCameras;

    [Header("Y Damping during free fall")]
    [SerializeField] float _fallPanAmount = 0.25f;
    [SerializeField] float _fallYPanTime = 0.35f;
    float _fallSpeedYDampingChangeThreshold = -15f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromplayerFalling { get; set;}

    CinemachineVirtualCamera _currentCamera;
    CinemachineFramingTransposer _framingTransposer;

    float _normYPanAmount;
    Vector2 _startingTrackedObjectOffset;

    private void Awake()
    {
        if (instance == null) instance = this;

        for (int i = 0; i < _allVirtualCameras.Length; i++)
        {
            if (_allVirtualCameras[i].enabled)
            {
                _currentCamera = _allVirtualCameras[i];

                _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
        }

        _normYPanAmount = _framingTransposer.m_YDamping;

        _startingTrackedObjectOffset = _framingTransposer.m_TrackedObjectOffset;
    }


    #region Lerp Y Damping

    public void LerpYDamping(bool isPlayerFalling)
    {
        StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        float startDampAmount = _framingTransposer.m_YDamping;
        float endDampAmount = 0f;

        if (isPlayerFalling)
        {
            endDampAmount = _fallPanAmount;
            LerpedFromplayerFalling = true;
        }

        else
        {
            endDampAmount = _normYPanAmount;
        }

        float elapsedTime = 0f;
        while (elapsedTime < _fallYPanTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp
                (
                    startDampAmount,
                    endDampAmount,
                    elapsedTime/_fallYPanTime
                );
            _framingTransposer.m_YDamping = lerpedPanAmount;

            yield return null;
        }

        IsLerpingYDamping = false;
    }

    #endregion


    #region Pan Camera

    public void PanCameraOnContact
        (
            float panDistance,
            float panTime,
            PanDirection panDirection,
            bool panToStartingPos
        )
    {
        StartCoroutine
            (
                PanCamera
                (
                    panDistance,
                    panTime,
                    panDirection,
                    panToStartingPos
                )
            );
    }

    public void PanCameraOnContact
        (
            float panDistance,
            float panTime,
            Vector2 panDirection,
            bool panToStartingPos
        )
    {
        StartCoroutine
            (
                PanCamera
                    (
                        panDistance,
                        panTime,
                        panDirection,
                        panToStartingPos
                    )
            );
    }

    public IEnumerator PanCamera
        (
            float panDistance,
            float panTime,
            PanDirection panDirection,
            bool panToStartingPos
        )
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = Vector2.zero;

        if (!panToStartingPos)
        {
            switch (panDirection)
            {
                case PanDirection.Up:
                    endPos = Vector2.up;
                    break;
                case PanDirection.Down:
                    endPos = Vector2.down;
                    break;
                case PanDirection.Left:
                    endPos = Vector2.left;
                    break;
                case PanDirection.Right:
                    endPos = Vector2.right;
                    break;
                default:
                    break;
            }

            endPos *= panDistance;
            startingPos = _startingTrackedObjectOffset;
            endPos += startingPos;
        }

        else
        {
            startingPos = _framingTransposer.m_TrackedObjectOffset;
            endPos = _startingTrackedObjectOffset;
        }

        float elapsedTime = 0f;
        while (elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;

            Vector3 panLerp = Vector3.Lerp
                (
                    startingPos,
                    endPos,
                    elapsedTime/panTime
                );
            _framingTransposer.m_TrackedObjectOffset = panLerp;

            yield return null;
        }
    }

    public IEnumerator PanCamera
        (
            float panDistance,
            float panTime,
            Vector2 panDirection,
            bool panToStartingPos
        )
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = Vector2.zero;

        if (!panToStartingPos)
        {
            endPos = panDirection;
            endPos *= panDistance;
            startingPos = _startingTrackedObjectOffset;
            endPos += startingPos;
        }
        
        else
        {
            startingPos = _framingTransposer.m_TrackedObjectOffset;
            endPos = _startingTrackedObjectOffset;
        }

        float elapsedTime = 0f;
        while (elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;

            Vector3 panLerp = Vector3.Lerp
                (
                    startingPos,
                    endPos,
                    elapsedTime/panTime
                );
            _framingTransposer.m_TrackedObjectOffset = panLerp;

            yield return null;
        }
    }

    #endregion


    #region Swap Cameras

    public void SwapCamera
        (
            CinemachineVirtualCamera cameraFromLeft,
            CinemachineVirtualCamera cameraFromRight,
            Vector2 triggerExitDirection
        )
    {
        if
        (
            _currentCamera == cameraFromLeft &&
            triggerExitDirection.x > 0f
        )
        {
            cameraFromRight.enabled = true;
            cameraFromLeft.enabled = false;

            _currentCamera = cameraFromRight;
            _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        else if
        (
            _currentCamera == cameraFromRight &&
            triggerExitDirection.x < 0f
        )
        {
            cameraFromLeft.enabled = true;
            cameraFromRight.enabled = false;

            _currentCamera = cameraFromLeft;
            _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
    }

    #endregion
}