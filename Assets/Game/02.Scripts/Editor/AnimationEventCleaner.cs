using System.Collections.Generic;
using Game.Core;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public static class AnimationEventCleaner
    {
        public static void CleanEmptyAnimationEvents()
        {
            // Third-party asset modification: Layer Lab animations contain empty events.
            // Safe to clean as these are local assets that won't be re-imported.

            var clipGuids = AssetDatabase.FindAssets("t:AnimationClip", new[] { "Assets/Layer Lab" });
            var cleanedClipCount = 0;
            var cleanedEventCount = 0;

            foreach (var guid in clipGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);

                if (clip == null)
                {
                    continue;
                }

                var events = AnimationUtility.GetAnimationEvents(clip);
                var validEventList = new List<AnimationEvent>();
                var hasEmptyEvent = false;

                foreach (var evt in events)
                {
                    if (string.IsNullOrEmpty(evt.functionName))
                    {
                        hasEmptyEvent = true;
                        cleanedEventCount++;
                    }
                    else
                    {
                        validEventList.Add(evt);
                    }
                }

                if (hasEmptyEvent)
                {
                    AnimationUtility.SetAnimationEvents(clip, validEventList.ToArray());
                    EditorUtility.SetDirty(clip);
                    cleanedClipCount++;
                }
            }

            AssetDatabase.SaveAssets();

            GameLogger.Log($"[AnimationEventCleaner] Cleaned {cleanedEventCount} empty events from {cleanedClipCount} animation clips.");
        }
    }
}
