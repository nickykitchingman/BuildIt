public class UserLevelPlayButton : LevelButton
{
    void Start()
    {
        //Add listeners to buttons
        Name.onClick.AddListener(LoadLevel);
    }

    public void LoadLevel()
    {
        GameData.SetLevel(LevelLoader.UserLevels[LevelName]);
        GameData.LevelFileName = LevelName;
        SceneSwitch.LoadWorld();
    }
}
