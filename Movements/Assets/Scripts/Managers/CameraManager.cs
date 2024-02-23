using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    // for multiple cameras used
    [SerializeField] private CinemachineVirtualCamera[] _allVirtualCameras;

    [Header("Controls for lerping the Y Damping during player fall")]
    [SerializeField] private float _fallPanAmount = 0.25f;
    [SerializeField] private float _fallYPanTime = 0.35f;
    public float _fallSpeedYDampingChangeThreshold = -15f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    private Coroutine _lerpYPanCoroutine;
    private Coroutine _panCameraCoroutine;

    // whatever camera is currently active in a scene
    private CinemachineVirtualCamera _currentCamera;
    private CinemachineFramingTransposer _framingTransposer;

    private float _normYPanAmount;

    private Vector2 _startingTrackedObjectOffset;

    private void Awake() 
    {
        if(instance == null)
        {
            // single instance makes calling public methods from this class easier
            instance = this;
        }

        for(int i = 0; i < _allVirtualCameras.Length; i++)
        {
            if(_allVirtualCameras[i].enabled)
            {
                // set the current active camera
                _currentCamera = _allVirtualCameras[i];

                // set the framing transposer from the camera
                _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
        }

        // set YDamping amount so it's based on the inspector value
        _normYPanAmount = _framingTransposer.m_YDamping;

        // set the starting position of the tracked object offset
        _startingTrackedObjectOffset = _framingTransposer.m_TrackedObjectOffset;
    }

    #region Lerp the Y Damping

    public void LerpYDamping(bool isPlayerFalling)
    {
        _lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        // grab the starting damping amount
        float startDampAmount = _framingTransposer.m_YDamping;
        float endDampAmount = 0f;

        // determine the end damping amount
        if(isPlayerFalling)
        {
            endDampAmount = _fallPanAmount;
            LerpedFromPlayerFalling = true;
        }

        else
        {
            endDampAmount = _normYPanAmount;
        }

        // lerp the pan amount
        float elapsedTime = 0f;
        while(elapsedTime < _fallYPanTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime/_fallYPanTime));
            _framingTransposer.m_YDamping = lerpedPanAmount;

            yield return null;
        }

        IsLerpingYDamping = false;
    }

    #endregion

    #region Pan Camera

    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos) // bool is passed if the camera is to return to its normal level or not to not need to create 2 different functions
    {
        _panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));
    }

    public void PanCameraOnContact(float panDistance, float panTime, Vector2 panDirection, bool panToStartingPos) // bool is passed if the camera is to return to its normal level or not to not need to create 2 different functions
    {
        _panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));
    }

    public IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = Vector2.zero;

        // handle pan from trigger
        if(!panToStartingPos)
        {
            // set direction and distance
            switch (panDirection)
            {
                case PanDirection.Up:
                    endPos = Vector2.up;
                    break;
                case PanDirection.Down:
                    endPos = Vector2.down;
                    break;
                case PanDirection.Left:
                    endPos = Vector2.right;
                    break;
                case PanDirection.Right:
                    endPos = Vector2.left;
                    break;
                default:
                    break;
            }

            endPos *= panDistance;

            startingPos = _startingTrackedObjectOffset;

            endPos += startingPos;
        }

        // handle the direction settings when moving back to the starting position
        else
        {
            startingPos = _framingTransposer.m_TrackedObjectOffset;
            endPos = _startingTrackedObjectOffset;
        }

        float elapsedTime = 0f;
        while(elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;

            Vector3 panLerp = Vector3.Lerp(startingPos, endPos, (elapsedTime/panTime));
            _framingTransposer.m_TrackedObjectOffset = panLerp;

            yield return null;
        }

    }

    public IEnumerator PanCamera(float panDistance, float panTime, Vector2 panDirection, bool panToStartingPos)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = Vector2.zero;

        // handle pan from trigger
        if(!panToStartingPos)
        {
            endPos = panDirection;

            endPos *= panDistance;

            startingPos = _startingTrackedObjectOffset;

            endPos += startingPos;
        }

        // handle the direction settings when moving back to the starting position
        else
        {
            startingPos = _framingTransposer.m_TrackedObjectOffset;
            endPos = _startingTrackedObjectOffset;
        }

        float elapsedTime = 0f;
        while(elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;

            Vector3 panLerp = Vector3.Lerp(startingPos, endPos, (elapsedTime/panTime));
            _framingTransposer.m_TrackedObjectOffset = panLerp;

            yield return null;
        }

    }

    #endregion

    #region Swap Cameras

    public void SwapCamera(CinemachineVirtualCamera cameraFromLeft, CinemachineVirtualCamera cameraFromRight, Vector2 triggerExitDetection)
    {
        // if the current camera is the camera on the left and the trigger exit direction is on the right
        if(_currentCamera == cameraFromLeft && triggerExitDetection.x > 0f)
        {
            // activate the new camera
            cameraFromRight.enabled = true;

            // deactivate the old camera
            cameraFromLeft.enabled = false;

            // set the new camera as the current camera
            _currentCamera = cameraFromRight;

            // update the composer value
            _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>(); // otherwise some functionality won't work when camera switches
        }

        // if the current camera is the camera on the right and the trigger hit direction was on the left
        else if(_currentCamera == cameraFromRight && triggerExitDetection.x < 0f)
        {
            // activate the new camera
            cameraFromLeft.enabled = true;

            // deactivate the old camera
            cameraFromRight.enabled = false;

            // set the new camera as the current camera
            _currentCamera = cameraFromLeft;

            // update the composer value
            _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
    }

    #endregion

}
