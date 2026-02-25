using System;
using UnityEngine;

public enum GameMode
{
    Intro,
    Tour,
    FreeRoam
}

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance { get; private set; }

    public GameMode CurrentMode { get; private set; } = GameMode.Intro;

    public event Action<GameMode> OnModeChanged;

    [Header("References")]
    public IntroCard introCard;
    public TourManager tourManager;
    public FreeRoamMenu freeRoamMenu;
    public VRMenu vrMenu;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SetMode(GameMode.Intro);
    }

    public void SetMode(GameMode newMode)
    {
        // One-way progression: Tour can never return
        if (CurrentMode == GameMode.FreeRoam && newMode == GameMode.Tour)
        {
            Debug.LogWarning("Cannot switch back to Tour mode from Free Roam.");
            return;
        }

        CurrentMode = newMode;
        Debug.Log($"[GameModeManager] Mode changed to: {newMode}");

        // Disable all mode-specific systems first
        SetSystemActive(introCard, false);
        SetSystemActive(tourManager, false);
        SetFreeRoamActive(false);

        switch (newMode)
        {
            case GameMode.Intro:
                SetSystemActive(introCard, true);
                break;

            case GameMode.Tour:
                SetSystemActive(tourManager, true);
                break;

            case GameMode.FreeRoam:
                SetFreeRoamActive(true);
                break;
        }

        OnModeChanged?.Invoke(newMode);
    }

    private void SetSystemActive(MonoBehaviour system, bool active)
    {
        if (system != null)
        {
            system.gameObject.SetActive(active);
        }
    }

    private void SetFreeRoamActive(bool active)
    {
        if (freeRoamMenu != null)
        {
            freeRoamMenu.gameObject.SetActive(active);
        }
        if (vrMenu != null)
        {
            vrMenu.gameObject.SetActive(active);
        }
    }
}
