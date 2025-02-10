using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class HealthBarManager : MonoBehaviour
{
    public static HealthBarManager Instance { get; private set; }

    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject enemyHPBarPrefab;
    [SerializeField] private Vector3 hpBarOffset = new(0, 0.2f, 0);

    private readonly Dictionary<Transform, IHealthDisplay> enemyHealthBars = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        if (canvas == null)
            canvas = GameObject.Find("CanvasWorldSpace").GetComponent<Canvas>();
    }

    public void RegisterEnemy(Transform enemyTransform, float currentHealth, float maxHealth)
    {
        if (enemyHPBarPrefab == null) return;

        GameObject hpBarInstance = Instantiate(enemyHPBarPrefab, canvas.transform);
        UpdateHPBarPosition(enemyTransform, hpBarInstance);

        if (hpBarInstance.TryGetComponent<IHealthDisplay>(out var hpDisplay))
        {
            hpDisplay.UpdateHealth(currentHealth, maxHealth);
            enemyHealthBars.Add(enemyTransform, hpDisplay);
        }
    }

    public void UnregisterEnemy(Transform enemyTransform)
    {
        if (enemyHealthBars.TryGetValue(enemyTransform, out IHealthDisplay hpDisplay))
        {
            MonoBehaviour mb = hpDisplay as MonoBehaviour;
            if (mb != null)
            {
                Destroy(mb.gameObject);
            }
            enemyHealthBars.Remove(enemyTransform);
        }
    }

    private void LateUpdate()
    {
        foreach (var pair in enemyHealthBars)
        {
            UpdateHPBarPosition(pair.Key, (pair.Value as MonoBehaviour).gameObject);
        }
    }

    private void UpdateHPBarPosition(Transform enemyTransform, GameObject hpBarObject)
    {
        Vector3 posOffset = new(0, enemyTransform.GetComponent<BoxCollider2D>().size.y, 0);
        Vector3 worldPos = enemyTransform.position + hpBarOffset + posOffset;
        hpBarObject.transform.position = worldPos;
    }

    public void UpdateEnemyHealth(Transform enemyTransform, float currentHealth, float maxHealth)
    {
        if (enemyHealthBars.TryGetValue(enemyTransform, out IHealthDisplay hpDisplay))
        {
            hpDisplay.UpdateHealth(currentHealth, maxHealth);
        }
    }
}