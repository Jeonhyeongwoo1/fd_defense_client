using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Game.Core;

namespace Game.Editor
{
    public static class UnitAnimationAssetBuilder
    {
        private const string PetSourceRoot = "Assets/Layer Lab/2D Characters-PetPack1/Sprites/ImageSequence";
        private const string BossSourceRoot = "Assets/Layer Lab/2D Minimal-BossMonster/Sprites/ImageSequence";
        private const string OutputRoot = "Assets/Game/03.Resources/Units";
        private const int FrameRate = 12;

        private static readonly string[] PetAnimations = { "Idle", "Walk", "Attack" };
        private static readonly string[] BossAnimations = { "Idle_12f", "Walk_12f", "Attack_12f", "Skill_12f", "Die_12f" };
        private static readonly HashSet<string> LoopingAnimations = new HashSet<string> { "Idle", "Walk" };

        public static void BuildAllUnitAssets()
        {
            var totalUnits = 0;
            var totalClips = 0;

            BuildPets(ref totalUnits, ref totalClips);
            BuildBosses(ref totalUnits, ref totalClips);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            GameLogger.Log($"[UnitAnimationAssetBuilder] Build complete: {totalUnits} units, {totalClips} animation clips.");
        }

        private static void BuildPets(ref int totalUnits, ref int totalClips)
        {
            if (!Directory.Exists(PetSourceRoot))
            {
                GameLogger.LogWarning($"[UnitAnimationAssetBuilder] Pet source root not found: {PetSourceRoot}");
                return;
            }

            var petFolders = Directory.GetDirectories(PetSourceRoot)
                .Select(Path.GetFileName)
                .Where(name => !string.IsNullOrEmpty(name))
                .ToArray();

            foreach (var petName in petFolders)
            {
                var sourceFolder = Path.Combine(PetSourceRoot, petName);
                var outputFolder = Path.Combine(OutputRoot, "Pets", petName);

                var animationDict = new Dictionary<string, Sprite[]>();

                foreach (var animName in PetAnimations)
                {
                    var animFolder = Path.Combine(sourceFolder, animName);
                    var sprites = LoadSortedSprites(animFolder, petName, animName, isPetPack: true);

                    if (sprites.Length == 0)
                    {
                        GameLogger.LogWarning($"[UnitAnimationAssetBuilder] No sprites found for {petName}/{animName}, skipping animation.");
                        continue;
                    }

                    animationDict[animName] = sprites;
                }

                if (!animationDict.ContainsKey("Idle"))
                {
                    GameLogger.LogWarning($"[UnitAnimationAssetBuilder] Pet {petName} has no Idle animation, skipping unit.");
                    continue;
                }

                var clipsCreated = BuildUnit(petName, animationDict, outputFolder);
                totalClips += clipsCreated;
                totalUnits++;

                GameLogger.Log($"[UnitAnimationAssetBuilder] Built pet: {petName} ({clipsCreated} clips)");
            }
        }

        private static void BuildBosses(ref int totalUnits, ref int totalClips)
        {
            if (!Directory.Exists(BossSourceRoot))
            {
                GameLogger.LogWarning($"[UnitAnimationAssetBuilder] Boss source root not found: {BossSourceRoot}");
                return;
            }

            var styles = new[] { ("1.0", "S1"), ("2.0", "S2") };
            var bossIds = new[] { "100", "101", "102", "103", "104" };

            foreach (var (styleFolder, styleCode) in styles)
            {
                var styleRoot = Path.Combine(BossSourceRoot, styleFolder);

                if (!Directory.Exists(styleRoot))
                {
                    GameLogger.LogWarning($"[UnitAnimationAssetBuilder] Boss style folder not found: {styleRoot}");
                    continue;
                }

                foreach (var bossId in bossIds)
                {
                    var sourceFolder = Path.Combine(styleRoot, bossId);

                    if (!Directory.Exists(sourceFolder))
                    {
                        GameLogger.LogWarning($"[UnitAnimationAssetBuilder] Boss folder not found: {sourceFolder}");
                        continue;
                    }

                    var unitName = $"Boss_{styleCode}_{bossId}";
                    var outputFolder = Path.Combine(OutputRoot, "Bosses", unitName);

                    var animationDict = new Dictionary<string, Sprite[]>();

                    foreach (var animWithSuffix in BossAnimations)
                    {
                        var animName = animWithSuffix.Replace("_12f", "");
                        // 스타일 1.0은 "Attack_12f", 2.0은 접미사 없는 "Attack" 폴더명을 사용한다
                        var animFolder = Path.Combine(sourceFolder, animWithSuffix);

                        if (!Directory.Exists(animFolder))
                        {
                            animFolder = Path.Combine(sourceFolder, animName);
                        }
                        var sprites = LoadSortedSprites(animFolder, bossId, animName, isPetPack: false);

                        if (sprites.Length == 0)
                        {
                            GameLogger.LogWarning($"[UnitAnimationAssetBuilder] No sprites found for {unitName}/{animName}, skipping animation.");
                            continue;
                        }

                        animationDict[animName] = sprites;
                    }

                    if (!animationDict.ContainsKey("Idle"))
                    {
                        GameLogger.LogWarning($"[UnitAnimationAssetBuilder] Boss {unitName} has no Idle animation, skipping unit.");
                        continue;
                    }

                    var clipsCreated = BuildUnit(unitName, animationDict, outputFolder);
                    totalClips += clipsCreated;
                    totalUnits++;

                    GameLogger.Log($"[UnitAnimationAssetBuilder] Built boss: {unitName} ({clipsCreated} clips)");
                }
            }
        }

