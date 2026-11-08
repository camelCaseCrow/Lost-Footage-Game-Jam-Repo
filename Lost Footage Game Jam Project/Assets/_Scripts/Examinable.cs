using UnityEngine;

public class Examinable : MonoBehaviour
{
    [SerializeField] private DialogueBox dialogueBox;
    [SerializeField, TextArea(2, 5)] private string[] lines;       // what she sees; the last line is the question if endsWithQuestion
    [SerializeField] private bool endsWithQuestion;               // ticked: Yes/No on the last line
    [SerializeField, TextArea(2, 5)] private string[] yesLines;    // after Yes

    // Wired to this spot's Interactable Object > On Interact.
    public void Examine()
    {
        if (endsWithQuestion)
        {
            dialogueBox.PlayThenAsk(lines, PlayYesLines, null);
        }
        else
        {
            dialogueBox.Play(lines, null);
        }
    }

    private void PlayYesLines()
    {
        dialogueBox.Play(yesLines, null);
    }
}
