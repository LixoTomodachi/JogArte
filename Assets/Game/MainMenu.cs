using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    // Nome da cena do jogo a ser carregada ao iniciar
    public string gameSceneName = "Cena1";

    // Referências aos painéis do menu (arrastar no Inspector)
    public GameObject mainMenuPanel;
    public GameObject creditsPanel;

    // Referências para créditos (arrastar no Inspector)
    public Text creditsText;
    public ScrollRect creditsScrollRect;
    public float autoScrollSpeed = 20f;

    void Start()
    {
        ShowMainMenu();
        SetupCredits();
    }

    void SetupCredits()
    {
        // Monta o texto de créditos
        string[] credits = new string[]
        {
            "JOGARTE - Expedição em Sergipe",
            "",
            "Desenvolvido por:",
            "João Victor Modesto dos Santos",
            "",
            "Motor:",
            "Unity Engine",
            "",
            "Assets visuais (game designer):",
            "João Victor Modesto dos Santos",
            "",
            "Música e Efeitos Sonoros:",
            "'Bus Ride' e 'bus arrival', por freesound_community",
            "'Retro Arcade Game Music', por Monume",
            "'8-Bit Gaming Background Music', por MaksymMalko",
            "So Happy With My 8-Bit Game, por DJARTMUSIC",
            "",
            "Sons variados de efeitos sonoros do site Myinstants.com",
            "",
            "Gravuras utilizadas do acervo:",
            "Gabinete de Estampas",
            "",
            "Artistas e obras:",
            "Gladston Barroso, 'Carangueijo'",

            "Agradecimentos Especiais:",
            "Aos meus amores, Gildete Ancelma e Pâmela Evany.",
            "Aos meus irmãos e irmãs Tainara, Paulinho, Ailton, Júnior, Jhoe, Shini e Arianny.",
            "À Gabi, Francy, Amélia, John, Daniel, Luiz e André",
            "À minha gerente de produção, minha amiga e uma das pessoas que me incentivaram bastante, Flávia.",
            "À meus professores: Prof. Xakal, Prof. Dr. Wellington Cesário e Profa. Dra. Marjorie Severo Garrido",
            "por acreditar em mim e neste trabalho",
            "E aos testadores alpha, por todo apoio e incentivo durante o desenvolvimento do jogo.",
            "",
            "Obrigado por jogar!"
        };

        if (creditsText != null)
        {
            creditsText.text = string.Join("\n", credits);
        }
    }

    void Update()
    {
        // Auto-scroll de créditos
        if (creditsPanel != null && creditsPanel.activeSelf && creditsScrollRect != null && autoScrollSpeed > 100)
        {
            creditsScrollRect.verticalNormalizedPosition -= Time.deltaTime * autoScrollSpeed / 50f;
            creditsScrollRect.verticalNormalizedPosition = Mathf.Clamp01(creditsScrollRect.verticalNormalizedPosition);
        }
    }

    // Métodos de scroll manual (conectar em botões se quiser)
    public void ScrollCreditsUp()
    {
        if (creditsScrollRect != null)
        {
            creditsScrollRect.verticalNormalizedPosition += 0.1f;
        }
    }

    public void ScrollCreditsDown()
    {
        if (creditsScrollRect != null)
        {
            creditsScrollRect.verticalNormalizedPosition -= 0.1f;
        }
    }

    // Método equivalente ao botão "Jogar" (conectar Jogar -> MainMenu.Jogar())
    public void Jogar()
    {
        SceneManager.LoadScene("Transition");
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
        if (!GameSaveSystem.ContinueGame())
        {
            Debug.Log("Nenhum progresso salvo. Iniciando novo jogo...");
            StartGame();
        }
    }

    public void ShowMainMenu()
    {
        SetPanels(true, false);
    }

    public void ShowCredits()
    {
        SetPanels(false, true);

        // Reseta scroll de créditos ao abrir
        if (creditsScrollRect != null)
        {
            creditsScrollRect.verticalNormalizedPosition = 1f;
        }
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
