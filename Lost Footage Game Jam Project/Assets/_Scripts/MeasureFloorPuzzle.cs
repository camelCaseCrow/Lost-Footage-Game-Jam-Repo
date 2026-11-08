using System;
using System.Collections.Generic;
using UnityEngine;

public class MeasureFloorPuzzle : MonoBehaviour
{
    [SerializeField] private Transform[] corners;        // in measuring order: A, B, C
    [SerializeField] private Transform hera;             // the Player; the tape's loose end follows her
    [SerializeField] private LineRenderer tape;          // draws the measuring tape
    [SerializeField] private AK.Wwise.Event tapeOutSound;     // first corner: tape pulled out
    [SerializeField] private AK.Wwise.Event tapePinSound;     // each later corner: tape pinned
    [SerializeField] private AK.Wwise.Event tapeAwaySound;    // after the solved lines: tape reeled in
    [SerializeField] private AK.Wwise.Event shardSound;       // the shard found on the floor after measuring
    [SerializeField] private DialogueBox dialogueBox;
    [SerializeField, TextArea(2, 5)] private string[] tapeOutLines;   // after the first corner
    [SerializeField, TextArea(2, 5)] private string[] solvedLines;    // after the last corner
    [SerializeField, TextArea(2, 5)] private string[] wallLengthLines;   // one per wall, in order: "(The tape reads 36 feet.)" …

    private bool active;                                 // true once GameFlow starts this puzzle
    private int nextCorner;                              // which corner she has to press next
    private readonly List<Vector3> tapePoints = new List<Vector3>();   // corners measured so far
    private Action onSolved;

    private void Awake()
    {
        if (corners.Length < 2)
        {
            throw new InvalidOperationException($"MeasureFloorPuzzle on '{name}': add at least 2 corners, in measuring order.");
        }

        if (wallLengthLines.Length != corners.Length - 1)
        {
            throw new InvalidOperationException($"MeasureFloorPuzzle on '{name}': Wall Length Lines needs one line per wall ({corners.Length - 1}).");
        }

        // Every corner needs its red outline.
        for (int i = 0; i < corners.Length; i++)
        {
            if (corners[i].GetComponent<LineRenderer>() == null)
            {
                throw new InvalidOperationException($"MeasureFloorPuzzle: corner '{corners[i].name}' needs a Line Renderer for its red outline.");
            }
        }

        tape.positionCount = 0;      // tape hidden until the first corner
        ShowNextCornerOutline();     // not started yet, so this hides every outline
    }

    // Called by GameFlow. whenSolved runs after the solved lines.
    public void Begin(Action whenSolved)
    {
        onSolved = whenSolved;
        active = true;
        nextCorner = 0;
        ShowNextCornerOutline();   // shows A
    }

    // Wired to each corner's Interactable Object > On Interact, with that corner as the argument.
    public void UseCorner(Transform corner)
    {
        if (!active || corner != corners[nextCorner]) return;   // not started yet, or wrong corner

        tapePoints.Add(corner.position);
        nextCorner++;

        bool firstCorner = nextCorner == 1;
        bool lastCorner = nextCorner == corners.Length;

        // Pulling the tape out at the first corner, pinning it at the others.
        if (firstCorner)
        {
            tapeOutSound.Post(hera.gameObject);
        }
        else
        {
            tapePinSound.Post(hera.gameObject);
        }

        if (lastCorner)
        {
            active = false;
            dialogueBox.PlayWithShardSound(Prepend(CurrentWallLength(), solvedLines), shardSound, PutTapeAway);   // the width, the room size, then the shard on the floor …
        }
        else if (firstCorner)
        {
            dialogueBox.Play(tapeOutLines, null);
        }
        else
        {
            dialogueBox.Play(new[] { CurrentWallLength() }, null);   // a middle corner says its wall's length
        }

        ShowNextCornerOutline();   // moves to the next corner, or hides all once solved
    }

    // The length line for the wall that was just finished. Corner B finishes wall 0, corner C wall 1.
    private string CurrentWallLength()
    {
        return wallLengthLines[nextCorner - 2];
    }

    // One line followed by the rest.
    private string[] Prepend(string firstLine, string[] rest)
    {
        string[] lines = new string[rest.Length + 1];
        lines[0] = firstLine;
        rest.CopyTo(lines, 1);
        return lines;
    }

    // Runs after the solved lines. Hides the tape, then tells GameFlow the puzzle is done.
    private void PutTapeAway()
    {
        tapeAwaySound.Post(hera.gameObject);
        tapePoints.Clear();
        tape.positionCount = 0;
        onSolved.Invoke();
    }

    // Shows the red outline only on the corner she has to press next.
    private void ShowNextCornerOutline()
    {
        for (int i = 0; i < corners.Length; i++)
        {
            LineRenderer outline = corners[i].GetComponent<LineRenderer>();
            bool isNextCorner = active && i == nextCorner;
            outline.enabled = isNextCorner;
        }
    }

    private void Update()
    {
        if (tapePoints.Count == 0) return;   // tape not out yet

        // While measuring, one extra point: the loose end in Hera's hand.
        if (active)
        {
            tape.positionCount = tapePoints.Count + 1;
        }
        else
        {
            tape.positionCount = tapePoints.Count;
        }

        for (int i = 0; i < tapePoints.Count; i++)
        {
            tape.SetPosition(i, tapePoints[i]);
        }

        if (active)
        {
            tape.SetPosition(tapePoints.Count, hera.position);
        }
    }
}
