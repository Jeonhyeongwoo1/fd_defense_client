using UnityEngine;

namespace Game.View
{
    public class BaseView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        private Shader _originalShader;
        private Color _originalColor;
        private Vector3 _originalScale;
        private Coroutine _currentReactionCoroutine;

        private void Awake()
        {
            if (spriteRenderer != null)
            {
                _originalShader = spriteRenderer.material.shader;
                _originalColor = spriteRenderer.color;
            }

            _originalScale = transform.localScale;
        }

        public void PlayHitReaction()
        {
            if (_currentReactionCoroutine != null)
            {
                StopCoroutine(_currentReactionCoroutine);
                RestoreOriginalState();
            }

            _currentReactionCoroutine = StartCoroutine(HitReactionCoroutine());
        }

        private System.Collections.IEnumerator HitReactionCoroutine()
        {
            var flashShader = Shader.Find("GUI/Text Shader");

            if (flashShader != null && spriteRenderer != null)
            {
                spriteRenderer.material.shader = flashShader;
                spriteRenderer.color = Color.white;
            }

            yield return new WaitForSeconds(0.08f);

            if (spriteRenderer != null)
            {
                spriteRenderer.material.shader = _originalShader;
                spriteRenderer.color = _originalColor;
            }

            var targetScale = _originalScale * 1.12f;
            transform.localScale = targetScale;

            var elapsed = 0f;
            var duration = 0.15f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = elapsed / duration;
                transform.localScale = Vector3.Lerp(targetScale, _originalScale, t);
                yield return null;
            }

            transform.localScale = _originalScale;
            _currentReactionCoroutine = null;
        }

        private void RestoreOriginalState()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.material.shader = _originalShader;
                spriteRenderer.color = _originalColor;
            }

            transform.localScale = _originalScale;
        }
    }
}
