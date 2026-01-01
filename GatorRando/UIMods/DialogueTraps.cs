using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GatorRando.UIMods;

internal class DialogueTraps
{
    private void PlaceGameObjectInFrontOfPlayer(GameObject gameObject)
    {
        // Place the GameObject 2 units in front of the player
        gameObject.transform.position = Player.RawPosition + Player.transform.rotation * new Vector3(0, 0, 2);
        gameObject.transform.rotation = Player.transform.rotation;
        gameObject.transform.Rotate(new Vector3(0, 180, 0)); // Face the player
    }
    private GameObject gooseCopy = null;
    private DialogueSequencer courtroomSequence = null;

    internal IEnumerator RunCourtroomTrap()
    {
        if (courtroomSequence == null || gooseCopy == null)
        {
            yield return SetupCourtroom();
        }
        DialogueModifier.SetTrapDialogue(true);
        PlaceGameObjectInFrontOfPlayer(gooseCopy);
        gooseCopy.transform.GetChild(0).gameObject.SetActive(true);
        gooseCopy.transform.GetChild(1).gameObject.SetActive(true);
        yield return courtroomSequence.StartSequence();
        DialogueModifier.SetTrapDialogue(false);
    }

    private IEnumerator SetupCourtroom()
    {
        // N.B. The yield return null lines were determined empirically to make sure changes are applied before continuing
        GameObject gooseQuest = Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Goose Quest/");
        bool temp = gooseQuest.activeSelf;
        gooseQuest.SetActive(false);
        gooseCopy = Object.Instantiate(gooseQuest);
        gooseQuest.SetActive(temp);
        PlaceGameObjectInFrontOfPlayer(gooseCopy);
        Object.Destroy(gooseCopy.GetComponent<SyncQuestStates>());
        yield return null;
        Object.Destroy(gooseCopy.GetComponent<SyncQuestStates>());
        Object.Destroy(gooseCopy.GetComponent<QuestRewards>());
        Object.Destroy(gooseCopy.GetComponent<QuestRewardCrafts>());
        Object.Destroy(gooseCopy.GetComponent<QuestStates>());
        Object.Destroy(gooseCopy.transform.GetChild(3).gameObject);
        Object.Destroy(gooseCopy.transform.GetChild(0).gameObject);
        yield return null;
        GameObject cameras = gooseCopy.transform.GetChild(2).gameObject;
        GameObject courtroom = gooseCopy.transform.GetChild(3).gameObject;
        Vector3 cameraMinusCourtroom = cameras.transform.position - courtroom.transform.position;
        PlaceGameObjectInFrontOfPlayer(courtroom);
        cameras.transform.position = courtroom.transform.position + cameraMinusCourtroom;
        yield return null;
        PlaceGameObjectInFrontOfPlayer(courtroom.transform.GetChild(0).gameObject);
        yield return null;
        gooseCopy.SetActive(true);
        foreach (DSDialogue dialogue in courtroom.GetComponents<DSDialogue>())
        {
            dialogue.presetPosition = false;
        }
        courtroomSequence = courtroom.GetComponent<DialogueSequencer>();
        courtroomSequence.chainedSequence.transform.GetChild(0).GetComponent<SetPlayerActorState>().applyTransform = false;
        courtroomSequence.chainedSequence.chainedSequence = null;
        Object.Destroy(courtroomSequence.chainedSequence.transform.GetChild(1).GetComponent<Cinemachine.CinemachineVirtualCamera>());
    }

    private readonly List<string> products = [
        "1 WHOLE HYLIAN RUPEE",
        "THE FOURTH WALL",
        "WORLD PEACE",
        "THE GAME MANUAL",
        "HERO'S LAURELS",
        "A SENSE OF ACCOMPLISHMENT",
        "HAPPINESS",
        "A RANDOMIZER FOR YOUR FAVORITE GAME",
    ];
    private readonly List<string> costs = [
        "YOUR SOUL. MUHAHAHAHAHA!",
        "5 WHOLE HYLIAN RUPEES.",
        "A DEFENSE OFFERING.",
        "10 BAJILLION CURRENCY.",
        "42 LINES OF CODE",
        "5 RINGS",
        "TRUE FRIENDSHIP",
        "A STRAWBERRY",
        "A RANDOMIZER FOR MY FAVORITE GAME",
    ];

    private GameObject newMonkey;

