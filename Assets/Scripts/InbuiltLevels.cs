using System.IO;
using UnityEngine;

[ExecuteInEditMode]
public class InbuiltLevels : MonoBehaviour
{
    private void Start()
    {
        if (CheckForLevels())
            MoveLevels();
    }

    private bool CheckForLevels()
    {
        string FolderDestination = Application.dataPath + "/Levels/";

        if (Directory.Exists(FolderDestination))
            if (Directory.GetFiles(FolderDestination, "*.dat").Length > 0)
                return true;
        return false;
    }

    private void MoveLevels()
    {
        string OldFolderDestination = Application.dataPath + "/Levels/";
        string NewFolderDestination = Application.persistentDataPath + "/Levels/";

        //Move each file from project file to game save
        foreach (string oldfile in Directory.EnumerateFiles(OldFolderDestination, "*.dat"))
        {
            string newfile = NewFolderDestination + Path.GetFileName(oldfile);
            if (File.Exists(newfile))
                File.Delete(oldfile);
            else
                File.Move(oldfile, newfile);
        }
    }
}
