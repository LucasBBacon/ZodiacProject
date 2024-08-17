using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardVisualMainMenu : MonoBehaviour
{
    bool initalise = false;

    [Header("Card")]
    public CardMainMenu parentCard;
    Transform cardTransform;
    int _savedIndex;
    Canvas _canvas;
    Canvas _cardCanvas;
    TMP_Text _text;

    [Header("References")]
    public Transform visualShadow;
    float _shadowOffset = 20;
    Vector2 _shadowDistance;
    Canvas _shadowCanvas;
    [SerializeField] Transform shakeParent;
    [SerializeField] Transform _tiltParent;

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
        // FollowRotation();
        CardTilt();

        if (parentCard.isHovering)
        {
            _cardCanvas.overrideSorting = true;
            _cardCanvas.sortingOrder = 20;
        }
        else if (!parentCard.isHovering)
        {
            _cardCanvas.overrideSorting = false;
            _cardCanvas.sortingOrder = 0;
        }
    }

    public void Initalise(CardMainMenu target, string Text = "default", int index = 0)
    {
        parentCard = target;
        cardTransform = target.transform;

        _canvas = GetComponent<Canvas>();
        _cardCanvas = GetComponentInChildren<Canvas>();
        _shadowCanvas = visualShadow.GetComponent<Canvas>();

        _text = GetComponentInChildren<TMP_Text>();
        _text.text = Text;

        parentCard.PointerEnterEventTEST.AddListener(PointerEnter);
        parentCard.PointerExitEventTEST.AddListener(PointerExit);
        parentCard.PointerDownEventTEST.AddListener(PointerDown);
        parentCard.PointerUpEventTEST.AddListener(PointerUp);
        parentCard.SubmitEventTEST.AddListener(Submit);
        parentCard.SelectEventTEST.AddListener(Select);
        parentCard.DeselectEventTEST.AddListener(Deselect);

        initalise = true;
    }

    void HandlePositioning()
    {
        _curveYOffset = (_curve.positioning.Evaluate(parentCard.NormalizedPosition()) * _curve.positioningInfluence) * parentCard.SiblingAmount();
        _curveYOffset = parentCard.SiblingAmount() < 2 ? 0 : _curveYOffset;
        _curveRotationOffset = _curve.rotation.Evaluate(parentCard.NormalizedPosition());
    }

    void SmoothFollow()
    {
        Vector3 verticalOffset = Vector3.up * _curveYOffset;
        transform.position = Vector3.Lerp(transform.position, cardTransform.position + verticalOffset, _followSpeed * Time.deltaTime);
    }

    void PointerEnter(CardMainMenu card)
    {
        Debug.Log("Pointer Entered");

        if (_scaleAnimations)
            transform.DOScale(_scaleOnHover, _scaleTransition).SetEase(_scaleEase);

        DOTween.Kill(2, true);
    }

    void PointerExit(CardMainMenu card) 
    {
        Debug.Log("Pointer Exited");
        
        transform.DOScale(1, _scaleTransition).SetEase(_scaleEase);
    }

    void PointerUp(CardMainMenu card, bool longPress)
    {
        Debug.Log("Pointer Up");
        if (_scaleAnimations)
        {
            transform.DOScale(longPress ? _scaleOnHover : _scaleOnSelect, _scaleTransition).SetEase(_scaleEase);
        }
        _canvas.overrideSorting = false;

        visualShadow.localPosition = _shadowDistance;
        _shadowCanvas.overrideSorting = true;
    }

    void PointerDown(CardMainMenu card)
    {
        Debug.Log("Pointer Down");
        if (_scaleAnimations)
        {
            transform.DOScale(_scaleOnSelect, _scaleTransition).SetEase(_scaleEase);
        }

        visualShadow.localPosition += -Vector3.up * _shadowOffset;
        _shadowCanvas.overrideSorting = false;
    }

    void Select(CardMainMenu card, bool state)
    {
        Debug.Log("Selected!");
        DOTween.Kill(2, true);
        float dir = state ? 1 : 0;
        //shakeParent.DOPunchPosition(shakeParent.up * _selectPunchAmount * dir, _scaleTransition, 10, 1);
        //shakeParent.DOPunchRotation(Vector3.forward * )

        if (_scaleAnimations)
            transform.DOScale(_scaleOnHover, _scaleTransition).SetEase(_scaleEase);
    }

    void Deselect(CardMainMenu card, bool state)
    {
        Debug.Log("Deselected!");
        DOTween.Kill(2, true);
        float dir = state ? 1 : 0;
        //shakeParent.DOPunchPosition(shakeParent.up * _selectPunchAmount * dir, _scaleTransition, 10, 1);
        //shakeParent.DOPunchRotation(Vector3.forward * )

        transform.DOScale(1, _scaleTransition).SetEase(_scaleEase);
    }

    void Submit(CardMainMenu card)
    {
        if (_scaleAnimations)
        {
            transform.DOScale(_scaleOnSelect, _scaleTransition).SetEase(_scaleEase);
        }

        visualShadow.localPosition += -Vector3.up * _shadowOffset;
        _shadowCanvas.overrideSorting = false;
    }

    void CardTilt()
    {
        _savedIndex = parentCard.ParentIndex();

        Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float tiltZ = _curveRotationOffset * (_curve.rotationInfluence * parentCard.SiblingAmount());
        float lerpZ = Mathf.LerpAngle(_tiltParent.eulerAngles.z, tiltZ, _tiltSpeed / 2 * Time.deltaTime);

        _tiltParent.eulerAngles = new Vector3(0f, 0f, lerpZ);
    }

    public void UpdateIndex(int length)
    => transform.SetSiblingIndex(parentCard.transform.parent.GetSiblingIndex());
}
