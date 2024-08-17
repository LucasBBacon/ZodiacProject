using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwapManager : MonoBehaviour
{
    public static SceneSwapManager instance;

    DoorTriggerInteraction.DoorToSpawnAt _doorToSpawnTo;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public static void SwapSceneFromDoorUse(SceneField myScene, DoorTriggerInteraction.DoorToSpawnAt doorToSpawnAt)
    {
        instance.StartCoroutine(instance.FadeOutThenChangeScene(myScene, doorToSpawnAt));
    }

    IEnumerator FadeOutThenChangeScene(SceneField myScene, DoorTriggerInteraction.DoorToSpawnAt doorToSpawnAt = DoorTriggerInteraction.DoorToSpawnAt.None)
    {
        // begins fading to black
        SceneFadeManager.instance.StartFadeOut();

        while (SceneFadeManager.instance.IsFadingOut)
        {
            yield return null;
        }

        _doorToSpawnTo = doorToSpawnAt;
        SceneManager.LoadScene(myScene);
    }
}
