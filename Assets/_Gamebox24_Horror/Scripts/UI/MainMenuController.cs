using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject level;

    public void StartButton()
    {
        level.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
