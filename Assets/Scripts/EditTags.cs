using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EditTags : MonoBehaviour
{
    public Inspector inspector;
    [Header("Tag prefab")]
    public GameObject TagPrefab;
    public Transform TagsContent;
    public TMP_InputField NewTagField;

    private CurrentAssets currentAssets;

    void Start()
    {
        //Add a reference to the current assets
        currentAssets = SceneAssets.currentAssets;

        List<string> Tags = new List<string>();
        LoadTagsIntoMenu();
    }

    public void OpenTagMenu()
    {
        inspector.Activate(true);
        inspector.OpenMenu("All Tags");
        LoadTagsIntoMenu();
    }

    public void LoadTagsIntoMenu()
    {
        ClearPanel();
        foreach(string tagName in currentAssets.Tags)
        {
            CreateNewTag(tagName);
        }
    }

    private void ClearPanel()
    {
        //Destroy each child of panel content but add new button
        foreach (Transform tagButton in TagsContent)
            if (tagButton.name != "NewTag")
                Destroy(tagButton.gameObject);
    }

    public void SaveTag()
    {
        string tagName = NewTagField.textComponent.text;
        //Check tag is not empty
        if (NewTagField.text == "")
        {
            StartCoroutine(inspector.Notification("Tag must contain at least one character"));
            return;
        }
        //Check tag does not already exist
        if (currentAssets.Tags.Contains(tagName))
        {
            StartCoroutine(inspector.Notification("Tag already exists"));
            return;
        }

        //Save tag to current assets
        currentAssets.Tags.Add(tagName);

        //Add tag to menu
        CreateNewTag(tagName);
    }

    private void CreateNewTag(string tagName)
    {
        GameObject newTag = Instantiate(TagPrefab);
        newTag.GetComponentInChildren<TextMeshProUGUI>().text = tagName;
        newTag.transform.SetParent(TagsContent);
        newTag.transform.localScale = transform.root.localScale;
        //Edit tag listener
        //newTag.GetComponentsInChildren<Button>()[0].onClick.AddListener(() =>
        //{
        //    EditTag(newTag.GetComponentInChildren<TextMeshProUGUI>());
        //});
        //Delete tag listener
        newTag.GetComponentsInChildren<Button>()[1].onClick.AddListener(() =>
        {
            DeleteTag(newTag.GetComponentInChildren<TextMeshProUGUI>());
        });
    }

    public void DeleteTag(TextMeshProUGUI text)
    {
        currentAssets.Tags.Remove(text.text);
        Destroy(text.transform.parent.parent.gameObject);
    }

    public void EditTag(TextMeshProUGUI text)
    {
        inspector.AddLowerMenu("Edit Tag");
        NewTagField.textComponent.text = text.text;
    }
}