    private void SetupMonkey()
    {

        GameObject monkeyObject = Util.GetByPath("NorthWest (Tutorial Island)/Side/Monkey Quest/Monkey");
        newMonkey = Object.Instantiate(monkeyObject);
        newMonkey.gameObject.SetActive(false);
        DialogueActor monkeyActor = newMonkey.GetComponent<DialogueActor>();
        List<int> firstDiagActors = [1, 0, 1, 1, 0, 1, 1];
        List<int> firstEmotes = [0, 0, 0, 0, 0, 0, 0];
        foreach (string product in products)
        {
            foreach (string cost in costs)
            {
                string dialogueName = product + " " + cost;
                List<string> dialogueStrings = [
                    "GOOD EVENING, GOOD GENTLEGATOR!",
                    "it's clearly midday??",
                    "NO MATTER, I AM HERE TO PRESENT TO YOU THE FINEST IN SYNERGISTIC AND NATURALLY INTELLIGENT PURCHASES.",
                    product,
                    "how much does it cost?",
                    "IT CAN BE YOURS FOR THE LOW, LOW PRICE OF ",
                    cost,
                ];
                DialogueModifier.AddNewMultiLineDialogueChunk(dialogueName, dialogueStrings, firstDiagActors, firstEmotes);
            }
        }
        string yesResponse = "yes, that sounds fair to me!";
        ChoiceQuest.Prompt.Choice yes = new()
        {
            isCorrect = false,
            response = yesResponse,
        };
        List<string> yesLines = [
            yesResponse,
                "I DON'T BELIEVE YOU. CONVINCE ME.",
            ];
        DialogueModifier.AddNewMultiLineDialogueChunk(yesResponse, yesLines, [0, 1], [0, 0]);
        string noResponse = "no, thank you";
        ChoiceQuest.Prompt.Choice no = new()
        {
            isCorrect = true,
            response = noResponse,
        };
        List<string> noLines = [
            noResponse,
            ];
        DialogueModifier.AddNewMultiLineDialogueChunk(noResponse, noLines, [0], [0]);
        DialogueModifier.AddChoiceQuest(newMonkey, [monkeyActor], [yes, no], "WOULD YOU LIKE TO MAKE A PURCHASE?", "CHEAPSKATE. YOU DON'T APPRECIATE TRUE VALUE!");
    }

    internal IEnumerator RunMonkeyTrap()
    {
        if (newMonkey == null)
        {
            SetupMonkey();
        }
        int productChoice = Random.Range(0, products.Count);
        int costChoice = Random.Range(0, costs.Count);
        string dialogueChosen = products[productChoice] + " " + costs[costChoice];
        DialogueActor monkeyActor = newMonkey.GetComponent<DialogueActor>();
        DSDialogue trapDialogue = new()
        {
            dialogue = dialogueChosen,
            actors = [monkeyActor],
        };
        yield return null;
        PlaceGameObjectInFrontOfPlayer(newMonkey);
        newMonkey.GetComponentInChildren<PoofObject>().destroyObject = new();
        newMonkey.gameObject.SetActive(true);
        DialogueModifier.SetTrapDialogue(true);
        yield return trapDialogue.Run();
        newMonkey.GetComponent<ChoiceQuest>().State = false;
        while (!newMonkey.GetComponent<ChoiceQuest>().State)
        {
            yield return newMonkey.GetComponent<ChoiceQuest>().RunConversation();
        }
        yield return newMonkey.GetComponent<BraceletShopDialogue>().Poof();
        newMonkey.GetComponentInChildren<PoofObject>().Poof();
        yield return null;
        newMonkey.gameObject.SetActive(false);
        DialogueModifier.SetTrapDialogue(false);
        yield return null;
    }

	private DialogueActor trishActor;

	private const int lookAroundEmote = 485016557;
    private void SetupTrish()
    {
        List<string> lines = [
            "lalalala",
            "trish, is that you? i can't see you",
            "i am one with the wind",
            "trish, i really can't see you this time",
            "perhaps, one day, the fairies will visit you again",
            "..."
        ];
        GameObject trish = Util.GetByPath("West (Forest)/Side Quests/Invisibile Horse Quest/Invisible Horse");
        trishActor = trish.GetComponent<DialogueActor>();
        DialogueModifier.AddNewMultiLineDialogueChunk("trish", lines, [1, 0, 1, 0, 1, 0], [0, lookAroundEmote, 0, lookAroundEmote, 0, 0]);
    }

    internal IEnumerator RunTrishTrap()
    {
        if (trishActor == null)
        {
            SetupTrish();
        }
        DialogueModifier.SetTrapDialogue(true);
        DSDialogue trapDialogue = new()
        {
            dialogue = "trish",
            actors = [trishActor],
        };
        yield return trapDialogue.Run();
        DialogueModifier.SetTrapDialogue(false);
    }
}