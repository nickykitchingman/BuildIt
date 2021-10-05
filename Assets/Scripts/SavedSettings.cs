using System;
using System.Collections.Generic;
using UnityEngine;

public class SavedSettings : MonoBehaviour
{
    private GenericDictionary settings;


    private void Awake()
    {
        //Add references
        settings = new GenericDictionary();
        settings.Add("Music", new VariableReference<bool>(ref GameData.MusicOn));
        settings.Add("Effects", new VariableReference<bool>(ref GameData.EffectsOn));

        //Assign settings
        string contents = FileHandler.ReadSettings();
        if (contents != null)
            ReadContents(contents);

        //Keep througout application
        DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationQuit()
    {
        //Save user settings on exit
        string contents = createContents();
        FileHandler.SaveSettings(contents);
    }

    private string createContents()
    {
        string contents = "";

        //Loop references, adding each value
        int count = settings.Dict.Count;
        foreach (KeyValuePair<string, object> setting in settings.Dict)
        {
            count--;
            contents += setting.Key;
            contents += "=";
            contents += setting.Value;
            if (count != 0)
                contents += Environment.NewLine;
        }

        return contents;
    }

    private void ReadContents(string contents)
    {
        foreach(string setting in contents.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
        {
            string[] data = setting.Split('=');
            string key = data[0];
            string value = data[1];
            Type type = settings.Dict[key].GetType();
            settings.GetValue<VariableReference<object>>("Effects").variable = Convert.ChangeType(value, type);
        }
    }


    private class VariableReference<T>
    {
        public T variable;

        public VariableReference(ref T variable)
        {
            this.variable = variable;
        }
    }

    private class GenericDictionary
    {
        private Dictionary<string, object> _dict = new Dictionary<string, object>();

        public void Add<T>(string key, T value) where T : class
        {
            _dict.Add(key, value);
        }

        public T GetValue<T>(string key) where T : class
        {
            return _dict[key] as T;
        }

        public IDictionary<string, object> Dict
        {
            get { return _dict; }
        }
    }
}
