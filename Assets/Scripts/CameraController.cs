using System.Collections;
using System.Collections.Generic;
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

    // Update is called once per frame
    void Update()
    {
        pos = _player.position;
        pos.z = -10f;

        transform.position = pos;
        //transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 2);
    }
}
