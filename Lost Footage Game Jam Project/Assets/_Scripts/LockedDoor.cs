using System;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [SerializeField] private DialogueBox dialogueBox;
    [SerializeField, TextArea(2, 5)] private string[] lockedLines;   // when she tries the door

    private bool active;          // true once GameFlow wants her to try the door
    private Action onFinished;    // the method to run after the last line

    // Called by GameFlow. whenFinished runs after the locked-door lines.
    public void Begin(Action whenFinished)
    {
        onFinished = whenFinished;
        active = true;
    }

    // Wired to the door's Interactable Object > On Interact.
    public void TryOpen()
    {
        if (!active) return;   // too early: nothing happens yet

        active = false;
        dialogueBox.Play(lockedLines, onFinished);
    }
}
