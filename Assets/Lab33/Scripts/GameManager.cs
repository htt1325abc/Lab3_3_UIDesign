using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lab33
{
    /// <summary>
    /// Owns the popup, injects its dynamic TMP content, and handles both buttons.
    /// This component is attached to Main Camera as required by the lab slides.
    /// </summary>
    public sealed class GameManager : MonoBehaviour
    {
        [Header("Popup UI")]
        [SerializeField] private Canvas gameMenu;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text message;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button restartButton;

        [Header("Navigation")]
        [SerializeField] private string nextSceneName = "Lab33_Complete";

        private bool isShowingMenu;

        public void Configure(Canvas menu, TMP_Text titleLabel, TMP_Text messageLabel, Button continueControl, Button restartControl)
        {
            gameMenu = menu;
            title = titleLabel;
            message = messageLabel;
            continueButton = continueControl;
            restartButton = restartControl;
        }

        private void Start()
        {
            isShowingMenu = false;
            if (gameMenu != null) gameMenu.enabled = false;

            // Slide 29: title, message and button labels are loaded from code.
            if (title != null) title.text = "RUNWAY CLEARED";
            if (message != null)
            {
                message.text = "Takeoff confirmed. Continue to the next training sector or restart this approach.";
            }

            TMP_Text continueLabel = continueButton != null ? continueButton.GetComponentInChildren<TMP_Text>() : null;
            TMP_Text restartLabel = restartButton != null ? restartButton.GetComponentInChildren<TMP_Text>() : null;
            if (continueLabel != null) continueLabel.text = "CONTINUE";
            if (restartLabel != null) restartLabel.text = "RESTART";
        }

        public void ShowPopup()
        {
            if (isShowingMenu || gameMenu == null) return;
            isShowingMenu = true;
            gameMenu.enabled = true;
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void NextScene()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nextSceneName);
        }

        public void RestartScene()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }
    }
}
