using UnityEngine;
using UnityEngine.UI;

public class LevelDesignButton : LevelButton
{
    public Button Options;
    public Button Delete;

    private bool DeleteState;

    void Awake()
    {
        //Add listeners to buttons
        Name.onClick.AddListener(EditLevel);
        Options.onClick.AddListener(ToggleOptions);
        Delete.onClick.AddListener(DeleteLevel);
    }

    private void ToggleOptions()
    {
        //Switch between name and delete state
        if (DeleteState)
        {
            Delete.gameObject.SetActive(false);
            Name.gameObject.SetActive(true);
        }
        else
        {
            Name.gameObject.SetActive(false);
            Delete.gameObject.SetActive(true);
        }
        DeleteState = !DeleteState;
    }

    private void DeleteLevel()
    {
        //Delete level from file and destroy this button
        FileHandler.DeleteLevel(LevelName);
        Destroy(gameObject);
        //Update the menu
        LevelLoader.Refresh();
        MainMenu.RefreshLevels();
    }

    public void EditLevel()
    {
        //Prepare gamedata for level design
        GameData.SetDesign(LevelLoader.UserLevels[LevelName]);
        GameData.LevelFileName = LevelName;
        //Load level design environment
        SceneSwitch.LoadDesign();
    }
}
