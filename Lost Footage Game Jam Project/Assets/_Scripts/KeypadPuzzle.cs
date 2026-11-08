using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeypadPuzzle : MonoBehaviour
{
    [SerializeField] private DialogueBox dialogueBox;
    [SerializeField] private PlayerInput playerInput;      // on the Player
    [SerializeField] private GameObject keypadScreen;      // the panel that shows what's typed
    [SerializeField] private TMP_Text keypadText;          // the text inside it
    [SerializeField] private GameObject keypad;            // the keypad spot; sounds play here
    [SerializeField] private int correctAnswer;            // floor area: 42 × 31 = 1302 (filled by the tool)
    [SerializeField] private AK.Wwise.Event keyPressSound;
    [SerializeField] private AK.Wwise.Event wrongSound;
    [SerializeField] private AK.Wwise.Event correctSound;
    [SerializeField] private AK.Wwise.Event shardSound;          // shard #4 picked up (at the keypad)
    [SerializeField, TextArea(2, 5)] private string[] firstLines;     // 1st time: "(You hadn't noticed it before…" … "(Didn't you?)"
    [SerializeField, TextArea(2, 5)] private string[] laterLines;     // after that: "('ENTER SQUARE FOOTAGE')"
    [SerializeField, TextArea(2, 5)] private string[] wrongLines;     // "(Nothing happens.)"
    [SerializeField, TextArea(2, 5)] private string[] correctLines;   // glass piece #3 … "(Putting the shards together, you see the completed mosaic.)"
    [SerializeField] private GameObject mosaicPicture;                 // all4shards, shown for a moment after the right number
    [SerializeField] private AK.Wwise.Event mosaicCompleteSound;       // the completed mosaic appears
    [SerializeField, TextArea(2, 5)] private string[] mosaicLines;      // while the picture shows: "...", "(YOU NEED TO GET OUT OF HERE.)"

    private const int MaxDigits = 6;

    private string typed = "";
    private bool examinedBefore;
    private bool screenOpen;
    private int openedOnFrame;      // the frame the screen opened
    private Action onSolved;

    private void Awake()
    {
        keypadScreen.SetActive(false);
        mosaicPicture.SetActive(false);
    }

    // Called by GameFlow at the start of state 06. whenSolved runs after the correct lines.
    public void Begin(Action whenSolved)
    {
        onSolved = whenSolved;
    }

    // Wired to the keypad spot in Keypad Spots. The lines lead straight into the keypad.
    public void ExamineKeypad()
    {
        if (!examinedBefore)
        {
            examinedBefore = true;
            dialogueBox.Play(firstLines, OpenKeypad);
        }
        else
        {
            dialogueBox.Play(laterLines, OpenKeypad);
        }
    }

    private void OpenKeypad()
    {
        playerInput.SwitchCurrentActionMap("UI");    // freezes her while she types
        keypadScreen.SetActive(true);
        screenOpen = true;
        openedOnFrame = Time.frameCount;
        typed = "";
        Keyboard.current.onTextInput += TypeCharacter;
        Redraw();
    }

    private void CloseKeypad()
    {
        Keyboard.current.onTextInput -= TypeCharacter;
        keypadScreen.SetActive(false);
        screenOpen = false;
        playerInput.SwitchCurrentActionMap("Player");
    }

    // Gets every typed character while the keypad is open. Only digits count.
    private void TypeCharacter(char character)
    {
        if (!char.IsDigit(character) || typed.Length >= MaxDigits) return;

        typed += character;
        keyPressSound.Post(keypad);
        Redraw();
    }

    // Backspace has no character, so it's checked every frame instead.
    private void Update()
    {
        if (!screenOpen) return;

        if (Keyboard.current.backspaceKey.wasPressedThisFrame && typed.Length > 0)
        {
            typed = typed.Substring(0, typed.Length - 1);   // drop the last digit
            Redraw();
        }
    }

    // Wired to PlayerInput > Events > UI > Submit (Enter or E).
    public void Confirm(InputAction.CallbackContext context)
    {
        if (!screenOpen || !context.performed) return;
        if (Time.frameCount == openedOnFrame) return;   // this press just closed the lines
        if (typed.Length == 0) return;                  // nothing typed yet

        bool isCorrect = typed == correctAnswer.ToString();
        CloseKeypad();

        if (isCorrect)
        {
            correctSound.Post(keypad);
            dialogueBox.PlayWithShardSound(correctLines, shardSound, ShowMosaic);
        }
        else
        {
            wrongSound.Post(keypad);
            dialogueBox.Play(wrongLines, null);   // E on the keypad again to retry
        }
    }

    // The completed mosaic stays up for "..." and "(YOU NEED TO GET OUT OF HERE.)".
    private void ShowMosaic()
    {
        mosaicPicture.SetActive(true);
        mosaicCompleteSound.Post(keypad);
        dialogueBox.Play(mosaicLines, HideMosaic);
    }

    // After the last line: the mosaic goes with the box, then on to state 07.
    private void HideMosaic()
    {
        mosaicPicture.SetActive(false);
        onSolved.Invoke();
    }

    // Wired to PlayerInput > Events > UI > Cancel. Esc steps away.
    public void StepAway(InputAction.CallbackContext context)
    {
        if (!screenOpen || !context.performed) return;
        CloseKeypad();
    }

    private void Redraw()
    {
        keypadText.text = "ENTER SQUARE FOOTAGE\n\n" + typed + "_\n\n0-9: type   Backspace: delete   Enter: confirm   Esc: step away";
    }
}
