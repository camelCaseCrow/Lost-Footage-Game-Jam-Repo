using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    [SerializeField] private GameObject introScreen;   // black screen with the intro text and Continue button
    [SerializeField] private AK.Wwise.Event startClickSound;
    [SerializeField] private AK.Wwise.Event continueClickSound;

    private void Awake()
    {
        introScreen.SetActive(false);
    }

    // Wired to the Start button's On Click.
    public void OnStartClick()
    {
        startClickSound.Post(gameObject);
        introScreen.SetActive(true);
    }

    // Wired to the Continue button's On Click.
    public void OnContinueClick()
    {
        continueClickSound.Post(gameObject);
        SceneManager.LoadScene("Game");
    }
}
