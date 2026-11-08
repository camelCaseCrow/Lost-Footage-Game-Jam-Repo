using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueBox : MonoBehaviour
{
    [SerializeField] private GameObject panel;          // the box that shows and hides
    [SerializeField] private TMP_Text lineText;         // the text inside the box
    [SerializeField] private PlayerInput playerInput;   // on the Player
    [SerializeField] private AK.Wwise.Event openSound;  // the box pops up
    [SerializeField] private GameObject choices;        // holds Yes and No, only shown for questions
    [SerializeField] private TMP_Text yesText;
    [SerializeField] private TMP_Text noText;

    private string[] lines;
    private int currentLine;
    private Action onFinished;   // the method to run after the last line

    private bool hasQuestionAtEnd;   // is the last line a Yes/No question?
    private bool asking;             // are Yes/No showing right now?
    private bool yesSelected;        // which one E will pick
    private Action onYes;            // the method to run after Yes

    private void Awake()
    {
        panel.SetActive(false);
        choices.SetActive(false);
    }

    // Shows the lines one at a time. whenFinished runs after the last line; null means nothing happens.
    public void Play(string[] linesToShow, Action whenFinished)
    {
        onFinished = whenFinished;
        Open(linesToShow, false);
    }

    // Like Play, but the last line is a Yes/No question. whenYes runs after Yes; No just closes the box.
    public void PlayThenAsk(string[] linesToShow, Action whenYes)
    {
        onFinished = null;
        onYes = whenYes;
        Open(linesToShow, true);
    }

    private void Open(string[] linesToShow, bool lastLineIsQuestion)
    {
        if (linesToShow.Length == 0)
        {
            throw new ArgumentException("DialogueBox: there are no lines to show.");
        }

        lines = linesToShow;
        currentLine = 0;
        hasQuestionAtEnd = lastLineIsQuestion;

        // The Player map holds Move and Interact, so switching away from it freezes the player.
        playerInput.SwitchCurrentActionMap("UI");
        panel.SetActive(true);
        openSound.Post(gameObject);
        ShowCurrentLine();
    }

    // Puts the current line in the box. On a question's last line, also shows Yes/No.
    private void ShowCurrentLine()
    {
        lineText.text = lines[currentLine];

        bool onLastLine = currentLine == lines.Length - 1;
        asking = hasQuestionAtEnd && onLastLine;
        choices.SetActive(asking);

        if (asking)
        {
            SelectYes(true);   // Yes is highlighted first
        }
    }

    // Wired to PlayerInput > Events > UI > Submit
    public void Advance(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (asking)
        {
            Choose();   // E confirms the highlighted choice
            return;
        }

        currentLine++;

        if (currentLine < lines.Length)
        {
            ShowCurrentLine();
        }
        else
        {
            Close();
        }
    }

    // Wired to PlayerInput > Events > UI > Navigate. Left = Yes, right = No.
    public void Navigate(InputAction.CallbackContext context)
    {
        if (!asking) return;

        float x = context.ReadValue<Vector2>().x;

        if (x < -0.5f)
        {
            SelectYes(true);
        }
        else if (x > 0.5f)
        {
            SelectYes(false);
        }
    }

    // Highlights one choice white and the other grey.
    private void SelectYes(bool selectYes)
    {
        yesSelected = selectYes;

        if (selectYes)
        {
            yesText.color = Color.white;
            noText.color = Color.gray;
        }
        else
        {
            yesText.color = Color.gray;
            noText.color = Color.white;
        }
    }

    // Closes the box, then runs onYes if Yes was picked.
    private void Choose()
    {
        bool choseYes = yesSelected;
        Close();

        if (choseYes)
        {
            onYes.Invoke();
        }
    }

    private void Close()
    {
        panel.SetActive(false);
        choices.SetActive(false);
        asking = false;
        playerInput.SwitchCurrentActionMap("Player");

        // null means nothing happens after these lines.
        if (onFinished != null)
        {
            onFinished.Invoke();
        }
    }
}
