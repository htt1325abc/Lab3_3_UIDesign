using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lab33
{
    public sealed class CompletionScreen : MonoBehaviour
    {
        public void ReplayDemo()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Lab33_UI_Popup");
        }
    }
}
