using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StringVariable : ScriptableObject
{
    public string Value;

    public void SetValue(string s)
    {
        Value = s;
    }

    public void SetValue(StringVariable s)
    {
        Value = s.Value;
    }
}
