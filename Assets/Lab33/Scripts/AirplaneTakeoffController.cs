using UnityEngine;

namespace Lab33
{
    /// <summary>
    /// A compact automatic takeoff sequence used to demonstrate the Enter Wall trigger.
    /// </summary>
    public sealed class AirplaneTakeoffController : MonoBehaviour
    {
        [SerializeField] private float taxiSpeed = 5.5f;
        [SerializeField] private float takeoffDistance = 6f;
        [SerializeField] private float cruiseAltitude = 3.2f;
        [SerializeField] private float climbSpeed = 1.8f;

        private float startZ;
        private float groundHeight;

        public bool HasTakenOff { get; private set; }

        private void Start()
        {
            startZ = transform.position.z;
            groundHeight = transform.position.y;
        }

        private void Update()
        {
            transform.position += Vector3.forward * (taxiSpeed * Time.deltaTime);

            if (!HasTakenOff && transform.position.z - startZ >= takeoffDistance)
            {
                HasTakenOff = true;
            }

            if (!HasTakenOff) return;

            Vector3 position = transform.position;
            position.y = Mathf.MoveTowards(position.y, groundHeight + cruiseAltitude, climbSpeed * Time.deltaTime);
            transform.position = position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(-7f, 0f, 0f), Time.deltaTime * 2.5f);
        }
    }
}
