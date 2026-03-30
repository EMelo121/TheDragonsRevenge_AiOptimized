using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class DeleteSystem
{
    public static void DeleteData(string directory)
    {
        string path = PathMaker.SetPath(directory);

        if(File.Exists(path))
        {
            try
            {
                Debug.Log($"Deleting file at directory: {path}");
                File.Delete(path);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Unable to delete file due to:  {e.Message} {e.StackTrace}");
            }
        }
        else
        {
            Debug.LogWarning($"No file found at {path}");
            return;
        }
    }
}
