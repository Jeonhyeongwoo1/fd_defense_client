using Game.Core;
using Game.Model;
using UnityEngine;

namespace Game.View
{
    public class UnitView : MonoBehaviour
    {
        private static readonly int WalkHash = Animator.StringToHash("Walk");
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int IdleHash = Animator.StringToHash("Idle");
        private static readonly int DeadHash = Animator.StringToHash("Dead");

        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private int _currentStateHash;

        public void Initialize(UnitSide side)
        {
            _animator = GetComponentInChildren<Animator>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (_animator == null)
            {
                GameLogger.LogWarning($"[UnitView] Animator not found on {gameObject.name}");
            }

            if (_spriteRenderer == null)
            {
                GameLogger.LogWarning($"[UnitView] SpriteRenderer not found on {gameObject.name}");
            }

            _currentStateHash = 0; // Reset state hash for pool reuse — prevents stale state from skipping PlayAnimation
            SetFacing(side == UnitSide.Ally);
        }

        public void PlayWalk()
        {
            PlayAnimation(WalkHash);
        }

        public void PlayAttack()
        {
            PlayAnimation(AttackHash);
        }

        public void PlayIdle()
        {
            PlayAnimation(IdleHash);
        }

        public void PlayDead()
        {
            PlayAnimation(DeadHash);
        }

        private void SetFacing(bool isFacingRight)
        {
            if (_spriteRenderer == null)
            {
                return;
            }

            _spriteRenderer.flipX = !isFacingRight;
        }

        private void PlayAnimation(int stateHash)
        {
            if (_animator == null)
            {
                return;
            }

            if (_currentStateHash == stateHash)
            {
                return;
            }

            if (!_animator.HasState(0, stateHash))
            {
                return;
            }

            _animator.Play(stateHash);
            _currentStateHash = stateHash;
        }
    }
}
