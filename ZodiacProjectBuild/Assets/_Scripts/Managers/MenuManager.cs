using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject _pauseMenu;

    private void Start()
    {
        _pauseMenu.SetActive(false);
    }

    private void Update() {
        if (InputManager.instance.MenuOpen)
        {
            if (!PauseManager.instance.IsPaused)
            {
                Pause();
            }
        }
        else if (InputManager.instance.UIMenuClose)
        {
            if (PauseManager.instance.IsPaused)
            {
                UnPause();
            }
        }
    }

    public void Pause()
    {
        PauseManager.instance.PauseGame();
        OpenMainMenu();
    }

    public void UnPause()
    {
        PauseManager.instance.UnpauseGame();
        CloseAllMenus();
    }

    void OpenMainMenu()
    {
        _pauseMenu.SetActive(true);
    }

    void CloseAllMenus()
    {
        _pauseMenu.SetActive(false);
    }
}
