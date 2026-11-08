using System;
using UnityEngine;

public class EyePuzzle : MonoBehaviour
{
    [SerializeField] private DialogueBox dialogueBox;
    [SerializeField] private GameObject leftStatue;
    [SerializeField] private GameObject rightStatue;
    [SerializeField] private AK.Wwise.Event eyeTakenSound;   // plays on the right statue when she takes its eye
    [SerializeField] private AK.Wwise.Event eyeGivenSound;   // plays on the left statue when she gives it the eye
    [SerializeField] private AK.Wwise.Event shardSound;      // shard #2 picked up (at the left statue)
    [SerializeField, TextArea(2, 5)] private string[] takeEyeLines;    // right statue, ends with "(Reach for its eye?)"
    [SerializeField, TextArea(2, 5)] private string[] tookEyeLines;    // after Yes: "(You extend your measuring tape…" …
    [SerializeField, TextArea(2, 5)] private string[] leaveEyeLines;   // after No: "(I'd probably have to pay for damages.)"
    [SerializeField, TextArea(2, 5)] private string[] giveEyeLines;    // left statue, ends with "(Relinquish the eye?)"
    [SerializeField, TextArea(2, 5)] private string[] gaveEyeLines;    // after Yes: "(You place the eye into its hand.)" …

    private Action onEyeTaken;
    private Action onEyeGiven;

    // Called by GameFlow at the start of state 03. whenTaken runs after the took-eye lines.
    public void BeginTaking(Action whenTaken)
    {
        onEyeTaken = whenTaken;
    }

    // Called by GameFlow at the start of state 04. whenGiven runs after the gave-eye lines.
    public void BeginGiving(Action whenGiven)
    {
        onEyeGiven = whenGiven;
    }

    // State 03: wired to the right statue spot in Take Eye Spots.
    public void ExamineRightStatue()
    {
        dialogueBox.PlayThenAsk(takeEyeLines, TakeEye, LeaveEye);
    }

    private void TakeEye()
    {
        eyeTakenSound.Post(rightStatue);
        dialogueBox.Play(tookEyeLines, onEyeTaken);
    }

    private void LeaveEye()
    {
        dialogueBox.Play(leaveEyeLines, null);
    }

    // State 04: wired to the left statue spot in Give Eye Spots.
    public void ExamineLeftStatue()
    {
        dialogueBox.PlayThenAsk(giveEyeLines, GiveEye, null);
    }

    private void GiveEye()
    {
        eyeGivenSound.Post(leftStatue);
        dialogueBox.PlayWithShardSound(gaveEyeLines, shardSound, onEyeGiven);
    }
}
