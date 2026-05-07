using System;
using UnityEngine;

public static class GlobalEvents
{
    public static Action OnStartGame { get; set; }
    public static Action OnPauseGame { get; set; }
    public static Action OnResumeGame { get; set; }
    public static Action OnReturnToMenu { get; set; }
    public static Action<int> OnLoadLevel { get; set; }
    public static Action<Character> OnEndGame { get; set; }
}
