using UnityEngine;

public class GameManager : PersistantSingleton<GameManager>
{   
    /// <summary>
    /// Unpauses the game, sets the time scale to 0
    /// </summary>
    public void FreezeTime()
    {
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Unpauses the game, sets the time scale to 1
    /// </summary>
    public void UnfreezeTime()
    {
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Exits game by quitting the application 
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
}
