using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class CardVisual : MonoBehaviour
{
    bool initalise = false;

    [Header("Card")]
    public Card parentCard;
    Transform cardTransform;
    Canvas _canvas;

    [Header("References")]
    public Transform visualShadow;
    float _shadowOffset = 20;
    Vector2 _shadowDistance;
    Canvas _shadowCanvas;
    [SerializeField] Transform shakeParent;

    [Header("Follow Parameters")]
    [SerializeField] float _followSpeed = 30;

    [Header("Rotation Parameters")]
    [SerializeField] float _rotationAmount = 20;
    [SerializeField] float _rotationSpeed = 20;
    [SerializeField] float _autoTiltAmount = 30;
    [SerializeField] float _manualTiltAmount = 20;
    [SerializeField] float _tiltSpeed = 20;

    [Header("Scale Parameters")]
    [SerializeField] bool _scaleAnimations = true;
    [SerializeField] float _scaleOnHover = 1.15f;
    [SerializeField] float _scaleOnSelect = 1.25f;
    [SerializeField] float _scaleTransition = 0.15f;
    [SerializeField] Ease _scaleEase;

    [Header("Select Parameters")]
    [SerializeField] float _selectPunchAmount = 20;

    [Header("Swap Parameters")]
    [SerializeField] bool _swapAnimations = true;
    [SerializeField] float _swapRotationAngle = 30;
    [SerializeField] float _swapTransition = .15f;
    [SerializeField] int _swapVibrato = 5;

    [Header("Curve")]
    [SerializeField] CurveParameters _curve;

    float _curveYOffset;
    float _curveRotationOffset;

    private void Start()
    {
        _shadowDistance = visualShadow.localPosition;    
    }
    
    private void Update() {
        if (!initalise || parentCard == null)
            return;

        HandlePositioning();
        SmoothFollow();
    }

    public void Initalise(Card target, int index = 0)
    {
        parentCard = target;
        cardTransform = target.transform;
        _canvas = GetComponent<Canvas>();
        _shadowCanvas = visualShadow.GetComponent<Canvas>();

        parentCard.BeginDragEvent.AddListener(BeginDrag);
        parentCard.EndDragEvent.AddListener(EndDrag);
        parentCard.PointerEnterEvent.AddListener(PointerEnter);
        parentCard.PointerExitEvent.AddListener(PointerExit);
        parentCard.PointerDownEvent.AddListener(PointerDown);
        parentCard.PointerUpEvent.AddListener(PointerUp);
        parentCard.SelectEvent.AddListener(Select);

        initalise = true;
    }

    void HandlePositioning()
    {
        _curveYOffset = _curve.positioning.Evaluate(parentCard.NormalizedPosition()) * _curve.positioningInfluence * parentCard.SiblingAmount();
        _curveYOffset = parentCard.SiblingAmount() < 5 ? 0 : _curveYOffset;

        _curveRotationOffset = _curve.rotation.Evaluate(parentCard.NormalizedPosition());
    }

    void SmoothFollow()
    {
        Vector3 verticalOffset = Vector3.up * (parentCard.isDragging ? 0 : _curveYOffset);
        transform.position = Vector3.Lerp(transform.position, cardTransform.position + verticalOffset, _followSpeed * Time.deltaTime);
    }

    void BeginDrag(Card card)
    {
        if (_scaleAnimations)
            transform.DOScale(_scaleOnSelect, _scaleTransition).SetEase(_scaleEase);
        
        _canvas.overrideSorting = true;
    }

    void EndDrag(Card card)
    {
        _canvas.overrideSorting = false;
        transform.DOScale(1, _scaleTransition).SetEase(_scaleEase);
    }

    void PointerEnter(Card card)
    {
        if (_scaleAnimations)
            transform.DOScale(_scaleOnHover, _scaleTransition).SetEase(_scaleEase);

        DOTween.Kill(2, true);
    }

    void PointerExit(Card card) 
    {
        if (!parentCard.wasDragged)
            transform.DOScale(1, _scaleTransition).SetEase(_scaleEase);
    }

    void PointerUp(Card card, bool longPress)
    {
        if (_scaleAnimations)
        {
            transform.DOScale(longPress ? _scaleOnHover : _scaleOnSelect, _scaleTransition).SetEase(_scaleEase);
        }
        _canvas.overrideSorting = false;

        visualShadow.localPosition = _shadowDistance;
        _shadowCanvas.overrideSorting = true;
    }

    void PointerDown(Card card)
    {
        if (_scaleAnimations)
        {
            transform.DOScale(_scaleOnSelect, _scaleTransition).SetEase(_scaleEase);
        }

        visualShadow.localPosition += -Vector3.up * _shadowOffset;
        _shadowCanvas.overrideSorting = false;
    }

    void Select(Card card, bool state)
    {
        DOTween.Kill(2, true);
        float dir = state ? 1 : 0;
        shakeParent.DOPunchPosition(shakeParent.up * _selectPunchAmount * dir, _scaleTransition, 10, 1);
        //shakeParent.DOPunchRotation(Vector3.forward * )

        if (_scaleAnimations)
            transform.DOScale(_scaleOnHover, _scaleTransition).SetEase(_scaleEase);
    }

    public void Swap(float dir = 1)
    {
        if (!_swapAnimations)
            return;
        
        DOTween.Kill(2, true);
        shakeParent.DOPunchRotation
            (
                Vector3.forward * _swapRotationAngle * dir,
                _swapTransition,
                _swapVibrato,
                1
            ).SetId(3);
    }

    public void UpdateIndex(int length)
    => transform.SetSiblingIndex(parentCard.transform.parent.GetSiblingIndex());
}
