using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HealingBeam : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] SOStarAbilityData _abilityData;

    [Header("Lights")]
    [SerializeField] Light2D _shaftLight;
    [SerializeField] Light2D _floorLight;


    bool _startCooldown;
    bool _isHealing;
    float _startTime;
    
    Player player;

    #region Callback Methods
    
    private void OnEnable()
    {
        _startTime = Time.time;
        Debug.Log(_startTime);
    }

    private void Update()
    {
        if (Time.time >= _startTime + _abilityData.HealingBeamActiveTime)
        {
            StartCoroutine(LightFlashOut());
        }   
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            _startCooldown = true;

            _isHealing = true;
            player = collision.GetComponent<Player>();
            StartCoroutine(WaitCooldown());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            _isHealing = false;
            StopCoroutine(WaitCooldown());
        }
    }

    #endregion
    

    #region Effect Timers

    IEnumerator WaitCooldown()
    {
        while (_isHealing && _startCooldown)
        {
            yield return new WaitForSeconds(_abilityData.HealingBeamInterHealCooldown);
            player.Heal(_abilityData.HealingBeamHealFactor);
        }

        _startCooldown = false;
    }

    IEnumerator LightFlashOut()
    {
        float lightDecreaseTimer = 0;
        while (lightDecreaseTimer <= _abilityData.LightShaftFlashTimer)
        {
            lightDecreaseTimer += Time.deltaTime;

            _shaftLight.intensity = Mathf.Lerp(
                _shaftLight.intensity, 0,
                lightDecreaseTimer / _abilityData.LightShaftFlashTimer
                );
            _shaftLight.pointLightOuterRadius = Mathf.Lerp(
                _shaftLight.pointLightOuterRadius, 0,
                lightDecreaseTimer / _abilityData.LightShaftFlashTimer
                );
            _shaftLight.pointLightOuterAngle = Mathf.Lerp(
                _shaftLight.pointLightOuterAngle, 0,
                lightDecreaseTimer / _abilityData.LightShaftFlashTimer
                );
            _shaftLight.pointLightInnerAngle = Mathf.Lerp(
                _shaftLight.pointLightInnerAngle, 0,
                lightDecreaseTimer / _abilityData.LightShaftFlashTimer
                );
            _shaftLight.pointLightInnerRadius = 0f;

            _floorLight.intensity = Mathf.Lerp(
                _floorLight.intensity, 0,
                lightDecreaseTimer / _abilityData.LightShaftFlashTimer
                );

            yield return null;
        }

        Destroy(gameObject);
    }

    #endregion
}
