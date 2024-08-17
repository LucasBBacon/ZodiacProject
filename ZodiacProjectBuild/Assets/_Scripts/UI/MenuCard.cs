using DG.Tweening;
using UnityEngine;

public class MenuCard : MonoBehaviour
{
    bool initalise = false;

    public MenuButton parentButton;
    Canvas _canvas;

    public Transform visualShadow;
    float _shadowOffset;
    Vector2 _shadowDistance;
    Canvas _shadowCanvas;

    [Header("Scale Parameters")]
    [SerializeField] bool _scaleAnimations = true;
    [SerializeField] float _scaleOnHover = 1.15f;
    [SerializeField] float _scaleOnSelect = 1.25f;
    [SerializeField] float _scaleTransition = 0.15f;
    [SerializeField] Ease _scaleEase;

    [Header("Curve")]
    [SerializeField] CurveParameters _curve;

    float _curveYOffset;
    float _curveRotationOffset;

    private void Start()
    {
        _shadowDistance = visualShadow.localPosition;    
    }
    
    private void Update() {
        if (!initalise || parentButton == null)
            return;

        HandlePositioning();
    }

    void HandlePositioning()
    {
        _curveYOffset = _curve.positioning.Evaluate(parentButton.NormalizedPosition()) * _curve.positioningInfluence * parentButton.SiblingAmount();
        _curveYOffset = parentButton.SiblingAmount() < 5 ? 0 : _curveYOffset;

        _curveRotationOffset = _curve.rotation.Evaluate(parentButton.NormalizedPosition());
    }

    public void Initalise(MenuButton target, int index = 0)
    {
        parentButton = target;
        _canvas = GetComponent<Canvas>();
        _shadowCanvas = visualShadow.GetComponent<Canvas>();

        parentButton.MenuPointerEnterEvent.AddListener(PointerEnter);
        parentButton.MenuPointerExitEvent.AddListener(PointerExit);
        parentButton.MenuPointerDownEvent.AddListener(PointerDown);
        parentButton.MenuPointerUpEvent.AddListener(PointerUp);
        parentButton.MenuSubmitEvent.AddListener(Submit);
        parentButton.MenuSelectEvent.AddListener(Select);
        parentButton.MenuDeselectEvent.AddListener(Deselect);

        initalise = true;
    }

    void PointerEnter(MenuButton button)
    {
        Debug.Log("Pointer Entered");

        if (_scaleAnimations)
            transform.DOScale(_scaleOnHover, _scaleTransition).SetEase(_scaleEase);

        DOTween.Kill(2, true);
    }

    void PointerExit(MenuButton button) 
    {
        Debug.Log("Pointer Exited");
        
        transform.DOScale(1, _scaleTransition).SetEase(_scaleEase);
    }

    void PointerUp(MenuButton button)
    {
        Debug.Log("Pointer Up");

        if (_scaleAnimations)
        {
            transform.DOScale(_scaleOnHover, _scaleTransition).SetEase(_scaleEase);
        }
        _canvas.overrideSorting = false;

        visualShadow.localPosition = _shadowDistance;
        _shadowCanvas.overrideSorting = true;
    }

    void PointerDown(MenuButton button)
    {
        Debug.Log("Pointer Down");

        if (_scaleAnimations)
        {
            transform.DOScale(_scaleOnSelect, _scaleTransition).SetEase(_scaleEase);
        }

        visualShadow.localPosition += -Vector3.up * _shadowOffset;
        _shadowCanvas.overrideSorting = false;
    }

    void Select(MenuButton button, bool state)
    {
        Debug.Log("Selected!");
        DOTween.Kill(2, true);
        float dir = state ? 1 : 0;
        //shakeParent.DOPunchPosition(shakeParent.up * _selectPunchAmount * dir, _scaleTransition, 10, 1);
        //shakeParent.DOPunchRotation(Vector3.forward * )

        if (_scaleAnimations)
            transform.DOScale(_scaleOnHover, _scaleTransition).SetEase(_scaleEase);
    }

    void Deselect(MenuButton button, bool state)
    {
        Debug.Log("Deselected!");
        DOTween.Kill(2, true);
        float dir = state ? 1 : 0;
        //shakeParent.DOPunchPosition(shakeParent.up * _selectPunchAmount * dir, _scaleTransition, 10, 1);
        //shakeParent.DOPunchRotation(Vector3.forward * )

        transform.DOScale(1, _scaleTransition).SetEase(_scaleEase);
    }

    void Submit(MenuButton button)
    {
        if (_scaleAnimations)
        {
            transform.DOScale(_scaleOnSelect, _scaleTransition).SetEase(_scaleEase);
        }

        visualShadow.localPosition += -Vector3.up * _shadowOffset;
        _shadowCanvas.overrideSorting = false;
    }
}