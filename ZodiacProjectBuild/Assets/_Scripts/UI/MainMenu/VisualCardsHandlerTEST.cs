using UnityEngine;

public class VisualCardsHandlerTEST : MonoBehaviour
{
    public static VisualCardsHandlerTEST instance;

    private void Awake() {
        if (instance == null)
            instance = this;
    }
}