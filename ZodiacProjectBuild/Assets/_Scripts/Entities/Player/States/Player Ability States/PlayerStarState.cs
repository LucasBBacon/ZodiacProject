using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerStarState : PlayerAbilityState
{
    SOStarAbilityData _abilityData;
    
    Vector2 _starLightPos;

    // Effects
    bool _shouldBurst;
    float _cameraLensSize;
    GameObject _starlightShaft;
    Light2D _starlightShaftLight;
    GameObject _starlightFloor;
    Light2D _starlightFloorLight;

    public PlayerStarState(
        Player player,
        PlayerStateMachine stateMachine,
        SOStarAbilityData abilityData,
        string animBoolName,
        AbilityInputs input
        ) : base(player, stateMachine, animBoolName, input)
    {
        _abilityData = abilityData;
    }

    #region Callback Methods

    public override void StateEnter()
    {
        manaCost = _abilityData.ManaCost;
        cooldownTimer = _abilityData.AbilityCooldown;

        base.StateEnter();

        _starLightPos = CheckSpawnArea();

        _starlightShaft = GameManager.Instance.SpawnObjectOBJ(
            _abilityData.InitialLightPrefab,
            _starLightPos,
            Quaternion.identity
            );
        _starlightFloor = GameManager.Instance.SpawnObjectOBJ(
            _abilityData.FloorLightPrefab,
            _starLightPos,
            Quaternion.identity
            );
        
        _starlightShaftLight = _starlightShaft.GetComponentInChildren<Light2D>();
        _starlightFloorLight = _starlightFloor.GetComponent<Light2D>();

        _starlightShaftLight.pointLightInnerRadius = 0f;

        _cameraLensSize = CameraManager.instance.CurrentCamera.m_Lens.OrthographicSize;

        _shouldBurst = true;
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (isAbilityDone)
            ChangeState(player.IdleState);
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void HeldBehaviour()
    {
        base.HeldBehaviour();

        // clamp hold time
        if (inputHoldTime >= _abilityData.MinHoldTime + 0.1f)
            inputHoldTime = _abilityData.MinHoldTime + 0.1f;

        // effects
        else
        {
            CameraManager.instance.CurrentCamera.m_Lens.OrthographicSize -= Time.deltaTime / 10;

            if (_starlightShaftLight != null)
            {
                UpdateInitialLight();
            }
        }

        CameraManager.instance.CurrentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 1f;

        /*
        if (
            !(_inputHoldTime >= _abilityData.MinHoldTime
            && _inputHoldTime <= _abilityData.MinHoldTime + _abilityData.RumbleBurstTime
            && _shouldBurst)
            )
        {     
            RumbleManager.Instance.RumblePulse(
            _abilityData.RumbleConstLowFreq * _inputHoldTime /10,
            _abilityData.RumbleConstHighFreq * _inputHoldTime /10
            );
        }
        */

        if (inputHoldTime >= _abilityData.MinHoldTime && _shouldBurst)
        {
            player.StartCoroutine(LightFlash());

            RumbleManager.Instance.RumblePulse(
                _abilityData.RumbleBurstLowFreq,
                _abilityData.RumbleBurstHighFreq,
                _abilityData.RumbleBurstTime
                );
            _shouldBurst = false;
        }
    }

    public override void ReleaseBehaviour()
    {
        base.ReleaseBehaviour();

        CameraManager.instance.CurrentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
        CameraManager.instance.CurrentCamera.m_Lens.OrthographicSize = _cameraLensSize;

        RumbleManager.Instance.StopRumblePulse();

        if (inputHoldTime >= _abilityData.MinHoldTime)
        {
            InstantiateStarLight();
        }
        else
        {
            GameObject.Destroy(_starlightShaft);
            GameObject.Destroy(_starlightFloor);
            isAbilityDone = true;
        }
    }

    #endregion


    #region Check Methods

    public override bool CanCheck()
    {
        return base.CanCheck() && player.Stats.Mana.CurrentValue >= manaCost;
    }

    #endregion


    #region Functionality

    void InstantiateStarLight()
    {
        GameManager.Instance.SpawnObject(_abilityData.LightPrefab, _starLightPos);
        GameObject.Destroy(_starlightShaft);
        GameObject.Destroy(_starlightFloor);

        isAbilityDone = true;
        //Debug.Log("Got here");
    }

    Vector2 CheckSpawnArea()
    {
        float leftPoint = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, Camera.main.nearClipPlane)).x;
        float rightPoint = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0f, Camera.main.nearClipPlane)).x;

        return new Vector2(
            Random.Range(leftPoint, rightPoint),
            player.transform.position.y - (1.6f / 2)
            );
    } 

    

    #endregion


    #region Effects

    void UpdateInitialLight()
    {
        _starlightShaftLight.pointLightOuterRadius = Utilities.MappingUtil.Map(
            inputHoldTime,
            0, _abilityData.MinHoldTime,
            0, _abilityData.LightMaxOuterRadius,
            true
            );
        _starlightShaftLight.pointLightOuterAngle = Utilities.MappingUtil.Map(
            inputHoldTime,
            0, _abilityData.MinHoldTime,
            0, _abilityData.LightMaxOuterAngle,
            true
            );
        _starlightShaftLight.pointLightInnerAngle = Utilities.MappingUtil.Map(
            inputHoldTime,
            0, _abilityData.MinHoldTime,
            0, _abilityData.LightMaxInnerAngle,
            true
            );

        _starlightFloor.transform.localScale = new Vector3(
            Utilities.MappingUtil.Map(
                inputHoldTime,
                0, _abilityData.MinHoldTime,
                0, 1f,
                true
                ),
            Utilities.MappingUtil.Map(
                inputHoldTime,
                0, _abilityData.MinHoldTime,
                0, 0.34f,
                true
                ),
            0f
            );
        _starlightFloorLight.intensity = Utilities.MappingUtil.Map(
            inputHoldTime,
            0, _abilityData.MinHoldTime,
            0, 1,
            true
            );
    }

    IEnumerator LightFlash()
    {
        float lightIncreaseTimer = 0;
        while (lightIncreaseTimer <= _abilityData.LightShaftFlashTimer)
        {
            lightIncreaseTimer += Time.deltaTime;

            _starlightShaftLight.pointLightInnerRadius = Mathf.Lerp(
                _starlightShaftLight.pointLightInnerRadius,
                _starlightShaftLight.pointLightOuterRadius,
                lightIncreaseTimer / _abilityData.LightShaftFlashTimer
                );

            yield return null;
        }
    }

    #endregion
}
