using UnityEngine;

public class FollowCamera : ICameraMovement
{
    Transform _target;
    public Transform target { get { return _target; } set { _target = value; } }

    Vector3 _offset = new Vector3(0, 5, -11);

    float _zoom = 1f;
    public float zoom { get { return _zoom; } set { _zoom = Mathf.Clamp(value, _minZoom, _maxZoom); } }

    [SerializeField] float _minZoom = 1f;
    public float minZoom { get { return _minZoom; } set { _minZoom = value; } }

    [SerializeField] float _maxZoom = 3f;
    public float maxZoom { get { return _maxZoom; } set { _maxZoom = value; } }

    float _smoothTime = 0f;
    Vector3 _velocity = Vector3.zero;

    public FollowCamera(Transform target, Vector3 offset)
    {
        _target = target;
        _offset = offset;
    }

    public void Update(Camera camera)
    {
        Vector3 targetPosition = _target.transform.position + _offset * _zoom;
        //Vector3 targetPosition = _target.TransformPoint(_offset); to look toward the target

        camera.transform.position = Vector3.SmoothDamp(camera.transform.position, targetPosition, ref _velocity, _smoothTime);
        camera.transform.LookAt(_target);
    }
}