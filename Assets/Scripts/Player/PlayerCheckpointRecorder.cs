using Platformer.Player;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheckpointRecorder : MonoBehaviour
{
    private class CheckpointRecord
    {
        public float time;
        public Vector3 position;
    }

    private Queue<CheckpointRecord> checkpointRecords = new Queue<CheckpointRecord>();

    [SerializeField] private float recordDuration = 5f;

    private PlayerMovement playerMovement;
    private PlayerController playerController;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        // Записываем позицию только если игрок жив и стоит на земле
        if (playerController != null && !playerController.IsDead && playerMovement != null && playerMovement.IsGrounded && playerMovement.IsOnCheckpointGround)
        {
            checkpointRecords.Enqueue(new CheckpointRecord { time = Time.time, position = transform.position });
        }

        // Удаляем записи
        while (checkpointRecords.Count > 1 && Time.time - checkpointRecords.Peek().time > recordDuration)
        {
            checkpointRecords.Dequeue();
        }
    }

    public Vector3 GetCheckpointPosition()
    {
        if (checkpointRecords.Count > 0)
        {
            return checkpointRecords.Peek().position;
        }
        return transform.position;
    }
}
