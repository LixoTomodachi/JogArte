using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    // Nome da cena do jogo a ser carregada ao iniciar
    public string gameSceneName = "GameScene";

    // Referências aos painéis do menu (arrastar no Inspector)
    public GameObject mainMenuPanel;
    public GameObject creditsPanel;

    void Start()
    {
        ShowMainMenu();
    }

    void Update()
    {
        // Compatibilidade com Input Manager (antigo): "Submit" e "Cancel"
        // Submit -> confirma/ativa (ex.: tecla Enter, botão de UI mapeado)
        if (Input.GetButtonDown("Submit"))
        {
            if (mainMenuPanel != null && mainMenuPanel.activeSelf)
            {
                Jogar();
            }
        }

        // Cancel ou Back/Escape -> volta ou sai
        if (Input.GetButtonDown("Cancel") || Input.GetKeyDown(KeyCode.Escape))
        {
            if (creditsPanel != null && creditsPanel.activeSelf)
            {
                ShowMainMenu();
            }
            else if (mainMenuPanel != null && mainMenuPanel.activeSelf)
            {
                QuitGame();
            }
        }
    }

    // Método equivalente ao botão "Jogar" (conectar Jogar -> MainMenu.Jogar())
    public void Jogar()
    {
        StartGame();
    }

    public void StartGame()
    {
        if (string.IsNullOrEmpty(gameSceneName))
        {
            Debug.LogWarning("Nome da cena de jogo não definido.");
            return;
        }
        SceneManager.LoadScene(gameSceneName);
    }

    // Método para o botão "Continuar"
    public void Continuar()
    {
        if (PlayerPrefs.HasKey("SavedScene"))
        {
            string saved = PlayerPrefs.GetString("SavedScene");
            if (!string.IsNullOrEmpty(saved))
            {
                SceneManager.LoadScene(saved);
                return;
            }
        }
        Debug.Log("Nenhum progresso salvo. Iniciando novo jogo...");
        StartGame();
    }

    public void ShowMainMenu()
    {
        SetPanels(true, false);
    }

    public void ShowCredits()
    {
        SetPanels(false, true);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Salva a cena atual para o recurso "Continuar"
    public void SaveCurrentScene()
    {
        var scene = SceneManager.GetActiveScene();
        if (scene != null && !string.IsNullOrEmpty(scene.name))
        {
            PlayerPrefs.SetString("SavedScene", scene.name);
            PlayerPrefs.Save();
            Debug.Log($"Cena '{scene.name}' salva para Continuar.");
        }
        else
        {
            Debug.LogWarning("Não foi possível salvar a cena atual.");
        }
    }

    void SetPanels(bool main, bool credits)
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(main);
        if (creditsPanel != null) creditsPanel.SetActive(credits);
    }
}
