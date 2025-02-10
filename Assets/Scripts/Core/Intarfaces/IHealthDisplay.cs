public interface IHealthDisplay
{
    /// <summary>
    /// Обновляет отображение здоровья.
    /// </summary>
    /// <param name="currentHealth">Текущее здоровье.</param>
    /// <param name="maxHealth">Максимальное здоровье.</param>
    void UpdateHealth(float currentHealth, float maxHealth);
}