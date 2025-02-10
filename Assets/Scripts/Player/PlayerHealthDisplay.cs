using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthDisplay : MonoBehaviour, IHealthDisplay
{
    [SerializeField] private Slider healthSlider;

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }
}