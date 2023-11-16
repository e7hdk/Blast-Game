using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using DG.Tweening;

public class ClearablePiece : MonoBehaviour
{
    // Start is called before the first frame update

    protected GamePiece piece;
    private bool isBeingCleared = false;
    public const float TweenDuration = 0.25f;

    public bool IsBeingCleared
    {
        get { return isBeingCleared; }
    }
    void Awake()
    {
        piece = GetComponent<GamePiece>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Clear()
    {
        
        isBeingCleared = true;
        piece.transform.DOScale(Vector3.zero, TweenDuration);
        Destroy(piece.gameObject);
    }
}
