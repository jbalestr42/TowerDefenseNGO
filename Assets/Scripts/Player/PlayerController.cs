using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] Transform _cameraTarget;
    [SerializeField] float _playerSpeed = 40.0f;
    [SerializeField] float _zoomSpeed = 50.0f;
    FollowCamera _cameraMovement = null;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            _cameraMovement = new FollowCamera(_cameraTarget, new Vector3(0f, 10, -10));
            CameraManager.instance.SetCameraMovement(_cameraMovement);
        }
    }

    void Update()
    {
        if (_cameraMovement != null)
        {
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            _cameraMovement.zoom += Input.mouseScrollDelta.y * Time.deltaTime * _zoomSpeed * -1f;
            _cameraTarget.Translate(move * Time.deltaTime * _playerSpeed);
        }
    }
}