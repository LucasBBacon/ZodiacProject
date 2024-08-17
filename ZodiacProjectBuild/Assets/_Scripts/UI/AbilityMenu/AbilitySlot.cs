using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilitySlot : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    ISelectHandler,
    IDeselectHandler
{
    [Header("Data")]
    public SOAbilityCard abilityCard;
    [SerializeField] AbilityCardMiddleHolder _middleHolder;
    [SerializeField] AbilityCardSelectionManager _manager;
    [SerializeField] Image _cardImage;

    [Header("Selection")]
    public bool selected;

    [HideInInspector] public Image selectImage;
    
    
    public Button selectButton;

    public bool IsHovered;
    public bool IsPickedUp;

    private void Start()
    {
        selectImage = GetComponent<Image>();
        selectButton = GetComponent<Button>();

        selectButton.enabled = false;
        selectImage.enabled = false;
        _cardImage.sprite = null;
        _cardImage.enabled = false;
        //_cardImage.sprite = abilityCard.CardImage;
        
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
        IsHovered = true;
        _middleHolder.MoveMiddle(transform.position.x);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        IsHovered = false;
        
    }

    public void PlaceCardInMiddle()
    {
        selectImage.enabled = true;
        selectButton.enabled = true;
    }

    public void ReturnCardToDeck()
    {
        selectButton.enabled = false;
        selectImage.enabled = false;
    }

    public void PlaceCardInSlot(int currentCard)
    {
        if (abilityCard != null)
        {
            foreach (AbilityCard disabledDeckCard in _manager.DisabledCards)
            {
                if (abilityCard == disabledDeckCard.abilityCard)
                {
                    _manager.CardPlacedOnDeck(abilityCard);
                    break;
                }
            }
            
        }

        abilityCard = _middleHolder.cardData;
        GameManager.Instance.SetPlayerAbility(abilityCard.CardIndex, currentCard);

        EventSystem.current.SetSelectedGameObject(_manager.Cards[^1].gameObject);
        
        _cardImage.enabled = true;
        _cardImage.sprite = abilityCard.CardImage;
    }
}