using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuCardHolder : MonoBehaviour
{
    [SerializeField] GameObject _mainMenuFirst;
    [SerializeField] CardMainMenu _selectedCard;
    [SerializeField] CardMainMenu _hoveredCard;
    public List<CardMainMenu> _cards;

    void Start()
    {
        _cards = GetComponentsInChildren<CardMainMenu>().ToList();

        foreach (CardMainMenu card in _cards)
        {
            card.PointerEnterEventTEST.AddListener(CardPointEnter);
            card.PointerExitEventTEST.AddListener(CardPointerExit);
            card.SelectEventTEST.AddListener(CardSelect);
            card.DeselectEventTEST.AddListener(CardDeselect);
        }    

        StartCoroutine(Frame());

        IEnumerator Frame()
        {
            yield return new WaitForSecondsRealtime(.1f);
            for (int i = 0; i < _cards.Count; i++)
            {
                if (_cards[i].cardVisual != null)
                {
                    _cards[i].cardVisual.UpdateIndex(transform.childCount);
                }
            }
        }

        EventSystem.current.SetSelectedGameObject(_mainMenuFirst);
    }

    void Update()
    {
        
        if (Input.GetMouseButtonDown(1))
        {
            foreach (CardMainMenu card in _cards)
            {
                card.Deselect();
            }
        }

        if (_selectedCard == null)
            return;
    }

    void CardPointEnter(CardMainMenu card)
    {
        _hoveredCard = card;
    }

    void CardPointerExit(CardMainMenu card)
    {
        _hoveredCard = null;
    }

    void CardSelect(CardMainMenu card, bool selected)
    {
        _hoveredCard = card;
    }

    void CardDeselect(CardMainMenu card, bool selected)
    {
        _hoveredCard = null;
    }
}
