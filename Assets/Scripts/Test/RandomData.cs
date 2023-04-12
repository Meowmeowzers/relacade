using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RandomData
{
    public int id;
    public string data;

    public RandomData(int id, string data)
    {
        this.id = id;
        this.data = data;
    }
}
