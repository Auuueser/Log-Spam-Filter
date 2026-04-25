using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace LogSpamFilter
{
    [BepInPlugin("yourname.logspamfilter", "Log Spam Filter", "1.2.7")]
    public class Plugin : BaseUnityPlugin
    {
        private const string PluginVersion = "1.2.7";
        internal static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;

            try
            {
                BepInExListenerFilter.Install();
                Harmony.CreateAndPatchAll(typeof(DebugLogPatches), "yourname.logspamfilter");
                Logger.LogInfo($"Log Spam Filter loaded. Version: {PluginVersion}");
                Logger.LogInfo("Changelog: Expanded spam suppression for Stingray, HoarderBug, Bracken, Cadaver, MouthDog, OpenBodyCams, rope, targeting, goUp, spawn planner, BoxCollider, and audio spatializer spam.");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to patch logging: {ex}");
            }
        }
    }

    internal static class BepInExListenerFilter
    {
        private static bool _installed;

        public static void Install()
        {
            if (_installed)
                return;

            List<ILogListener> listeners = BepInEx.Logging.Logger.Listeners.ToList();
            if (listeners.Count == 0)
            {
                _installed = true;
                return;
            }

            BepInEx.Logging.Logger.Listeners.Clear();
            foreach (ILogListener listener in listeners)
            {
                BepInEx.Logging.Logger.Listeners.Add(new FilteringLogListener(listener));
            }

            _installed = true;
        }

        private sealed class FilteringLogListener : ILogListener
        {
            private readonly ILogListener _inner;

            public FilteringLogListener(ILogListener inner)
            {
                _inner = inner;
            }

            public void LogEvent(object sender, LogEventArgs eventArgs)
            {
                if (eventArgs != null && SpamRules.ShouldBlock(eventArgs.Data))
                    return;

                _inner.LogEvent(sender, eventArgs);
            }

            public void Dispose()
            {
                _inner.Dispose();
            }
        }
    }

    internal static class SpamRules
    {
        private static readonly string[] StartsWithRules =
        {
            // 旧刷屏
            "name 1 float:",
            "Start game A",
            "Start game B",
            "Start game C",
            "Start game D",
            "Start game E",
            "Start game successful",
            "Stop special animation F",
            "Stop special animation G",
            "LOADING GAME!!!!!",
            "Waiting for all players to load!",
            "DropAllHeldItems called on player",

            // Puma / V80 hostile entity
            "Puma: ",
            "PUMA AI ",
            "Got target player?:",
            "Tree state:",
            "Starting puma climb",
            "Set puma climbing up!",
            "Set puma tree mode to idle;",
            "Getting end target tree; hiding meter :",
            "Get end target tree : Player #",
            "puma: Trees nearby player #",
            "tree #",
            "Puma F 1",
            "Puma F 2",
            "Puma F 3",
            "Puma : Got EndTargetTree!!!",
            "Got target tree: ",
            "Got nav pos:",
            "Got dest ",
            "No target tree found!",
            "GetTargetTree ",
            "Puma dist to closest player;",
            "First node gotten:",
            "Checking node #",
            "Stalking nodes in use contains node #",
            "Path is intersected by line of sight",
            "Position distance from ",
            "mostOptimalDistance:",
            "Puma leaped; leap attempts:",
            "Puma leap; target tree:",
            "Num of trees in physics check with range ",
            "Adding tree '",
            "i:",
            "Puma: Unable to leap to tree ",
            "Set target tree to '",
            "Puma: Unable to relocate",
            "Puma Relocating! Set target tree to '",
            "Puma dropped from tree! Target tree:",
            "Attempted to drop but unable. Adding tree '",
            "Attempt drop. Relocate.",
            "Received tree climb RPC ",
            "Tree width:",
            "Dist:",
            "treeBottomPosition magnitude:",
            "Distance from startingPos to Target tree:",
            "Puma error: destination is far away from target tree.",
            "Puma: Tree has no capsule collider.",
            "Puma: Tree is too short off the ground.",
            "Puma: Attempted to remove tree '",
            "Checking Tree #",
            "Tree #",
            "Set GetTargetTree: Got target tree?:",
            "Stalking frozen.",
            "Hearing noise from ",
            "Send Stalk frozen RPC on local client!",
            "player knowledge ",
            "The puma is on its target tree!",
            "On end target tree:",

            // Stingray
            "Time since hitting < 8!",
            "Whining audio playing?:",
            "Whining audio playing? B:",
            "slipperyFloor:",
            "push force magnitude:",
            "cloak speed lunge:",
            "Start whining audio!",

            // Bracken / Flowerman
            "Add to anger meter called! amount to add:",
            "Increasing agent speed by ",

            // Cadaver / Baby bird
            "Baby bird distance to nest :",
            "Counting scream timer; timer:",

            // Misc creature debug
            "Setting position of ropes",
            "Targetable A",
            "goUp: ",

            // MouthDog / truck collision debug
            "dog 'MouthDog(Clone)': Heard noise! Distance:",
            "Mouth dog targetPos 1:",
            "Mouth dog targetPos 2:",
            "Dog lastheardnoisePosition:",
            "Truck got collision from enemy; MouthDogModel",
            "Truck collision: is enemyCollisoinScript null? :",
            "Truck collision:",

            // Vanilla / spawn planner spam
            "HoarderBug(Clone): Setting target object and going towards it.",
            "hour: ",
            "Got enemy that will spawn for hour #",
            "Setting prob to 0; ",
            "Round manager: No more spawnable outside enemies.",
            "enemy rush index is ",
            "Hours: ",
            "Probability: ",
            "ADDING ENEMY #",
            "Adding 1 to power level, enemy: ",
            "Adding 1 to diversity level for enemy '",

            // Unity warning spam
            "BoxCollider does not support negative scale or size.",
            "The effective box size has been forced positive and is likely to give unexpected collision geometry.",
            "If you absolutely need to use negative scaling you can use the convex MeshCollider. Scene hierarchy path ",
            "Audio source failed to initialize audio spatializer.",
            "BigDoor(Clone) creating doorcode object in parent MapScreenUIWorldSpace",
            "Roundmanager: Refreshed OutsideAINodes list!: ",
            "No recording devices found"
        };

        private static readonly string[] ExactRules =
        {
            // 单字符调试日志，只有完全一致时才拦截
            "D",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "T",
            "True, False"
        };

        private static readonly string[] ContainsRules =
        {
            // 更宽松的兜底
            "puma climb",
            "puma tree mode",
            "TargetTree",
            "EndTargetTree",
            "Puma dropped from tree",
            "Puma Relocating",
            "GetTargetTree",
            "ChooseClosestNodeToPositionPuma",
            "PathIsIntersectedByLineOfSightPuma",

            // 已补充
            "Stalking frozen.",
            "seenByPumaThroughTrees:",
            "hidingQuickly:",
            "Hearing noise from ",
            "Send Stalk frozen RPC on local client!",
            "Attempt drop. Relocate.",
            "is seen by puma",
            "The puma is on its target tree!",
            "On end target tree:",
            "third-person and 0 cosmetics for ",
            "without a viewmodel replacement in ",
            "hit enemy MouthDog(Clone) with force of "
        };

        public static bool ShouldBlock(object message)
        {
            if (message == null)
                return false;

            string text = message.ToString();
            if (string.IsNullOrWhiteSpace(text))
                return false;

            return ExactRules.Any(exact => string.Equals(text, exact, StringComparison.Ordinal))
                || StartsWithRules.Any(prefix => text.StartsWith(prefix, StringComparison.Ordinal))
                || ContainsRules.Any(part => text.IndexOf(part, StringComparison.Ordinal) >= 0);
        }

        public static bool ShouldBlockFormatted(string format, object[] args)
        {
            if (string.IsNullOrEmpty(format))
                return false;

            string rendered;
            try
            {
                rendered = (args != null && args.Length > 0) ? string.Format(format, args) : format;
            }
            catch
            {
                rendered = format;
            }

            return ShouldBlock(rendered);
        }
    }

    internal static class DebugLogPatches
    {
        [HarmonyPatch(typeof(Debug), nameof(Debug.Log), new Type[] { typeof(object) })]
        [HarmonyPrefix]
        private static bool DebugLog_Object_Prefix(object message)
        {
            return !SpamRules.ShouldBlock(message);
        }

        [HarmonyPatch(typeof(Debug), nameof(Debug.Log), new Type[] { typeof(object), typeof(UnityEngine.Object) })]
        [HarmonyPrefix]
        private static bool DebugLog_ObjectContext_Prefix(object message, UnityEngine.Object context)
        {
            return !SpamRules.ShouldBlock(message);
        }

        [HarmonyPatch(typeof(Debug), nameof(Debug.LogFormat), new Type[] { typeof(string), typeof(object[]) })]
        [HarmonyPrefix]
        private static bool DebugLogFormat_Prefix(string format, object[] args)
        {
            return !SpamRules.ShouldBlockFormatted(format, args);
        }

        [HarmonyPatch(typeof(Debug), nameof(Debug.LogFormat), new Type[] { typeof(UnityEngine.Object), typeof(string), typeof(object[]) })]
        [HarmonyPrefix]
        private static bool DebugLogFormat_Context_Prefix(UnityEngine.Object context, string format, object[] args)
        {
            return !SpamRules.ShouldBlockFormatted(format, args);
        }

        [HarmonyPatch(typeof(Debug), nameof(Debug.LogWarning), new Type[] { typeof(object) })]
        [HarmonyPrefix]
        private static bool DebugLogWarning_Object_Prefix(object message)
        {
            return !SpamRules.ShouldBlock(message);
        }

        [HarmonyPatch(typeof(Debug), nameof(Debug.LogWarning), new Type[] { typeof(object), typeof(UnityEngine.Object) })]
        [HarmonyPrefix]
        private static bool DebugLogWarning_ObjectContext_Prefix(object message, UnityEngine.Object context)
        {
            return !SpamRules.ShouldBlock(message);
        }

        [HarmonyPatch(typeof(Debug), nameof(Debug.LogError), new Type[] { typeof(object) })]
        [HarmonyPrefix]
        private static bool DebugLogError_Object_Prefix(object message)
        {
            return !SpamRules.ShouldBlock(message);
        }

        [HarmonyPatch(typeof(Debug), nameof(Debug.LogError), new Type[] { typeof(object), typeof(UnityEngine.Object) })]
        [HarmonyPrefix]
        private static bool DebugLogError_ObjectContext_Prefix(object message, UnityEngine.Object context)
        {
            return !SpamRules.ShouldBlock(message);
        }

        [HarmonyPatch(typeof(UnityEngine.Logger), nameof(UnityEngine.Logger.Log), new Type[] { typeof(LogType), typeof(object) })]
        [HarmonyPrefix]
        private static bool LoggerLog_LogTypeObject_Prefix(LogType logType, object message)
        {
            return !SpamRules.ShouldBlock(message);
        }

        [HarmonyPatch(typeof(UnityEngine.Logger), nameof(UnityEngine.Logger.Log), new Type[] { typeof(LogType), typeof(object), typeof(UnityEngine.Object) })]
        [HarmonyPrefix]
        private static bool LoggerLog_LogTypeObjectContext_Prefix(LogType logType, object message, UnityEngine.Object context)
        {
            return !SpamRules.ShouldBlock(message);
        }

        [HarmonyPatch(typeof(UnityEngine.Logger), nameof(UnityEngine.Logger.Log), new Type[] { typeof(object) })]
        [HarmonyPrefix]
        private static bool LoggerLog_Object_Prefix(object message)
        {
            return !SpamRules.ShouldBlock(message);
        }

        [HarmonyPatch(typeof(UnityEngine.Logger), nameof(UnityEngine.Logger.Log), new Type[] { typeof(string), typeof(object) })]
        [HarmonyPrefix]
        private static bool LoggerLog_TagObject_Prefix(string tag, object message)
        {
            return !SpamRules.ShouldBlock(message);
        }

        [HarmonyPatch(typeof(UnityEngine.Logger), nameof(UnityEngine.Logger.Log), new Type[] { typeof(string), typeof(object), typeof(UnityEngine.Object) })]
        [HarmonyPrefix]
        private static bool LoggerLog_TagObjectContext_Prefix(string tag, object message, UnityEngine.Object context)
        {
            return !SpamRules.ShouldBlock(message);
        }

        [HarmonyPatch(typeof(UnityEngine.Logger), nameof(UnityEngine.Logger.LogFormat), new Type[] { typeof(LogType), typeof(string), typeof(object[]) })]
        [HarmonyPrefix]
        private static bool LoggerLogFormat_LogType_Prefix(LogType logType, string format, object[] args)
        {
            return !SpamRules.ShouldBlockFormatted(format, args);
        }

        [HarmonyPatch(typeof(UnityEngine.Logger), nameof(UnityEngine.Logger.LogFormat), new Type[] { typeof(LogType), typeof(UnityEngine.Object), typeof(string), typeof(object[]) })]
        [HarmonyPrefix]
        private static bool LoggerLogFormat_LogTypeContext_Prefix(LogType logType, UnityEngine.Object context, string format, object[] args)
        {
            return !SpamRules.ShouldBlockFormatted(format, args);
        }

        [HarmonyPatch(typeof(UnityEngine.Logger), nameof(UnityEngine.Logger.LogWarning), new Type[] { typeof(string), typeof(object) })]
        [HarmonyPrefix]
        private static bool LoggerLogWarning_TagObject_Prefix(string tag, object message)
        {
            return !SpamRules.ShouldBlock(message);
        }

        [HarmonyPatch(typeof(UnityEngine.Logger), nameof(UnityEngine.Logger.LogWarning), new Type[] { typeof(string), typeof(object), typeof(UnityEngine.Object) })]
        [HarmonyPrefix]
        private static bool LoggerLogWarning_TagObjectContext_Prefix(string tag, object message, UnityEngine.Object context)
        {
            return !SpamRules.ShouldBlock(message);
        }

        [HarmonyPatch(typeof(UnityEngine.Logger), nameof(UnityEngine.Logger.LogError), new Type[] { typeof(string), typeof(object) })]
        [HarmonyPrefix]
        private static bool LoggerLogError_TagObject_Prefix(string tag, object message)
        {
            return !SpamRules.ShouldBlock(message);
        }

        [HarmonyPatch(typeof(UnityEngine.Logger), nameof(UnityEngine.Logger.LogError), new Type[] { typeof(string), typeof(object), typeof(UnityEngine.Object) })]
        [HarmonyPrefix]
        private static bool LoggerLogError_TagObjectContext_Prefix(string tag, object message, UnityEngine.Object context)
        {
            return !SpamRules.ShouldBlock(message);
        }
    }
}
