using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem
{

    public static void saveLevelModule(LevelModule levelModuleToSave)
    {
        string json = string.Empty;

        json = JsonUtility.ToJson(levelModuleToSave);
        System.IO.File.WriteAllText(Application.dataPath + "/SaveData/LevelModules/" + levelModuleToSave.name +".json", json);
    }
    public static void loadLevelModule(LevelModule levelModuleToLoad) 
    {

    }

}
