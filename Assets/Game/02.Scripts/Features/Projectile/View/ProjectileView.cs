using UnityEngine;

namespace Game.View
{
    public class ProjectileView : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;

        public void Initialize(Sprite sprite, bool isFacingRight)
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }

            _spriteRenderer.sprite = sprite;
            transform.localScale = new Vector3(isFacingRight ? 1f : -1f, 1f, 1f);
        }
    }
}
