using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public Image LoadBar;

    private Scene scene;
    private float currentLevel = 0;

    private void Start()
    {
        if (GameData.SceneOperation != null)
        {
            scene = SceneManager.GetSceneAt(1);
            LoadBar.fillAmount = 0;
        }
    }

    private void Update()
    {
        if (GameData.SceneOperation != null)
        {
            float progress = GameData.SceneOperation.progress;
            LoadBar.fillAmount = progress;

            if (GameData.SceneOperation.progress == 0.9f)
            {
                GameData.SceneOperation.allowSceneActivation = true;
            }

            if (scene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(9);
                SceneManager.SetActiveScene(scene);
            }
        }
    }
}
