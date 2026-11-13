
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Painel de Pausa")]
    [SerializeField] private GameObject painelPausa;

    [Header("Personagem")]
    [SerializeField] private Transform personagem;
    [SerializeField] private Rigidbody2D rbPersonagem;

    [Header("Configurações")]
    [SerializeField] private string cenaMenuPrincipal = "MainMenuBackup";

    private void Awake()
    {
        Time.timeScale = 1f;

        if (painelPausa != null)
            painelPausa.SetActive(false);

        if (personagem != null && rbPersonagem == null)
            rbPersonagem = personagem.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        RestaurarJogoSalvo();
    }

    // Abre o painel e pausa o jogo.
    public void PauseGame()
    {
        if (painelPausa == null)
        {
            Debug.LogError("Painel de pausa não foi atribuído!");
            return;
        }

        painelPausa.SetActive(true);
        Time.timeScale = 0f;
    }

    // Fecha o painel e retoma o jogo.
    public void ResumeGame()
    {
        if (painelPausa != null)
            painelPausa.SetActive(false);

        Time.timeScale = 1f;
    }

    // Salva a cena, a posição e a velocidade do personagem.
    public void SaveGame()
    {
        if (personagem == null)
        {
            Debug.LogError("Personagem não foi atribuído no PauseMenu!");
            return;
        }

        Vector2 velocidade = Vector2.zero;

        if (rbPersonagem != null)
            velocidade = rbPersonagem.linearVelocity;

        GameSaveSystem.Save(
            SceneManager.GetActiveScene().name,
            personagem.position,
            velocidade
        );

        Debug.Log("Jogo salvo com sucesso!");
    }

    // Retorna ao menu principal sem apagar o salvamento.
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(cenaMenuPrincipal);
    }

    // Restaura a posição quando o jogo é carregado pelo botão Continuar.
    private void RestaurarJogoSalvo()
    {
        if (!GameSaveSystem.ConsumePendingRestore())
            return;

        GameSaveData dados = GameSaveSystem.Load();

        if (dados == null)
        {
            Debug.LogWarning("Não foi possível encontrar os dados salvos.");
            return;
        }

        if (SceneManager.GetActiveScene().name != dados.sceneName)
        {
            Debug.LogWarning("A cena carregada não corresponde à cena salva.");
            return;
        }

        if (personagem == null)
        {
            Debug.LogError("Personagem não foi atribuído no PauseMenu!");
            return;
        }

        personagem.position = dados.playerPosition;

        if (rbPersonagem != null)
        {
            rbPersonagem.position = new Vector2(
                dados.playerPosition.x,
                dados.playerPosition.y
            );

            rbPersonagem.linearVelocity = dados.playerVelocity;
        }

        Debug.Log("Posição do personagem restaurada!");
    }
}


// Estrutura dos dados armazenados.
[System.Serializable]
public class GameSaveData
{
    public string sceneName;
    public Vector3 playerPosition;
    public Vector2 playerVelocity;
}


// Sistema responsável por guardar e recuperar o salvamento.
public static class GameSaveSystem
{
    private const string SaveKey = "GAME_SAVE_DATA";
    private const string PendingRestoreKey = "PENDING_GAME_RESTORE";

    public static void Save(
        string sceneName,
        Vector3 playerPosition,
        Vector2 playerVelocity)
    {
        GameSaveData dados = new GameSaveData
        {
            sceneName = sceneName,
            playerPosition = playerPosition,
            playerVelocity = playerVelocity
        };

        string json = JsonUtility.ToJson(dados);

        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public static GameSaveData Load()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
            return null;

        string json = PlayerPrefs.GetString(SaveKey);

        if (string.IsNullOrEmpty(json))
            return null;

        return JsonUtility.FromJson<GameSaveData>(json);
    }

    // Será chamado pelo botão Continuar do menu principal.
    public static bool ContinueGame()
    {
        GameSaveData dados = Load();

        if (dados == null || string.IsNullOrEmpty(dados.sceneName))
            return false;

        PlayerPrefs.SetInt(PendingRestoreKey, 1);
        PlayerPrefs.Save();

        Time.timeScale = 1f;
        SceneManager.LoadScene(dados.sceneName);

        return true;
    }

    // Informa se a cena foi aberta para recuperar um salvamento.
    public static bool ConsumePendingRestore()
    {
        bool pendente = PlayerPrefs.GetInt(PendingRestoreKey, 0) == 1;

        if (pendente)
        {
            PlayerPrefs.SetInt(PendingRestoreKey, 0);
            PlayerPrefs.Save();
        }

        return pendente;
    }
}
