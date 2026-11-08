using UnityEngine;

public class GameFlow : MonoBehaviour
{
    [SerializeField] private DoorCutscene doorCutscene;
    [SerializeField] private MeasureFloorPuzzle measureFloorPuzzle;
    [SerializeField] private LockedDoor lockedDoor;
    [SerializeField] private WhisperTimer whisperTimer;
    [SerializeField] private FindTheSoundPuzzle findTheSoundPuzzle;
    [SerializeField] private EyePuzzle eyePuzzle;
    [SerializeField] private OrganPuzzle organPuzzle;
    [SerializeField] private GameObject measuringSpots;    // examinable from the start until the door locks
    [SerializeField] private GameObject findSoundSpots;    // examinable while finding the sound
    [SerializeField] private GameObject takeEyeSpots;      // examinable in state 03
    [SerializeField] private GameObject giveEyeSpots;      // examinable in state 04
    [SerializeField] private GameObject organSpots;        // examinable in state 05

    private void Start()
    {
        measuringSpots.SetActive(true);
        findSoundSpots.SetActive(false);
        takeEyeSpots.SetActive(false);
        giveEyeSpots.SetActive(false);
        organSpots.SetActive(false);
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
        measuringSpots.SetActive(false);
        findSoundSpots.SetActive(true);
        whisperTimer.Begin();
        findTheSoundPuzzle.Begin(StartPuzzle3);
    }

    // State 03: take the right statue's eye.
    private void StartPuzzle3()
    {
        findSoundSpots.SetActive(false);
        takeEyeSpots.SetActive(true);
        eyePuzzle.BeginTaking(StartPuzzle4);
    }

    // State 04: give the eye to the left statue.
    private void StartPuzzle4()
    {
        takeEyeSpots.SetActive(false);
        giveEyeSpots.SetActive(true);
        eyePuzzle.BeginGiving(StartPuzzle5);
    }

    // State 05: the organ stops.
    private void StartPuzzle5()
    {
        giveEyeSpots.SetActive(false);
        organSpots.SetActive(true);
        organPuzzle.Begin(StartPuzzle6);
    }

    // Next state: waiting on the script.
    private void StartPuzzle6()
    {
        organSpots.SetActive(false);
        Debug.Log("Organ solved, glass piece #2 found. State 06 starts here.");
    }
}
