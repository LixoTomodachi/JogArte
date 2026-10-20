using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour
{
    // Nome da cena do jogo
    public string gameSceneName = "Cena1";

    // Nome da cena de seleção de personagem
    public string characterSelectSceneName = "SelectCharacter";

    // Nome da cena do menu principal
    public string menuSceneName = "MainMenuBackup";

    /// <summary>
    /// Chamado pelos botões de seleção de personagem
    /// </summary>
    public void SelectCharacter(string characterName)
    {
        Debug.Log($"Personagem selecionado: {characterName}");

        // Aqui você pode salvar a escolha do personagem se necessário
        PlayerPrefs.SetString("SelectedCharacter", characterName);
        PlayerPrefs.Save();

        // Carrega Cena1 (o jogo principal)
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Botão voltar ao menu principal
    /// </summary>
    public void BackToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}

