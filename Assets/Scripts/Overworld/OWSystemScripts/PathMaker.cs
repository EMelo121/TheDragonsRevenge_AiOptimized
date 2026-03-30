using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathMaker 
{
    public static string SetPath(string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName + ".json";
        return path;
    }  
}