        private static int BuildUnit(string unitName, Dictionary<string, Sprite[]> animationDict, string outputFolder)
        {
            EnsureFolder(outputFolder);

            var clipsCreated = 0;
            var createdClipList = new List<AnimationClip>();

            foreach (var kvp in animationDict)
            {
                var animName = kvp.Key;
                var sprites = kvp.Value;

                var clipPath = Path.Combine(outputFolder, $"{unitName}_{animName}.anim");
                var clip = CreateOrUpdateAnimationClip(clipPath, sprites, LoopingAnimations.Contains(animName));

                if (clip != null)
                {
                    createdClipList.Add(clip);
                    clipsCreated++;
                }
            }

            var controllerPath = Path.Combine(outputFolder, $"{unitName}.controller");
            var controller = CreateOrUpdateAnimatorController(controllerPath, createdClipList);

            var prefabPath = Path.Combine(outputFolder, $"{unitName}.prefab");
            CreateOrUpdatePrefab(prefabPath, unitName, animationDict["Idle"][0], controller);

            return clipsCreated;
        }

        // Pet sprites are non-zero-padded (_0, _1, ... _10), so string sort breaks. Parse and sort numerically.
        private static Sprite[] LoadSortedSprites(string folder, string prefix, string animName, bool isPetPack)
        {
            if (!Directory.Exists(folder))
            {
                return new Sprite[0];
            }

            var spriteList = new List<(int index, Sprite sprite)>();
            var pattern = new Regex(@"_(\d+)$");

            var pngFiles = Directory.GetFiles(folder, "*.png");

            foreach (var filePath in pngFiles)
            {
                var assetPath = filePath.Replace("\\", "/");
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

                if (sprite == null)
                {
                    GameLogger.LogWarning($"[UnitAnimationAssetBuilder] Failed to load sprite: {assetPath}");
                    continue;
                }

                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
                var match = pattern.Match(fileNameWithoutExt);

                if (!match.Success)
                {
                    GameLogger.LogWarning($"[UnitAnimationAssetBuilder] Could not parse frame index from: {fileNameWithoutExt}");
                    continue;
                }

                var index = int.Parse(match.Groups[1].Value);
                spriteList.Add((index, sprite));
            }

            return spriteList.OrderBy(x => x.index).Select(x => x.sprite).ToArray();
        }

        private static AnimationClip CreateOrUpdateAnimationClip(string clipPath, Sprite[] sprites, bool isLooping)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);

            if (clip == null)
            {
                clip = new AnimationClip();
                AssetDatabase.CreateAsset(clip, clipPath);
            }
            else
            {
                clip.ClearCurves();
            }

            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = isLooping;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            var keyframes = new ObjectReferenceKeyframe[sprites.Length];

            for (var i = 0; i < sprites.Length; i++)
            {
                keyframes[i] = new ObjectReferenceKeyframe
                {
                    time = i / (float)FrameRate,
                    value = sprites[i]
                };
            }

            var binding = new EditorCurveBinding
            {
                type = typeof(SpriteRenderer),
                path = "",
                propertyName = "m_Sprite"
            };

            AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);

            clip.frameRate = FrameRate;
            EditorUtility.SetDirty(clip);

            return clip;
        }

        private static AnimatorController CreateOrUpdateAnimatorController(string controllerPath, List<AnimationClip> clips)
        {
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);

            if (controller == null)
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            }
            else
            {
                var layers = controller.layers;

                if (layers.Length > 0)
                {
                    var stateMachine = layers[0].stateMachine;

                    foreach (var state in stateMachine.states)
                    {
                        stateMachine.RemoveState(state.state);
                    }
                }
            }

            var rootStateMachine = controller.layers[0].stateMachine;
            AnimatorState defaultState = null;

            foreach (var clip in clips)
            {
                var state = rootStateMachine.AddState(clip.name.Split('_').Last());
                state.motion = clip;

                if (clip.name.Contains("Idle"))
                {
                    defaultState = state;
                }
            }

            if (defaultState != null)
            {
                rootStateMachine.defaultState = defaultState;
            }

            EditorUtility.SetDirty(controller);
            return controller;
        }

        private static void CreateOrUpdatePrefab(string prefabPath, string unitName, Sprite defaultSprite, RuntimeAnimatorController controller)
        {
            var existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (existingPrefab != null)
            {
                var instance = PrefabUtility.LoadPrefabContents(prefabPath);

                var spriteRenderer = instance.GetComponent<SpriteRenderer>();

                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = defaultSprite;
                }

                var animator = instance.GetComponent<Animator>();

                if (animator != null)
                {
                    animator.runtimeAnimatorController = controller;
                }

                PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
                PrefabUtility.UnloadPrefabContents(instance);
            }
            else
            {
                var tempGO = new GameObject(unitName);
                var spriteRenderer = tempGO.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = defaultSprite;

                var animator = tempGO.AddComponent<Animator>();
                animator.runtimeAnimatorController = controller;

                PrefabUtility.SaveAsPrefabAsset(tempGO, prefabPath);
                Object.DestroyImmediate(tempGO);
            }
        }

        private static void EnsureFolder(string folderPath)
        {
            var parts = folderPath.Replace("\\", "/").Split('/');
            var current = parts[0];

            for (var i = 1; i < parts.Length; i++)
            {
                var next = current + "/" + parts[i];

                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }
        }
    }
}
