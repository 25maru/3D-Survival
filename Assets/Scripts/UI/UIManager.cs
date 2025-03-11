using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private UIConfig uIConfig;
    [SerializeField] private Transform uIRoot; // UI를 표시할 부모 오브젝트
    [SerializeField] private Transform popupRoot; // 팝업 전용 부모 오브젝트

    private readonly Dictionary<string, BaseUI> activeUIs = new();
    private readonly Queue<BaseUI> uIPool = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        uIConfig.Initialize();

        SceneManager.sceneUnloaded += _ => ClearAllUI(); // 씬이 변경될 때 UI 초기화
        SceneManager.sceneLoaded += (scene, mode) => InitializeUIRoots();
    }

    private void InitializeUIRoots()
    {
        if (uIRoot == null)
        {
            GameObject rootObj = GameObject.Find("Root_UI");
            if (rootObj != null) uIRoot = rootObj.transform;
            else Debug.LogError("UIManager: Root_UI를 찾을 수 없습니다!");
        }

        if (popupRoot == null)
        {
            GameObject popupObj = GameObject.Find("Root_Popup");
            if (popupObj != null) popupRoot = popupObj.transform;
            else Debug.LogError("UIManager: Root_Popup을 찾을 수 없습니다!");
        }
    }

    public T ShowUI<T>(bool isPopup = false) where T : BaseUI
    {
        string uIName = typeof(T).Name;

        if (activeUIs.TryGetValue(uIName, out var existingUI))
        {
            if (existingUI == null) // 기존 UI가 삭제된 경우
            {
                activeUIs.Remove(uIName); // 안전하게 제거
            }
            else
            {
                existingUI.OnShow();
                return existingUI as T;
            }
        }

        GameObject prefab = uIConfig.GetPrefab(uIName);
        if (prefab == null) return null;

        BaseUI newUI;
        if (uIPool.Count > 0)
        {
            newUI = uIPool.Dequeue();
            newUI.gameObject.SetActive(true);
        }
        else
        {
            Transform parent = isPopup ? popupRoot : uIRoot;
            newUI = Instantiate(prefab, parent).GetComponent<BaseUI>();
        }

        newUI.Initialize();
        newUI.OnShow();
        activeUIs[uIName] = newUI; // 다시 등록
        return newUI as T;
    }

    public void HideUI<T>() where T : BaseUI
    {
        string uIName = typeof(T).Name;

        if (activeUIs.TryGetValue(uIName, out var uI))
        {
            uI.OnHide();
        }
        else
        {
            Debug.LogWarning($"UIManager: {uIName} UI가 활성화 상태가 아닙니다.");
        }
    }

    public void RemoveUI<T>() where T : BaseUI
    {
        string uIName = typeof(T).Name;

        if (activeUIs.TryGetValue(uIName, out var uI))
        {
            uI.OnHide();
            activeUIs.Remove(uIName);
            uIPool.Enqueue(uI);
        }
    }

    public void ClearAllUI()
    {
        List<string> keysToRemove = new List<string>();

        foreach (var kvp in activeUIs)
        {
            if (kvp.Value == null) continue; // 이미 삭제된 UI는 무시

            kvp.Value.OnHide();

            if (kvp.Value.gameObject != null)
            {
                Destroy(kvp.Value.gameObject); // UI를 안전하게 제거
            }

            keysToRemove.Add(kvp.Key);
        }

        // 제거할 UI들을 Dictionary에서 삭제
        foreach (var key in keysToRemove)
        {
            activeUIs.Remove(key);
        }

        uIPool.Clear(); // 씬이 바뀌면 풀도 초기화
    }
}
