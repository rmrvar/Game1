using System;
using System.Collections;
using System.Collections.Generic;
using Game.Common;
using Game.RoomSystem;
using UnityEngine;
using Util;

namespace Game.Animation
{
    public class AnimationManager : MonoSingleton<AnimationManager>
    {
        public int RunningAttackAnimationCount { get; private set; }
        public int RunningMoveAnimationCount { get; private set; }
        
        public void PlayAttackAnimation(
            Transform animatorPrefab,
            Vector2Int position,
            Action onAnimationFinished
          )
        {
            if (!_animatorPools.TryGetValue(animatorPrefab, out var pool))
            {
                _animatorPools[animatorPrefab] = pool = new Stack<Transform>();
            }
            if (!pool.TryPop(out var animatorObject))
            {
                animatorObject = Instantiate(animatorPrefab);
            }
            animatorObject.transform.position = new Vector3(position.x, position.y, -1);
            animatorObject.gameObject.SetActive(true);
            var animator = animatorObject.GetComponentInChildren<Animator>();
            StartCoroutine(IE_PlayAttackAnimation(animator, () =>
            {
                animatorObject.gameObject.SetActive(false);
                _animatorPools[animatorPrefab].Push(animatorObject);
                onAnimationFinished?.Invoke();
            }));
        }

        private IEnumerator IE_PlayAttackAnimation(Animator animator, Action onAnimationFinished)
        {
            ++RunningAttackAnimationCount;
            animator.SetTrigger("Attack");

            yield return null;
            // yield return new WaitUntil(() =>
            //     animator.GetCurrentAnimatorStateInfo(0).IsName("IsAttacking"));

            yield return new WaitUntil(() =>
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99F);
            
            onAnimationFinished?.Invoke();
            --RunningAttackAnimationCount;
        }

        public void PlayMoveAnimation(
            RoomEntity roomEntity,
            EDirection direction,
            float duration,
            Action<float> onCompleted
          )
        {
            StartCoroutine(IE_PlayMoveAnimation(roomEntity, direction, duration, onCompleted));
        }

        private IEnumerator IE_PlayMoveAnimation(
            RoomEntity roomEntity,
            EDirection direction,
            float duration,
            Action<float> onCompleted
          )
        {
            ++RunningMoveAnimationCount;
            yield return new WaitUntil(() => RunningAttackAnimationCount <= 0);
            roomEntity.Move(
                direction,
                duration,
                onCompleted
              );
            --RunningMoveAnimationCount;
        }
        
        private readonly Dictionary<Transform, Stack<Transform>> _animatorPools = new();
    }
}