using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Editor-only tool (it's in an Editor folder, so it's left out of builds).
// Builds examine spots on top of the statues, pews and organ under "Structure Objects".
public static class ExamineSpotsTool
{
    private const float Margin = 1f;   // how much bigger than the object each spot is

    [MenuItem("Tools/Lost Footage/Create Measuring Spots")]
    private static void CreateMeasuringSpots()
    {
        GameObject set = FindOrCreateEmptySet("Measuring Spots", "measuringSpots");
        string[] none = new string[0];

        foreach (Transform structure in StructureObjects())
        {
            string kind = KindOf(structure);

            if (kind == "statue")
            {
                AddExamineSpot(set, structure,
                    new[] { "(A massive angel statue. It only has one eye.)",
                            "(…No matter the angle, it seems to be looking right at you.)" },
                    false, none);
            }
            else if (IsMarkedPew(structure))
            {
                AddExamineSpot(set, structure, new[] { "(A rotting pew. The seat is marked with a purple eye.)" }, false, none);
            }
            else if (kind == "pewpew")
            {
                AddExamineSpot(set, structure, new[] { "(A rotting pew.)" }, false, none);
            }
            else if (kind == "organ")
            {
                AddExamineSpot(set, structure,
                    new[] { "(A massive pipe organ, mostly intact. There are 8 stops, all marked with a purple eye. Yellowed sheet music sits on its stand.)",
                            "(Play it?)" },
                    true,
                    new[] { "(…No sound comes out.)", "Probably shouldn't fuck with that." });
            }
            // Banners and pillars have no examine text, so they get no spot.
        }
    }

    [MenuItem("Tools/Lost Footage/Create Find Sound Spots")]
    private static void CreateFindSoundSpots()
    {
        GameObject set = FindOrCreateEmptySet("Find Sound Spots", "findSoundSpots");
        FindTheSoundPuzzle puzzle = FindInScene<FindTheSoundPuzzle>();
        Transform leftStatue = LeftMostStatue();
        string[] notHere = { "(…No, it's not coming from here.)" };

        foreach (Transform structure in StructureObjects())
        {
            string kind = KindOf(structure);

            if (structure == leftStatue)
            {
                AddPuzzleSpot(set, structure, "Left Statue Spot", puzzle.ExamineLeftStatue);   // listen closer
            }
            else if (kind == "statue")
            {
                AddExamineSpot(set, structure,
                    new[] { "(One of two massive angel statues. It only has one eye.)", "(Listen closer?)" },
                    true, notHere);
            }
            else if (IsMarkedPew(structure))
            {
                AddExamineSpot(set, structure,
                    new[] { "(A rotting pew. The seat is marked with a purple eye.)", "(Listen closer?)" },
                    true, notHere);
            }
            else if (kind == "pewpew")
            {
                AddExamineSpot(set, structure, new[] { "(A rotting pew.)", "(Listen closer?)" }, true, notHere);
            }
            else if (kind == "organ")
            {
                AddExamineSpot(set, structure,
                    new[] { "(A massive pipe organ, mostly intact.)", "(Listen closer?)" },
                    true, notHere);
            }
        }
    }

    [MenuItem("Tools/Lost Footage/Create Take Eye Spots")]
    private static void CreateTakeEyeSpots()
    {
        GameObject set = FindOrCreateEmptySet("Take Eye Spots", "takeEyeSpots");
        EyePuzzle puzzle = FindInScene<EyePuzzle>();
        Transform leftStatue = LeftMostStatue();
        string[] none = new string[0];

        foreach (Transform structure in StructureObjects())
        {
            string kind = KindOf(structure);

            if (structure == leftStatue)
            {
                AddExamineSpot(set, structure,
                    new[] { "('Open my eyes, so that I may see'…)", "But how can you, with only one?" },
                    false, none);
            }
            else if (kind == "statue")
            {
                AddPuzzleSpot(set, structure, "Right Statue Spot", puzzle.ExamineRightStatue);   // take its eye
            }
            else if (IsMarkedPew(structure))
            {
                AddExamineSpot(set, structure,
                    new[] { "(A rotting pew. The seat is marked with a purple eye.)",
                            "I don't think a drawing of an eye is gonna help either of us." },
                    false, none);
            }
            else if (kind == "pewpew")
            {
                AddExamineSpot(set, structure, new[] { "(A rotting pew.)", "No eyes here." }, false, none);
            }
            else if (kind == "organ")
            {
                AddExamineSpot(set, structure,
                    new[] { "(A massive pipe organ, mostly intact. There are 8 stops, all marked with a purple eye. Yellowed sheet music sits on its stand.)",
                            "(Play it?)" },
                    true,
                    new[] { "(…No sound comes out.)", "No eyes here." });
            }
        }
    }

