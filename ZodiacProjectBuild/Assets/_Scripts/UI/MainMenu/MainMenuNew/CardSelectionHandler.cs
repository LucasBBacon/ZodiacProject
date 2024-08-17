using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelectionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] float _verticalMoveAmount = 30f;
    [SerializeField] float _moveTime = 0.1f;
    [Range(0f, 1f), SerializeField] float _scaleAmount = 1.1f;

    Vector3 _startPos;
    Vector3 _startScale;
    Canvas _canvas;

    CardSelectionManager _manager;

    private void Start()
    {
        _startPos = transform.position;
        _startScale = transform.localScale;

        _manager = GetComponentInParent<CardSelectionManager>();
        //_canvas = GetComponent<Canvas>();   
    }

    IEnumerator MoveCard(bool startingAnimation)
    {
        Vector3 endPos;
        Vector3 endScale;

        float elapsedTime = 0f;
        while (elapsedTime < _moveTime)
        {
            elapsedTime += Time.deltaTime;

            endPos = startingAnimation ? _startPos + new Vector3(0f, _verticalMoveAmount, 0f) : _startPos;
            endScale = startingAnimation ? _startScale * _scaleAmount : _startScale;

            Vector3 lerpedPos = Vector3.Lerp(transform.position, endPos, elapsedTime / _moveTime);
            Vector2 lerpedScale = Vector3.Lerp(transform.localScale, endScale, elapsedTime / _moveTime);

            transform.position = lerpedPos;
            transform.localScale = lerpedScale;

            yield return null;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("PointerEnter");
        eventData.selectedObject = gameObject;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        eventData.selectedObject = null;
    }

    public void OnSelect(BaseEventData eventData)
    {
        StartCoroutine(MoveCard(true));
        transform.SetAsLastSibling();

        _manager.LastSelected = gameObject;

        for (int i = 0; i < _manager.Cards.Length; i++)
        {
            if (_manager.Cards[i] == gameObject)
            {
                _manager.LastSelectedIndex = i;
                return;
            }
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        StartCoroutine(MoveCard(false));
    }
}
