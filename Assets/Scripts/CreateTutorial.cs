using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CreateTutorial : MonoBehaviour
{
    public PlayTutorial playTutorial;

    public List<GameObject> Steps;
    public Transform StepParent;
    public List<GameObject> Responses;
    public Transform ResponseParent;

    public float TextMaxExpand;
    public int TextExpandSpeed;
    public GameObject TextPrefab;
    public Color TextColour;
    public TMP_FontAsset TextFont;
    
    public float ImageMaxExpand;
    public int ImageExpandSpeed;
    public Sprite DefaultImage;
    
    public float ResponseMaxExpand;
    public int ResponseExpandSpeed;

    /// <summary>
    /// Creates new step on the end 
    /// </summary>
    /// <param name="actionOptions"></param>
    public void CreateNewStep(ActionComponent actionOptions)
    {
        CreateNewStep(actionOptions, Steps.Count);
    }

    /// <summary>
    /// Creates new step at given index
    /// </summary>
    /// <param name="actionOptions"></param>
    /// <param name="index"></param>
    public void CreateNewStep(ActionComponent actionOptions, int index)
    {
        //Deactivate all steps
        ActivateAllSteps(false);

        //Create new step
        GameObject newStep = new GameObject("Step " + index, typeof(RectTransform));
        newStep.transform.SetParent(StepParent);
        newStep.transform.localScale = Vector3.one;
        newStep.transform.localPosition = Vector3.zero;

        //Add actions as components
        List<Type> actions = actionOptions.ConvertToList();
        foreach (Type action in actions)
            newStep.AddComponent(action);

        //Add text
        AddText(newStep.transform);

        //Add image
        AddImage(newStep.transform);

        //Add to list
        Steps.Insert(index, newStep);

        //Update names
        RenameSteps();
        UpdateSteps();

        //Select it
        EventSystem.current.SetSelectedGameObject(newStep);
    }
    
    public void CreateNewResponse(string word, Color colour)
    {
        //Create new step
        GameObject newResponse = Instantiate(TextPrefab);
        newResponse.name = "Response " + Responses.Count;
        newResponse.transform.SetParent(ResponseParent);
        newResponse.transform.position = Vector3.zero;

        //Text
        TextMeshProUGUI text = newResponse.GetComponent<TextMeshProUGUI>();
        if (text)
        {
            text.text = word;
            text.color = colour;
        }

        //Add expand
        Expand expand = newResponse.GetComponent<Expand>();
        if (!expand)
            expand = newResponse.AddComponent<Expand>();
        expand.MaxScale = ResponseMaxExpand;
        expand.Speed = ResponseExpandSpeed;

        //Add to list
        Responses.Add(newResponse);

        //Update names
        RenameResponses();
        UpdateResponses();

        //Select it
        EventSystem.current.SetSelectedGameObject(newResponse);
    }

    public void ActivateAllSteps(bool value)
    {
        Steps.ForEach(f => f.SetActive(value));
    }
    
    public void ActivateAllResponses(bool value)
    {
        foreach (GameObject response in Responses)
            response.SetActive(value);
    }

    public void UpdateAllTexts()
    {
        //Get all text components
        foreach (GameObject step in Steps)
        {
            foreach (TextMeshProUGUI text in step.GetComponentsInChildren<TextMeshProUGUI>())
            {
                text.color = TextColour;
                //Update every expand component
                Expand component = text.GetComponent<Expand>();
                if (component)
                {
                    component.MaxScale = TextMaxExpand;
                    component.Speed = TextExpandSpeed;
                }
            }
        }
    }
    
    public void UpdateAllImageExpands()
    {
        //Get all text components
        foreach (GameObject step in Steps)
        {
            foreach (Image image in step.GetComponentsInChildren<Image>())
            {
                //Update every expand component
                Expand component = image.GetComponent<Expand>();
                if (component)
                {
                    component.MaxScale = ImageMaxExpand;
                    component.Speed = ImageExpandSpeed;
                }
            }
        }
    }  
    
    public void UpdateAllResponseExpands()
    {
        //Get all text components
        foreach (GameObject response in Responses)
        {
            //Update every expand component
            Expand component = response.GetComponent<Expand>();
            if (component)
            {
                component.MaxScale = ResponseMaxExpand;
                component.Speed = ResponseExpandSpeed;
            }
        }
    }

    public void SetLists()
    {
        if (playTutorial)
        {
            Steps = playTutorial.Steps;
            Responses = playTutorial.Responses;
        }
    }
    
    private void RenameSteps()
    {
        if (Steps != null)
            for (int index = 0; index < Steps.Count; index++)
                Steps[index].name = "Step " + (index + 1);
    }
    
    private void RenameResponses()
    {
        if (Responses != null)
            for (int index = 0; index < Responses.Count; index++)
                Responses[index].name = "Response " + (index + 1);
    }

    private void AddText(Transform parent)
    {
        GameObject obj = Instantiate(TextPrefab);
        obj.name = "Text";
        TextMeshProUGUI text = obj.GetComponent<TextMeshProUGUI>();
        if (text)
            text.color = TextColour;

        //Expand
        Expand expand = obj.GetComponent<Expand>();
        if (!expand)
            expand = obj.AddComponent<Expand>();
        expand.MaxScale = TextMaxExpand;
        expand.Speed = TextExpandSpeed;

        //Set parent
        obj.transform.SetParent(parent);
        obj.transform.localPosition = Vector3.zero;
    }
    
    private void AddImage(Transform parent)
    {
        GameObject obj = new GameObject("Image", typeof(RectTransform));

        //Image
        Image image = obj.AddComponent<Image>();
        image.sprite = DefaultImage;
        obj.name = image.sprite.name;

        //Expand
        Expand expand = obj.AddComponent<Expand>();
        expand.MaxScale = TextMaxExpand;
        expand.Speed = TextExpandSpeed;

        //Set parent
        obj.transform.SetParent(parent);
        obj.transform.localPosition = Vector3.zero;
    }

    public void UpdateSteps()
    {
        if (Steps != null)
        {
            Steps.Clear();

            foreach (Transform child in StepParent)
                if (child.parent == StepParent)
                    Steps.Add(child.gameObject);

            RenameSteps();
        }
    }
    
    public void UpdateResponses()
    {
        if (Responses != null)
        {
            Responses.Clear();

            foreach (Transform child in ResponseParent)
                if (child.parent == ResponseParent)
                    Responses.Add(child.gameObject);

            RenameResponses();
        }
    }
}

public class ActionComponent
{
    public bool button;
    public bool select;
    public bool wait;
    public bool key;
    public bool look;
    public bool box;
    public bool boxes;
    public bool sphere;
    public bool text;
    public bool texts;
    public bool dropdown;
    public bool tmpdropdown;

    public List<Type> ConvertToList()
    {
        List<Type> actions = new List<Type>();

        //Add actions as list of types
        if (button)
            actions.Add(typeof(ButtonAction));
        if (select)
            actions.Add(typeof(SelectAction));
        if (wait)
            actions.Add(typeof(WaitAction));
        if (key)
            actions.Add(typeof(KeyPressAction));
        if (look)
            actions.Add(typeof(LookAtAction));
        if (box)
            actions.Add(typeof(BoxAreaAction));
        if (boxes)
            actions.Add(typeof(BoxAreasAction));
        if (sphere)
            actions.Add(typeof(SphereAreaAction));
        if (text)
            actions.Add(typeof(TextAction));
        if (texts)
            actions.Add(typeof(TextsAction));
        if (dropdown)
            actions.Add(typeof(DropdownAction));
        if (tmpdropdown)
            actions.Add(typeof(TMPDropdownAction));

        return actions;
    }
}
