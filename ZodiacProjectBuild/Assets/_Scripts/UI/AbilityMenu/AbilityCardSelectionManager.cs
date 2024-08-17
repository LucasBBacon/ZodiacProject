using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityCardSelectionManager : MonoBehaviour
{
    public List<AbilityCard> Cards = new();
    public List<AbilityCard> DisabledCards = new();

    public List<SOAbilityCard> AbilityCardDatas = new();

    public GameObject LastSelected { get; set; }
    public int LastSelectedIndex { get; set; }

    [SerializeField] GameObject cardPrefab;
    [SerializeField] GameObject _returnObject;
    [SerializeField] AbilityCardMiddleHolder _middleHold;
    

    private void Start()
    {
        InstantiateCards();

        _returnObject.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        StartCoroutine(SetSelectedAfterOneFrame());
    }

    IEnumerator SetSelectedAfterOneFrame()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(Cards[^1].gameObject);    
    }

    public void CardRemovedFromDeck(AbilityCard abilityCard)
    {
        foreach (AbilityCard card in Cards)
        {
            if (abilityCard == card)
            {
                DisabledCards.Add(card);

                _middleHold.CardPlacedInMiddle(card.abilityCard);

                _returnObject.gameObject.SetActive(true);

                card.gameObject.SetActive(false);

                break;
            }
        }

        //UpdateCards();
    }

    public void CardPlacedOnDeck(SOAbilityCard abilityCardData)
    {
        foreach (AbilityCard card in Cards)
        {
            if (card.abilityCard == abilityCardData)
            {
                DisabledCards.Remove(card);

                card.gameObject.SetActive(true);

                EventSystem.current.SetSelectedGameObject(Cards[^1].gameObject);

                break;
            }
        }
    }

    public void ReturnCard()
    => CardPlacedOnDeck(_middleHold.cardData);

    void InstantiateCards()
    {
        int cardCount = 0;

        foreach (SOAbilityCard cardData in AbilityCardDatas)
        {
            GameObject cardObj = Instantiate(cardPrefab, transform);
        
            if (cardPrefab.GetComponent<AbilityCard>() == null)
            {
                return;
            }
            
            Cards.Add(cardObj.GetComponent<AbilityCard>());
            Cards[cardCount].abilityCard = cardData;

            cardCount++;
        }

        StartCoroutine(SetSelectedAfterOneFrame());
    }
}
