using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ApplyEffectOnWalk : NetworkBehaviour
{
    [SerializeField] float _speedFactor = -0.8f;

    public GameObject GetTarget()
    {
        return null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            Debug.Log("[DEBUG] ENTER");
            var target = other.gameObject.GetComponentInParent<ITargetable>();
            if (target != null)
            {
                var attributes = other.gameObject.GetComponentInParent<AttributeManager>();
                Debug.Log("[DEBUG] STARt " + other.gameObject.name + " - " + attributes);
                // TODO: modifier from SO ?
                attributes.Get<SKU.Attribute>(AttributeType.Speed).AddRelativeModifier(gameObject, Factory.CreateModifier(ModifierType.Flat, _speedFactor));
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (IsServer)
        {
            Debug.Log("[DEBUG] EXIT");
            var target = other.gameObject.GetComponentInParent<ITargetable>();
            if (target != null)
            {
                Debug.Log("[DEBUG] OVER");
                var attributes = other.gameObject.GetComponentInParent<AttributeManager>();
                attributes.Get<SKU.Attribute>(AttributeType.Speed).RemoveRelativeModifierFromSource(gameObject);
            }
        }
    }
}
