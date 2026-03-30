using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Serializing Parsers
public class JsonList<T>
{
    public List<T> ListOfObjects { get; set; }
}
public class JsonVector3
{
    public float[] V3Coords { get; set; }
}
#endregion

