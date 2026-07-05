using System.Collections.Generic;
using System.Text;
using Game.Core;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Editor
{
    /// <summary>
    /// 빌드된 UI 프리팹을 전수 감사해, 뷰 필드에 배선되지 않았고
    /// 배선된 자손도 없는 "활성 데모 요소"(텍스트·이미지·버튼)를 리포트한다.
    /// 리포트 결과는 UIPrefabBuilder의 명시적 비활성화 목록에 반영한다.
    /// </summary>
    public static class UiAuditor
    {
        private static readonly string[] PrefabPaths =
        {
            "Assets/Game/03.Resources/UI/GameHud.prefab",
            "Assets/Game/03.Resources/UI/StageSelectScreen.prefab"
        };

        public static void AuditAllUiPrefabs()
        {
            foreach (var path in PrefabPaths)
            {
                AuditPrefab(path);
            }
        }

        private static void AuditPrefab(string path)
        {
            var root = PrefabUtility.LoadPrefabContents(path);

            if (root == null)
            {
                GameLogger.LogWarning($"[UiAuditor] Failed to load prefab: {path}");
                return;
            }

            var usedSet = CollectWiredObjects(root);
            var candidateList = new List<string>();

            foreach (var transform in root.GetComponentsInChildren<Transform>(false))
            {
                if (!transform.gameObject.activeInHierarchy)
                {
                    continue;
                }

                if (usedSet.Contains(transform))
                {
                    continue;
                }

                if (HasUsedDescendant(transform, usedSet))
                {
                    continue;
                }

                var descriptor = DescribeVisual(transform.gameObject);

                if (descriptor == null)
                {
                    continue;
                }

                candidateList.Add($"{GetPath(transform, root.transform)} :: {descriptor}");
            }

            var report = new StringBuilder();
            report.AppendLine($"[UiAuditor] {path} — unused active candidates: {candidateList.Count}");
            foreach (var candidate in candidateList)
            {
                report.AppendLine($"[UiAuditor]   {candidate}");
            }
            GameLogger.Log(report.ToString());

            PrefabUtility.UnloadPrefabContents(root);
        }

        private static HashSet<Transform> CollectWiredObjects(GameObject root)
        {
            var usedSet = new HashSet<Transform>();

            foreach (var component in root.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (component == null || component.GetType().Namespace == null)
                {
                    continue;
                }

                if (!component.GetType().Namespace.StartsWith("Game"))
                {
                    continue;
                }

                MarkWithAncestors(component.transform, root.transform, usedSet);

                var serializedObject = new SerializedObject(component);
                var property = serializedObject.GetIterator();

                while (property.NextVisible(true))
                {
                    if (property.propertyType != SerializedPropertyType.ObjectReference || property.objectReferenceValue == null)
                    {
                        continue;
                    }

                    Transform referencedTransform = null;

                    if (property.objectReferenceValue is GameObject referencedObject)
                    {
                        referencedTransform = referencedObject.transform;
                    }
                    else if (property.objectReferenceValue is Component referencedComponent)
                    {
                        referencedTransform = referencedComponent.transform;
                    }

                    if (referencedTransform != null && referencedTransform.IsChildOf(root.transform))
                    {
                        MarkWithAncestors(referencedTransform, root.transform, usedSet);

                        // 배선된 요소의 자식(버튼 라벨·아이콘 등)은 사용 중으로 간주
                        foreach (var child in referencedTransform.GetComponentsInChildren<Transform>(true))
                        {
                            usedSet.Add(child);
                        }
                    }
                }
            }

            return usedSet;
        }

        private static void MarkWithAncestors(Transform target, Transform root, HashSet<Transform> usedSet)
        {
            var current = target;

            while (current != null && current != root)
            {
                usedSet.Add(current);
                current = current.parent;
            }

            usedSet.Add(root);
        }

        private static bool HasUsedDescendant(Transform transform, HashSet<Transform> usedSet)
        {
            foreach (var child in transform.GetComponentsInChildren<Transform>(true))
            {
                if (child != transform && usedSet.Contains(child))
                {
                    return true;
                }
            }
            return false;
        }

        private static string DescribeVisual(GameObject target)
        {
            var text = target.GetComponent<TMP_Text>();
            if (text != null)
            {
                return $"Text='{text.text}'";
            }

            if (target.GetComponent<Button>() != null)
            {
                return "Button";
            }

            var image = target.GetComponent<Image>();
            if (image != null && image.sprite != null)
            {
                return $"Image({image.sprite.name})";
            }

            return null;
        }

        private static string GetPath(Transform target, Transform root)
        {
            var pathStack = new Stack<string>();
            var current = target;

            while (current != null && current != root)
            {
                pathStack.Push(current.name);
                current = current.parent;
            }

            return string.Join("/", pathStack);
        }
    }
}
