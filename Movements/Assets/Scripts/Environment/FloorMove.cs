using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloorMove : MonoBehaviour
{
    [SerializeField] private TMP_Text _displayText;
    [SerializeField] private Transform[] _wall;
    //[SerializeField] private GameObject _interactiveObj;
    [SerializeField] private Transform _otherWall;
    private Collider2D _interactiveColl;
    //private float distanceMoved = 0f;
    public float distanceToMove = 0.1f;

    private bool _collided = false;

    private bool _movingFloor = false;

    private void Start() 
    {
        _interactiveColl = GetComponent<Collider2D>();
        _displayText.text = "Press Interact to move stairs up and down";
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
                _displayText.text = "Press Interact to move stairs up and down";
            }
        }
        
        if(_movingFloor)
        {
            
            float positionY = UserInput.instance.InteractMoveInput.y * distanceToMove;

            if(Mathf.Abs(_wall[0].position.y - _otherWall.position.y) < 0f && UserInput.instance.InteractMoveInput.y < 0)
            {
                positionY = 0;
            }
            _wall[0].position = new Vector2(_wall[0].position.x, _wall[0].position.y + (positionY * 1/100));

            for (int i = 1; i < _wall.Length; i++)
            {
                _wall[i].position = new Vector2(_wall[i].position.x, _wall[i].position.y + (positionY * 1/100 * (i+1)));
            }
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
        _movingFloor = true;
    }

    public void InteractionStop()
    {
        InteractionManager.instance.StopInteract();
        _movingFloor = false;

    }
}
