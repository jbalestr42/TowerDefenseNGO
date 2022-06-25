using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBullet : ASkill
{
    void Start()
    {
        Debug.Log("ShootBullet Start");
        var requirement = new List<IRequirement>();
        requirement.Add(new ValidTargetReq(gameObject));
        SKU.Attribute rate = GetComponent<AttributeManager>().Get<SKU.Attribute>(AttributeType.AttackRate);
        base.Init(rate, requirement);
    }

    public override void Cast(GameObject owner)
    {
        Debug.Log("ShootBullet Cast");
        var tower = owner.GetComponent<TowerBehaviour>();
        var target = tower.GetTarget();
        EntityManager.instance.SpawnBulletServer(tower.GetBulletType(), tower.NetworkObjectId, target.GetComponent<EnemyBehaviour>().NetworkObjectId);
    }
}