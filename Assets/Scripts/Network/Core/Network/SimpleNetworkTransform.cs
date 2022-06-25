using Unity.Netcode;
using UnityEngine;

public class SimpleNetworkTransform : NetworkBehaviour
{
    NetworkVariable<Vector3> _position = new NetworkVariable<Vector3>();
    NetworkVariable<Vector3> _rotation = new NetworkVariable<Vector3>();

    [SerializeField]
    Transform _target;

    Vector3 _velocity = Vector3.zero;

    void Update()
    {
        if (IsClient && IsOwner)
        {
            SetTransformServerRpc(_target.position, _target.rotation.eulerAngles);
        }
        else
        {
            _target.position = Vector3.SmoothDamp(_target.position, _position.Value, ref _velocity, Time.deltaTime);
            _target.rotation = Quaternion.Euler(_rotation.Value);
        }
    }

    [ServerRpc]
    public void SetTransformServerRpc(Vector3 position, Vector3 rotation)
    {
        _position.Value = position;
        _rotation.Value = rotation;
    }
}