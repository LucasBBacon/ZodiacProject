using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    Canvas _canvas;
    MenuCard _card;
    // [SerializeField] bool instantiateVisual = true;
    // VisualCardsHandler _visualHandler;

    [Header("Selection")]
    public bool selected;

    [Header("Visual")]
    [SerializeField] GameObject _cardVisualPrefab;
    [HideInInspector] public CardVisual cardVisual;

    [Header("Events")]
    [HideInInspector] public UnityEvent<MenuButton> MenuPointerUpEvent;
    [HideInInspector] public UnityEvent<MenuButton> MenuPointerDownEvent;
    [HideInInspector] public UnityEvent<MenuButton> MenuPointerEnterEvent;
    [HideInInspector] public UnityEvent<MenuButton> MenuPointerExitEvent;
    [HideInInspector] public UnityEvent<MenuButton> MenuSubmitEvent;
    [HideInInspector] public UnityEvent<MenuButton, bool> MenuSelectEvent;
    [HideInInspector] public UnityEvent<MenuButton, bool> MenuDeselectEvent;

    private void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
        _card = GetComponentInChildren<MenuCard>();

        // if (!instantiateVisual)
        //     return;

        // _visualHandler = FindObjectOfType<VisualCardsHandler>();

        _card.Initalise(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        MenuPointerUpEvent.Invoke(this);

        //selected = !selected;
        //MenuSelectEvent.Invoke(this, selected);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        MenuPointerDownEvent.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MenuPointerEnterEvent.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MenuPointerExitEvent.Invoke(this);
    }

    public void OnSelect(BaseEventData eventData)
    {
        MenuSelectEvent.Invoke(this, selected);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        MenuDeselectEvent.Invoke(this, selected);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        MenuSubmitEvent.Invoke(this);
    }

    public int SiblingAmount()
    => transform.parent.CompareTag("Slot") ? transform.parent.parent.childCount - 1 : 0;

    public int ParentIndex()
    => transform.parent.CompareTag("Slot") ? transform.parent.GetSiblingIndex() : 0;

    public float NormalizedPosition()
    => transform.parent.CompareTag("Slot") ? Utilities.MappingUtil.Map((float)ParentIndex(), 0, (float)(transform.parent.parent.childCount - 1), 0, 1, true) : 0;
}
