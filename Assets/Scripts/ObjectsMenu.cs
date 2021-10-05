using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectsMenu : MonoBehaviour
{
    public GameObject ObjectPrefab;
    [Header("Inspector")]
    public Inspector inspector;
    public Transform ObjectContent;

    private CurrentAssets currentAssets;

    void Start()
    {
        currentAssets = SceneAssets.currentAssets;
    }

    public void OpenObjectMenu()
    {
        inspector.Activate(true);
        inspector.OpenMenu("Objects");
        LoadObjectsIntoMenu();
    }

    private void LoadObjectsIntoMenu()
    {
        foreach (ParentData objectData in HandleUserObjects.ObjectDatas)
        {
            CreateNewObjectButton(objectData);
        }
    }

    private void CreateNewObjectButton(ParentData objectData)
    {
        GameObject newObjectButton = Instantiate(ObjectPrefab);
        newObjectButton.GetComponentInChildren<TextMeshProUGUI>().text = objectData.Name;
        newObjectButton.transform.SetParent(ObjectContent);

        //Edit tag listener
        //newTag.GetComponentsInChildren<Button>()[0].onClick.AddListener(() =>
        //{
        //    EditTag(newTag.GetComponentInChildren<TextMeshProUGUI>());
        //});

        //Delete object listener
        newObjectButton.GetComponentsInChildren<Button>()[1].onClick.AddListener(() =>
        {
            DeleteObject(objectData.Name, newObjectButton);
        });
    }

    private void DeleteObject(string Name, GameObject ObjectButton)
    {
        //Get destination
        string Destination = Application.persistentDataPath + "/Objects/" + Name + ".bytes";

        //Delete file with destination
        File.Delete(Destination);

        //Remove button
        Destroy(ObjectButton);
    }
}
