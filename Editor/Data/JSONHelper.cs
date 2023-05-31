using System;
using UnityEngine;

//Source: Velvary YT
public static class JSONHelper
{
    public static T[] FromJSON<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.items;
    }

    public static string ToJSON<T>(T[] array)
    {
        Wrapper<T> wrapper = new(array);
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJSON<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new(array);
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] items;

        public Wrapper(T[] value)
        {
            items = value;
        }
    }
}