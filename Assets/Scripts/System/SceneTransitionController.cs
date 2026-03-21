using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionController : MonoBehaviour
{
    [SerializeField] GameObject targetPanel;

    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void SetPanelActive(bool isActive)
    {
        if (targetPanel == null)
        {
            return;
        }

        targetPanel.SetActive(isActive);
    }

    public void TogglePanel()
    {
        if (targetPanel == null)
        {
            return;
        }

        targetPanel.SetActive(!targetPanel.activeSelf);
    }
}
