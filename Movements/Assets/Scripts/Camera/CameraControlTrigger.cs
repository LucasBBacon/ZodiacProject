using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEditor;


public class CameraControlTrigger : MonoBehaviour
{
    public CustomInspectorObjects customInspectorObjects;

    private Collider2D _coll;
    
    private float _panDistance = 3f;
    private float _panTime = 0.5f;

    private void Start() 
    {
        _coll = GetComponent<Collider2D>();
    }

    // private void FixedUpdate() {
    //     PanningCamera();
    // }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.CompareTag("Player")) // if inside collider
        {
            
            if(customInspectorObjects.panCameraOnContact) // if the bool is selected for the pan camera options
            {

                // pan the camera
                CameraManager.instance.PanCameraOnContact(customInspectorObjects.panDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection, false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) 
    {
        if(collision.CompareTag("Player")) // if inside collider
        {
            Vector2 exitDirection = (collision.transform.position - _coll.bounds.center).normalized;

            if(customInspectorObjects.swapCameras && customInspectorObjects.cameraOnLeft != null && customInspectorObjects.cameraOnRight != null)
            {
                // swap cameras
                CameraManager.instance.SwapCamera(customInspectorObjects.cameraOnLeft, customInspectorObjects.cameraOnRight, exitDirection);
            }

            if(customInspectorObjects.panCameraOnContact) // if the bool is selected for the pan camera options
            {
                // pan the camera
                CameraManager.instance.PanCameraOnContact(customInspectorObjects.panDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection, true);
            }
        }
    }

    // private void PanningCamera()
    // {
    //     CameraManager.instance.PanCameraOnContact(_panDistance, _panTime, UserInput.instance.LookInput.normalized, false);
    // }
}

// These will be the serialised objects for the code, keeping the file clean
[System.Serializable]
public class CustomInspectorObjects
{
    public bool swapCameras = false;
    public bool panCameraOnContact = false;


    // if swap cameras is checked on inspector, then the following 2 variables will show on inspector
    [HideInInspector] public CinemachineVirtualCamera cameraOnLeft;
    [HideInInspector] public CinemachineVirtualCamera cameraOnRight;

    // if pan camera on contact is checked on the inspector, the the following 3 variables will show on inspector
    [HideInInspector] public PanDirection panDirection;
    [HideInInspector] public float panDistance = 3f;
    [HideInInspector] public float panTime = 0.35f;
}

// Enum to select which direction the camera will pan in the inspector
public enum PanDirection
{
    Up,
    Down,
    Left,
    Right
}

// inherits from editor
[CustomEditor(typeof(CameraControlTrigger))]
public class MyScriptEditor : Editor
{
    // target in inspector set
    CameraControlTrigger cameraControlTrigger;
    private void OnEnable() 
    {
        cameraControlTrigger = (CameraControlTrigger)target;
    }

    // built in unity method to create custom inspector objects
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(cameraControlTrigger.customInspectorObjects.swapCameras) // if the variable is checked on inspector
        {
            // two serialised fields are created for the virtual camera
            cameraControlTrigger.customInspectorObjects.cameraOnLeft = EditorGUILayout.ObjectField("Camera on Left", cameraControlTrigger.customInspectorObjects.cameraOnLeft, typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
            cameraControlTrigger.customInspectorObjects.cameraOnRight = EditorGUILayout.ObjectField("Camera on Right", cameraControlTrigger.customInspectorObjects.cameraOnRight, typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
        }

        if(cameraControlTrigger.customInspectorObjects.panCameraOnContact)
        {
            cameraControlTrigger.customInspectorObjects.panDirection = (PanDirection)EditorGUILayout.EnumPopup("Camera Pan Direction", cameraControlTrigger.customInspectorObjects.panDirection);

            cameraControlTrigger.customInspectorObjects.panDistance = EditorGUILayout.FloatField("Pan Distance", cameraControlTrigger.customInspectorObjects.panDistance);
            cameraControlTrigger.customInspectorObjects.panTime = EditorGUILayout.FloatField("Pan Time", cameraControlTrigger.customInspectorObjects.panTime);
        }

        if(GUI.changed)
        {
            EditorUtility.SetDirty(cameraControlTrigger); // check if GUI has changed, stops objects from resseting
        }
    }
        
}