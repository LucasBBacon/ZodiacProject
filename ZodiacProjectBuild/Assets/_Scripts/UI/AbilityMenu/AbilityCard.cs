using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityCard : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    ISelectHandler,
    IDeselectHandler
{
    Canvas _canvas;

    [Header("Data")]
    public SOAbilityCard abilityCard;

    [Header("Movement")]
    [SerializeField] float _verticalMoveAmount = 30f;
    [SerializeField] float _moveTime = 0.1f;
    [Range(0, 10f), SerializeField] float _scaleAmount = 1.1f;
    [SerializeField] float tiltSpeed = 20f;

    [Header("Selection")]
    public bool selected;
    

    [Header("Visual")]
    Transform _moveObj;
    Image _cardImage;
    
    [Header("Curve")]
    [SerializeField] CurveParameters _curve;
    float _curveYOffset;
    float _curveRotationOffset;

    Vector3 _startPos;
    Vector3 _startScale;

    AbilityCardSelectionManager _manager;

    public bool IsHovered;
    public bool IsPickedUp;

    private void Start()
    {
        _canvas = GetComponentInChildren<Canvas>();
        _cardImage = GetComponentInChildren<Image>();
        _moveObj = _cardImage.gameObject.transform;
        _cardImage.sprite = abilityCard.CardImage;
        _manager = GetComponentInParent<AbilityCardSelectionManager>();

        HandlePositioning();
        _startPos = _moveObj.localPosition + (Vector3.up * _curveYOffset);
        _moveObj.localPosition = _startPos;
        _startScale = _moveObj.localScale;

        

        //_visualCard.localPosition = _startPos;
    }

    private void Update()
    {
        if (!IsPickedUp)
        {
            HandlePositioning();

            CardTilt();
        }
        //SmoothFollow();
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

            Vector3 lerpedPos = Vector3.Lerp(_moveObj.localPosition, endPos, elapsedTime / _moveTime);
            Vector2 lerpedScale = Vector3.Lerp(_moveObj.localScale, endScale, elapsedTime / _moveTime);

            _moveObj.localPosition = lerpedPos;
            _moveObj.localScale = lerpedScale;

            yield return null;
        }
    }

    

    public void OnPointerEnter(PointerEventData eventData)
    {
        eventData.selectedObject = gameObject;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        eventData.selectedObject = null;
    }

    public void OnSelect(BaseEventData eventData)
    {
        StartCoroutine(MoveCard(true));
        //_canvas.sortingOrder = 100;
        IsHovered = true;

        _manager.LastSelected = gameObject;

        for (int i = 0; i < _manager.Cards.Count; i++)
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
        IsHovered = false;
        StartCoroutine(MoveCard(false));
    }

    void HandlePositioning()
    {
        _curveYOffset = _curve.positioning.Evaluate(NormalizedPosition()) * _curve.positioningInfluence * SiblingAmount();
        _curveYOffset = SiblingAmount() < 5 ? 0 : _curveYOffset;

        _curveRotationOffset = _curve.rotation.Evaluate(NormalizedPosition());
        //Debug.Log("siblings:" + SiblingAmount());
    }

    void CardTilt()
    {
        float tiltZ = _curveRotationOffset * (_curve.rotationInfluence * SiblingAmount());
        float lerpZ = Mathf.LerpAngle(_moveObj.eulerAngles.z, tiltZ, tiltSpeed / 2 * Time.deltaTime);

        _moveObj.eulerAngles = new Vector3(0, 0, lerpZ);
    }

    public void PickUpCard()
    {
        IsPickedUp = true;
        
        _moveObj.localPosition = _startPos;
        _moveObj.localScale = _startScale;

        _manager.CardRemovedFromDeck(this);
    }

    IEnumerator MoveCardToCenter(Vector3 to, float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _moveTime)
        {
            elapsedTime += Time.deltaTime;
            
            Vector3 pos = Vector3.Lerp(transform.position, to, time * Time.deltaTime);
            transform.position = pos;

            yield return null;
        }
    }

    float NormalizedPosition()
    => Utilities.MappingUtil.Remap((float)transform.GetSiblingIndex(), 0, (float)(SiblingAmount()), 0, 1);

    float ParentIndex()
    => transform.parent.GetSiblingIndex();

    int SiblingAmount()
    {
        int count = 0;
        foreach(Transform child in transform.parent.transform)
        {
            if(child.gameObject.activeSelf)
            count++;
        }
        return count;
    }
}