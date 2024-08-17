using System;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public event Action OnFinish;
    public event Action OnStartMovement;
    public event Action OnStopMovement;
    public event Action OnAttackAction;
    public event Action OnMinHoldPassed;

    /// <summary>
    /// Trigger used to indicate when the input should be "used".
    /// </summary>
    /// <remarks>
    /// Once the input is "used", the input must be released and pressed again in order to trigger it.
    /// Generally, this animation event is added to the first "action" frame of an animation.
    /// </remarks>
    /// <example>
    /// Used in the firs sword strike frame. Used in the frame where the bow is released.
    /// </example>
    public event Action OnUseInput;

    public event Action OnEnableInterrupt;
    
    public event Action<bool> OnSetOptionalSpriteActive;

    public event Action<bool> OnFlipSetActive;

    public event Action<AttackPhases> OnEnterAttackPhase;

    public event Action<AnimationWindows> OnStartAnimationWindow;
    public event Action<AnimationWindows> OnStopAnimationWindow;

    void AnimationEventFinishedTrigger() => OnFinish?.Invoke();
    void MovementStartTrigger() => OnStartMovement?.Invoke();
    void MovementStopTrigger() => OnStopMovement?.Invoke();
    void AttackActionTrigger() => OnAttackAction?.Invoke();
    void MinHoldPassedTrigger() => OnMinHoldPassed?.Invoke();
    void UseInputTrigger() => OnUseInput?.Invoke();

    void SetOptionalSpriteEnabled() => OnSetOptionalSpriteActive?.Invoke(true);
    void SetOptionalSpriteDisabled() => OnSetOptionalSpriteActive?.Invoke(false);

    void SetFlipActive() => OnFlipSetActive?.Invoke(true);
    void SetFlipInactive() => OnFlipSetActive?.Invoke(false);

    void EnterAttackPhase(AttackPhases phase) => OnEnterAttackPhase?.Invoke(phase);

    void StartAnimationWindow(AnimationWindows window) => OnStartAnimationWindow?.Invoke(window);
    void StopAnimationWindow(AnimationWindows window) => OnStopAnimationWindow?.Invoke(window);

    void EnableInterrupt() => OnEnableInterrupt?.Invoke();
}
