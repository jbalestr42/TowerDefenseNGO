using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkClientChildrenTransform : NetworkBehaviour
{
    internal struct NTransform : INetworkSerializable, IEquatable<NTransform>
    {
        public float positionX, positionY, positionZ;
        public float quaternionX, quaternionY, quaternionZ, quaternionW;
        public Vector3 localPosition
        {
            get { return new Vector3(positionX, positionY, positionZ); }
            set { positionX = value.x; positionY = value.y; positionZ = value.z; }
        }

        public Quaternion localRotation
        {
            get { return new Quaternion(quaternionX, quaternionY, quaternionZ, quaternionW); }
            set { quaternionX = value.x; quaternionY = value.y; quaternionZ = value.z; quaternionW = value.w; }
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref positionX);
            serializer.SerializeValue(ref positionY);
            serializer.SerializeValue(ref positionZ);
            serializer.SerializeValue(ref quaternionX);
            serializer.SerializeValue(ref quaternionY);
            serializer.SerializeValue(ref quaternionZ);
            serializer.SerializeValue(ref quaternionW);
        }

        public bool Equals(NTransform other)
        {
            return positionX == other.positionX && positionY == other.positionY && positionZ == other.positionZ
            && quaternionW == other.quaternionW && quaternionX == other.quaternionX && quaternionY == other.quaternionY && quaternionZ == other.quaternionZ;
        }
    }

    internal struct NTArray : INetworkSerializable
    {
        public NTransform[] transformsdata;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            int length = 0;
            if (!serializer.IsReader)
            {
                length = transformsdata.Length;
            }

            serializer.SerializeValue(ref length);

            // Array
            if (serializer.IsReader)
            {
                transformsdata = new NTransform[length];
            }

            for (int n = 0; n < length; ++n)
            {
                transformsdata[n].NetworkSerialize(serializer);
            }

        }
    }

    NTArray _transformsData;
    Transform[] _transforms;

    void Awake()
    {
        CreateArrayFromTransforms();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsSpawned)
        {
            return;
        }

        if (IsOwner)
        {
            UpdateArrayFromTransforms();
            UpdateArrayServerRPC(_transformsData);
        }
        else if (IsServer)
        {
            UpdateArrayClientRPC(_transformsData);
        }
        else
        {
            UpdateTransformsFromArray();
        }
    }

    void CreateArrayFromTransforms()
    {
        _transforms = GetComponentsInChildren<Transform>(true);
        _transformsData = new NTArray();
        _transformsData.transformsdata = new NTransform[_transforms.Length];
        for (int i = 0; i < _transforms.Length; i++)
        {
            NTransform nt = new NTransform();
            nt.localPosition = _transforms[i].localPosition;
            nt.localRotation = _transforms[i].localRotation;
            _transformsData.transformsdata[i] = nt;
        }
    }

    [ServerRpc]
    void UpdateArrayServerRPC(NTArray nt)
    {
        if (_transformsData.transformsdata.Length != nt.transformsdata.Length)
        {
            Debug.Log("Data don't match");
            return;
        }
        for (int i = 0; i < nt.transformsdata.Length; i++)
        {
            _transformsData.transformsdata[i] = nt.transformsdata[i];
        }
    }

    [ClientRpc]
    void UpdateArrayClientRPC(NTArray nt)
    {
        if (IsLocalPlayer)
        {
            return;
        }
        if (_transformsData.transformsdata.Length != nt.transformsdata.Length)
        {
            Debug.Log("Data don't match");
            return;
        }
        for (int i = 0; i < nt.transformsdata.Length; i++)
        {
            _transformsData.transformsdata[i] = nt.transformsdata[i];
        }
    }
    void UpdateArrayFromTransforms()
    {
        if (_transformsData.transformsdata.Length != _transforms.Length)
        {
            Debug.Log("Data don't match");
            return;
        }
        for (int i = 0; i < _transforms.Length; i++)
        {
            NTransform nt = new NTransform();
            nt.localPosition = _transforms[i].localPosition;
            nt.localRotation = _transforms[i].localRotation;
            _transformsData.transformsdata[i] = nt;
        }
    }


    void UpdateTransformsFromArray()
    {
        if (_transformsData.transformsdata.Length != _transforms.Length)
        {
            Debug.Log("Data don't match");
            return;
        }
        for (int i = 0; i < _transforms.Length; i++)
        {
            _transforms[i].localPosition = _transformsData.transformsdata[i].localPosition;
            _transforms[i].localRotation = _transformsData.transformsdata[i].localRotation;
        }
    }
}