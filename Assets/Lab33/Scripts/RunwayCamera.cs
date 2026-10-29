using UnityEngine;

namespace Lab33
{
    /// <summary>
    /// Smoothly follows the airplane while keeping the runway and trigger gate readable.
    /// </summary>
    public sealed class RunwayCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new(0f, 5.4f, -9.5f);
        [SerializeField] private float followSpeed = 4f;

        public void SetTarget(Transform followTarget)
        {
            target = followTarget;
        }

        private void LateUpdate()
        {
            if (target == null) return;
            Vector3 desired = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * followSpeed);
            transform.LookAt(target.position + Vector3.forward * 4f + Vector3.up * 0.5f);
        }
    }
}
