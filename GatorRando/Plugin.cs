using System;
using System.Collections;
using System.Linq;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace GatorRando
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;

        //fix to be options from Archipelago
        readonly bool freeplay_from_start = true;

        private void Awake()
        {
            Instance = this;
            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll(); // automatically patch based on harmony attributes
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Logger.LogDebug("THIS IS A DEBUG LOG! LOOK AT ME! @@@@@@@@@@@");
        }

        void OnEnable()
        {
            Debug.Log("OnEnable called");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("OnSceneLoaded: " + scene.name);
            Debug.Log(mode);
            if (scene.name == "Island")
            {
                //Allow Freeplay
                if (freeplay_from_start)
                {
                    ReenableTutorialQuests();
                    GameObject act1 = GameObject.Find("Act 1");
                    QuestStates act1_qs = act1.GetComponent<QuestStates>();
                    act1_qs.states[0].onDeactivate.AddListener(AdvanceToEndOfTutorial);
                    act1_qs.states[3].onActivate.AddListener(ReenableTutorialQuests);
                    GameObject manager = GameObject.Find("Managers");
                    Game game = manager.GetComponent<Game>();
                    game.SetToStory();
                }

                //Edits to Martin's Quest
                MartinEdits();

                //Edits to Jada's Quest
                JadaEdits();

                //Edits to Prep Quest
                GeneEdits();
                SusanneEdits();
                // TODO: Check if Antone's quest requires bug net already

                //Edits to Esme's Quest
                EsmeEdits();

                KasenEdits();

            }
        }

        public static void LogDebug(String s)
        {
            Instance.Logger.LogDebug(s);
        }

        static void LogCall(String typeName, String methodName)
        {
            Instance.Logger.LogDebug($"{typeName}.{methodName}!");
            Instance.Logger.LogDebug(new System.Diagnostics.StackTrace().ToString());
        }

        static void LogCheck(String typeName, String methodName, String checkName)
        {
            Instance.Logger.LogDebug($"{typeName}.{methodName} gave {checkName}");
        }

        // Not yet working
        // public static GameObject GetGameObjectByPath(string path)
        // {
        //     var elements = path.Split('/');
        //     var root = SceneManager.GetActiveScene().GetRootGameObjects().First((go) => go.name == elements[0]);
        //     GameObject current = root;
        //     foreach (var element in elements.Skip(1))
        //     {
        //         foreach (var transform in current.GetComponentsInChildren<Transform>(true))
        //         {
        //             if (transform.name == element)
        //             {
        //                 current = transform.gameObject;
        //                 break;
        //             }
        //         }
        //     }
        //     return current;

        // }

        private static void MartinEdits()
        {
            GameObject get_pot_lid = GameObject.Find("Get Pot Lid");
            DialogueSequencer get_pot_sequence = get_pot_lid.GetComponent<DialogueSequencer>();
            get_pot_sequence.beforeSequence.ObliteratePersistentListenerByIndex(0);

            if (ArchipelagoManager.LocationIsCollected("Pot? Pickup"))
            {
                CollectedPot();
            }
            if (ArchipelagoManager.ItemIsUnlocked("Pot?"))
            {
                UnlockedPot();
            }
        }

        private static void UnlockedPot()
        {
            // GameObject pot_pickup = GetGameObjectByPath("Act 1/Quests/Martin Quest/Pickup");
            GameObject act1 = GameObject.Find("Act 1");
            Transform act1_quests = act1.transform.Find("Quests");
            GameObject martin_quest = act1_quests.Find("Martin Quest").gameObject;
            QuestStates martin_quest_qs = martin_quest.GetComponent<QuestStates>();
            GameObject pot_pickup = martin_quest.transform.Find("Pickup").gameObject;

            if (martin_quest_qs.StateID == 1 && !pot_pickup.activeSelf)
            {
                martin_quest_qs.JustProgressState();
            }
            else
            {
                GameObject get_pot_lid = GameObject.Find("Get Pot Lid");
                DialogueSequencer get_pot_sequence = get_pot_lid.GetComponent<DialogueSequencer>();
                get_pot_sequence.beforeSequence.AddListener(martin_quest_qs.JustProgressState);
            }
        }

        private static void CollectedPot()
        {
            GameObject act1 = GameObject.Find("Act 1");
            Transform act1_quests = act1.transform.Find("Quests");
            GameObject martin_quest = act1_quests.Find("Martin Quest").gameObject;
            QuestStates martin_quest_qs = martin_quest.GetComponent<QuestStates>();
            GameObject pot_pickup = martin_quest.transform.Find("Pickup").gameObject;
            martin_quest_qs.states[2].stateObjects.Remove(pot_pickup);
            pot_pickup.SetActive(false);
        }

        private static void JadaEdits()
        {
            GameObject cool_kids_quest = GameObject.Find("Cool Kids Quest");
            Transform cool_kids_subquests = cool_kids_quest.transform.Find("Subquests");
            GameObject boar_quest = cool_kids_subquests.Find("Boar Quest").gameObject;
            QuestStates boar_quest_qs = boar_quest.GetComponent<QuestStates>();
            // Jada: Grass Clippings Section
            // Need to remove OnProgress() delegate
            boar_quest_qs.states[2].onProgress.ObliteratePersistentListenerByIndex(0);

            if (ArchipelagoManager.ItemIsUnlocked("Grass Clippings"))
            {
                UnlockedGrassClippings();
            }
            // Jada: Water Bucket Section
            Transform sprout = boar_quest.transform.Find("Sprout");
            GameObject water_seq = sprout.Find("Water Sequence").gameObject;
            DSDialogue water_dia = water_seq.GetComponents<DSDialogue>()[1];
            //Need to remove give bucket delegate from water_seq.Dialogue.onStart()
            water_dia.onStart.ObliteratePersistentListenerByIndex(2);
        }

        private static void UnlockedGrassClippings()
        {
            GameObject cool_kids_quest = GameObject.Find("Cool Kids Quest");
            Transform cool_kids_subquests = cool_kids_quest.transform.Find("Subquests");
            GameObject boar_quest = cool_kids_subquests.Find("Boar Quest").gameObject;
            QuestStates boar_quest_qs = boar_quest.GetComponent<QuestStates>();

            LogicStateCollectGrass ls_grass = boar_quest_qs.GetComponent<LogicStateCollectGrass>();
            if (boar_quest_qs.StateID == 1 && !ls_grass.enabled)
            {
                boar_quest_qs.JustProgressState();
            }
            else
            {
                GameObject grass_seq = GameObject.Find("Got Enough Grass Sequence");
                DialogueSequencer grass_sequencer = grass_seq.GetComponent<DialogueSequencer>();
                grass_sequencer.afterSequence.AddListener(boar_quest_qs.JustProgressState);
            }
        }

        private static void GeneEdits()
        {
            GameObject prep_quest = GameObject.Find("Prep Quest");
            Transform prep_subquests = prep_quest.transform.Find("Subquests");
            //Edits to Gene's Quest
            GameObject economist_quest = prep_subquests.Find("Economist").gameObject;
            QuestStates economist_quest_qs = economist_quest.GetComponent<QuestStates>();
            // Need to remove Loot Get Sequence from economist_quest_qs.states[2].onProgress()
            economist_quest_qs.states[2].onProgress.ObliteratePersistentListenerByIndex(0);

            if (ArchipelagoManager.ItemIsUnlocked("Cheese Sandwich"))
            {
                UnlockedCheeseSandwich();
            }
        }

        private static void UnlockedCheeseSandwich()
        {
            GameObject prep_quest = GameObject.Find("Prep Quest");
            Transform prep_subquests = prep_quest.transform.Find("Subquests");
            GameObject economist_quest = prep_subquests.Find("Economist").gameObject;
            QuestStates economist_quest_qs = economist_quest.GetComponent<QuestStates>();

            LSDestroy ls_destroy = economist_quest_qs.GetComponent<LSDestroy>();
            if (economist_quest_qs.StateID == 1 && !ls_destroy.enabled)
            {
                economist_quest_qs.JustProgressState();
            }
            else
            {
                GameObject loot_seq = economist_quest.transform.Find("Loot Sequence").gameObject;
                DialogueSequencer loot_sequencer = loot_seq.GetComponent<DialogueSequencer>();
                loot_sequencer.afterSequence.AddListener(economist_quest_qs.JustProgressState);
            }
        }

        private static void SusanneEdits()
        {
            //Edits to Susanne's Quest
            // Need to remove QuestState.JustProgressState from Rock Get Sequence
            GameObject prep_quest = GameObject.Find("Prep Quest");
            Transform prep_subquests = prep_quest.transform.Find("Subquests");
            GameObject engineer_quest = prep_subquests.Find("Engineer").gameObject;
            GameObject rock_seq = engineer_quest.transform.Find("Rock Get Sequence").gameObject;
            DialogueSequencer rock_sequencer = rock_seq.GetComponent<DialogueSequencer>();
            rock_sequencer.beforeSequence.ObliteratePersistentListenerByIndex(0);
            if (ArchipelagoManager.ItemIsUnlocked("Magic Ore"))
            {
                UnlockedMagicOre();
            }
        }

        private static void UnlockedMagicOre()
        {
            GameObject prep_quest = GameObject.Find("Prep Quest");
            Transform prep_subquests = prep_quest.transform.Find("Subquests");
            GameObject engineer_quest = prep_subquests.Find("Engineer").gameObject;
            QuestStates engineer_quest_qs = engineer_quest.GetComponent<QuestStates>();
            GameObject special_rocks = engineer_quest.transform.Find("Special Rocks").gameObject;
            if (engineer_quest_qs.StateID == 1 && !special_rocks.activeSelf)
            {
                engineer_quest_qs.JustProgressState();
            }
            else
            {
                GameObject rock_seq = engineer_quest.transform.Find("Rock Get Sequence").gameObject;
                DialogueSequencer rock_sequencer = rock_seq.GetComponent<DialogueSequencer>();
                rock_sequencer.beforeSequence.AddListener(engineer_quest_qs.JustProgressState);
            }
        }

        private static void EsmeEdits()
        {
            GameObject theatre_quest = GameObject.Find("Theatre Quest");
            Transform theatre_subquests = theatre_quest.transform.Find("Subquests");
            GameObject vampire_quest = theatre_subquests.Find("Vampire").gameObject;
            GameObject get_ice_cream = vampire_quest.transform.Find("Get Ice Cream").gameObject;
            DialogueSequencer get_ice_cream_seq = get_ice_cream.GetComponent<DialogueSequencer>();
            get_ice_cream_seq.afterSequence.ObliteratePersistentListenerByIndex(0);
            if (ArchipelagoManager.LocationIsCollected("Ice Cream"))
            {
                CollectedSorbet();
            }
            if (ArchipelagoManager.ItemIsUnlocked("Sorbet"))
            {
                UnlockedSorbet();
            }

            GameObject become_vampire = vampire_quest.transform.Find("Become Vampire").gameObject;
            DSDialogue vampire_hat = become_vampire.GetComponents<DSDialogue>()[1];
            vampire_hat.onStart.ObliteratePersistentListenerByIndex(0);
            vampire_hat.onStart.AddListener(() => { ArchipelagoManager.CollectLocationForItem("Hat_Vampire"); });
        }

        private static void CollectedSorbet()
        {
            GameObject theatre_quest = GameObject.Find("Theatre Quest");
            Transform theatre_subquests = theatre_quest.transform.Find("Subquests");
            GameObject vampire_quest = theatre_subquests.Find("Vampire").gameObject;
            GameObject ice_cream = vampire_quest.transform.Find("IceCream").gameObject;
            ice_cream.SetActive(false);
        }

        private static void UnlockedSorbet()
        {
            GameObject theatre_quest = GameObject.Find("Theatre Quest");
            Transform theatre_subquests = theatre_quest.transform.Find("Subquests");
            GameObject vampire_quest = theatre_subquests.Find("Vampire").gameObject;
            QuestStates vampire_quest_qs = vampire_quest.GetComponent<QuestStates>();
            GameObject ice_cream = vampire_quest.transform.Find("IceCream").gameObject;

            if (vampire_quest_qs.StateID == 1 && !ice_cream.activeSelf)
            {
                vampire_quest_qs.JustProgressState();
            }
            else
            {
                GameObject get_ice_cream = GameObject.Find("Get Ice Cream");
                DialogueSequencer get_ice_cream_seq = get_ice_cream.GetComponent<DialogueSequencer>();
                get_ice_cream_seq.afterSequence.AddListener(vampire_quest_qs.JustProgressState);
            }
        }

        private static void KasenEdits()
        {
            GameObject kasen_quest = GameObject.Find("FetchVulture");
            QuestStates kasen_quest_qs = kasen_quest.GetComponent<QuestStates>();
            GameObject scooter_pickup = kasen_quest_qs.states[0].stateObjects[0];
            kasen_quest_qs.states[0].stateObjects.Remove(scooter_pickup);
            if (ArchipelagoManager.LocationIsCollected("Scooter Pickup"))
            {
                scooter_pickup.SetActive(false);
            }
            else
            {
                scooter_pickup.SetActive(true);
            }
            if (ArchipelagoManager.ItemIsUnlocked("Broken Scooter"))
            {
                UnlockedScooter();
            }
        }

        private static void UnlockedScooter()
        {
            GameObject kasen_quest = GameObject.Find("FetchVulture");
            QuestStates kasen_quest_qs = kasen_quest.GetComponent<QuestStates>();
            if (kasen_quest_qs.StateID == 0)
            {
                kasen_quest_qs.JustProgressState();
            }
        }

        [HarmonyPatch(typeof(QuestRewardCrafts))]
        private static class QuestRewardCraftsPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("GiveReward")]
            static bool PreGiveReward(QuestRewardCrafts __instance)
            {
                LogCheck("QuestRewardCrafts", "GiveReward", __instance.rewards[0].Name);
                ArchipelagoManager.CollectLocationForItem(__instance.rewards[0].Name);
                return false;
                // TODO: UI for what item you picked up
            }
        }

        // [HarmonyPatch(typeof(QuestRewardItem))]
        // private static class QuestRewardItemPatch
        // {
        //     [HarmonyPrefix]
        //     [HarmonyPatch("GiveReward")]
        //     static bool PreGiveReward(QuestRewardItem __instance)
        //     {
        //         LogCheck("QuestRewardItem", "GiveReward", __instance.item);
        //         ArchipelagoManager.CollectLocationForItem(__instance.item);
        //         return false;
        //     }
        // }

        // [HarmonyPatch(typeof(QuestRewardConfetti))]
        // private static class QuestRewardConfettiPatch
        // {
        //     [HarmonyPrefix]
        //     [HarmonyPatch("GiveReward",[typeof(int)])]
        //     static void PreGiveReward(int amount, QuestRewardConfetti __instance)
        //     {
        //         LogCheck("QuestRewardConfetti","GiveReward",__instance.amount.ToString());
        //         ArchipelagoManager.CollectLocationForConfetti(__instance.name);          //This line is potentially redundant with Particle Pickup patch       
        //     }
        // }

        [HarmonyPatch(typeof(QuestRewardNPCs))]
        private static class QuestRewardNPCsPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("GiveReward")]
            static void PreGiveReward(QuestRewardNPCs __instance)
            {
                LogCheck("QuestRewardNPCs", "GiveReward", __instance.rewardCount.ToString());
                ArchipelagoManager.CollectLocationForNPCs(__instance.rewards); //BUG: This line fails with ???                
            }
        }

        [HarmonyPatch(typeof(DSItem))]
        private static class DSItemPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("RunItemSequence")]
            static bool PreRunItemSequence(DSItem __instance)
            {
                //TODO: Don't intercept Craft Stuff, Pot Lid?, LITTER
                // TODO: decide how to handle Sword_Pencil
                string name = "";
                if (__instance.item == null && __instance.name != "POT?" && __instance.name != "POT LID?")
                {
                    name = __instance.itemName;
                }
                else
                {
                    name = __instance.item.name;
                }
                LogCheck("DSItem", "RunItemSequence", name);
                if (ArchipelagoManager.CollectLocationForItem(name))
                {
                    __instance.document = null;
                    __instance.dialogue = "Collected an AP Item!"; // Need to replace this with a valid dialogue?
                    __instance.isRealItem = false;
                    __instance.itemName = "AP Item Here!";
                    // Eventually replace itemSprite too
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(InteractItemUnlock))]
        private static class InteractItemUnlockPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("Interact")]
            static bool PreInteract(InteractItemUnlock __instance)
            {
                LogCheck("InteractItemUnlock", "Interact", __instance.itemName);
                __instance.gameObject.SetActive(false);
                __instance.SaveTrue();
                ArchipelagoManager.CollectLocationForItem(__instance.itemName);
                return false;
            }
        }

        // [HarmonyPatch(typeof(PositionChallenge))]
        // private static class PositionChallengePatch
        // {
        //     [HarmonyPrefix]
        //     [HarmonyPatch("FinishChallenge")]
        //     static void PreFinishChallenge(PositionChallenge __instance)
        //     {
        //         LogCheck("PositionChallenge", "FinishChallenge", __instance.rewardAmount.ToString());
        //         ArchipelagoManager.CollectLocationForConfetti(__instance.name);
        //     }
        // }


        [HarmonyPatch(typeof(TownNPCManager))]
        private static class TownNPCManagerPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("Awake")]
            static void PreAwake(TownNPCManager __instance)
            {
                Instance.Logger.LogDebug($"TownNPCManager.Awake!");
                ItemResource dummyResource = new()
                {
                    id = "Dummy_Resource_Population",
                    name = "Dummy Resource Population",
                    itemGetID = "Dummy_Pop",
                    showItemGet = false,
                    onAmountChanged = new UnityEvent<int>()
                };
                __instance.populationResource = dummyResource;
            }
        }

        [HarmonyPatch(typeof(ParticlePickup))]
        private static class ParticlePickupPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("Start")]
            static void PreStart(ParticlePickup __instance)
            {
                // Check if Pot Confetti for Pots, Confetti for Chests and Racetracks
                Instance.Logger.LogDebug($"ParticlePickup.Start for {__instance.particleSystem.name}");
                if (__instance.particleSystem.name == "Pot Confetti" || __instance.particleSystem.name == "Confetti")
                {
                    ItemResource dummyResource = new()
                    {
                        id = "Dummy_Resource_particle",
                        name = "Dummy Resource Particles",
                        itemGetID = "Dummy_Part",
                        showItemGet = false,
                        onAmountChanged = new UnityEvent<int>()
                    };
                    __instance.resource = dummyResource;
                }
            }
        }

        [HarmonyPatch(typeof(BreakableObject))]
        private static class BreakableObjectPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("Break", [typeof(bool), typeof(Vector3), typeof(bool)])]
            static void PreBreak(BreakableObject __instance, bool fromAttachment, Vector3 velocity, bool isHeavy)
            {
                ArchipelagoManager.CollectLocationForBreakableObject(__instance.id, __instance.name);
            }
        }

        [HarmonyPatch(typeof(BreakableObjectMulti))]
        private static class BreakableObjectMultiPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("Break", [typeof(bool), typeof(Vector3), typeof(bool)])]
            static void PreBreak(BreakableObjectMulti __instance, bool fromAttachment, Vector3 velocity, bool isSturdy)
            {
                ArchipelagoManager.CollectLocationForBreakableObject(__instance.id, __instance.name);
            }
        }

        [HarmonyPatch(typeof(TimedChallenge))]
        private static class TimedChallengePatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("FinishRace")]
            static void PreFinishRace(TimedChallenge __instance)
            {
                if (__instance is Racetrack)
                {
                    ArchipelagoManager.CollectLocationForRace(__instance.id);
                }
            }
        }


        [HarmonyPatch(typeof(BraceletShopFailsafe))]
        private static class BraceletShopFailsafePatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("CheckFailsafe")]
            static bool PreCheckFailsafe()
            {
                LogCall("BraceletShopFailsafe", "CheckFailsafe");
                return false;
            }
        }

        private static void AdvanceToEndOfTutorial()
        {
            GameObject act1 = GameObject.Find("Act 1");
            QuestStates act1qs = act1.GetComponent<QuestStates>();
            if (act1qs.StateID < 2)
            {
                act1qs.ProgressState(2);
                ReenableTutorialQuests();
            }
        }

        private static void ReenableTutorialQuests()
        {
            GameObject act1 = GameObject.Find("Act 1");
            LSQuests act1lsq = act1.GetComponent<LSQuests>();
            act1lsq.enabled = true;
            Transform act1questsTransform = act1.transform.Find("Quests");
            GameObject act1quests = act1questsTransform.gameObject;
            act1quests.SetActive(true);
        }

        [HarmonyPatch(typeof(LogicStateCollectGrass))]
        private static class LogicStateCollectGrassPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("OnDetailsCut")]
            static bool PreOnDetailsCut(LogicStateCollectGrass __instance, int cutAmount)
            {
                LogCall("LogicStateCollectGrass", "OnDetailsCut");
                Traverse traverse = Traverse.Create(__instance).Field("currentCutAmount");
                int currentCutAmount = traverse.GetValue<int>();

                currentCutAmount += cutAmount;
                if (currentCutAmount > __instance.cutAmountNeeded)
                {
                    GameObject grass_seq = GameObject.Find("Got Enough Grass Sequence");
                    DialogueSequencer grass_sequencer = grass_seq.GetComponent<DialogueSequencer>();
                    grass_sequencer.JustStartSequence();
                    __instance.enabled = false;
                }

                traverse.SetValue(currentCutAmount);
                return false;
            }
        }

        [HarmonyPatch(typeof(LogicStateSubmerge))]
        private static class LogicStateSubmergePatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("FixedUpdate")]
            static bool PreFixedUpdate(LogicStateSubmerge __instance)
            {
                //Only collect water if have the bucket
                if (ArchipelagoManager.ItemIsUnlocked("Bucket"))
                {
                    Traverse traverse = Traverse.Create(__instance).Field("swimmingCounter");
                    int swimmingCounter = traverse.GetValue<int>();
                    if (Player.movement.IsSwimming)
                    {
                        swimmingCounter++;
                    }
                    else
                    {
                        swimmingCounter = 0;
                    }
                    if (swimmingCounter > 10)
                    {
                        __instance.LogicCompleted();
                    }
                    traverse.SetValue(swimmingCounter);
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(LSDestroy))]
        private static class LSDestroyPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("CheckLogic")]
            static bool PreCheckLogic(LSDestroy __instance)
            {
                //Only modify behavior if Gene's Quest 
                if (__instance.stateName == "Defeat the slimes")
                {
                    if (!__instance.enabled)
                    {
                        return false;
                    }
                    int num = 0;
                    for (int i = 0; i < __instance.targets.Length; i++)
                    {
                        if (!__instance.targets[i].IsBroken)
                        {
                            num++;
                        }
                    }
                    Traverse traverse = Traverse.Create(__instance).Field("lastAliveTargets");
                    int lastAliveTargets = traverse.GetValue<int>();
                    if (lastAliveTargets != num)
                    {
                        foreach (LSDestroy.DestroyEvent destroyEvent in __instance.events)
                        {
                            if ((!destroyEvent.disableOnAwake || lastAliveTargets != -1) && destroyEvent.aliveTargetCount == num)
                            {
                                destroyEvent.onReachCount.Invoke();
                            }
                        }
                    }
                    if (num <= __instance.desiredUnbrokenTargets)
                    {
                        GameObject loot_seq = GameObject.Find("Loot Sequence");
                        DialogueSequencer loot_sequencer = loot_seq.GetComponent<DialogueSequencer>();
                        loot_sequencer.JustStartSequence();
                        __instance.enabled = false;
                    }
                    traverse.SetValue(num);
                    return false;
                }
                else
                {
                    return true;
                }

            }
        }

        [HarmonyPatch(typeof(BraceletShopDialogue))]
        private static class BraceletShopDialoguePatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("Interact")]
            static bool PreInteract(BraceletShopDialogue __instance)
            {
                CoroutineUtil.Start(RunShopModified(__instance));
                return false;
            }

            private static IEnumerator RunShopModified(BraceletShopDialogue bsd)
            {
                //TODO: Revise to remove duplicated code
                Game.DialogueDepth++;
                bsd.state = GameData.g.ReadInt(bsd.SaveID, 0);
                bsd.itemResource.ForceShow = true;
                int price = bsd.price;
                MultilingualTextDocument.SetPlaceholder(0, price.ToString("0"));
                yield return bsd.LoadDialogue(bsd.promptDialogue);
                bsd.itemResource.ForceShow = false;
                if (DialogueManager.optionChosen == 1)
                {
                    if (bsd.itemResource.Amount >= price)
                    {
                        bsd.itemResource.Amount -= price;
                        yield return bsd.LoadDialogue(bsd.purchaseDialogue);
                        bsd.itemResource.ForceShow = false;
                        Player.movement.Stamina = 0f;
                        ArchipelagoManager.CollectLocationForBracelet(bsd.SaveID);
                        yield return null;
                        // Player.itemManager.Refresh();
                        // yield return bsd.DoBraceletGet(); //TODO: OVERWRITE with appropriate UI for archipelago item
                        bsd.state++;
                        GameData.g.Write(bsd.SaveID, bsd.state);
                        if (bsd.state >= bsd.braceletsInStock)
                        {
                            bsd.actors[0].showNpcMarker = false;
                            if (bsd.CheckIfAllBraceletShops())
                            {
                                Game.DialogueDepth++;
                                yield return bsd.LoadDialogue(bsd.allPurchased);
                                yield return bsd.StartCoroutine(bsd.Poof());
                                yield return new WaitForSeconds(1.5f);
                                yield return CoroutineUtil.Start(DialogueManager.d.LoadChunk(bsd.document.FetchChunk(bsd.afterAllPurchased), null, DialogueManager.DialogueBoxBackground.Standard, true, true, false, false));
                                bsd.rewardNPC.GiveReward();
                                Game.DialogueDepth--;
                            }
                            else
                            {
                                yield return bsd.LoadDialogue(bsd.leaveDialogue);
                                yield return bsd.StartCoroutine(bsd.Poof());
                            }
                        }
                    }
                    else
                    {
                        yield return bsd.LoadDialogue(bsd.notEnoughDialogue);
                    }
                }
                else
                {
                    yield return bsd.LoadDialogue(bsd.noPurchaseDialogue);
                }
                bsd.itemResource.ForceShow = false;
                GameData.g.Write(bsd.SaveID, bsd.state);
                Game.DialogueDepth--;
                yield break;
            }
        }

        [HarmonyPatch(typeof(JunkShop))]
        private static class JunkShopPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("RunShopDialogue")]
            static bool PreRunShopDialogue(JunkShop __instance)
            {
                CoroutineUtil.Start(RunShopDialogueSequenceModified(__instance));
                return false;
            }

            private static IEnumerator RunShopDialogueSequenceModified(JunkShop js)
            {
                //TODO: Revise to remove duplicated code
                Game.DialogueDepth++;
                js.itemResource.ForceShow = true;
                GameObject[] array = js.shopStateObjects;
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].SetActive(true);
                }
                yield return CoroutineUtil.Start(DialogueManager.d.LoadChunk(js.document.FetchChunk(js.shopIntro), js.actors, DialogueManager.DialogueBoxBackground.Standard, true, true, false, false));
                DialogueManager.optionChosen = -1;
                CoroutineUtil.Start(DialogueManager.d.RunDialogueOptions(js.GetChoiceList()));
                int selectedOption = 0;
                while (DialogueManager.optionChosen == -1)
                {
                    int currentlySelectedIndex = DialogueOptions.CurrentlySelectedIndex;
                    for (int j = 0; j < js.cameras.Length; j++)
                    {
                        js.cameras[j].SetActive(j == currentlySelectedIndex - 1);
                    }
                    if (currentlySelectedIndex != selectedOption)
                    {
                        if (currentlySelectedIndex == 0)
                        {
                            js.uiItemResource.ClearPrice();
                        }
                        else
                        {
                            js.uiItemResource.SetPrice(js.shopItems[js.displayedItems[currentlySelectedIndex - 1]].cost);
                        }
                    }
                    yield return null;
                }
                array = js.cameras;
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].SetActive(false);
                }
                if (DialogueManager.optionChosen == 0)
                {
                    js.uiItemResource.ClearPrice();
                    js.itemResource.ForceShow = false;
                    yield return CoroutineUtil.Start(DialogueManager.d.LoadChunk(js.document.FetchChunk(js.cancelDialogue), js.actors, DialogueManager.DialogueBoxBackground.Standard, true, true, false, false));
                }
                else
                {
                    int num = DialogueManager.optionChosen - 1;
                    JunkShop.ShopItem shopItem = js.shopItems[js.displayedItems[num]];
                    if (js.itemResource.Amount >= shopItem.cost)
                    {
                        js.uiItemResource.ClearPrice();
                        js.itemResource.ForceShow = false;
                        js.itemResource.Amount -= shopItem.cost;
                        ArchipelagoManager.CollectLocationForJunkShop(shopItem.item.name);
                        shopItem.isHidden = true;
                        js.shopItems[js.displayedItems[num]] = shopItem;
                        js.UpdateInventory();
                        yield return CoroutineUtil.Start(js.itemGet.RunSequence(shopItem.item.sprite, shopItem.item.DisplayName, js.document.FetchChunk(shopItem.unlockChunk), js.actors));
                    }
                    else
                    {
                        yield return CoroutineUtil.Start(DialogueManager.d.LoadChunk(js.document.FetchChunk(js.notEnoughDialogue), js.actors, DialogueManager.DialogueBoxBackground.Standard, true, true, false, false));
                        js.uiItemResource.ClearPrice();
                        js.itemResource.ForceShow = false;
                    }
                }
                array = js.shopStateObjects;
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].SetActive(false);
                }
                if (js.displayedItemCount == 0)
                {
                    yield return DialogueManager.d.LoadChunk(js.document.FetchChunk(js.soldOutDialogue), js.actors, DialogueManager.DialogueBoxBackground.Standard, true, true, false, false);
                    yield return js.stateMachine.ProgressState(-1);
                }
                Game.DialogueDepth--;
                yield break;
            }
        }

    }
}
