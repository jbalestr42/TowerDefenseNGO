using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    GameObject _target;
    public GameObject target { get { return _target; } set { _target = value; } }

    GameObject _owner;
    public GameObject owner { get { return _owner; } set { _owner = value; } }

    public float _speed;

	void Update()
    {
		if (_target)
        {
            var direction = _target.transform.position - transform.position;
            transform.Translate(direction.normalized * _speed * Time.deltaTime);
        }
        else
        {
            Destroy(gameObject);
        }
	}

    void OnTriggerEnter(Collider other)
    {
        var target = other.gameObject.GetComponentInParent<ITargetable>();
        if (target != null)
        {
            target.OnHit(_owner);
            EntityManager.instance.DestroyBullet(gameObject);
        }
    }
}
