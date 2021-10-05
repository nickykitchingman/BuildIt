using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UserDataSettings : MonoBehaviour
{
    public bool loaded = false;

    private Dictionary<string, Pointer<bool>> boolptrs;
    private Dictionary<string, Pointer<float>> floatptrs;


    private void Awake()
    {
        //Add references
        boolptrs = new Dictionary<string, Pointer<bool>>();
        floatptrs = new Dictionary<string, Pointer<float>>();

        unsafe
        {
            fixed (bool* p = &GameData.MusicOn) { boolptrs.Add("Music", new Pointer<bool>(p)); }
            fixed (bool* p = &GameData.EffectsOn) { boolptrs.Add("Effects", new Pointer<bool>(p)); }

            fixed (float* p = &GameData.MusicVolume) { floatptrs.Add("MusicVolume", new Pointer<float>(p)); }
            fixed (float* p = &GameData.EffectsVolume) { floatptrs.Add("EffectsVolume", new Pointer<float>(p)); }
        }

        //Assign settings
        string contents = FileHandler.ReadSettings();
        if (contents != null)
            ReadContents(contents);

        //Keep througout application
        DontDestroyOnLoad(gameObject);

        loaded = true;
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
        foreach (KeyValuePair<string, Pointer<bool>> setting in boolptrs)
        {
            //Add format: key=value
            contents += setting.Key;
            contents += "=";
            contents += setting.Value.Value;
            //New line
                contents += Environment.NewLine;
        }
        
        int count = floatptrs.Count;
        foreach (KeyValuePair<string, Pointer<float>> setting in floatptrs)
        {
            count--;
            //Add format: key=value
            contents += setting.Key;
            contents += "=";
            contents += setting.Value.Value;
            //New line
            if (count != 0)
                contents += Environment.NewLine;
        }

        return contents;
    }

    private void ReadContents(string contents)
    {
        foreach (string setting in contents.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
        {
            //Split lines into key and value
            string[] data = setting.Split('=');
            if (data.Length < 2)
                return;

            string key = data[0];
            string value = data[1];

            //Use pointers to assign values
            if (boolptrs.ContainsKey(key))
            {
                Pointer<bool> reference = boolptrs[key];
                try
                {
                    reference.Value = Convert.ToBoolean(value);
                }
                catch (FormatException)
                {
                    Debug.Log("Not Boolean Format");
                }
            }
            
            if (floatptrs.ContainsKey(key))
            {
                Pointer<float> reference = floatptrs[key];
                try
                {
                    reference.Value = (float)Convert.ToDouble(value);
                }
                catch (FormatException)
                {
                    Debug.Log("Not Float Format");
                }
            }
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

    private unsafe struct Pointer<T>
    {
        private readonly void* pointer;

        public Pointer(void* pointer)
        {
            this.pointer = pointer;
        }

        public T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return Unsafe.Read<T>(pointer); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { Unsafe.Write(pointer, value); }
        }
    }
}
