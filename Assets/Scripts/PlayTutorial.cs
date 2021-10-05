using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Reflection;

public class PlayTutorial : MonoBehaviour
{
    [Header("Tutorial")]
    public List<GameObject> Responses;
    public List<GameObject> Steps;
    public bool Editor;
    [Header("UI")]
    public GameObject[] UIElements;
    public GameObject[] Ignore;
    [Header("Scene Objects")]
    public GameObject[] TutorialObjects;
    [Header("Scripts")]
    public PreRenderObject RenderScript;

    private GameObject currentStep;
    private int step = -1;
    private bool end = false;
    private bool loopResponses;
    private float timer;
    private float delay = 1;
    private int element = -1;

    bool[] activeResponses;

    private void Start()
    {
        activeResponses = new bool[Responses.Count];

        ReadyUp();
        if (Editor)
            step = -1;
        else
            step = GameData.TutorialState;
    }

    private void OnDestroy()
    {
        GameData.CanConfirm = true;
        GameData.NumpadEnabled = true;
        GameData.ControlsEnabled = true;
        GameData.CanSelect = true;
    }

    private void OnDisable()
    {
        GameData.CanConfirm = true;
        GameData.NumpadEnabled = true;
        GameData.ControlsEnabled = true;
        GameData.CanSelect = true;
    }

    private void Update()
    {
        if (loopResponses)
        {
            if (timer > delay)
            {
                GiveResponse(2f);
                timer = 0;
            }
            timer += Time.deltaTime;
        }

        if (end)
            return;

        if (currentStep == null)
        {
            //First or end
            NextStep();            
        }
        else 
        {
            UserAction[] actions = currentStep.GetComponents<UserAction>();
            if (actions.Any(f => f.Complete))
            {
                //Completed a step, give response and continue
                foreach(UserAction action in actions.Where(f => f.Response))
                    GiveResponse(1.5f);
                NextStep();
            }
        }
    }

    public void ReadyUp()
    {
        //Set every step and response to inactive
        Steps.ForEach(f => f.SetActive(false));
        Responses.ForEach(f => f.SetActive(false));
        //Set all tutorial objects to inactive
        foreach (GameObject obj in TutorialObjects)
            obj.SetActive(false);
        //Disable all UI
        foreach (GameObject element in UIElements)
            element.GetComponentsInChildren<Selectable>(true).ToList().ForEach(f => f.interactable = false);
        foreach (GameObject element in Ignore)
            element.GetComponentsInChildren<Selectable>(true).ToList().ForEach(f => f.interactable = true);
        //Disable controls
        GameData.CanSelect = false;
        GameData.CanConfirm = false;
        GameData.NumpadEnabled = false;
    }

    public void Restart()
    {
        GameData.TutorialState = -1;
        GameData.ResetLevel();
        SceneSwitch.LoadPlayTutorial();
    }

    public void AllResponses(bool value)
    {
        loopResponses = value;
    }

    public void End()
    {
        end = true;
        currentStep = null;
        ActivateAllSteps(false);
    }

    public void RecordElement(int value)
    {
        if (element == -1)
            if (typeof(Elements).GetFields().Length != value)
                element = value;
    }

    public string GetElement()
    {
        if (element > -1)
        {
            FieldInfo[] ListOfElements = typeof(Elements).GetFields();
            string elementstr = ListOfElements.Select(f => f.Name).ToArray()[element];
            return elementstr;
        }
        else return "";
    }

    private void NextStep()
    {
        //Deactivate last step
        if (currentStep)
            currentStep.SetActive(false);
        //Set new step
        step++;
        if (!Editor)
            GameData.TutorialState = step;
        if (step < Steps.Count)
        {
            currentStep = Steps[step];
            currentStep.SetActive(true);
        }
        //No more steps
        else
        {
            End();
        }
    }

    private void GiveResponse(float seconds)
    {
        //Give random response
        int response = UnityEngine.Random.Range(0, Responses.Count);
        if (!activeResponses[response])
        {
            activeResponses[response] = true;
            GiveResponse(response, seconds);
        }
    }

    private void GiveResponse(int id, float seconds)
    {
        //Activate given response then deactivate after some time
        Responses[id].SetActive(true);
        StartCoroutine(DeactivateResponse(id, seconds));
    }

    private IEnumerator DeactivateResponse(int id, float seconds)
    {
        //Wait for seconds then deactivate
        yield return new WaitForSecondsRealtime(seconds);
        Responses[id].SetActive(false);
        activeResponses[id] = false;
    }

    private void ActivateAllSteps(bool value)
    {
        Steps.ForEach(f => f.SetActive(value));
    }
}
