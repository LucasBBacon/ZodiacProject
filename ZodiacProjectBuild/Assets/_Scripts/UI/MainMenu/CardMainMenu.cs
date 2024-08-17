using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CardMainMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    Canvas _canvas;
    [SerializeField] bool instantiateVisual = true;
    public string Text = "default";
    VisualCardsHandlerTEST visualHandler;

    [Header("Selection")]
    public bool selected;
    
    [Header("States")]
    public bool isHovering;

    [Header("Visual")]
    [SerializeField] GameObject _cardVisualPrefab;
    [HideInInspector] public CardVisualMainMenu cardVisual;

    [Header("Events")]
    [HideInInspector] public UnityEvent<CardMainMenu> PointerDownEventTEST;
    [HideInInspector] public UnityEvent<CardMainMenu, bool> PointerUpEventTEST;
    [HideInInspector] public UnityEvent<CardMainMenu> PointerEnterEventTEST;
    [HideInInspector] public UnityEvent<CardMainMenu> PointerExitEventTEST;
    [HideInInspector] public UnityEvent<CardMainMenu, bool> SelectEventTEST;
    [HideInInspector] public UnityEvent<CardMainMenu, bool> DeselectEventTEST;
    [HideInInspector] public UnityEvent<CardMainMenu> SubmitEventTEST;    
    float pointerUpTime;
    float pointerDownTime;

    private void Start()
    {
        _canvas = GetComponentInParent<Canvas>();

        if(!instantiateVisual)
            return;

        visualHandler = FindObjectOfType<VisualCardsHandlerTEST>();
        cardVisual = Instantiate
            (
                _cardVisualPrefab,
                visualHandler ? visualHandler.transform : _canvas.transform
            ).GetComponent<CardVisualMainMenu>();
        cardVisual.Initalise(this, Text);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnterEventTEST.Invoke(this);
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExitEventTEST.Invoke(this);
        isHovering = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        PointerDownEventTEST.Invoke(this);   
        pointerDownTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        pointerUpTime = Time.time;
        
        PointerUpEventTEST.Invoke(this, pointerUpTime - pointerDownTime > .2f);

        //selected = !selected;
        //SelectEventTEST.Invoke(this, selected);

        //if (selected)
        //    transform.localPosition += cardVisual.transform.up * selectionOffset;
        //else
        //    transform.localPosition = Vector3.zero;
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
    
    public void OnSelect(BaseEventData eventData)
    {
        SelectEventTEST.Invoke(this, selected);
        isHovering = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        DeselectEventTEST.Invoke(this, selected);
        isHovering = false;
    }

    public void OnSubmit(BaseEventData eventData)
    {
        SubmitEventTEST.Invoke(this);
    }

    public int SiblingAmount()
    => transform.parent.CompareTag("Slot") ? transform.parent.parent.childCount - 1 : 0;

    public int ParentIndex()
    => transform.parent.CompareTag("Slot") ? transform.parent.GetSiblingIndex() : 0;

    public float NormalizedPosition()
    => transform.parent.CompareTag("Slot") ? Utilities.MappingUtil.Remap((float)ParentIndex(), 0, (float)(transform.parent.parent.childCount - 1), 0, 1) : 0;
}
