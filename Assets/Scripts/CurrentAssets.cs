using System.Collections.Generic;
using UnityEngine;

public class CurrentAssets 
{
    private List<GameObject> _userObjectsInScene { get; set; } = new List<GameObject>();
    private List<GameObject> _levelObjectsInScene = new List<GameObject>();
    private List<Trigger> _triggersInScene { get; set; } = new List<Trigger>();
    public List<Condition> LevelConditions { get; set; } = new List<Condition>();    

    public Dictionary<Condition, Objective> ObjectiveGraphics { get; set; } = new Dictionary<Condition, Objective>();
    public List<string> Tags { get; set; } = new List<string>();


    //Only expose readonly lists
    public IList<GameObject> LevelObjectsInScene
    {
        get
        { return _levelObjectsInScene.AsReadOnly(); }
    }
    public IList<GameObject> UserObjectsInScene
    {
        get
        { return _userObjectsInScene.AsReadOnly(); }
    }

    public IList<Trigger> TriggersInScene
    {
        get
        { return _triggersInScene.AsReadOnly(); }
    }
       

    private Dictionary<string, int> NameFrequencies = new Dictionary<string, int>();


    /// <summary>
    /// Creates unique name for each object
    /// </summary>
    /// <param name="name"></param>
    private void AddNewName(string name)
    {
        string strippedName = name;
        bool exists = false;
        foreach (KeyValuePair<string, int> nameFrequency in NameFrequencies)
        {
            //Strip name of each frequency
            {
                strippedName = name.Substring(0, name.Length - nameFrequency.Value.ToString().Length);
            }

            if (strippedName == nameFrequency.Key)
            {
                //Name already exists 
                exists = true;
                break;
            }
            else
                //Reset stripped if doesn't exist
                strippedName = name;
        }

        if (exists)
            //Increment frequency if it exists
            NameFrequencies[strippedName]++;
        else 
            //Create new name if it doesn't
            NameFrequencies.Add(strippedName, 1);
    }

    public int GetNameIndex(string name)
    {
        if (NameFrequencies.ContainsKey(name))
        {
            //Increment frequency if it exists
            return NameFrequencies[name] + 1;
        }

        return 1;
    }

    //Custom getters and setters
    public void AddLevelObject(GameObject LevelObject)
    {
        //Add level object then increment index for naming
        _levelObjectsInScene.Add(LevelObject);
        AddNewName(LevelObject.name);
    }

    public void RemoveLevelObject(GameObject LevelObject)
    {
        //Remove level object from list
        _levelObjectsInScene.Remove(LevelObject);
    }

    public void AddUserObject(GameObject LevelObject)
    {
        //Add user object then increment index for naming
        _userObjectsInScene.Add(LevelObject);
        AddNewName(LevelObject.name);
    }

    public void RemoveObject(GameObject LevelObject)
    {
        //Remove user object from list
        _userObjectsInScene.Remove(LevelObject);
    }
    
    public void AddTrigger(Trigger trigger)
    {
        //Add user object then increment index for naming
        _triggersInScene.Add(trigger);
        AddNewName(trigger.Name);
        Debug.Log("Added trigger");
    }

    public void RemoveTrigger(Trigger trigger)
    {
        //Remove user object from list
        _triggersInScene.Remove(trigger);
    }

    public Trigger FindTrigger(string Name)
    {
      
        foreach (Trigger trigger in _triggersInScene)
        {
            if (Name == trigger.Name)
                return trigger;
        }
        return null;
    }

    public void SetConditionComplete(Condition condition, bool value)
    {
        int index = LevelConditions.IndexOf(condition);
        if (index != -1)
        {
            //Update condition list
            Condition newCondition = condition;
            newCondition.complete = value;
            LevelConditions[index] = newCondition;
            //Update objective graphic dictionary
            Objective objective = ObjectiveGraphics[condition];
            ObjectiveGraphics.Remove(condition);
            ObjectiveGraphics.Add(newCondition, objective);
        }
    }
}