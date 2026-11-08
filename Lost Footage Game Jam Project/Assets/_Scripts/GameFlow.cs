using UnityEngine;

public class GameFlow : MonoBehaviour
{
    [SerializeField] private DialogueBox dialogueBox;
    [SerializeField, TextArea(2, 5)] private string[] introLines;   // the opening cutscene text

    private void Start()
    {
        // Show the intro, then run OnIntroFinished after the last line.
        dialogueBox.Play(introLines, OnIntroFinished);
    }

    private void OnIntroFinished()
    {
        // Step 4 replaces this line with "start puzzle 1".
        Debug.Log("Intro finished.");
    }
}
