using UnityEngine;

public class GameMenu : MonoBehaviour
{
    public GameObject Panel;
    public GameObject Main;
    public GameObject Settings;
    public bool PauseOnOpen;

    private bool IsPlaying;

    void Start()
    {
        //Panel
        if (Panel)
            Panel.SetActive(false);
        else
            Debug.LogError("No Panel");
        //Menu
        if (Main)
            Main.SetActive(true);
        //Settings
        if (Settings)
            Settings.SetActive(false);
    }

    private void OnDestroy()
    {
        GameData.ControlsEnabled = true;
    }

    public void PlayAgain()
    {
        GameData.ResetLevel();
        SceneSwitch.LoadWorld();
    }

    public void LoadMainMenu()
    {
        GameData.ControlsEnabled = true;
        SceneSwitch.LoadMenu();
    }

    public void Open()
    {
        //Open
        Panel.SetActive(true);
        //Stop player controls and pause
        GameData.ControlsEnabled = false;
        GameData.CanSelect = false;
        IsPlaying = PhysicsControl.IsPlaying;
        if (PauseOnOpen)
            PhysicsControl.Stop();
    }

    public void OpenSettings(bool value)
    {
        //Open
        Main.SetActive(!value);
        Settings.SetActive(value);
    }

    public void Back()
    {
        if (Settings.activeSelf)
            //Close settings
            OpenSettings(false);
        else
        {
            //Close
            Panel.SetActive(false);
            //Allow player controls and reset play state
            GameData.ControlsEnabled = true;
            GameData.CanSelect = true;
            if (PauseOnOpen)
                if (IsPlaying)
                    PhysicsControl.Play();
                else
                    PhysicsControl.Stop();
        }
    }
}
