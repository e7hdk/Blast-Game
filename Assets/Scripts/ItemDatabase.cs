using DefaultNamespace;
using UnityEngine;

public static class ItemDatabase
{
    public static GamePiece[] Items { get; private set; }
    

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {

        Items = Resources.LoadAll<GamePiece>("Items/");
        
    }
    

}