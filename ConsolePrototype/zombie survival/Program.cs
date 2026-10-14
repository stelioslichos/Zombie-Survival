using System;

namespace zombie_survival;

public class Program
{
    public static void Main()
    {
        GameUI UI = GameUI.Instance();

        UI.InitialiseGame();

        UI.Run();
    }
}