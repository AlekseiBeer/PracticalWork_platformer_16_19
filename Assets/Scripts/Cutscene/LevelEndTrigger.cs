using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    [SerializeField] private FinishCutsceneManager cutsceneManager;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
            cutsceneManager.StartDoorCutscene(other.gameObject);
        }
    }
}