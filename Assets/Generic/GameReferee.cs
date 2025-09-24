using UnityEngine;

public enum GameState {
    MAINMENU,
    GAME,
    PAUSE,
    LOADING
}

public class GameReferee : MonoBehaviour
{
    public static GameReferee Instance { get; private set; }

    [SerializeField] protected GameState currentGameState;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ChangeGameState(GameState state) {
        currentGameState = state;

        switch (currentGameState) {
            case GameState.MAINMENU:
                break; 
            case GameState.GAME:
                break;
            case GameState.PAUSE: 
                break;
            case GameState.LOADING: 
                break;
        }
    }
}
