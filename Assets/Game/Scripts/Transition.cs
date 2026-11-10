using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    // Nome da cena a ser carregada
    public string gameSceneName = "SelectCharacter";

    /// <summary>
    /// Carrega a cena de seleção de personagem
    /// </summary>
    public void GameScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}