using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour
{
    // Nomes das cenas
    public string gameSceneName = "Cena1";
    public string menuSceneName = "MainMenuBackup";

    // Nomes dos personagens
    private const string ABOBRINHA = "Abobrinha";
    private const string JOAOMELAO = "Joaomelao";

    // Chave para salvar o personagem selecionado em PlayerPrefs
    private const string SELECTED_CHARACTER_KEY = "SelectedCharacter";

    void Start()
    {
        Debug.Log("CharacterSelect carregado com sucesso!");
    }

    /// <summary>
    /// Selecao do personagem Abobrinha - conectar ao botao "Abobrinha"
    /// </summary>
    public void SelectAbobrinha()
    {
        Debug.Log("=== BOTAO ABOBRINHA CLICADO ===");
        SelectCharacter(ABOBRINHA);
    }

    /// <summary>
    /// Selecao do personagem Joaomelao - conectar ao botao "Joaomelao"
    /// </summary>
    public void SelectJoaomelao()
    {
        SelectCharacter(JOAOMELAO);
    }

    /// <summary>
    /// Metodo generico para selecionar um personagem
    /// Salva a escolha em PlayerPrefs e carrega Cena1
    /// </summary>
    private void SelectCharacter(string characterName)
    {
        Debug.Log("=== SELECTCHARACTER CHAMADO ===");
        Debug.Log("Personagem selecionado: " + characterName);

        // Salva o personagem selecionado para o jogo acessar
        PlayerPrefs.SetString(SELECTED_CHARACTER_KEY, characterName);
        PlayerPrefs.Save();

        Debug.Log("Carregando cena: " + gameSceneName);
        // Carrega a cena do jogo principal
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Retorna ao menu principal - conectar ao botao "BackToMenu"
    /// </summary>
    public void BackToMenu()
    {
        Debug.Log("Voltando ao menu principal...");
        SceneManager.LoadScene(menuSceneName);
    }

    /// <summary>
    /// Metodo estatico para o jogo principal acessar qual personagem foi selecionado
    /// Use em Cena1: string character = CharacterSelect.GetSelectedCharacter();
    /// </summary>
    public static string GetSelectedCharacter()
    {
        return PlayerPrefs.GetString(SELECTED_CHARACTER_KEY, ABOBRINHA);
    }
}
