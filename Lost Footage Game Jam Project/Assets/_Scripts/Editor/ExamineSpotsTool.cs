using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.Video;

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
        FillMeasureFloorPuzzle();
        FillKeypadPuzzle();
        FillEnding();
    }

    // Points Ending → Camera Overlay at the overlay canvas: the one canvas that isn't the dialogue box's.
    private static void FillEnding()
    {
        Canvas dialogueCanvas = FindInScene<DialogueBox>().GetComponent<Canvas>();
        Canvas overlayCanvas = null;

        foreach (Canvas canvas in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (canvas == dialogueCanvas || !canvas.isRootCanvas) continue;

            if (overlayCanvas != null)
            {
                throw new System.InvalidOperationException($"Two canvases besides the dialogue box's ('{overlayCanvas.name}' and '{canvas.name}'); expected only the camera overlay.");
            }
            overlayCanvas = canvas;
        }

        if (overlayCanvas == null)
        {
            throw new System.InvalidOperationException("No camera overlay canvas in the open scene.");
        }

        SerializedObject endingSlots = new SerializedObject(FindInScene<Ending>());
        endingSlots.FindProperty("cameraOverlay").objectReferenceValue = overlayCanvas.gameObject;
        endingSlots.ApplyModifiedProperties();
    }

    // The floor Hera measures in state 01. The tape lines, the solved line and the keypad answer all come from these.
    private const int RoomLength = 42;   // metres: corner A to B
    private const int RoomWidth = 31;    // metres: corner B to C  (42 × 31 = 1302, the keypad answer)

    private static void FillMeasureFloorPuzzle()
    {
        SerializedObject puzzle = new SerializedObject(FindInScene<MeasureFloorPuzzle>());

        SetLines(puzzle.FindProperty("wallLengthLines"), new[] {
            $"{RoomLength} metres… Alright, that's the floor length. Now for the width.",
            $"{RoomWidth} metres… Alright, that's the width." });

        SetLines(puzzle.FindProperty("solvedLines"), new[] {
            $"The room is {RoomLength} metres by {RoomWidth} metres.",
            "(Something glints on the floor near your foot.)",
            "(A piece of coloured glass. Weird.)",
            "(You pick it up.)",
            "That was painless. Mostly. Okay, I'm out of here.",
            "(You hear a distinct click.)",
            "..? " });

        puzzle.ApplyModifiedProperties();
    }

    [MenuItem("Tools/Lost Footage/Set Up Keypad Puzzle")]
    private static void SetUpKeypadPuzzle()
    {
        if (Object.FindFirstObjectByType<KeypadPuzzle>(FindObjectsInactive.Include) != null)
        {
            throw new System.InvalidOperationException("The keypad puzzle is already set up. Ctrl + Z, or delete PuzzleFiveKeypad, Keypad Screen and Keypad Spots' children, to rebuild.");
        }

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();

        GameObject keypadScreen = CreateTextScreen("Keypad Screen", "Keypad Text");
        KeypadPuzzle puzzle = CreateKeypadPuzzle(keypadScreen);
        WireKeypadControls(puzzle);
        CreateKeypadSpots();

        Undo.CollapseUndoOperations(undoGroup);   // one Ctrl + Z undoes the whole setup
    }

    private static KeypadPuzzle CreateKeypadPuzzle(GameObject keypadScreen)
    {
        GameObject puzzleObject = new GameObject("PuzzleFiveKeypad");
        Undo.RegisterCreatedObjectUndo(puzzleObject, "Create PuzzleFiveKeypad");
        puzzleObject.transform.SetParent(PuzzlesGroup(), false);
        KeypadPuzzle puzzle = puzzleObject.AddComponent<KeypadPuzzle>();

        SerializedObject puzzleSlots = new SerializedObject(puzzle);
        puzzleSlots.FindProperty("keypadScreen").objectReferenceValue = keypadScreen;
        puzzleSlots.FindProperty("keypadText").objectReferenceValue = keypadScreen.transform.Find("Keypad Text").GetComponent<TMP_Text>();
        puzzleSlots.ApplyModifiedProperties();

        FillKeypadPuzzle();   // dialogue box, player input, answer and the lines

        SerializedObject gameFlowSlots = new SerializedObject(FindInScene<GameFlow>());
        gameFlowSlots.FindProperty("keypadPuzzle").objectReferenceValue = puzzle;
        gameFlowSlots.ApplyModifiedProperties();

        return puzzle;
    }

    // Same as clicking + under Player Input > Events > UI and picking the KeypadPuzzle method.
    private static void WireKeypadControls(KeypadPuzzle puzzle)
    {
        PlayerInput playerInput = FindInScene<PlayerInput>();
        Undo.RecordObject(playerInput, "Wire keypad controls");

        WireUIAction(playerInput, "Submit", puzzle.Confirm);
        WireUIAction(playerInput, "Cancel", puzzle.StepAway);

        EditorUtility.SetDirty(playerInput);
    }

    [MenuItem("Tools/Lost Footage/Create Keypad Spots")]
    private static void CreateKeypadSpots()
    {
        GameObject set = FindOrCreateEmptySet("Keypad Spots", "keypadSpots");
        KeypadPuzzle puzzle = FindInScene<KeypadPuzzle>();
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
                AddExamineSpot(set, structure, new[] { "(A rotting pew. The seat is marked with a purple eye.)" }, false, none);
            }
            else if (kind == "pewpew")
            {
                AddExamineSpot(set, structure, new[] { "(A rotting pew.)" }, false, none);
            }
            else if (kind == "organ")
            {
                AddExamineSpot(set, structure,
                    new[] { "(A massive pipe organ, mostly intact.)", "(You've done what you needed to do here.)" },
                    false, none);
            }
        }

        CreateKeypadSpot(set, puzzle);
    }

    // Where the keypad spots go: on the keypad art that GameFlow shows after the organ.
    private static Vector3 KeypadPosition()
    {
        SerializedObject gameFlowSlots = new SerializedObject(FindInScene<GameFlow>());
        GameObject keypad = (GameObject)gameFlowSlots.FindProperty("keypad").objectReferenceValue;
        if (keypad == null)
        {
            throw new System.InvalidOperationException("GameFlow → Keypad is empty. Run Set Up Keypad Art first.");
        }
        return keypad.transform.position;
    }

    private static readonly Vector2 KeypadSpotSize = new Vector2(2f, 3f);   // tall, so standing on the floor below the wall reaches it

    [MenuItem("Tools/Lost Footage/Set Up Keypad Art")]
    // Swaps the keypad tile painted on "Tilemap Wall Items" for a Keypad object, which GameFlow hides until the organ is solved.
    private static void SetUpKeypadArt()
    {
        SerializedObject gameFlowSlots = new SerializedObject(FindInScene<GameFlow>());
        if (gameFlowSlots.FindProperty("keypad").objectReferenceValue != null)
        {
            throw new System.InvalidOperationException("The keypad art is already set up (GameFlow → Keypad is filled).");
        }

        TileBase keypadTile = AssetDatabase.LoadAssetAtPath<TileBase>("Assets/Palettes/keypad.asset");
        Sprite keypadSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/keypad.PNG");
        if (keypadTile == null || keypadSprite == null)
        {
            throw new System.InvalidOperationException("Expected the keypad tile at 'Assets/Palettes/keypad.asset' and its sprite at 'Assets/Sprites/keypad.PNG'.");
        }

        GameObject wallItems = GameObject.Find("Tilemap Wall Items");
        if (wallItems == null)
        {
            throw new System.InvalidOperationException("No 'Tilemap Wall Items' in the open scene.");
        }
        Tilemap tilemap = wallItems.GetComponent<Tilemap>();
        Vector3Int keypadCell = FindTileCell(tilemap, keypadTile);

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();

        GameObject keypad = new GameObject("Keypad");
        Undo.RegisterCreatedObjectUndo(keypad, "Create Keypad");
        keypad.transform.position = tilemap.GetCellCenterWorld(keypadCell);   // exactly where the tile was
        SpriteRenderer keypadRenderer = keypad.AddComponent<SpriteRenderer>();
        keypadRenderer.sprite = keypadSprite;
        keypadRenderer.sortingOrder = wallItems.GetComponent<TilemapRenderer>().sortingOrder;   // drawn the same as the tile was

        Undo.RecordObject(tilemap, "Remove keypad tile");
        tilemap.SetTile(keypadCell, null);

        gameFlowSlots.FindProperty("keypad").objectReferenceValue = keypad;
        gameFlowSlots.ApplyModifiedProperties();

        Undo.CollapseUndoOperations(undoGroup);   // one Ctrl + Z puts the tile back
    }

    private static Vector3Int FindTileCell(Tilemap tilemap, TileBase tile)
    {
        foreach (Vector3Int cell in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.GetTile(cell) == tile)
            {
                return cell;
            }
        }

        throw new System.InvalidOperationException($"No '{tile.name}' tile painted on '{tilemap.name}'.");
    }

    private static void CreateKeypadSpot(GameObject set, KeypadPuzzle puzzle)
    {
        GameObject spot = CreateSpotAt(set, "Keypad Spot", KeypadPosition(), KeypadSpotSize);

        InteractableObject interactable = spot.AddComponent<InteractableObject>();
        WireOnInteract(interactable, puzzle.ExamineKeypad);
        SetInteractSound(interactable, "keypad");

        // Keypad sounds play from this spot.
        SerializedObject puzzleSlots = new SerializedObject(puzzle);
        puzzleSlots.FindProperty("keypad").objectReferenceValue = spot;
        puzzleSlots.ApplyModifiedProperties();
    }

    private static void FillKeypadPuzzle()
    {
        SerializedObject puzzle = new SerializedObject(FindInScene<KeypadPuzzle>());

        puzzle.FindProperty("dialogueBox").objectReferenceValue = FindInScene<DialogueBox>();
        puzzle.FindProperty("playerInput").objectReferenceValue = FindInScene<PlayerInput>();
        puzzle.FindProperty("correctAnswer").intValue = RoomLength * RoomWidth;

        SetLines(puzzle.FindProperty("firstLines"), new[] {
            "(You hadn't noticed it before, but a small keypad hangs on the wall.)",
            "(Words are written above it in tiny, messy letters:)",
            "('ENTER SQUARE FOOTAGE')",
            "The square footage… of the floor? The thing I came here to measure?",
            "(Is this some sick joke?)",
            "(Is this just a note Jo wrote to herself before?)",
            "(Was this always here?)",
            "(Is something alive in here, moving things around?)",
            "(Or are you just that unobservant?)",
            "(Are you really that stupid?)",
            "(Couldn't this all have been avoided if you just propped the front door open?)",
            "(Why did the angel statue move?)",
            "(Shouldn't you have called someone?)",
            "(WHO IS WATCHING?)",
            "(…)",
            "(It doesn't matter.)",
            "(Just enter the square footage.)",
            "(You can do that much. You noted it down.)",
            "(Didn't you?)" });

        SetLines(puzzle.FindProperty("laterLines"), new[] {
            "('ENTER SQUARE FOOTAGE')" });

        SetLines(puzzle.FindProperty("wrongLines"), new[] {
            "(Nothing happens.)",
            "(Wasn't the room some number times some number?)" });

        // Placeholder until the script has these lines.
        SetLines(puzzle.FindProperty("correctLines"), new[] {
            "(Something clicks inside the wall.)",
            "(A small panel beneath the keypad swings open. Another piece of coloured glass sits inside.)",
            "(You pick it up.)",
            "(That's four pieces. Four slots in the door.)",
            "(Putting the shards together, you see the completed mosaic.)" });

        SetLines(puzzle.FindProperty("mosaicLines"), new[] {
            "...",
            "(YOU NEED TO GET OUT OF HERE.)" });

        puzzle.ApplyModifiedProperties();
    }

    [MenuItem("Tools/Lost Footage/Set Up Ending")]
    private static void SetUpEnding()
    {
        if (Object.FindFirstObjectByType<Ending>(FindObjectsInactive.Include) != null)
        {
            throw new System.InvalidOperationException("The ending is already set up. Ctrl + Z, or delete Ending, End Screen and Final Spots' children, to rebuild.");
        }

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();

        GameObject endScreen = CreateTextScreen("End Screen", "End Text");
        RectTransform screenRect = endScreen.GetComponent<RectTransform>();
        screenRect.anchorMin = Vector2.zero;   // fill the whole screen
        screenRect.anchorMax = Vector2.one;
        screenRect.offsetMin = Vector2.zero;
        screenRect.offsetMax = Vector2.zero;
        TextMeshProUGUI endText = endScreen.transform.Find("End Text").GetComponent<TextMeshProUGUI>();
        endText.enableAutoSizing = true;       // long credits shrink to fit

        Ending ending = CreateEnding(endScreen, endText);

        PlayerInput playerInput = FindInScene<PlayerInput>();
        Undo.RecordObject(playerInput, "Wire ending controls");
        WireUIAction(playerInput, "Submit", ending.BackToMenu);
        EditorUtility.SetDirty(playerInput);

        CreateFinalSpots();

        Undo.CollapseUndoOperations(undoGroup);   // one Ctrl + Z undoes the whole setup
    }

    private static Ending CreateEnding(GameObject endScreen, TMP_Text endText)
    {
        GameObject cutscenes = GameObject.Find("Cutscenes");
        if (cutscenes == null)
        {
            throw new System.InvalidOperationException("No 'Cutscenes' group in the open scene.");
        }

        VideoClip clip = AssetDatabase.LoadAssetAtPath<VideoClip>("Assets/Animations/animation at the end.mov");
        if (clip == null)
        {
            throw new System.InvalidOperationException("No video at 'Assets/Animations/animation at the end.mov'.");
        }

        VideoClip creditsClip = AssetDatabase.LoadAssetAtPath<VideoClip>("Assets/Animations/credits.mov");
        if (creditsClip == null)
        {
            throw new System.InvalidOperationException("No video at 'Assets/Animations/credits.mov'.");
        }

        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            throw new System.InvalidOperationException("No camera tagged MainCamera in the open scene.");
        }

        GameObject endingObject = new GameObject("Ending");
        Undo.RegisterCreatedObjectUndo(endingObject, "Create Ending");
        endingObject.transform.SetParent(cutscenes.transform, false);

        VideoPlayer video = endingObject.AddComponent<VideoPlayer>();
        video.clip = clip;
        video.playOnAwake = false;
        video.isLooping = false;
        video.renderMode = VideoRenderMode.CameraNearPlane;   // in front of the whole game world
        video.targetCamera = mainCamera;
        video.aspectRatio = VideoAspectRatio.FitInside;
        video.audioOutputMode = VideoAudioOutputMode.None;    // the clip has no sound; Ending Sound covers it

        Ending ending = endingObject.AddComponent<Ending>();
        SerializedObject endingSlots = new SerializedObject(ending);
        endingSlots.FindProperty("videoPlayer").objectReferenceValue = video;
        endingSlots.FindProperty("playerInput").objectReferenceValue = FindInScene<PlayerInput>();
        endingSlots.FindProperty("endScreen").objectReferenceValue = endScreen;
        endingSlots.FindProperty("endText").objectReferenceValue = endText;
        endingSlots.FindProperty("creditsClip").objectReferenceValue = creditsClip;
        endingSlots.ApplyModifiedProperties();

        SerializedObject gameFlowSlots = new SerializedObject(FindInScene<GameFlow>());
        gameFlowSlots.FindProperty("ending").objectReferenceValue = ending;
        gameFlowSlots.ApplyModifiedProperties();

        return ending;
    }

    [MenuItem("Tools/Lost Footage/Create Final Spots")]
    private static void CreateFinalSpots()
    {
        GameObject set = FindOrCreateEmptySet("Final Spots", "finalSpots");
        string[] getOut = { "(YOU NEED TO GET OUT OF HERE.)" };
        string[] none = new string[0];

        foreach (Transform structure in StructureObjects())
        {
            string kind = KindOf(structure);

            if (kind == "statue" || kind == "pewpew" || kind == "organ")
            {
                AddExamineSpot(set, structure, getOut, false, none);
            }
        }

        GameObject keypadSpot = CreateSpotAt(set, "Keypad Spot", KeypadPosition(), KeypadSpotSize);
        AddExamineText(keypadSpot, "keypad", getOut, false, none);
    }

    [MenuItem("Tools/Lost Footage/Set Up Mosaic Picture")]
    private static void SetUpMosaicPicture()
    {
        KeypadPuzzle puzzle = FindInScene<KeypadPuzzle>();
        SerializedObject puzzleSlots = new SerializedObject(puzzle);
        if (puzzleSlots.FindProperty("mosaicPicture").objectReferenceValue != null)
        {
            throw new System.InvalidOperationException("The mosaic picture is already set up.");
        }

        Sprite mosaic = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/all4shards.png");
        if (mosaic == null)
        {
            throw new System.InvalidOperationException("No sprite at 'Assets/Sprites/all4shards.png'.");
        }

        GameObject picture = new GameObject("Mosaic Picture", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        Undo.RegisterCreatedObjectUndo(picture, "Create Mosaic Picture");
        picture.transform.SetParent(FindInScene<DialogueBox>().transform, false);   // on Text Canvas
        picture.transform.SetSiblingIndex(0);                                        // first, so the dialogue box draws over it

        RectTransform pictureRect = picture.GetComponent<RectTransform>();
        pictureRect.anchoredPosition = new Vector2(0f, 150f);   // above the dialogue box
        pictureRect.sizeDelta = new Vector2(480f, 480f);        // 5 × the 96-pixel art, so it stays sharp

        Image image = picture.GetComponent<Image>();
        image.sprite = mosaic;
        image.raycastTarget = false;

        puzzleSlots.FindProperty("mosaicPicture").objectReferenceValue = picture;
        puzzleSlots.ApplyModifiedProperties();
    }

    private static Transform PuzzlesGroup()
    {
        GameObject puzzles = GameObject.Find("Puzzles");
        if (puzzles == null)
        {
            throw new System.InvalidOperationException("No 'Puzzles' group in the open scene.");
        }
        return puzzles.transform;
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
        GameObject stopsScreen = CreateTextScreen("Stops Screen", "Stops Text");
        OrganPuzzle puzzle = CreateOrganPuzzle(stopsScreen);
        WireOrganControls(puzzle);
        BuildOrganSpots(set, puzzle);

        Undo.CollapseUndoOperations(undoGroup);   // one Ctrl + Z undoes the whole setup
    }

    // A black panel with one text box in it (the stops grid, the keypad), on the same canvas as the dialogue box.
    private static GameObject CreateTextScreen(string screenName, string textName)
    {
        DialogueBox dialogueBox = FindInScene<DialogueBox>();   // it sits on Text Canvas

        GameObject screen = new GameObject(screenName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        Undo.RegisterCreatedObjectUndo(screen, "Create " + screenName);
        screen.transform.SetParent(dialogueBox.transform, false);
        screen.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 500);   // centred by default
        screen.GetComponent<Image>().color = Color.black;

        GameObject textObject = new GameObject(textName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        Undo.RegisterCreatedObjectUndo(textObject, "Create " + textName);
        textObject.transform.SetParent(screen.transform, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;            // stretch to fill the panel…
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(30, 30);     // …with a 30 margin
        textRect.offsetMax = new Vector2(-30, -30);

        // Same font as the dialogue box's text.
        SerializedObject dialogueSlots = new SerializedObject(dialogueBox);
        TMP_Text lineText = (TMP_Text)dialogueSlots.FindProperty("lineText").objectReferenceValue;

        TextMeshProUGUI screenText = textObject.GetComponent<TextMeshProUGUI>();
        screenText.font = lineText.font;
        screenText.fontSize = 40;
        screenText.color = Color.white;
        screenText.alignment = TextAlignmentOptions.Center;

        return screen;
    }

    private static OrganPuzzle CreateOrganPuzzle(GameObject stopsScreen)
    {
        GameObject puzzleObject = new GameObject("PuzzleFourOrgan");
        Undo.RegisterCreatedObjectUndo(puzzleObject, "Create PuzzleFourOrgan");
        puzzleObject.transform.SetParent(PuzzlesGroup(), false);
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
        EmptySet("Keypad Spots");
        EmptySet("Final Spots");

        CreateMeasuringSpots();
        CreateFindSoundSpots();
        CreateTakeEyeSpots();
        CreateGiveEyeSpots();
        CreateOrganSpots();
        CreateKeypadSpots();
        CreateFinalSpots();

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

        SetLines(door.FindProperty("openLines"), new[] {
            "(The Eye is complete.)",
            "(The door creaks open,)",
            "(but you can't move)",
            "(because)",
            "(He is watching)",
            "(…)",
            "(Above you.)" });

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

        Vector2 structureScale = new Vector2(Mathf.Abs(structure.lossyScale.x), Mathf.Abs(structure.lossyScale.y));   // mirrored objects have a negative scale
        Vector2 structureSize = Vector2.Scale(structureBox.size, structureScale);

        return CreateSpotAt(set, spotName, structure.TransformPoint(structureBox.offset), structureSize + new Vector2(Margin, Margin));
    }

    // A trigger spot of the given size at a position in the world.
    private static GameObject CreateSpotAt(GameObject set, string spotName, Vector3 position, Vector2 size)
    {
        GameObject spot = new GameObject(spotName);
        Undo.RegisterCreatedObjectUndo(spot, "Create " + spotName);
        spot.transform.SetParent(set.transform, false);
        spot.transform.position = position;

        BoxCollider2D trigger = spot.AddComponent<BoxCollider2D>();
        trigger.isTrigger = true;
        trigger.size = size;

        return spot;
    }

    // A spot with Examinable text, wired so E calls Examine.
    private static void AddExamineSpot(GameObject set, Transform structure, string[] lines, bool endsWithQuestion, string[] yesLines)
    {
        GameObject spot = CreateSpot(set, structure, structure.name + " Spot");
        AddExamineText(spot, KindOf(structure), lines, endsWithQuestion, yesLines);
    }

    // Puts Examinable text on a spot, wired so E calls Examine. kind picks its sound from Spot Sounds.
    private static void AddExamineText(GameObject spot, string kind, string[] lines, bool endsWithQuestion, string[] yesLines)
    {
        Examinable examinable = spot.AddComponent<Examinable>();
        SerializedObject examinableSlots = new SerializedObject(examinable);
        examinableSlots.FindProperty("dialogueBox").objectReferenceValue = FindInScene<DialogueBox>();
        SetLines(examinableSlots.FindProperty("lines"), lines);
        examinableSlots.FindProperty("endsWithQuestion").boolValue = endsWithQuestion;
        SetLines(examinableSlots.FindProperty("yesLines"), yesLines);
        examinableSlots.ApplyModifiedPropertiesWithoutUndo();

        InteractableObject interactable = spot.AddComponent<InteractableObject>();
        WireOnInteract(interactable, examinable.Examine);
        SetInteractSound(interactable, kind);
    }

    // A spot with no text of its own: E calls a puzzle method instead.
    private static void AddPuzzleSpot(GameObject set, Transform structure, string spotName, UnityAction method)
    {
        GameObject spot = CreateSpot(set, structure, spotName);
        InteractableObject interactable = spot.AddComponent<InteractableObject>();
        WireOnInteract(interactable, method);
        SetInteractSound(interactable, KindOf(structure));
    }

    private const string SpotSoundsPath = "Assets/Spot Sounds.asset";

    // Dylan's per-object sounds. The asset is made the first time it's needed.
    private static SpotSounds LoadSpotSounds()
    {
        SpotSounds sounds = AssetDatabase.LoadAssetAtPath<SpotSounds>(SpotSoundsPath);
        if (sounds == null)
        {
            sounds = ScriptableObject.CreateInstance<SpotSounds>();
            AssetDatabase.CreateAsset(sounds, SpotSoundsPath);
        }
        return sounds;
    }

    // Copies the sound for this kind of object into the spot's Interact Sound.
    private static void SetInteractSound(InteractableObject interactable, string kind)
    {
        string soundSlot;
        if (kind == "statue") soundSlot = "statueSound";
        else if (kind == "pewpew") soundSlot = "pewSound";
        else if (kind == "organ") soundSlot = "organSound";
        else if (kind == "keypad") soundSlot = "keypadSound";
        else throw new System.InvalidOperationException($"No spot sound for '{kind}'.");

        SerializedObject soundSlots = new SerializedObject(LoadSpotSounds());
        Object chosenEvent = soundSlots.FindProperty(soundSlot).FindPropertyRelative("WwiseObjectReference").objectReferenceValue;

        SerializedObject spotSlots = new SerializedObject(interactable);
        spotSlots.FindProperty("interactSound").FindPropertyRelative("WwiseObjectReference").objectReferenceValue = chosenEvent;
        spotSlots.ApplyModifiedPropertiesWithoutUndo();
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
            throw new System.InvalidOperationException($"No {typeof(T).Name} in the open scene. Is the Game scene open?");
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
