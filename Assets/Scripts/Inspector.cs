using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class Inspector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject inspector;
    public bool InitiallyActive = false;
    public List<GameObject> DeactivateObjectsOnOpen;

    private Stack<string> States = new Stack<string>();
    private List<GameObject> SubMenus;
    public TextMeshProUGUI NotificationText;

    void Start()
    {
        SubMenus = FindChildrenWithTag(inspector.transform, "SubMenu");

        inspector.SetActive(InitiallyActive);
        ResetInspector();
    }

    public void ResetInspector()
    {
        //Deactivate all UI elements with tag SubMenu
        foreach (GameObject SubMenu in SubMenus)
            SubMenu.SetActive(false);
    }

    /// <summary>
    /// Activate or deactivate the inspector
    /// </summary>
    /// <param name="value"></param>
    public void Activate(bool value)
    {
        inspector.SetActive(value);
        if (!value)
            ResetInspector();

        foreach (GameObject ActiveObject in DeactivateObjectsOnOpen)
            ActiveObject.SetActive(!value);

        GameData.ControlsEnabled = false;
    }

    private bool ChangeMenu(string name)
    {
        ResetInspector();
        //Activate UI element of given name with SubMenu tag
        foreach (GameObject Menu in SubMenus)
        {
            if (Menu.name == name)
            {
                Menu.SetActive(true);
                return true;
            }
        }
        Debug.Log("Menu not Found", inspector);
        return false;
    }

    public void OpenMenu(string name)
    {        
        if (!ChangeMenu(name))
            return;

        inspector.SetActive(true);
        States.Clear();
        States.Push(name);
    }

    public void AddLowerMenu(string name)
    {
        if (!ChangeMenu(name))
            return;

        States.Push(name);
    }

    public void Back()
    {
        //Remove current state from stack
        States.Pop();
        //Close menu if no previous state
        if (States.Count == 0)
        {
            ResetInspector();
            Activate(false);
            GameData.ControlsEnabled = true;
            return;
        }
        //Set menu to previous state
        ChangeMenu(States.Peek());
    }

    public IEnumerator<WaitForSecondsRealtime> Notification(string message, float seconds = 2)
    {
        NotificationText.text = message;
        NotificationText.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(seconds);
        NotificationText.transform.parent.gameObject.gameObject.SetActive(false);
    }

    private List<GameObject> FindChildrenWithTag(Transform parent, string tag)
    {
        List<GameObject> Children = new List<GameObject>();
        foreach (Transform child in parent)
            if (child.tag == tag)
                Children.Add(child.gameObject);

        return Children;
    }

    private GameObject FindChildWithName(Transform parent, string name)
    {
        foreach (Transform child in parent)
            if (child.name == name)
                return child.gameObject;

        return null;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        GameData.ControlsEnabled = false;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        GameData.ControlsEnabled = true;
    }
}