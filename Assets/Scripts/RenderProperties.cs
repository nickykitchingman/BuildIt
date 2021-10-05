using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RenderProperties : MonoBehaviour
{
    public Image Background;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Position;
    public TextMeshProUGUI Rotation;
    public TextMeshProUGUI Scale;
    public TextMeshProUGUI Tag;
    public Toggle StaticToggle;

    public string objectName { get; private set; }
    public Vector3 positionValue { get; private set; }
    public Quaternion rotationValue { get; private set; }
    public Vector3 scaleValue { get; private set; }
    public string objectTag { get; private set; }
    public bool IsStatic { get; private set; }

    private GameObject CurrentObject;

    void Start()
    {
        //Add listeners to modifiable properties
        StaticToggle.onValueChanged.AddListener(HandleStaticChange);
    }

    void Update()
    {
        if (CurrentObject != null)
        {
            //Update variable properties every frame
            SetPosition(CurrentObject.transform.position);
            SetRotation(CurrentObject.transform.rotation);
            SetScale(CurrentObject.transform.localScale);
        }
    }

    public void UpdateObject(GameObject currentObject, string name)
    {
        RemoveListeners();

        CurrentObject = currentObject;
        SetName(name);
        SetStatic(currentObject.GetComponent<Properties>().IsStatic);
        SetTag(currentObject.GetComponent<Properties>().Tag);

        AddListeners();
    }

    public void Activate(bool value)
    {
        //Set active or inactive
        if (value)
        {
            LeanTween.scale(Background.gameObject, Vector3.zero, 0f);
            Background.gameObject.SetActive(true);
            LeanTween.scale(Background.gameObject, Vector3.one, 0.2f);
        }
        else
        {
            LeanTween.scale(Background.gameObject, Vector3.zero, 0.2f).setOnComplete(Disable);
        }
    }
    
    public void ActivateInstant(bool value)
    {
        //Set active or inactive
        if (value)
            Background.gameObject.SetActive(true);
        else
            Background.gameObject.SetActive(false);
    }

    private void Disable()
    {
        Background.gameObject.SetActive(false);
        LeanTween.scale(Background.gameObject, Vector3.one, 0f);
    }

    private void SetName(string name)
    {
        objectName = name;
        Name.text = name;
    }

    private void SetPosition(Vector3 position)
    {
        //Store position
        positionValue = position;
        //Format appropriately
        float x, y, z;
        x = Mathf.Round(position.x * 10) / 10f;
        y = Mathf.Round(position.y * 10) / 10f;
        z = Mathf.Round(position.z * 10) / 10f;
        Position.text = string.Format("X: {0} Y:{1} Z:{2}", x, y, z);
    }

    private void SetRotation(Quaternion rotation)
    {
        //Store rotation
        rotationValue = rotation;
        //Convert to euler format
        Vector3 eRotation = rotation.eulerAngles;
        float x, y, z;
        x = Mathf.Round(eRotation.x * 10) / 10f;
        y = Mathf.Round(eRotation.y * 10) / 10f;
        z = Mathf.Round(eRotation.z * 10) / 10f;
        Rotation.text = string.Format("X: {0} Y:{1} Z:{2}", x, y, z);
    }

    private void SetScale(Vector3 position)
    {
        //Store position
        scaleValue = position;
        //Format appropriately
        float x, y, z;
        x = Mathf.Round(position.x * 10) / 10f;
        y = Mathf.Round(position.y * 10) / 10f;
        z = Mathf.Round(position.z * 10) / 10f;
        Scale.text = string.Format("X: {0} Y:{1} Z:{2}", x, y, z);
    }

    private void SetTag(string tag)
    {
        objectTag = tag;
        //Check for no tag
        Tag.text = tag == "" ? "No Tag" : tag;
    }

    private void SetStatic(bool value)
    {
        //Set static by calling function
        IsStatic = value;
        StaticToggle.isOn = value;
    }

    private void HandleStaticChange(bool value)
    {
        //Set static by toggle
        IsStatic = value;
    }

    private void RemoveListeners()
    {
        //Static toggle
        StaticToggle.onValueChanged.RemoveAllListeners();
    }

    private void AddListeners()
    {
        //Static toggle
        StaticToggle.onValueChanged.AddListener(HandleStaticChange);
    }
}
