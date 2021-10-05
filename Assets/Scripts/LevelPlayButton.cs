public class LevelPlayButton : LevelButton
{
    void Start()
    {
        //Add listeners to buttons
        Name.onClick.AddListener(LoadLevel);
    }

    public void LoadLevel()
    {
        GameData.SetLevel(LevelLoader.Levels[LevelName]);
        GameData.LevelFileName = LevelName;
        SceneSwitch.LoadWorld();
    }
}