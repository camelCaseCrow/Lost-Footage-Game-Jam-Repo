using UnityEngine;

public class GameFlow : MonoBehaviour
{
    [SerializeField] private DoorCutscene doorCutscene;
    [SerializeField] private MeasureFloorPuzzle measureFloorPuzzle;
    [SerializeField] private LockedDoor lockedDoor;
    [SerializeField] private WhisperTimer whisperTimer;

    private void Start()
    {
        doorCutscene.Play(StartPuzzle1);
    }

    private void StartPuzzle1()
    {
        measureFloorPuzzle.Begin(StartLockedDoor);
    }

    // After the floor is measured, she tries to leave.
    private void StartLockedDoor()
    {
        lockedDoor.Begin(StartPuzzle2);
    }

    private void StartPuzzle2()
    {
        whisperTimer.Begin();
    }
}
