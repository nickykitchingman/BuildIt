using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelComplete : MonoBehaviour
{
    public GameMenu CompleteMenu;
    public GameObject CompletePrefab;
    public Transform NoticationParent;
    [Header("Scripts")]
    public TimeScale timeScale;

    private List<bool> completeConditions;

    private void Start()
    {
        //Save condition complete state
        completeConditions = new List<bool>();
        SceneAssets.currentAssets.LevelConditions.ForEach(f => completeConditions.Add(f.complete));
    }

    void Update()
    {
        //Check if new condition complete
        for (int i = 0; i < SceneAssets.currentAssets.LevelConditions.Count; i++)
            if (!completeConditions[i] && SceneAssets.currentAssets.LevelConditions[i].complete)
            {
                NotifyUser();
                completeConditions[i] = true;
            }
        //Check if level complete
        if (CheckIfComplete())
            EndLevel();
    }

    private void EndLevel()
    {
        //Open level complete screen
        CompleteMenu.Open();
    }

    public static void HandleTriggerCollision(Trigger trigger, Collider other)
    {
        for (int i = 0; i < SceneAssets.currentAssets.LevelConditions.Count; i++)
        {
            Condition condition = SceneAssets.currentAssets.LevelConditions[i];
            Objective objectiveGraphic = SceneAssets.currentAssets.ObjectiveGraphics[condition];
            string CollisionType = condition.collisionType;
            Properties properties;

            //Continue if trigger is not in condition
            if (condition.trigger != trigger || condition.complete)
                continue;

            //Deal with collision
            switch (CollisionType)
            {
                case "Element":
                    //Check properties of collider
                    properties = GetProperties(other.gameObject, false);
                    //Check element of collider against element of condition
                    if (properties.element.Name == condition.collider)
                    {
                        SceneAssets.currentAssets.SetConditionComplete(condition, true);
                        objectiveGraphic.UpdateObjective();
                    }
                    break;
                case "Tag":
                    //Check properties of parent collider
                    properties = GetProperties(other.gameObject, true);
                    //Check tag of collider against tag of condition
                    if (properties.Tag == condition.collider)
                    {
                        SceneAssets.currentAssets.SetConditionComplete(condition, true);
                        objectiveGraphic.UpdateObjective();
                    }
                    break;
            }
        }
    }

    private static bool CheckIfComplete()
    {
        List<Condition> conditions = SceneAssets.currentAssets.LevelConditions;
        //Return false if there are no conditions 
        if (conditions.Count == 0)
            return false;
        //Return false if at least one condition is not complete
        foreach (Condition condition in SceneAssets.currentAssets.LevelConditions)
            if (!condition.complete)
                return false;
        //Else return true if all complete
        return true;
    }

    private void NotifyUser()
    {
        //Make notification then destroy it
        GameObject notification = Instantiate(CompletePrefab);
        notification.transform.SetParent(NoticationParent);
        notification.transform.localScale = NoticationParent.parent.localScale;
        notification.transform.position = Vector3.zero;
        StartCoroutine(DestroyObjComplete(notification, 3));
    }

    private IEnumerator DestroyObjComplete(GameObject notication, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(notication);
    }

    private static Properties GetProperties(GameObject collider, bool fromParent)
    {
        try
        {
            //Try to access collider's properties from parent or self
            if (fromParent)
                return collider.GetComponentInParent<Properties>();

            return collider.GetComponent<Properties>();
        }
        catch (MissingComponentException)
        {
            //End check if object doesn't have properties
            return null;
        }
    }
}
