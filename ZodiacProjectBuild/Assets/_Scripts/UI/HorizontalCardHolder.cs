using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class HorizontalCardHolder : MonoBehaviour
{
    [SerializeField] Card _selectedCard;
    [SerializeField] Card _hoveredCard;
    [SerializeField] GameObject _slotPrefab;
    [SerializeField] int _cardsToSpawn = 7;
    public List<Card> _cards;

    RectTransform rect;
    bool _isCrossing;
    [SerializeField] bool _tweenCardReturn = true;

    void Start()
    {
        for (int i = 0; i < _cardsToSpawn; i++)
        {
            Instantiate(_slotPrefab, transform);
        }

        rect = GetComponent<RectTransform>();
        _cards = GetComponentsInChildren<Card>().ToList();

        foreach (Card card in _cards)
        {
            card.BeginDragEvent.AddListener(BeginDrag);
            card.EndDragEvent.AddListener(EndDrag);
            card.PointerEnterEvent.AddListener(CardPointEnter);
            card.PointerExitEvent.AddListener(CardPointerExit);
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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (_hoveredCard != null)
            {
                Destroy(_hoveredCard.transform.parent.gameObject);
                _cards.Remove(_hoveredCard);
            }
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            foreach (Card card in _cards)
            {
                card.Deselect();
            }
        }

        if (_selectedCard == null)
            return;

        if (_isCrossing)
            return;

        for (int i = 0; i < _cards.Count; i++)
        {
            if (_selectedCard.transform.position.x > _cards[i].transform.position.x)
            {
                if (_selectedCard.ParentIndex() < _cards[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }

            if (_selectedCard.transform.position.x < _cards[i].transform.position.x)
            {
                if (_selectedCard.ParentIndex() > _cards[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }
        }
    }

    void BeginDrag(Card card)
    {
        _selectedCard = card;
    }

    void EndDrag(Card card)
    {
        if (_selectedCard == null)
            return;

        _selectedCard.transform.DOLocalMove
            (
                _selectedCard.selected ?
                    new Vector3(0, _selectedCard.selectionOffset, 0) : Vector3.zero,
                    _tweenCardReturn ? .15f : 0
            ).SetEase(Ease.OutBack);

        rect.sizeDelta += Vector2.right;
        rect.sizeDelta -= Vector2.right;

        _selectedCard = null;
    }

    void CardPointEnter(Card card)
    {
        _hoveredCard = card;
    }

    void CardPointerExit(Card card)
    {
        _hoveredCard = null;
    }

    void Swap(int index)
    {
        _isCrossing = true;

        Transform focusedParent = _selectedCard.transform.parent;
        Transform crossedParent = _cards[index].transform.parent;

        _cards[index].transform.SetParent(focusedParent);
        _cards[index].transform.localPosition = Vector2.zero;
        _selectedCard.transform.SetParent(crossedParent);

        _isCrossing = false;

        bool swapIsRight = _cards[index].ParentIndex() > _selectedCard.ParentIndex();
        _cards[index].cardVisual.Swap(swapIsRight ? -1 : 1);

        foreach (Card card in _cards)
        {
            card.cardVisual.UpdateIndex(transform.childCount);
        }        
    }
}