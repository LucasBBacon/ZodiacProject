using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityCardMiddleHolder : MonoBehaviour
{
    public SOAbilityCard cardData;
    [SerializeField] GameObject _returnObject;
    [SerializeField] AbilitySlot[] _abilitySlots;
    [SerializeField] float moveTime = 5f;

    Image _cardImage;
    Vector2 endPos;

    private void Start()
    {
        _cardImage = GetComponentInChildren<Image>();
        _cardImage.gameObject.SetActive(false);
    }

    public void CardRemovedFromMiddle(bool isAbilitySlot)
    {
        if (!isAbilitySlot)
        {
            foreach (AbilitySlot slot in _abilitySlots)
            {
                slot.ReturnCardToDeck();
            }
        }
        

        _cardImage.sprite = null;
        cardData = null;

        if (cardData == null)
        {
            foreach (AbilitySlot slot in _abilitySlots)
            {
                slot.selectButton.enabled = false;
                slot.selectImage.enabled = false;
            }
        }

        _cardImage.gameObject.SetActive(false);
        _returnObject.gameObject.SetActive(false);
    }    

    public void CardPlacedInMiddle(SOAbilityCard cardData)
    {
        EventSystem.current.SetSelectedGameObject(_abilitySlots[0].gameObject);

        this.cardData = cardData;

        foreach (AbilitySlot slot in _abilitySlots)
        {
            slot.PlaceCardInMiddle();
        }

        _cardImage.gameObject.SetActive(true);
        _cardImage.sprite = this.cardData.CardImage;
    }

    public void MoveMiddle(float xPos)
    {
        Vector2 currentPos = gameObject.transform.position;
        endPos = new Vector2(xPos, gameObject.transform.position.y);

        //gameObject.transform.position = endPos;

        StopCoroutine(MoveAnimation());

        StartCoroutine(MoveAnimation());
    }

    IEnumerator MoveAnimation()
    {
        float timerToMove = 0;
        while (timerToMove <= moveTime)
        {
            timerToMove += Time.deltaTime;
            float xPos = Mathf.Lerp(gameObject.transform.position.x, endPos.x, timerToMove / moveTime);
            //Debug.Log(xPos);
            gameObject.transform.position = new Vector2(xPos, gameObject.transform.position.y);
            yield return null;
        }
    }
}
