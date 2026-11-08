using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrganPuzzle : MonoBehaviour
{
    [SerializeField] private DialogueBox dialogueBox;
    [SerializeField] private PlayerInput playerInput;      // on the Player
    [SerializeField] private GameObject stopsScreen;       // the panel that shows the 8 stops
    [SerializeField] private TMP_Text stopsText;           // the text grid inside it
    [SerializeField] private GameObject organ;             // sounds play here
    [SerializeField] private bool[] correctStops = new bool[8];   // ticked = must be pulled. 0–3 left column top→bottom, 4–7 right column
    [SerializeField] private AK.Wwise.Event stopPulledSound;
    [SerializeField] private AK.Wwise.Event stopPushedSound;
    [SerializeField] private AK.Wwise.Event stopsCompleteSound;   // the right pattern is set
    [SerializeField] private AK.Wwise.Event shardSound;           // shard #2 picked up
    [SerializeField, TextArea(2, 5)] private string[] firstLines;        // 1st time: "(A massive pipe organ…" … "(…the only thing you can mess with is the stops…)"
    [SerializeField, TextArea(2, 5)] private string[] laterLines;        // after that; ends with "(Mess with the stops?)"
    [SerializeField, TextArea(2, 5)] private string[] foundPieceLines;   // "(Something shimmers by the organ pedal.)" …

    private const int Rows = 4;

    private bool[] pulled = new bool[8];   // which stops are out right now
    private bool examinedBefore;
    private bool screenOpen;
    private int openedOnFrame;             // the frame the screen opened
    private int column;                    // 0 = left, 1 = right
    private int row;                       // 0 = top … 3 = bottom
    private Action onSolved;

    private void Awake()
    {
        if (correctStops.Length != 8)
        {
            throw new InvalidOperationException($"OrganPuzzle on '{name}': Correct Stops needs exactly 8 entries.");
        }

        stopsScreen.SetActive(false);
    }

    // Called by GameFlow at the start of state 05. whenSolved runs after the found-piece lines.
    public void Begin(Action whenSolved)
    {
        onSolved = whenSolved;
    }

    // Wired to the organ spot in Organ Spots.
    public void ExamineOrgan()
    {
        if (!examinedBefore)
        {
            examinedBefore = true;
            dialogueBox.Play(firstLines, null);
        }
        else
        {
            dialogueBox.PlayThenAsk(laterLines, OpenStops, null);
        }
    }

    private void OpenStops()
    {
        playerInput.SwitchCurrentActionMap("UI");   // freezes her while she's at the stops
        stopsScreen.SetActive(true);
        screenOpen = true;
        openedOnFrame = Time.frameCount;
        Redraw();
    }

    private void CloseStops()
    {
        stopsScreen.SetActive(false);
        screenOpen = false;
        playerInput.SwitchCurrentActionMap("Player");
    }

    // Wired to PlayerInput > Events > UI > Navigate. Moves the highlight.
    public void Navigate(InputAction.CallbackContext context)
    {
        if (!screenOpen || !context.performed) return;

        Vector2 direction = context.ReadValue<Vector2>();

        if (direction.y > 0.5f)
        {
            row = Mathf.Max(row - 1, 0);
        }
        else if (direction.y < -0.5f)
        {
            row = Mathf.Min(row + 1, Rows - 1);
        }
        else if (direction.x < -0.5f)
        {
            column = 0;
        }
        else if (direction.x > 0.5f)
        {
            column = 1;
        }

        Redraw();
    }

    // Wired to PlayerInput > Events > UI > Submit. E pulls or pushes the highlighted stop.
    public void PullOrPush(InputAction.CallbackContext context)
    {
        if (!screenOpen || !context.performed) return;
        if (Time.frameCount == openedOnFrame) return;   // this E press just answered "Yes"

        int index = column * Rows + row;
        pulled[index] = !pulled[index];

        if (pulled[index])
        {
            stopPulledSound.Post(organ);
        }
        else
        {
            stopPushedSound.Post(organ);
        }

        Redraw();

        if (MatchesPattern())
        {
            Solve();
        }
    }

    // Wired to PlayerInput > Events > UI > Cancel. Esc steps away from the organ.
    public void StepAway(InputAction.CallbackContext context)
    {
        if (!screenOpen || !context.performed) return;
        CloseStops();
    }

    // Draws the 2 × 4 grid: [|] = pulled out, [ ] = pushed in, the highlighted stop in yellow.
    private void Redraw()
    {
        string grid = "";

        for (int r = 0; r < Rows; r++)
        {
            grid += StopText(0, r) + "      " + StopText(1, r) + "\n";
        }

        grid += "\nWASD: move   E: pull / push   Esc: step away";
        stopsText.text = grid;
    }

    private string StopText(int stopColumn, int stopRow)
    {
        string stop = "[ ]";
        if (pulled[stopColumn * Rows + stopRow])
        {
            stop = "[|]";
        }

        bool isHighlighted = stopColumn == column && stopRow == row;
        if (isHighlighted)
        {
            stop = "<color=yellow>" + stop + "</color>";
        }

        return stop;
    }

    // True when every stop is exactly as Correct Stops says.
    private bool MatchesPattern()
    {
        for (int i = 0; i < pulled.Length; i++)
        {
            if (pulled[i] != correctStops[i])
            {
                return false;
            }
        }
        return true;
    }

    private void Solve()
    {
        CloseStops();
        stopsCompleteSound.Post(organ);
        dialogueBox.PlayWithShardSound(foundPieceLines, shardSound, onSolved);
    }
}
