public interface IHealthDisplay
{
    /// <summary>
    /// ��������� ����������� ��������.
    /// </summary>
    /// <param name="currentHealth">������� ��������.</param>
    /// <param name="maxHealth">������������ ��������.</param>
    void UpdateHealth(float currentHealth, float maxHealth);
}