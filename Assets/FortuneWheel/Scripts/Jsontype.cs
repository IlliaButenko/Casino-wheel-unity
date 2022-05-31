using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jsontype
{

    public float betAmount;
    public string token;
    public string userName;
}

public class ReceiveJsonObject
{
    public bool gameResult;
    public float earnAmount;
    public int randomNumber;
    public string message;
    public ReceiveJsonObject()
    {
    }
    public static ReceiveJsonObject CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<ReceiveJsonObject>(data);
    }
}
