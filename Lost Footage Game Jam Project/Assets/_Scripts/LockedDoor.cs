using System;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [SerializeField] private DialogueBox dialogueBox;
    [SerializeField, TextArea(2, 5)] private string[] notYetLines;            // "(Can't leave yet.)" …
    [SerializeField, TextArea(2, 5)] private string[] lockedLines;            // the locked scene
    [SerializeField, TextArea(2, 5)] private string[] stillLockedLines;       // next try: "(It still won't FUCKING open.)" …
    [SerializeField, TextArea(2, 5)] private string[] stillLockedAgainLines;  // every try after: "(And nope, it's still not opening.)"
    [SerializeField, TextArea(2, 5)] private string[] openLines;              // state 07: "(The Eye is complete.)" … "(Above you.)"
    [SerializeField] private AK.Wwise.Event tryOpenSound;   // she pulls the handle (not played for "Can't leave yet")
    [SerializeField] private AK.Wwise.Event doorOpenSound;  // "(The door creaks open,)"

    // The door's phases, in story order.
    private enum DoorState { NotYet, ReadyToTry, Tried, TriedTwice, Unlocked }

    private DoorState state = DoorState.NotYet;
    private Action onFinished;    // runs after the locked scene
    private Action onOpened;      // runs after the door's last line

    // Called by GameFlow once the floor is measured. whenFinished runs after the locked scene.
    public void Begin(Action whenFinished)
    {
        onFinished = whenFinished;
        state = DoorState.ReadyToTry;
    }

    // Called by GameFlow at the start of state 07. whenOpened runs after the open lines.
    public void Unlock(Action whenOpened)
    {
        onOpened = whenOpened;
        state = DoorState.Unlocked;
    }

    // Wired to the door's Interactable Object > On Interact.
    public void TryOpen()
    {
        if (state == DoorState.NotYet)
        {
            dialogueBox.Play(notYetLines, null);
            return;
        }

        if (state == DoorState.Unlocked)
        {
            doorOpenSound.Post(gameObject);
            dialogueBox.Play(openLines, onOpened);
            return;
        }

        tryOpenSound.Post(gameObject);   // every real attempt from here on

        if (state == DoorState.ReadyToTry)
        {
            state = DoorState.Tried;
            dialogueBox.Play(lockedLines, onFinished);
        }
        else if (state == DoorState.Tried)
        {
            state = DoorState.TriedTwice;
            dialogueBox.Play(stillLockedLines, null);
        }
        else
        {
            dialogueBox.Play(stillLockedAgainLines, null);
        }
    }
}
