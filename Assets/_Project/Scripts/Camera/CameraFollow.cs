using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("-------Target Settings---------")]
    [SerializeField] private Camera _camera;

    [Space(5)]
    [Header("----Follow Settings----")]
    [Header("Offsetting the camera relative to the target")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Smoothing time")] [Range(0.01f, 1f)]
    [SerializeField] private float smoothTime = 0.15f;

    [Header("Maximum camera speed")]
    [SerializeField] private float maxSpeed = float.PositiveInfinity;


    // Текущая скорость для SmoothDamp
    private Vector3 velocity = Vector3.zero;
    
    private Transform _target;

    private void LateUpdate()
    {
        if (!Services.PlayerService.HasPlayer)
        {
            _target = null;
            return;
        }
        
        if (_target == null) _target = Services.PlayerService.GetPlayerTransform();
        if (_target != null) FollowTarget();
    }


    private void FollowTarget()
    {
        Vector3 targetPosition = _target.position + offset;

        Vector3 smoothedPosition = Vector3.SmoothDamp(
            _camera.transform.position,
            targetPosition,
            ref velocity,
            smoothTime,
            maxSpeed,
            Time.deltaTime
        );

        _camera.transform.position = smoothedPosition;
    }


    public void SnapToTarget()
    {
        if (_target == null) return;

        _camera.transform.position = _target.position + offset;
        velocity = Vector3.zero;
    }
}
