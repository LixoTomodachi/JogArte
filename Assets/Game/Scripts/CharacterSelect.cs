
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour
{
    // Nomes das cenas
    public string gameSceneName = "Cena1";
    public string gameSceneName2 = "Cena1B";
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
    /// Seleção do personagem Abobrinha
    /// Carrega Cena1B
    /// </summary>
    public void SelectAbobrinha()
    {
        Debug.Log("=== BOTAO ABOBRINHA CLICADO ===");
        SelectCharacter(ABOBRINHA, gameSceneName2);
    }

    /// <summary>
    /// Seleção do personagem Joaomelao
    /// Carrega Cena1
    /// </summary>
    public void SelectJoaomelao()
    {
        Debug.Log("=== BOTAO JOAOMELAO CLICADO ===");
        SelectCharacter(JOAOMELAO, gameSceneName);
    }

    /// <summary>
    /// Método genérico para selecionar um personagem
    /// Salva a escolha e carrega a cena correspondente
    /// </summary>
    private void SelectCharacter(string characterName, string sceneName)
    {
        Debug.Log("=== SELECTCHARACTER CHAMADO ===");
        Debug.Log("Personagem selecionado: " + characterName);

        // Salva o personagem selecionado
        PlayerPrefs.SetString(SELECTED_CHARACTER_KEY, characterName);
        PlayerPrefs.Save();

        Debug.Log("Carregando cena: " + sceneName);

        // Carrega somente a cena correspondente ao personagem
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Retorna ao menu principal
    /// </summary>
    public void BackToMenu()
    {
        Debug.Log("Voltando ao menu principal...");
        SceneManager.LoadScene(menuSceneName);
    }

    /// <summary>
    /// Retorna o personagem selecionado
    /// </summary>
    public static string GetSelectedCharacter()
    {
        return PlayerPrefs.GetString(SELECTED_CHARACTER_KEY, ABOBRINHA);
    }
}
