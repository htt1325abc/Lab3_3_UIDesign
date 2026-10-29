using UnityEngine;

namespace Lab33
{
    /// <summary>
    /// Enter Wall trigger: opens the popup only after the airplane is airborne.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class EnterGame : MonoBehaviour
    {
        [SerializeField] private Canvas gameMenu;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private AirplaneTakeoffController airplane;

        private bool hasTriggered;

        public void Configure(Canvas menu, GameManager manager, AirplaneTakeoffController airplaneController)
        {
            gameMenu = menu;
            gameManager = manager;
            airplane = airplaneController;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasTriggered || !other.CompareTag("Player")) return;

            AirplaneTakeoffController enteredAirplane = other.GetComponentInParent<AirplaneTakeoffController>();
            AirplaneTakeoffController target = enteredAirplane != null ? enteredAirplane : airplane;
            if (target == null || !target.HasTakenOff) return;

            hasTriggered = true;
            if (gameManager != null) gameManager.ShowPopup();
            else if (gameMenu != null) gameMenu.enabled = true;
        }
    }
}
