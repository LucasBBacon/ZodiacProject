using UnityEngine;

public class DoorTriggerInteraction : TriggerInteractBase
{
    public enum DoorToSpawnAt
    {
        None,
        One,
        Two,
        Three,
        Four
    }

    [Header("Spawn To")]
    [SerializeField] DoorToSpawnAt DoorToSpawnTo; 
    [SerializeField] SceneField _sceneToLoad;

    [Space(10f)]
    [Header("This door")]
    public DoorToSpawnAt currentDoorPosition;

    public override void Interact()
    {
        base.Interact();

        // load new scene
        SceneSwapManager.SwapSceneFromDoorUse(_sceneToLoad, DoorToSpawnTo);
    }
}
