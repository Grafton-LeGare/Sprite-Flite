using UnityEngine;
using UnityEditor;
using System.Reflection;

public static class GameViewSize
{
    [MenuItem("Tools/Print Game View Size")]
    static void Print()
    {
        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        MethodInfo m = T.GetMethod("GetSizeOfMainGameView", BindingFlags.NonPublic | BindingFlags.Static);
        Vector2 size = (Vector2)m.Invoke(null, null);
        Debug.Log($"Game View: {size.x} x {size.y}");
    }
}
