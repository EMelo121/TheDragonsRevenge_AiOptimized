using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public static class SaveSys
{
    public static void WriteListToJson<T>(string fileName, T data)
    {
        string path = PathMaker.SetPath(fileName);

        //put data in JSON parser
        JsonList<T> jsonList = new()
        {
            ListOfObjects = new List<T>() { data }
        };
        
        try
        {
            //write list to file
            string convertToJson = JsonConvert.SerializeObject(jsonList, Formatting.Indented);
            File.WriteAllText(path, convertToJson);
            Debug.Log($"Successfully wrote to: {path}");
        }
        catch(Exception e)
        {
            ErrorMessage(e);
        }
    }
    public static void WriteVector3ToJson(string fileName, Vector3 pos)
    {
        string path = PathMaker.SetPath(fileName);
        JsonVector3 v3ToJson = new JsonVector3()
        {
            V3Coords = new float[3]
            {
                pos.x,
                pos.y,
                pos.z
            }
        };
        try
        {
            string convertToJson = JsonConvert.SerializeObject(v3ToJson, Formatting.Indented);
            File.WriteAllText(path, convertToJson);
            Debug.Log($"Successfully wrote Vector3 array to: {path}");
        }
        catch(Exception e) 
        {
            ErrorMessage(e);
        }
    }
    public static void AppendToJsonList<T>(string fileName, T data)
    {
        string path = PathMaker.SetPath(fileName);

        if(!File.Exists(path))
        {
            Debug.LogWarning($"Unable to load data. No file exists at: {path}");
            return;
        }
        else
        {
            string loadedFromFile = File.ReadAllText(path);
            JsonList<T> jsonList = JsonConvert.DeserializeObject<JsonList<T>>(loadedFromFile);
            jsonList.ListOfObjects.Add(data);
            string convertToJson = JsonConvert.SerializeObject(jsonList, Formatting.Indented);
            File.WriteAllText(path, convertToJson);
            Debug.Log($"Successfully appended to: {path}");
            
        }
    }
    static void ErrorMessage(Exception e)
    {
        Debug.Log($"Unable to save due to: {e.Message} {e.StackTrace}");
    }

}
