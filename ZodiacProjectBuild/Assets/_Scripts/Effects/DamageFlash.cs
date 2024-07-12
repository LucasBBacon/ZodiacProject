using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [ColorUsage(true, true)]
    [SerializeField] Color _flashColor = Color.white;
    [SerializeField] float _flashTime = 0.25f;
    [SerializeField] AnimationCurve _flashSpeedCurve;

    SpriteRenderer _spriteRenderer;
    private Material _material;

    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Init();
    }

    private void Init()
    {
        _material = _spriteRenderer.material;
    }

    public void CallDamageFlash()
    {
        StartCoroutine(DamageFlashing());
    }

    IEnumerator DamageFlashing()
    {
        // set the colour
        SetFlashColour();

        // lerp the flash
        float currentFlashAmount = 0f;
        float elapseFlashTime = 0f;

        while (elapseFlashTime < _flashTime)
        {
            // iterate elapsedTime
            elapseFlashTime += Time.deltaTime;

            // lerp the flash amount
            currentFlashAmount = Mathf.Lerp(1f, _flashSpeedCurve.Evaluate(elapseFlashTime), elapseFlashTime / _flashTime);
            SetFlashAmount(currentFlashAmount);

            yield return null;
        }
    }

    void SetFlashColour()
    {
        _material.SetColor("_FlashColour", _flashColor);
    }

    void SetFlashAmount(float amount)
    {
        _material.SetFloat("_FlashAmount", amount);
    }
}
