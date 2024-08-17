using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    Canvas _canvas;
    Image _imageComponent;
    [SerializeField] bool instantiateVisual = true;
    VisualCardsHandler visualHandler;
    Vector3 _offset;

    [Header("Movement")]
    [SerializeField] float _moveSpeedLimit = 50f;

    [Header("Selection")]
    public bool selected;
    public float selectionOffset = 50;
    
    [Header("Stats")]
    public bool isDragging;
    public bool wasDragged;

    [Header("Visual")]
    [SerializeField] GameObject _cardVisualPrefab;
    [HideInInspector] public CardVisual cardVisual;

    [Header("Events")]
    [HideInInspector] public UnityEvent<Card> BeginDragEvent;
    [HideInInspector] public UnityEvent<Card> EndDragEvent;
    [HideInInspector] public UnityEvent<Card> PointerDownEvent;
    [HideInInspector] public UnityEvent<Card, bool> PointerUpEvent;
    [HideInInspector] public UnityEvent<Card> PointerEnterEvent;
    [HideInInspector] public UnityEvent<Card> PointerExitEvent;
    [HideInInspector] public UnityEvent<Card, bool> SelectEvent;    
    float pointerUpTime;
    float pointerDownTime;

    private void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
        _imageComponent = GetComponent<Image>();

        if(!instantiateVisual)
            return;

        visualHandler = FindObjectOfType<VisualCardsHandler>();
        cardVisual = Instantiate
            (
                _cardVisualPrefab,
                visualHandler ? visualHandler.transform : _canvas.transform
            ).GetComponent<CardVisual>();
        cardVisual.Initalise(this);
    }

    private void Update()
    {
        ClampPosition();

        if (isDragging)
        {
            Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _offset;
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            Vector2 velocity = direction * Mathf.Min(_moveSpeedLimit, Vector2.Distance(transform.position, targetPosition) / Time.deltaTime);
            transform.Translate(velocity * Time.deltaTime);
        }    
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        BeginDragEvent.Invoke(this);

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _offset = mousePosition - (Vector2)transform.position;

        isDragging = true;

        _canvas.GetComponent<GraphicRaycaster>().enabled = false;

        wasDragged = true;
    }

    public void OnDrag(PointerEventData eventData) {    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndDragEvent.Invoke(this);

        isDragging = false;

        _canvas.GetComponent<GraphicRaycaster>().enabled = true;
        _imageComponent.raycastTarget = true;

        StartCoroutine(FrameWait());

        IEnumerator FrameWait()
        {
            yield return new WaitForEndOfFrame();
            wasDragged = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnterEvent.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExitEvent.Invoke(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        PointerDownEvent.Invoke(this);   
        pointerDownTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        pointerUpTime = Time.time;
        
        PointerUpEvent.Invoke(this, pointerUpTime - pointerDownTime > .2f);

        if (pointerUpTime - pointerDownTime > .2f)
            return;
        
        if (wasDragged)
            return;

        selected = !selected;
        SelectEvent.Invoke(this, selected);

        if (selected)
            transform.localPosition += cardVisual.transform.up * selectionOffset;
        else
            transform.localPosition = Vector3.zero;
    }

    private void OnDestroy()
    {
        if (cardVisual != null)
        {
            Destroy(cardVisual.gameObject);
        }
    }

    public void Deselect()
    {
        if (selected)
        {
            selected = false;

            if (selected)
                transform.localPosition += cardVisual.transform.up * 50;
            else
                transform.localPosition = Vector3.zero;
        }
    }

    void ClampPosition()
    {
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint
            (
                new Vector3
                    (
                        Screen.width,
                        Screen.height,
                        Camera.main.transform.position.z
                    )
            );
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp
            (
                clampedPosition.x,
                -screenBounds.x,
                screenBounds.x
            );
        clampedPosition.y = Mathf.Clamp
            (
                clampedPosition.y,
                -screenBounds.y,
                screenBounds.y
            );
        transform.position = new Vector3
            (
                clampedPosition.x,
                clampedPosition.y,
                0
            );
    }

    public int SiblingAmount()
    => transform.parent.CompareTag("Slot") ? transform.parent.parent.childCount - 1 : 0;

    public int ParentIndex()
    => transform.parent.CompareTag("Slot") ? transform.parent.GetSiblingIndex() : 0;

    public float NormalizedPosition()
    => transform.parent.CompareTag("Slot") ? Utilities.MappingUtil.Map((float)ParentIndex(), 0, (float)(transform.parent.parent.childCount - 1), 0, 1, true) : 0;
}
