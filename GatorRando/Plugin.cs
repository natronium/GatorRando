using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using HarmonyLib;
using UnityEngine;
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

                //Edits to Martin's Tutorial Quest
                MartinEdits();

                //Edits to Jada's Quest
                JadaEdits();

                //Edits to Prep Quest
                GeneEdits();
                SusanneEdits();
                AntoneEdits();

                //Edits to Esme's Quest
                EsmeEdits();

                //Edits to sidequests
                KasenEdits();

                Junk4ShopCollect();
            }
        }

        public static void LogDebug(String s)
        {
            Instance.Logger.LogDebug(s);
        }

        public static void LogCall(String typeName, String methodName)
        {
            Instance.Logger.LogDebug($"{typeName}.{methodName}!");
            Instance.Logger.LogDebug(new System.Diagnostics.StackTrace().ToString());
        }

        public static void LogCheck(String typeName, String methodName, String checkName)
        {
            Instance.Logger.LogDebug($"{typeName}.{methodName} gave {checkName}");
        }

        private static void MartinEdits()
        {
            GameObject get_pot_lid = GameObject.Find("Get Pot Lid");
            DialogueSequencer get_pot_sequence = get_pot_lid.GetComponent<DialogueSequencer>();
            get_pot_sequence.beforeSequence.ObliteratePersistentListenerByIndex(0);
            get_pot_sequence.beforeSequence.AddListener(CollectedPot);

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
            martin_quest_qs.states[2].stateObjects = martin_quest_qs.states[2].stateObjects.Remove(pot_pickup);
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
                GameObject loot_seq = economist_quest.transform.Find("Loot Get Sequence").gameObject;
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
            rock_sequencer.beforeSequence.AddListener(CollectedMagicOre);

            if (ArchipelagoManager.LocationIsCollected("Magic Ore Pickup"))
            {
                CollectedMagicOre();
            }
            if (ArchipelagoManager.ItemIsUnlocked("Magic Ore"))
            {
                UnlockedMagicOre();
            }
        }

        private static void CollectedMagicOre()
        {
            GameObject prep_quest = GameObject.Find("Prep Quest");
            Transform prep_subquests = prep_quest.transform.Find("Subquests");
            GameObject engineer_quest = prep_subquests.Find("Engineer").gameObject;
            GameObject rocks = engineer_quest.transform.Find("Special Rocks").gameObject;
            QuestStates engineer_quest_qs = engineer_quest.GetComponent<QuestStates>();
            engineer_quest_qs.states[1].stateObjects = engineer_quest_qs.states[1].stateObjects.Remove(rocks);
            rocks.SetActive(false);
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

        private static void AntoneEdits()
        {
            //TODO: these edits are not yet working: position for chill bug -95.9112 13.713 -43.595
            //Edits to Antone's Quest
            GameObject prep_quest = GameObject.Find("Prep Quest");
            Transform prep_subquests = prep_quest.transform.Find("Subquests");
            GameObject entomologist_quest = prep_subquests.Find("Entomologist").gameObject;
            QuestStates entomologist_quest_qs = entomologist_quest.GetComponent<QuestStates>();
            GameObject sneak_seq = entomologist_quest.transform.Find("Sneak up sequence").gameObject;
            entomologist_quest_qs.states[1].stateObjects = entomologist_quest_qs.states[1].stateObjects.Remove(sneak_seq);
            sneak_seq.SetActive(false);
            if (ArchipelagoManager.ItemIsUnlocked("Bug Net (Sword)"))
            {
                UnlockedBugNet();
            }
        }

        private static void UnlockedBugNet()
        {
            GameObject prep_quest = GameObject.Find("Prep Quest");
            Transform prep_subquests = prep_quest.transform.Find("Subquests");
            GameObject entomologist_quest = prep_subquests.Find("Entomologist").gameObject;
            QuestStates entomologist_quest_qs = entomologist_quest.GetComponent<QuestStates>();
            GameObject sneak_seq = entomologist_quest.transform.Find("Sneak up sequence").gameObject;
            entomologist_quest_qs.states[1].stateObjects.Add(sneak_seq);
            if (entomologist_quest_qs.StateID == 1)
            {
                sneak_seq.SetActive(true);
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
            get_ice_cream_seq.afterSequence.AddListener(CollectedSorbet);
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
            kasen_quest_qs.states[0].stateObjects = kasen_quest_qs.states[0].stateObjects.Remove(scooter_pickup);
            GameObject find = kasen_quest.transform.Find("find scooter").Find("find").gameObject;
            DialogueSequencer find_ds = find.GetComponent<DialogueSequencer>();
            find_ds.afterSequence.ObliteratePersistentListenerByIndex(1);
            find_ds.afterSequence.ObliteratePersistentListenerByIndex(0);

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
            GameObject northeast = GameObject.Find("NorthEast (Canyoney)");
            GameObject kasen_quest = northeast.transform.Find("SideQuests").Find("FetchVulture").gameObject;
            QuestStates kasen_quest_qs = kasen_quest.GetComponent<QuestStates>();
            if (kasen_quest_qs.StateID == 0)
            {
                kasen_quest_qs.JustProgressState();
            }
        }

        private static void Junk4ShopCollect()
        {
            GameObject junk_shop_object = Util.GetByPath("East (Creeklands)/Junk Shop/Cool Shop");
            JunkShop junk_shop = junk_shop_object.GetComponent<JunkShop>();
            List<string> junk4trash_items = ["Shield_Stretch", "Shield_TrashCanLid", "Item_StickyHand", "Item_PaintGun", "Sword_Wrench", "Sword_Grabby"];
            foreach (string item in junk4trash_items)
            {
                print(item);
                if (ArchipelagoManager.LocationIsCollected(item))
                {
                    foreach (int i in Enumerable.Range(0, junk_shop.shopItems.Length))
                    {
                        JunkShop.ShopItem shop_item = junk_shop.shopItems[i];
                        print(shop_item.item.name);
                        if (shop_item.item.name == item)
                        {
                            shop_item.isHidden = true;
                            junk_shop.shopItems[i] = shop_item;
                            // junk_shop.UpdateInventory();
                            print("hidden!");
                            break;
                        }
                    }
                }
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
    }
}
