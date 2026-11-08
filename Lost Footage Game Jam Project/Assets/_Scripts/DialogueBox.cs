using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueBox : MonoBehaviour
{
    [SerializeField] private GameObject panel;          // the box that shows and hides
    [SerializeField] private TMP_Text lineText;         // the text inside the box
    [SerializeField] private PlayerInput playerInput;   // on the Player

    private string[] lines;
    private int currentLine;
    private Action onFinished;   // the method to run after the last line

    private void Awake()
    {
        panel.SetActive(false);
    }

    // Shows the lines one at a time. whenFinished runs after the last line is closed.
    public void Play(string[] linesToShow, Action whenFinished)
    {
        if (linesToShow.Length == 0)
        {
            throw new ArgumentException("DialogueBox.Play: there are no lines to show.");
        }

        lines = linesToShow;
        currentLine = 0;
        onFinished = whenFinished;

        // The Player map holds Move and Interact, so switching away from it freezes the player.
        playerInput.SwitchCurrentActionMap("UI");
        panel.SetActive(true);
        lineText.text = lines[currentLine];
    }

    // Wired to PlayerInput > Events > UI > Submit
    public void Advance(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        currentLine++;

        if (currentLine < lines.Length)
        {
            lineText.text = lines[currentLine];
        }
        else
        {
            Close();
        }
    }

    private void Close()
    {
        panel.SetActive(false);
        playerInput.SwitchCurrentActionMap("Player");
        onFinished.Invoke();
    }
}