    [MenuItem("Tools/Lost Footage/Create Give Eye Spots")]
    private static void CreateGiveEyeSpots()
    {
        GameObject set = FindOrCreateEmptySet("Give Eye Spots", "giveEyeSpots");
        EyePuzzle puzzle = FindInScene<EyePuzzle>();
        Transform leftStatue = LeftMostStatue();
        string[] none = new string[0];

        foreach (Transform structure in StructureObjects())
        {
            string kind = KindOf(structure);

            if (structure == leftStatue)
            {
                AddPuzzleSpot(set, structure, "Left Statue Spot", puzzle.ExamineLeftStatue);   // give it the eye
            }
            else if (kind == "statue")
            {
                AddExamineSpot(set, structure,
                    new[] { "(The Blinded Angel.)", "(Its eyeless face sees right through you.)", "(Accusing.)" },
                    false, none);
            }
            else if (IsMarkedPew(structure))
            {
                AddExamineSpot(set, structure, new[] { "(A rotting pew. The seat is marked with a purple eye.)" }, false, none);
            }
            else if (kind == "pewpew")
            {
                AddExamineSpot(set, structure, new[] { "(A rotting pew.)" }, false, none);
            }
            else if (kind == "organ")
            {
                AddExamineSpot(set, structure, new[] { "(A massive pipe organ, mostly intact.)" }, false, none);
            }
        }
    }

    [MenuItem("Tools/Lost Footage/Fill Story Lines")]
    private static void FillStoryLines()
    {
        FillLockedDoor();
        FillWhisperTimer();
        FillFindTheSoundPuzzle();
        FillEyePuzzle();
        FillOrganPuzzle();
    }

    [MenuItem("Tools/Lost Footage/Set Up Organ Puzzle")]
    private static void SetUpOrganPuzzle()
    {
        if (Object.FindFirstObjectByType<OrganPuzzle>(FindObjectsInactive.Include) != null)
        {
            throw new System.InvalidOperationException("The organ puzzle is already set up. Ctrl + Z, or delete PuzzleFourOrgan, Stops Screen and Organ Spots' children, to rebuild.");
        }

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();

        GameObject set = FindOrCreateEmptySet("Organ Spots", "organSpots");
        GameObject stopsScreen = CreateStopsScreen();
        OrganPuzzle puzzle = CreateOrganPuzzle(stopsScreen);
        WireOrganControls(puzzle);
        BuildOrganSpots(set, puzzle);

        Undo.CollapseUndoOperations(undoGroup);   // one Ctrl + Z undoes the whole setup
    }

