using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIConfig", menuName = "UI/UIConfig")]
public class UIConfig : ScriptableObject
{
    [System.Serializable]
    public struct UIEntry
    {
        public string uIName;
        public GameObject prefab;
    }

    public List<UIEntry> uIPrefabs;

    private Dictionary<string, GameObject> _uICache;

    public void Initialize()
    {
        _uICache = new Dictionary<string, GameObject>();
        foreach (var entry in uIPrefabs)
        {
            if (!_uICache.ContainsKey(entry.uIName))
            {
                _uICache[entry.uIName] = entry.prefab;
            }
        }
    }

    public GameObject GetPrefab(string uIName)
    {
        if (_uICache.TryGetValue(uIName, out var prefab))
            return prefab;

        Debug.LogError($"UIConfig: {uIName} 프리팹을 찾을 수 없습니다.");
        return null;
    }
}
