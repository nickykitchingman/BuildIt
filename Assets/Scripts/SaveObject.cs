using UnityEngine;
using TMPro;

public class SaveObject : MonoBehaviour
{
    private string Name;

    public GameObject Object;
    public Notification notification;
    [Header("Saving")]
    public TMP_InputField SaveInputField;
    public GameObject SaveObjectPanel;
    public GameObject ObjectSavedText;
    
    public void Save()
    {
        UpdateName();
        UpdateCenter();

        if (Name.Length < 1)
        {
            notification.Notify("Name must be at least one character long", Color.red);
            return;
        }

        if (Object.transform.childCount < 1)
        {
            notification.Notify("Must be at least one object to save", Color.red);
            return;
        }

        if (FileHandler.SaveObject(Object))
            notification.Notify("Object Saved", Color.green);
        else
            notification.Notify("Object save failed", Color.red);

        ClosePanel();
    }

    void Update()
    {
        if (SaveObjectPanel.activeSelf)
        {
            //Confirm
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ClosePanel();
                Save();
            }

            //Exit
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ClosePanel();
            }
        }
    }

    /// <summary>
    /// Updates name using save panel input field
    /// </summary>
    public void UpdateName()
    {
        UpdateName(SaveInputField.text);
    }
    
    /// <summary>
    /// Updates name by parameter
    /// </summary>
    /// <param name="name"></param>
    public void UpdateName(string name)
    {
        Name = name;
        Object.name = name;
    }
    

    private void UpdateCenter()
    {
        Vector3 center = Object.GetComponent<Rigidbody>().centerOfMass;

        //Move object to center of object
        Object.transform.position = center;

        foreach (Transform child in Object.transform)
        {
            child.position -= center;
        }
    }

    public void ClosePanel()
    {
        SaveObjectPanel.SetActive(false);
        SaveObjectPanel.SetActive(false);
        GameData.ControlsEnabled = true;
    }

    public void OpenSaveName()
    {
        if (GameData.IsRenderingNew || GameData.IsRenderingOld)
        {
            notification.Notify("Cannot save while selecting object!", Color.red);
            return;
        }

        SaveInputField.text = Name;
        SaveObjectPanel.SetActive(true);
        SaveObjectPanel.SetActive(true);
        GameData.ControlsEnabled = false;
        SaveInputField.Select();
    }
}