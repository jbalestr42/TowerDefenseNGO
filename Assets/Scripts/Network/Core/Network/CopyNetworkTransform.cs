using Unity.Netcode;
using UnityEngine;

public class CopyNetworkTransform : NetworkBehaviour
{
    NetworkVariable<Vector3> _position = new NetworkVariable<Vector3>();
    NetworkVariable<Vector3> _rotation = new NetworkVariable<Vector3>();

    [SerializeField]
    Transform _source;

    [SerializeField]
    Transform _destination;

    Vector3 _velocity = Vector3.zero;

    void Update()
    {
        if (IsClient && IsOwner)
        {
            SetTransformServerRpc(_source.position, _source.rotation.eulerAngles);
        }
        else
        {
            _destination.position = Vector3.SmoothDamp(_destination.position, _position.Value, ref _velocity, Time.deltaTime);
            _destination.rotation = Quaternion.Euler(_rotation.Value);
        }
    }

    [ServerRpc]
    public void SetTransformServerRpc(Vector3 position, Vector3 rotation)
    {
        _position.Value = position;
        _rotation.Value = rotation;
    }
}