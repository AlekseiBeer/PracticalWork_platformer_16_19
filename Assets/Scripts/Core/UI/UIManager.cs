using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // Словарь для хранения UI-панелей по имени
    private Dictionary<string, GameObject> windows = new Dictionary<string, GameObject>();

    [SerializeField] private GameObject[] windowPanels;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Регистрируем и скрываем
        foreach (GameObject panel in windowPanels)
        {
            windows[panel.name] = panel;
            panel.SetActive(false);
        }
    }

    public void ShowWindow(string windowName)
    {
        if (windows.ContainsKey(windowName))
        {
            windows[windowName].SetActive(true);
        }
        else
        {
            Debug.LogWarning("Окно с именем " + windowName + " не найдено в UIManager.");
        }
    }

    public void HideWindow(string windowName)
    {
        if (windows.ContainsKey(windowName))
        {
            windows[windowName].SetActive(false);
        }
        else
        {
            Debug.LogWarning("Окно с именем " + windowName + " не найдено в UIManager.");
        }
    }

    public void HideAllWindows()
    {
        foreach (var panel in windows.Values)
        {
            panel.SetActive(false);
        }
    }
}