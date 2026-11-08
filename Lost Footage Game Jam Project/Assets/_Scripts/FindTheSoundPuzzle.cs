using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FindTheSoundPuzzle : MonoBehaviour
{
    [SerializeField] private DialogueBox dialogueBox;
    [SerializeField] private PlayerInput playerInput;      // on the Player; frozen during the silence
    [SerializeField] private GameObject leftStatue;        // the listen whisper plays from here
    [SerializeField] private AK.Wwise.Event listenWhisperSound;
    [SerializeField] private AK.Wwise.Event openMyEyesSound;     // plays with "('Open my eyes, so that I may see'…?)"
    [SerializeField] private float silenceSeconds = 5f;
    [SerializeField, TextArea(2, 5)] private string[] leftStatueLines;      // ends with "(Listen closer?)"
    [SerializeField, TextArea(2, 5)] private string[] comingFromHereLines;  // "(…It's coming from here, somehow!)"
    [SerializeField, TextArea(2, 5)] private string[] afterSilenceLines;    // "('Open my eyes…" to "(It is cold, and you are alone.)"

    private bool silent;          // true during the silence
    private float silenceLeft;
    private Action onSolved;

    // Called by GameFlow. whenSolved runs after the left statue's last line.
    public void Begin(Action whenSolved)
    {
        onSolved = whenSolved;
    }

    // Wired to the left statue spot's Interactable Object > On Interact.
    public void ExamineLeftStatue()
    {
        dialogueBox.PlayThenAsk(leftStatueLines, ListenAtLeftStatue, null);
    }

    private void ListenAtLeftStatue()
    {
        dialogueBox.Play(comingFromHereLines, StartSilence);
    }

    private void StartSilence()
    {
        playerInput.DeactivateInput();          // she stands still while it plays out
        listenWhisperSound.Post(leftStatue);
        silenceLeft = silenceSeconds;
        silent = true;
    }

    private void Update()
    {
        if (!silent) return;

        silenceLeft -= Time.deltaTime;
        if (silenceLeft > 0f) return;   // still silent

        silent = false;
        playerInput.ActivateInput();
        openMyEyesSound.Post(leftStatue);   // the first after-silence line is "('Open my eyes…')"
        dialogueBox.Play(afterSilenceLines, onSolved);
    }
}
