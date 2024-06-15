using System;
using System.Linq.Expressions;
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
                //Edits on Tutorial Island
                
                //Edits to Martin's Quest

                Debug.Log("J");
                //Edits to Jada's Quest
                GameObject cool_kids_quest = GameObject.Find("Cool Kids Quest");
                Transform cool_kids_subquests = cool_kids_quest.transform.Find("Subquests");
                GameObject boar_quest = cool_kids_subquests.Find("Boar Quest").gameObject;
                QuestStates boar_quest_qs = boar_quest.GetComponent<QuestStates>();
                Debug.Log("J");
                // Jada: Grass Clippings Section
                // Need to remove OnProgress() delegate...
                boar_quest_qs.states[2].onProgress.ObliteratePersistentListenerByIndex(0);

                if (ArchipelagoManager.ItemIsUnlocked("Grass Clippings"))
                {
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
                // Jada: Water Bucket Section
                Transform sprout = boar_quest.transform.Find("Sprout");
                GameObject water_seq = sprout.Find("Water Sequence").gameObject;
                //Need to remove give bucket delegate from water_seq.Dialogue.onStart() (how to find the right dialogue object?)

                Debug.Log("J");
                //Edits to Prep Quest
                GameObject prep_quest = GameObject.Find("Prep Quest");
                Transform prep_subquests = prep_quest.transform.Find("Subquests");
                Debug.Log("G");
                //Edits to Gene's Quest
                GameObject economist_quest = prep_subquests.Find("Economist").gameObject;
                QuestStates economist_quest_qs = economist_quest.GetComponent<QuestStates>();
                // Need to remove Loot Get Sequence from economist_quest_qs.states[2].onProgress()
                if (ArchipelagoManager.ItemIsUnlocked("Cheese Sandwich"))
                {
                    LSDestroy ls_destroy = economist_quest_qs.GetComponent<LSDestroy>();
                    if (economist_quest_qs.StateID == 1 && !ls_destroy.enabled)
                    {
                        economist_quest_qs.JustProgressState();
                    }
                    else
                    {
                        GameObject loot_seq = GameObject.Find("Loot Sequence");
                        DialogueSequencer loot_sequencer = loot_seq.GetComponent<DialogueSequencer>();
                        loot_sequencer.afterSequence.AddListener(economist_quest_qs.JustProgressState);
                    }
                }
                Debug.Log("S");
                //Edits to Susanne's Quest
                GameObject engineer_quest = prep_subquests.Find("Engineer").gameObject;
                QuestStates engineer_quest_qs = engineer_quest.GetComponent<QuestStates>();
                // Need to remove QuestState.JustProgressState from Rock Get Sequence
                if (ArchipelagoManager.ItemIsUnlocked("Magic Ore"))
                {
                    GameObject special_rocks = engineer_quest.transform.Find("Special Rocks").gameObject;
                    if (engineer_quest_qs.StateID == 1 && !special_rocks.activeSelf)
                    {
                        engineer_quest_qs.JustProgressState();
                    }
                    else
                    {
                        GameObject rock_seq = GameObject.Find("Rock Get Sequence");
                        DialogueSequencer rock_sequencer = rock_seq.GetComponent<DialogueSequencer>();
                        rock_sequencer.beforeSequence.AddListener(engineer_quest_qs.JustProgressState);
                    }
                }

                
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
            }
        }

        [HarmonyPatch(typeof(QuestRewardItem))]
        private static class QuestRewardItemPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("GiveReward")]
            static bool PreGiveReward(QuestRewardItem __instance)
            {
                LogCheck("QuestRewardItem", "GiveReward", __instance.item);
                ArchipelagoManager.CollectLocationForItem(__instance.item);
                return false;
            }
        }

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
                ArchipelagoManager.CollectLocationForConfetti(__instance.name);          //This line is potentially redundant with Particle Pickup patch       
            }
        }

        [HarmonyPatch(typeof(DSItem))]
        private static class DSItemPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("RunItemSequence")]
            static bool PreRunItemSequence(DSItem __instance)
            {
                LogCheck("DSItem", "RunItemSequence", __instance.itemName);
                __instance.document = null;
                __instance.dialogue = "Collected an AP Item!"; // Need to replace this with a valid dialogue?
                __instance.isRealItem = false;
                __instance.itemName = "AP Item Here!";
                // Eventually replace itemSprite too
                return true;
            }
        } //TODO: Collecting first Craft Stuff Fails!

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
                return false;
            }
        }

        [HarmonyPatch(typeof(PositionChallenge))]
        private static class PositionChallengePatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("FinishChallenge")]
            static void PreFinishChallenge(PositionChallenge __instance)
            {
                LogCheck("PositionChallenge", "FinishChallenge", __instance.rewardAmount.ToString());
                ArchipelagoManager.CollectLocationForConfetti(__instance.name);
            }
        }


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
                Instance.Logger.LogDebug($"ParticlePickup.Start!");
                ItemResource dummyResource = new()
                {
                    id = "Dummy_Resource_particle",
                    name = "Dummy Resource Particles",
                    itemGetID = "Dummy_Part",
                    showItemGet = false,
                    onAmountChanged = new UnityEvent<int>()
                };
                __instance.resource = dummyResource;
                ArchipelagoManager.CollectLocationForParticlePickup(__instance);
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
                else {
                    return true;
                }
                
            }
        }

    }
}
