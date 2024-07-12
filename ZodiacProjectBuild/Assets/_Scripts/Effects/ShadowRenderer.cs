using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ShadowRenderer : MonoBehaviour
{
    public Vector3 Offset = new Vector3(-0.1f, -0.1f);
    public Material Material;

    GameObject _shadow;
    SpriteRenderer sr;
    SpriteRenderer originalRenderer;
    Movement movement;
    

    private void Start()
    {
        movement = GetComponentInParent<Movement>();
        movement.TurnEvent.AddListener(FlipShadow);

        _shadow = new GameObject("Shadow");
        _shadow.transform.parent = transform;

        _shadow.transform.localPosition = Offset;
        _shadow.transform.localRotation = Quaternion.identity;

        originalRenderer = GetComponent<SpriteRenderer>();
        sr = _shadow.AddComponent<SpriteRenderer>();
        sr.sprite = originalRenderer.sprite;
        sr.material = Material;

        sr.sortingLayerName = originalRenderer.sortingLayerName;
        sr.sortingOrder = originalRenderer.sortingOrder - 1;
    }

    private void Update()
    {
        sr.sprite = originalRenderer.sprite;
    }

    private void LateUpdate()
    {
        _shadow.transform.localPosition = Offset;    
    }

    public void FlipShadow()
    {
        Debug.Log("FlipShadow");
        Offset.x *= -1;
    }
}
