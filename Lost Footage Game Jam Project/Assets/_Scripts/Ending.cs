using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Ending : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;      // plays "animation at the end" over the game
    [SerializeField] private PlayerInput playerInput;      // on the Player
    [SerializeField] private GameObject endScreen;         // black screen shown after the video
    [SerializeField] private TMP_Text endText;             // the text on it
    [SerializeField, TextArea(5, 20)] private string endWords = "THE END";   // type the credits under it
    [SerializeField] private AK.Wwise.Event endingSound;   // starts with the video (it has no sound of its own)
    [SerializeField] private VideoClip creditsClip;        // plays after the end animation
    [SerializeField] private AK.Wwise.Event creditsSound;  // starts with the credits (the video has no sound)
    [SerializeField] private GameObject cameraOverlay;     // the REC-style overlay; hidden for the end animation and credits

    private bool showingEndScreen;

    private void Awake()
    {
        if (cameraOverlay == null)   // fail at the start, not at the very end of the game
        {
            throw new System.InvalidOperationException($"Ending on '{name}': Camera Overlay is empty. Run Tools → Lost Footage → Fill Story Lines.");
        }

        endScreen.SetActive(false);
    }

    // Called by GameFlow after the door's last line.
    public void Play()
    {
        playerInput.DeactivateInput();   // she can't move during the videos
        cameraOverlay.SetActive(false);
        endingSound.Post(gameObject);
        videoPlayer.loopPointReached += PlayCredits;
        videoPlayer.Play();
    }

    // Runs when the end animation finishes: the same player switches to the credits video.
    private void PlayCredits(VideoPlayer finishedVideo)
    {
        videoPlayer.loopPointReached -= PlayCredits;
        videoPlayer.clip = creditsClip;
        creditsSound.Post(gameObject);
        videoPlayer.loopPointReached += ShowEndScreen;
        videoPlayer.Play();
    }

    // Runs when the credits video reaches its last frame.
    private void ShowEndScreen(VideoPlayer finishedVideo)
    {
        videoPlayer.loopPointReached -= ShowEndScreen;
        endText.text = endWords + "\n\n\nPress E to return to the menu";
        endScreen.SetActive(true);
        showingEndScreen = true;
        playerInput.ActivateInput();
        playerInput.SwitchCurrentActionMap("UI");
    }

    // Wired to PlayerInput > Events > UI > Submit.
    public void BackToMenu(InputAction.CallbackContext context)
    {
        if (!showingEndScreen || !context.performed) return;
        SceneManager.LoadScene("StartMenu");
    }
}
