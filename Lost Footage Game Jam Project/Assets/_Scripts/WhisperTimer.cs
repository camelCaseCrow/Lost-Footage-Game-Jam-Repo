using UnityEngine;

public class WhisperTimer : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event whisperSound;
    [SerializeField] private GameObject leftStatue;            // the whisper comes from here
    [SerializeField] private DialogueBox dialogueBox;
    [SerializeField, TextArea(2, 5)] private string[] firstWhisperLines;   // "…!" "(What was that noise?)"
    [SerializeField] private float firstDelay = 3f;            // seconds after she gets control back

    private bool waiting;
    private float secondsLeft;     // until the first whisper

    // Called by GameFlow when she gets control back after the locked door.
    public void Begin()
    {
        waiting = true;
        secondsLeft = firstDelay;
    }

    private void Update()
    {
        if (!waiting) return;

        secondsLeft -= Time.deltaTime;
        if (secondsLeft > 0f) return;   // not time yet

        waiting = false;
        whisperSound.Post(leftStatue);   // the Wwise event repeats the whispers by itself from here on
        dialogueBox.Play(firstWhisperLines, null);
    }
}
