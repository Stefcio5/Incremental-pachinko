using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GetScriptableObjects : MonoBehaviour
{
    // Start is called before the first frame update
    public List<UpgradeScriptableObject> scriptableList;
    void Start()
    {
        scriptableList = GetAllInstances<UpgradeScriptableObject>();
        foreach (var item in scriptableList)
        {
            item.ResetUpgrade();
        }
    }
    public static List<T> GetAllInstances<T>() where T : ScriptableObject
    {
        return AssetDatabase.FindAssets($"t: {typeof(T).Name}").ToList()
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<T>)
                    .ToList();
    }
}
