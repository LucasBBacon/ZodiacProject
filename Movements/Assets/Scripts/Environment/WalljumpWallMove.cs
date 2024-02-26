using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WalljumpWallMove : MonoBehaviour
{
    [SerializeField] private TMP_Text _displayText;
    [SerializeField] private Transform _wall;
    //[SerializeField] private GameObject _interactiveObj;
    [SerializeField] private Transform _otherWall;
    private Collider2D _interactiveColl;
    //private float distanceMoved = 0f;
    public float distanceToMove = 1f;

    private bool _collided = false;

    private bool _movingWall = false;

    private void Start() 
    {
        _interactiveColl = GetComponent<Collider2D>();
        _displayText.text = "Press Interact to move wall left and right";
    }

    private void Update() 
    {
        if(UserInput.instance.InteractInput && _collided)
        {
            if(!InteractionManager.instance.IsInteracting)
            {
                Interaction();
                _displayText.text = "Press Interact again to stop interacting!";
            }
        }

        else if(UserInput.instance.StopInteractInput && _collided)
        {
            if(InteractionManager.instance.IsInteracting)
            {
                InteractionStop();
                _displayText.text = "Press Interact to move wall left and right";
            }
        }
        
        if(_movingWall)
        {
            
            float positionX = UserInput.instance.InteractMoveInput.x * distanceToMove;

            if(Mathf.Abs(_wall.position.x - _otherWall.position.x) < 1.5f && UserInput.instance.InteractMoveInput.x < 0)
            {
                positionX = 0;
            }
            _wall.position = new Vector2(_wall.position.x + positionX, _wall.position.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.CompareTag("Player"))
        {
            _displayText.gameObject.SetActive(true);
            _collided = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) 
    {
        if(collision.CompareTag("Player"))
        {
            _displayText.gameObject.SetActive(false);
            _collided = false;
        }    
    }

    public void Interaction()
    {
        InteractionManager.instance.Interact();
        _movingWall = true;
    }

    public void InteractionStop()
    {
        InteractionManager.instance.StopInteract();
        _movingWall = false;

    }
   
}
