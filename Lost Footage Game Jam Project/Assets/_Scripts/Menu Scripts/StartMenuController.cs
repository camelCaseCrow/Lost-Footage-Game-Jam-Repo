using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    [SerializeField] private GameObject introScreen;   // black screen with the intro text and Continue button

    private void Awake()
    {
        introScreen.SetActive(false);
    }

    // Wired to the Start button's On Click.
    public void OnStartClick()
    {
        introScreen.SetActive(true);
    }

    // Wired to the Continue button's On Click.
    public void OnContinueClick()
    {
        SceneManager.LoadScene("Game");
    }
}
