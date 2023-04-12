using System.IO;
using UnityEditor;
using UnityEngine;

public class JSONWRITER:MonoBehaviour
{
    public RandomData randomData = new(123, "asdf");
    public RandomDatas randomData2;    

    public void WriteData()
    {
        string dataToString = JsonUtility.ToJson(randomData);
        File.WriteAllText(Application.dataPath + "/data.json", dataToString);
        Debug.Log("Pressed");
    }

    public void WriteData2()
    {
        string dataToString = JsonUtility.ToJson(randomData2);
        
        File.WriteAllText(Application.dataPath + "/data2.json", dataToString);
        
        Debug.Log("Pressed");
    }

    public void ReadData()
    {
        //AssetDatabase.Refresh();
        string path = Application.dataPath + "/data2.json";

        StreamReader reader = new(path);
        string content = reader.ReadToEnd();
        reader.Close();

        RandomDatas res = JsonUtility.FromJson<RandomDatas>(content);

        foreach(var x in res.randomData)
        {
            Debug.Log(x.id + " " + x.data);
        }
    }
}
