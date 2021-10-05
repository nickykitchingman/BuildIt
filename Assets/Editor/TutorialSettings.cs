using TMPro;
using TMPro.Examples;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(CreateTutorial))]
public class TutorialSettings : Editor
{
    SerializedProperty playTutorial;
    SerializedProperty stepParentSer;
    SerializedProperty responseParentSer;
    SerializedProperty textPrefab;
    SerializedProperty defaultImage;

    [SerializeField] private int index;
    [SerializeField] private bool useIndex = false;
    [SerializeField] private string word;
    [SerializeField] private Color responseColor;
    [SerializeField] private ActionComponent actions = new ActionComponent();

    private void OnEnable()
    {
        playTutorial = serializedObject.FindProperty("playTutorial");
        stepParentSer = serializedObject.FindProperty("StepParent");
        responseParentSer = serializedObject.FindProperty("ResponseParent");
        textPrefab = serializedObject.FindProperty("TextPrefab");
        defaultImage = serializedObject.FindProperty("DefaultImage");
        SetParents();
        index = 0;
    }

    private void SetParents()
    {
        CreateTutorial tutorial = ((CreateTutorial)target);
        foreach (Transform child in tutorial.transform)
            if (child.name == "Steps")
                tutorial.StepParent = child;
            else if (child.name == "Responses")
                tutorial.ResponseParent = child;
    }

    public override void OnInspectorGUI()
    {
        CreateTutorial tutorial = (CreateTutorial)target;

        GUI.color = Color.white;

        //Header Style
        GUIStyle headerStyle = new GUIStyle();
        headerStyle.fontStyle = FontStyle.Bold;
        
        //Subheader Style
        GUIStyle subHeaderStyle = new GUIStyle();
        subHeaderStyle.fontStyle = FontStyle.BoldAndItalic;
        
        //Middle header Style
        GUIStyle midHeaderStyle = new GUIStyle();
        midHeaderStyle.fontStyle = FontStyle.Bold;
        midHeaderStyle.alignment = TextAnchor.MiddleCenter;

        //Play tutorial
        GUILayout.Space(10);
        GUILayout.Label("Play Tutorial", headerStyle);
        EditorGUILayout.PropertyField(playTutorial, new GUIContent("Play Tutorial Script"));
        EditorGUILayout.PropertyField(stepParentSer, new GUIContent("Step Parent"));
        EditorGUILayout.PropertyField(responseParentSer, new GUIContent("Response Parent"));
        tutorial.SetLists();

        //Step options
        GUILayout.Space(10);
        GUILayout.Label("Step Options", midHeaderStyle);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Activate all"))
            tutorial.ActivateAllSteps(true);
        if (GUILayout.Button("Deactivate all"))
            tutorial.ActivateAllSteps(false);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Update Steps"))
            tutorial.UpdateSteps();

        GUILayout.Label("Add Step", subHeaderStyle);
        useIndex = GUILayout.Toggle(useIndex, "Insert");
        if (useIndex)
            index = EditorGUILayout.IntField("Index", index);
        GUILayout.Space(6);
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        actions.button = GUILayout.Toggle(actions.button, "Button");
        actions.select = GUILayout.Toggle(actions.select, "Select");
        actions.wait = GUILayout.Toggle(actions.wait, "Wait");
        actions.box = GUILayout.Toggle(actions.box, "Box Area");
        actions.boxes = GUILayout.Toggle(actions.boxes, "Box Areas");
        actions.sphere = GUILayout.Toggle(actions.sphere, "Sphere Area");
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        actions.key = GUILayout.Toggle(actions.key, "Key Press");
        actions.look = GUILayout.Toggle(actions.look, "Look");
        actions.text = GUILayout.Toggle(actions.text, "Text");
        actions.texts = GUILayout.Toggle(actions.texts, "Texts");
        actions.dropdown = GUILayout.Toggle(actions.dropdown, "Dropdown");
        actions.tmpdropdown = GUILayout.Toggle(actions.tmpdropdown, "TMP Dropdown");
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Create Step"))
            if (useIndex)
                tutorial.CreateNewStep(actions, index);
            else
                tutorial.CreateNewStep(actions);

        //Response options
        GUILayout.Space(10);
        GUILayout.Label("Response Options", midHeaderStyle);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Activate all"))
            tutorial.ActivateAllResponses(true);
        if (GUILayout.Button("Deactivate all"))
            tutorial.ActivateAllResponses(false);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Update Responses"))
            tutorial.UpdateResponses();

        GUILayout.Label("Add Response", subHeaderStyle);
        word = EditorGUILayout.TextField("Word", word);
        responseColor = EditorGUILayout.ColorField("Color", responseColor);
        if (GUILayout.Button("Create Response"))
            tutorial.CreateNewResponse(word, responseColor);

        //Options 
        GUILayout.Space(10);
        GUILayout.Label("Options", midHeaderStyle);
        //text
        GUILayout.Label("Text", subHeaderStyle);
        tutorial.TextMaxExpand = EditorGUILayout.Slider("Max expand", tutorial.TextMaxExpand, 1, 3);
        tutorial.TextExpandSpeed = EditorGUILayout.IntSlider("Expand Speed", tutorial.TextExpandSpeed, 0, 10);
        tutorial.TextColour = EditorGUILayout.ColorField("Text Colour", tutorial.TextColour);
        EditorGUILayout.PropertyField(textPrefab, new GUIContent("Text Prefab"));
        //EditorGUILayout.PropertyField(textFont, new GUIContent("Text Font"));        
        if (GUILayout.Button("Update Text"))
            tutorial.UpdateAllTexts();
        //image
        GUILayout.Label("Images", subHeaderStyle);
        tutorial.ImageMaxExpand = EditorGUILayout.Slider("Max expand", tutorial.ImageMaxExpand, 1, 3);
        tutorial.ImageExpandSpeed = EditorGUILayout.IntSlider("Expand Speed", tutorial.ImageExpandSpeed, 0, 10);
        EditorGUILayout.PropertyField(defaultImage, new GUIContent("Default Image"));        
        if (GUILayout.Button("Update Images"))
            tutorial.UpdateAllImageExpands();
        //responses
        GUILayout.Label("Responses", subHeaderStyle);
        tutorial.ResponseMaxExpand = EditorGUILayout.Slider("Max expand", tutorial.ResponseMaxExpand, 1, 3);
        tutorial.ResponseExpandSpeed = EditorGUILayout.IntSlider("Expand Speed", tutorial.ResponseExpandSpeed, 0, 10);
        if (GUILayout.Button("Update Responses"))
            tutorial.UpdateAllResponseExpands();

        tutorial.UpdateSteps();
        tutorial.UpdateResponses();
        serializedObject.ApplyModifiedProperties();
    }
}