    // A black panel with the stops grid text, on the same canvas as the dialogue box.
    private static GameObject CreateStopsScreen()
    {
        DialogueBox dialogueBox = FindInScene<DialogueBox>();   // it sits on Text Canvas

        GameObject screen = new GameObject("Stops Screen", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        Undo.RegisterCreatedObjectUndo(screen, "Create Stops Screen");
        screen.transform.SetParent(dialogueBox.transform, false);
        screen.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 500);   // centred by default
        screen.GetComponent<Image>().color = Color.black;

        GameObject textObject = new GameObject("Stops Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        Undo.RegisterCreatedObjectUndo(textObject, "Create Stops Text");
        textObject.transform.SetParent(screen.transform, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;            // stretch to fill the panel…
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(30, 30);     // …with a 30 margin
        textRect.offsetMax = new Vector2(-30, -30);

        // Same font as the dialogue box's text.
        SerializedObject dialogueSlots = new SerializedObject(dialogueBox);
        TMP_Text lineText = (TMP_Text)dialogueSlots.FindProperty("lineText").objectReferenceValue;

        TextMeshProUGUI stopsText = textObject.GetComponent<TextMeshProUGUI>();
        stopsText.font = lineText.font;
        stopsText.fontSize = 40;
        stopsText.color = Color.white;
        stopsText.alignment = TextAlignmentOptions.Center;

        return screen;
    }

    private static OrganPuzzle CreateOrganPuzzle(GameObject stopsScreen)
    {
        GameObject puzzles = GameObject.Find("Puzzles");
        if (puzzles == null)
        {
            throw new System.InvalidOperationException("No 'Puzzles' group in the open scene.");
        }

        GameObject puzzleObject = new GameObject("PuzzleFourOrgan");
        Undo.RegisterCreatedObjectUndo(puzzleObject, "Create PuzzleFourOrgan");
        puzzleObject.transform.SetParent(puzzles.transform, false);
        OrganPuzzle puzzle = puzzleObject.AddComponent<OrganPuzzle>();

        SerializedObject puzzleSlots = new SerializedObject(puzzle);
        puzzleSlots.FindProperty("stopsScreen").objectReferenceValue = stopsScreen;
        puzzleSlots.FindProperty("stopsText").objectReferenceValue = stopsScreen.transform.Find("Stops Text").GetComponent<TMP_Text>();
        puzzleSlots.ApplyModifiedProperties();

        FillOrganPuzzle();   // dialogue box, player input, organ and the lines

        SerializedObject gameFlowSlots = new SerializedObject(FindInScene<GameFlow>());
        gameFlowSlots.FindProperty("organPuzzle").objectReferenceValue = puzzle;
        gameFlowSlots.ApplyModifiedProperties();

        return puzzle;
    }

    // Same as clicking + under Player Input > Events > UI and picking the OrganPuzzle method.
    private static void WireOrganControls(OrganPuzzle puzzle)
    {
        PlayerInput playerInput = FindInScene<PlayerInput>();
        Undo.RecordObject(playerInput, "Wire organ controls");

        WireUIAction(playerInput, "Navigate", puzzle.Navigate);
        WireUIAction(playerInput, "Submit", puzzle.PullOrPush);
        WireUIAction(playerInput, "Cancel", puzzle.StepAway);

        EditorUtility.SetDirty(playerInput);
    }

    private static void WireUIAction(PlayerInput playerInput, string actionName, UnityAction<InputAction.CallbackContext> method)
    {
        InputAction action = playerInput.actions.FindAction("UI/" + actionName, true);

        foreach (PlayerInput.ActionEvent actionEvent in playerInput.actionEvents)
        {
            if (actionEvent.actionId == action.id.ToString())
            {
                UnityEventTools.AddPersistentListener(actionEvent, method);
                return;
            }
        }

        throw new System.InvalidOperationException($"Player Input has no event for UI/{actionName}.");
    }

    private static void BuildOrganSpots(GameObject set, OrganPuzzle puzzle)
    {
        Transform leftStatue = LeftMostStatue();
        string[] none = new string[0];

        foreach (Transform structure in StructureObjects())
        {
            string kind = KindOf(structure);

            if (structure == leftStatue)
            {
                AddExamineSpot(set, structure, new[] { "(The Sighted Angel.)", "('Praise He who is watching'.)" }, false, none);
            }
            else if (kind == "statue")
            {
                AddExamineSpot(set, structure, new[] { "(The Blinded Angel.)" }, false, none);
            }
            else if (IsMarkedPew(structure))
            {
                AddExamineSpot(set, structure,
                    new[] { "(A rotting pew. The seat is marked with a purple eye.)", PewPositionLine(structure) },
                    false, none);
            }
            else if (kind == "pewpew")
            {
                AddExamineSpot(set, structure, new[] { "(A rotting pew.)" }, false, none);
            }
            else if (kind == "organ")
            {
                AddPuzzleSpot(set, structure, "Organ Spot", puzzle.ExamineOrgan);
            }
        }
    }

    private static void FillOrganPuzzle()
    {
        SerializedObject puzzle = new SerializedObject(FindInScene<OrganPuzzle>());

        puzzle.FindProperty("dialogueBox").objectReferenceValue = FindInScene<DialogueBox>();
        puzzle.FindProperty("playerInput").objectReferenceValue = FindInScene<PlayerInput>();
        puzzle.FindProperty("organ").objectReferenceValue = Organ().gameObject;

        SetLines(puzzle.FindProperty("firstLines"), new[] {
            "(A massive pipe organ, mostly intact. There are 8 stops, in two columns of 4, all marked with a purple eye.)",
            "Please don't tell me I need to make the organ work…",
            "I've never touched an organ in my life!",
            "(You look back at the Sighted Angel. It offers no further guidance.)",
            "(If the keys don't work, the only thing you can mess with is the stops…)" });

        SetLines(puzzle.FindProperty("laterLines"), new[] {
            "(A massive pipe organ, mostly intact. There are 8 stops, in two columns of 4, all marked with a purple eye.)",
            "(Mess with the stops?)" });

        SetLines(puzzle.FindProperty("foundPieceLines"), new[] {
            "(Something shimmers by the organ pedal.)",
            "(You hadn't noticed it before, but a piece of coloured glass sits at the base of the instrument.)",
            "(You pick it up.)" });

        // Correct Stops: first 4 = left column, last 4 = right column, top to bottom (OrganPuzzle's order).
        SerializedProperty correctStops = puzzle.FindProperty("correctStops");
        correctStops.arraySize = 8;
        for (int row = 0; row < 4; row++)
        {
            correctStops.GetArrayElementAtIndex(row).boolValue = MarkedLeftColumn[row];
            correctStops.GetArrayElementAtIndex(4 + row).boolValue = MarkedRightColumn[row];
        }

        puzzle.ApplyModifiedProperties();
    }

    // The organ answer. The pews sit like the stops: 2 columns × 4 rows, row 1 nearest the organ.
    // A marked pew = a stop that must be pulled out.
    private static readonly bool[] MarkedLeftColumn  = { false, true,  true,  false };
    private static readonly bool[] MarkedRightColumn = { true,  false, false, true  };

    private static bool IsMarkedPew(Transform structure)
    {
        if (KindOf(structure) != "pewpew") return false;

        int row = PewRow(structure);
        if (PewIsOnLeft(structure))
        {
            return MarkedLeftColumn[row];
        }
        return MarkedRightColumn[row];
    }

    // True if the pew is left of the middle of all the pews.
    private static bool PewIsOnLeft(Transform pew)
    {
        float totalX = 0f;
        int pewCount = 0;

        foreach (Transform structure in StructureObjects())
        {
            if (KindOf(structure) == "pewpew")
            {
                totalX += structure.position.x;
                pewCount++;
            }
        }

        float middleX = totalX / pewCount;
        return pew.position.x < middleX;
    }

    // 0 = the row nearest the organ, 3 = the back row.
    private static int PewRow(Transform pew)
    {
        bool onLeft = PewIsOnLeft(pew);
        int row = 0;

        foreach (Transform structure in StructureObjects())
        {
            bool isPewInSameColumn = KindOf(structure) == "pewpew" && PewIsOnLeft(structure) == onLeft;
            if (isPewInSameColumn && structure.position.y > pew.position.y)
            {
                row++;   // another pew in this column is nearer the organ
            }
        }

        if (row > 3)
        {
            throw new System.InvalidOperationException($"Pew '{pew.name}' is in row {row + 1}; the organ puzzle expects 4 rows of pews.");
        }
        return row;
    }

    // e.g. "(Third row from the organ, left side.)"
    private static string PewPositionLine(Transform pew)
    {
        string[] rowNames = { "First", "Second", "Third", "Fourth" };

        string side = "right";
        if (PewIsOnLeft(pew))
        {
            side = "left";
        }

        return $"({rowNames[PewRow(pew)]} row from the organ, {side} side.)";
    }

    [MenuItem("Tools/Lost Footage/Create Organ Spots")]
    private static void CreateOrganSpots()
    {
        GameObject set = FindOrCreateEmptySet("Organ Spots", "organSpots");
        BuildOrganSpots(set, FindInScene<OrganPuzzle>());
    }

    [MenuItem("Tools/Lost Footage/Rebuild All Spots")]
    private static void RebuildAllSpots()
    {
        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();

        EmptySet("Measuring Spots");
        EmptySet("Find Sound Spots");
        EmptySet("Take Eye Spots");
        EmptySet("Give Eye Spots");
        EmptySet("Organ Spots");

        CreateMeasuringSpots();
        CreateFindSoundSpots();
        CreateTakeEyeSpots();
        CreateGiveEyeSpots();
        CreateOrganSpots();

        Undo.CollapseUndoOperations(undoGroup);   // one Ctrl + Z restores the old spots
    }

    // Deletes every spot inside the set except its "Hand-made" group, which survives rebuilds.
    // The set itself stays, so GameFlow keeps its slot.
    private static void EmptySet(string setName)
    {
        GameObject set = GameObject.Find(setName);
        if (set == null)
        {
            throw new System.InvalidOperationException($"No '{setName}' in the open scene.");
        }

        // Backwards, because deleting a child shifts the ones after it.
        for (int i = set.transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = set.transform.GetChild(i).gameObject;
            if (child.name == "Hand-made") continue;
            Undo.DestroyObjectImmediate(child);
        }
    }

    // True if the set holds anything besides its "Hand-made" group.
    private static bool HasToolSpots(GameObject set)
    {
        foreach (Transform child in set.transform)
        {
            if (child.name != "Hand-made") return true;
        }
        return false;
    }

    private static Transform Organ()
    {
        foreach (Transform structure in StructureObjects())
        {
            if (KindOf(structure) == "organ")
            {
                return structure;
            }
        }

        throw new System.InvalidOperationException("No organ under Structure Objects.");
    }

    private static void FillEyePuzzle()
    {
        SerializedObject puzzle = new SerializedObject(FindInScene<EyePuzzle>());

        puzzle.FindProperty("dialogueBox").objectReferenceValue = FindInScene<DialogueBox>();
        puzzle.FindProperty("leftStatue").objectReferenceValue = LeftMostStatue().gameObject;
        puzzle.FindProperty("rightStatue").objectReferenceValue = RightStatue().gameObject;

        SetLines(puzzle.FindProperty("takeEyeLines"), new[] {
            "(One of two massive angel statues. It only has one eye.)",
            "(It's silent.)",
            "Well, if this one doesn't care about seeing…",
            "…How pissed off would God be if I mess with his statues?",
            "(The measuring tape gleams from its usual spot on your belt.)",
            "(Reach for its eye?)" });

        SetLines(puzzle.FindProperty("tookEyeLines"), new[] {
            "(You extend your measuring tape and reach with it toward the silent angel's eye.)",
            "(You scrape.)",
            "(The stone eye pops out and rolls to your foot.)",
            "(You pick up the Eye of the Silent Angel.)",
            "(You shiver.)" });

        SetLines(puzzle.FindProperty("leaveEyeLines"), new[] {
            "(I'd probably have to pay for damages.)" });

        SetLines(puzzle.FindProperty("giveEyeLines"), new[] {
            "(The Sighted Angel.)",
            "(You roll the stolen eye in your palm.)",
            "(The angel's hands are beseeching.)",
            "(Relinquish the eye?)" });

        SetLines(puzzle.FindProperty("gaveEyeLines"), new[] {
            "(You place the eye into its hand.)",
            "(…)",
            "Obviously nothing would happen.",
            "It's just a statue.",
            "What the hell am I even-",
            "(Something shimmers at its feet.)",
            "(You hadn't noticed it before, but a piece of coloured glass sits at the base of the statue.)",
            "(You pick it up.)",
            "(The door! There were four slots for pieces like this, right?)",
            "(Is that the key out of here?)",
            "(You thank the angel, looking up at its face…)",
            "(It's looking towards the organ.)",
            "(It wasn't doing that before.)",
            "(You KNOW it wasn't looking that way before.)" });

        puzzle.ApplyModifiedProperties();
    }

    private static void FillLockedDoor()
    {
        SerializedObject door = new SerializedObject(FindInScene<LockedDoor>());

        SetLines(door.FindProperty("notYetLines"), new[] {
            "(Can't leave yet.)",
            "(You've got a stupid job to do.)" });

        SetLines(door.FindProperty("lockedLines"), new[] {
            "(You didn't notice before, but the main door has an oddly shaped hollow in its centre, separated into four slots. Strange.)",
            "(Not that you need to care. You reach for the handle-)",
            "?",
            "(The door doesn't open.)",
            "What the-",
            "(The door still doesn't open.)",
            "Fuck- I swear, is that you, Jo? I thought you said this lock doesn't work anymore! HEY!",
            "(The door   doesn't   open.)",
            "Shit.",
            "(Is there another way out of here?)" });

        SetLines(door.FindProperty("stillLockedLines"), new[] {
            "(It still won't FUCKING open.)",
            "Is it just jammed? Jo's an idiot, but she wouldn't lock me in here…",
            "She's too scared of me. I'd fuck her up.",
            "(The thought makes you smile.)",
            "(The door has an oddly shaped hollow in its centre, separated into four slots.)",
            "(Engraved around it are the words, 'PRAISE HE WHO IS WATCHING'.)" });

        SetLines(door.FindProperty("stillLockedAgainLines"), new[] {
            "(And nope, it's still not opening.)" });

        door.ApplyModifiedProperties();
    }

    private static void FillWhisperTimer()
    {
        SerializedObject timer = new SerializedObject(FindInScene<WhisperTimer>());

        timer.FindProperty("leftStatue").objectReferenceValue = LeftMostStatue().gameObject;
        timer.FindProperty("dialogueBox").objectReferenceValue = FindInScene<DialogueBox>();
        SetLines(timer.FindProperty("firstWhisperLines"), new[] { "…!", "(What was that noise?)" });

        timer.ApplyModifiedProperties();
    }

    private static void FillFindTheSoundPuzzle()
    {
        SerializedObject puzzle = new SerializedObject(FindInScene<FindTheSoundPuzzle>());

        puzzle.FindProperty("dialogueBox").objectReferenceValue = FindInScene<DialogueBox>();
        puzzle.FindProperty("playerInput").objectReferenceValue = FindInScene<PlayerInput>();
        puzzle.FindProperty("leftStatue").objectReferenceValue = LeftMostStatue().gameObject;

        SetLines(puzzle.FindProperty("leftStatueLines"), new[] {
            "(One of two massive angel statues. It only has one eye.)",
            "(Listen closer?)" });

        SetLines(puzzle.FindProperty("comingFromHereLines"), new[] {
            "(…It's coming from here, somehow!)" });

        SetLines(puzzle.FindProperty("afterSilenceLines"), new[] {
            "('Open my eyes, so that I may see'…?)",
            "(You look up at its face. It looks back.)",
            "I can't tell if it's me or you they're playing a sick joke on, angel.",
            "You can't open your 'eyes' if you only have one!",
            "(You try to laugh. It gets swallowed by the silence.)",
            "(…)",
            "(It is cold, and you are alone.)" });

        puzzle.ApplyModifiedProperties();
    }

    // A trigger spot on top of the structure, a bit bigger than its collider.
    private static GameObject CreateSpot(GameObject set, Transform structure, string spotName)
    {
        BoxCollider2D structureBox = structure.GetComponent<BoxCollider2D>();
        if (structureBox == null)
        {
            throw new System.InvalidOperationException($"'{structure.name}' has no Box Collider 2D to size its spot from.");
        }

        GameObject spot = new GameObject(spotName);
        Undo.RegisterCreatedObjectUndo(spot, "Create " + spotName);
        spot.transform.SetParent(set.transform, false);
        spot.transform.position = structure.TransformPoint(structureBox.offset);

        BoxCollider2D trigger = spot.AddComponent<BoxCollider2D>();
        trigger.isTrigger = true;
        Vector2 structureScale = new Vector2(Mathf.Abs(structure.lossyScale.x), Mathf.Abs(structure.lossyScale.y));   // mirrored objects have a negative scale
        Vector2 structureSize = Vector2.Scale(structureBox.size, structureScale);
        trigger.size = structureSize + new Vector2(Margin, Margin);

        return spot;
    }

    // A spot with Examinable text, wired so E calls Examine.
    private static void AddExamineSpot(GameObject set, Transform structure, string[] lines, bool endsWithQuestion, string[] yesLines)
    {
        GameObject spot = CreateSpot(set, structure, structure.name + " Spot");

        Examinable examinable = spot.AddComponent<Examinable>();
        SerializedObject examinableSlots = new SerializedObject(examinable);
        examinableSlots.FindProperty("dialogueBox").objectReferenceValue = FindInScene<DialogueBox>();
        SetLines(examinableSlots.FindProperty("lines"), lines);
        examinableSlots.FindProperty("endsWithQuestion").boolValue = endsWithQuestion;
        SetLines(examinableSlots.FindProperty("yesLines"), yesLines);
        examinableSlots.ApplyModifiedPropertiesWithoutUndo();

        InteractableObject interactable = spot.AddComponent<InteractableObject>();
        WireOnInteract(interactable, examinable.Examine);
    }

    // A spot with no text of its own: E calls a puzzle method instead.
    private static void AddPuzzleSpot(GameObject set, Transform structure, string spotName, UnityAction method)
    {
        GameObject spot = CreateSpot(set, structure, spotName);
        InteractableObject interactable = spot.AddComponent<InteractableObject>();
        WireOnInteract(interactable, method);
    }

    // The statue that isn't the left-most one.
    private static Transform RightStatue()
    {
        Transform leftStatue = LeftMostStatue();

        foreach (Transform structure in StructureObjects())
        {
            if (KindOf(structure) == "statue" && structure != leftStatue)
            {
                return structure;
            }
        }

        throw new System.InvalidOperationException("Only one statue under Structure Objects.");
    }

    // Uses the set if it exists and is empty, creates it if missing, and fills its GameFlow slot.
    private static GameObject FindOrCreateEmptySet(string setName, string gameFlowSlot)
    {
        GameObject set = GameObject.Find(setName);

        if (set == null)
        {
            set = new GameObject(setName);
            Undo.RegisterCreatedObjectUndo(set, "Create " + setName);
        }
        else if (HasToolSpots(set))
        {
            throw new System.InvalidOperationException($"'{setName}' already has spots in it. Use Rebuild All Spots, or delete them first.");
        }

        SerializedObject gameFlowSlots = new SerializedObject(FindInScene<GameFlow>());
        gameFlowSlots.FindProperty(gameFlowSlot).objectReferenceValue = set;
        gameFlowSlots.ApplyModifiedProperties();

        return set;
    }

    private static Transform StructureObjects()
    {
        GameObject structures = GameObject.Find("Structure Objects");
        if (structures == null)
        {
            throw new System.InvalidOperationException("No 'Structure Objects' in the open scene.");
        }
        return structures.transform;
    }

    // The name of the prefab this object was painted from: "statue", "pewpew", "organ", …
    private static string KindOf(Transform structure)
    {
        GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(structure.gameObject);
        if (prefab == null)
        {
            throw new System.InvalidOperationException($"'{structure.name}' under Structure Objects isn't a prefab.");
        }
        return prefab.name;
    }

    private static Transform LeftMostStatue()
    {
        Transform leftMost = null;

        foreach (Transform structure in StructureObjects())
        {
            if (KindOf(structure) != "statue") continue;

            if (leftMost == null || structure.position.x < leftMost.position.x)
            {
                leftMost = structure;
            }
        }

        if (leftMost == null)
        {
            throw new System.InvalidOperationException("No statue under Structure Objects.");
        }
        return leftMost;
    }

    // Finds the one object of type T in the open scene, or stops with a clear message.
    private static T FindInScene<T>() where T : Object
    {
        T found = Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
        if (found == null)
        {
            throw new System.InvalidOperationException($"No {typeof(T).Name} in the open scene.");
        }
        return found;
    }

    // Copies an array of lines into a list slot.
    private static void SetLines(SerializedProperty slot, string[] lines)
    {
        slot.arraySize = lines.Length;
        for (int i = 0; i < lines.Length; i++)
        {
            slot.GetArrayElementAtIndex(i).stringValue = lines[i];
        }
    }

    // Adds "method" to the spot's On Interact list: the same as clicking + and picking it in the Inspector.
    private static void WireOnInteract(InteractableObject interactable, UnityAction method)
    {
        // onInteract is private, so the tool looks it up by name.
        FieldInfo field = typeof(InteractableObject).GetField("onInteract", BindingFlags.NonPublic | BindingFlags.Instance);
        UnityEvent onInteract = (UnityEvent)field.GetValue(interactable);
        UnityEventTools.AddPersistentListener(onInteract, method);
        EditorUtility.SetDirty(interactable);
    }
}
