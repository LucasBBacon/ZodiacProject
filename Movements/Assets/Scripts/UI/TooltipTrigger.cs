using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string content;
    public string header;
    private static float delay = 0.5f;
    private Coroutine DelayCoroutine;

    public void OnPointerEnter(PointerEventData eventData)
    {
        DelayCoroutine = StartCoroutine(TooltipDelay());
        TooltipSystem.Show(content, header);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopCoroutine(TooltipDelay());
        TooltipSystem.Hide();
    }

    private IEnumerator TooltipDelay()
    {
        float elapsedTime = 0f;
        while(elapsedTime < delay)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
