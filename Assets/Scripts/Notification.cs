using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    public Image Background;
    public TextMeshProUGUI Text;

    void Start()
    {
        Background.gameObject.SetActive(false);
    }

    public void Notify(string message, Color colour)
    {
        //Update text, text colour and image size
        Text.text = message;
        Text.color = colour;
        Resize(message);
        //Activate notification
        Background.gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(Deactivate(3));
    }

    /// <summary>
    /// Deactivates the notification gameobject after given seconds
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    private IEnumerator Deactivate(float seconds)
    { 
        yield return new WaitForSecondsRealtime(seconds);
        Background.gameObject.SetActive(false);
    }

    private void Resize(string message)
    {
        //Resize image to fit message
        float height = Background.rectTransform.sizeDelta.y;
        Background.rectTransform.sizeDelta = new Vector2(message.Length * 15, height);
    }
}
