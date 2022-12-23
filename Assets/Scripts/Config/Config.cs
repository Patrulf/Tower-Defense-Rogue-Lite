using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct LevelModuleData
{
    public const int width = 7;
    public const int height = 7;
}

public static class Config
{

    public static LevelModuleData levelModuleData = new LevelModuleData();
    public static float epsilon = 0.01f;
}
