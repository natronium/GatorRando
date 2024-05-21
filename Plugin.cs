using System;
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
                ReenableTutorialQuests();
                GameObject act1 = GameObject.Find("Act 1");
                QuestStates act1qs = act1.GetComponent<QuestStates>();
                act1qs.states[0].onDeactivate.AddListener(AdvanceToEndOfTutorial);
                act1qs.states[3].onActivate.AddListener(ReenableTutorialQuests);
                GameObject manager = GameObject.Find("Managers");
                Game game = manager.GetComponent<Game>();
                game.SetToStory();
            }
        }

        public static void LogDebug(String s) {
            Instance.Logger.LogDebug(s);
        }

        static void LogCall(String typeName, String methodName)
        {
            Instance.Logger.LogDebug($"{typeName}.{methodName}!");
            Instance.Logger.LogDebug(new System.Diagnostics.StackTrace().ToString());
        }

        static void LogCheck(String typeName, String methodName, String checkName) {
            Instance.Logger.LogDebug($"{typeName}.{methodName} gave {checkName}");
        }

        [HarmonyPatch(typeof(QuestRewardCrafts))]
        private static class QuestRewardCraftsPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("GiveReward")]
            static bool PreGiveReward(QuestRewardCrafts __instance)
            {
                LogCheck("QuestRewardCrafts","GiveReward",__instance.rewards[0].Name);
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
                LogCheck("QuestRewardItem","GiveReward",__instance.item);
                ArchipelagoManager.CollectLocationForItem(__instance.item);                
                return false;
            }
        }

        [HarmonyPatch(typeof(QuestRewardConfetti))]
        private static class QuestRewardConfettiPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("GiveReward",[typeof(int)])]
            static void PreGiveReward(int amount, QuestRewardConfetti __instance)
            {
                LogCheck("QuestRewardConfetti","GiveReward",__instance.amount.ToString());
                ArchipelagoManager.CollectLocationForConfetti(__instance.name);          //This line is potentially redundant with Particle Pickup patch       
            }
        }

        [HarmonyPatch(typeof(QuestRewardNPCs))]
        private static class QuestRewardNPCsPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("GiveReward")]
            static void PreGiveReward(QuestRewardNPCs __instance)
            {
                LogCheck("QuestRewardNPCs","GiveReward",__instance.rewardCount.ToString());
                ArchipelagoManager.CollectLocationForConfetti(__instance.name);          //This line is potentially redundant with Particle Pickup patch       
            }
        }

        [HarmonyPatch(typeof(PositionChallenge))]
        private static class PositionChallengePatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("FinishChallenge")]
            static void PreFinishChallenge(PositionChallenge __instance)
            {
                LogCheck("PositionChallenge","FinishChallenge",__instance.rewardAmount.ToString());
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
    }
}
