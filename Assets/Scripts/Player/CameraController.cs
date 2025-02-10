using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _player;
    private Vector3 pos;

    private void Awake()
    {
        if (!_player)
            _player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (_player)
        {
            pos = _player.position;
            pos.z = -10f;
            transform.position = pos;
        }

        //transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 2);
    }
}