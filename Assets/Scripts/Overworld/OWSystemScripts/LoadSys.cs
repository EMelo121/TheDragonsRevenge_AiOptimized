using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using Newtonsoft.Json;

public static class LoadSys
{
    #region Public Methods
    public static float[] ReadVector3FromJson(string fileName)
    {
        string path = PathMaker.SetPath(fileName);

        if(!File.Exists(path))
        {
            DoesNotExistWarning(path);
            return null;
        }
        else
        {
            try
            {
                string loadedFromFile = File.ReadAllText(path);
                JsonVector3 deserializedObject = JsonConvert.DeserializeObject<JsonVector3>(loadedFromFile);

                float[] deserializedFloats = new float[3]
                {
                    deserializedObject.V3Coords[0],
                    deserializedObject.V3Coords[1],
                    deserializedObject.V3Coords[2]
                };

                return deserializedFloats;
            }
            catch (Exception e)
            {
                LoadError(e);
                throw e;
            }
        }

        
    }


    public static List<T> ReadListFromJson<T>(string fileName)
    {
        string path = PathMaker.SetPath(fileName);

        if(!File.Exists(path))
        {
            DoesNotExistWarning(path);
            return null;
        }
        else
        {
            try
            {
                string loadedFromJson = File.ReadAllText(path);
                JsonList<T> deserializedObject = JsonConvert.DeserializeObject<JsonList<T>>(loadedFromJson);
                return deserializedObject.ListOfObjects;
            }
            catch(Exception e) 
            {
                LoadError(e);
                throw e;
            }
        }
    }
    #endregion
    #region Private Methods
    static void DoesNotExistWarning(string path)
    {
        Debug.LogWarning($"Could not load from file: {path} does not exist.");
    }

    static void LoadError(Exception e)
    {
        Debug.LogError($"Could not read file due to: {e.Message} {e.StackTrace}");
    }
    #endregion
}
