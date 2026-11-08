using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorCutscene : MonoBehaviour
{
    [SerializeField] private PlayerMovement hera;               // on the Player
    [SerializeField] private PlayerInput playerInput;           // on the Player
    [SerializeField] private Collider2D heraCollider;           // on the Player
    [SerializeField] private Transform stopPoint;               // where she stops walking
    [SerializeField] private DialogueBox dialogueBox;           // the bottom text box
    [SerializeField, TextArea(2, 5)] private string[] lines;    // what she says after stopping
    [SerializeField] private AK.Wwise.Event doorEnterSound;

    private bool walking;
    private Action onFinished;   // the method to run after her last line

    // Called by GameFlow. whenFinished runs after her last line.
    public void Play(Action whenFinished)
    {
        onFinished = whenFinished;

        playerInput.DeactivateInput();   // keyboard can't move her during the walk
        heraCollider.enabled = false;    // lets her walk in through the wall
        doorEnterSound.Post(hera.gameObject);   // she's at the door right now
        hera.SetDirection(Vector2.up);
        walking = true;
    }

    private void Update()
    {
        if (!walking) return;

        bool reachedStopPoint = hera.transform.position.y >= stopPoint.position.y;
        if (reachedStopPoint)
        {
            Arrive();
        }
    }

    private void Arrive()
    {
        walking = false;
        hera.SetDirection(Vector2.zero);
        heraCollider.enabled = true;     // walls block her again
        playerInput.ActivateInput();
        dialogueBox.Play(lines, onFinished);
    }
}
