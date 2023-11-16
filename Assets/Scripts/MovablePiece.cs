using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DefaultNamespace
{
    public class MovablePiece: MonoBehaviour
    {
        private GamePiece piece;
        private IEnumerator moveCoroutine;
        public const float TweenDuration = 0.25f;
        //private DOTween seq;
        private void Awake()
        {
            piece = GetComponent<GamePiece>();
        }

        public void Move(int newX, int newY, float time)
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                
            }

            
            moveCoroutine = MoveCoroutine(newX, newY, time);
            StartCoroutine(moveCoroutine);
        }

        private IEnumerator MoveCoroutine(int newX, int newY, float time)
        {
            piece.X = newX;
            piece.Y = newY;
            Vector3 startPos = transform.position;
            Vector3 endPos = piece.GridRef.GetWorldPosition(newX, newY);
            //piece.transform.DOMove(endPos, TweenDuration);
            
            for (float i = 0; i <= time; i += Time.deltaTime)
            {
                piece.transform.position = Vector3.Lerp(startPos, endPos, i / time);
                yield return 0;
            }

            piece.transform.position = endPos;
            
        }
    }
}