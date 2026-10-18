using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    // Duração da animação de transição (em segundos)
    public float transitionDuration = 2f;

    /// <summary>
    /// Carrega a cena "transition" e depois carrega a cena alvo
    /// </summary>
    public void PlayTransition(string nextSceneName)
    {
        StartCoroutine(TransitionCoroutine(nextSceneName));
    }

    /// <summary>
    /// Coroutine que gerencia o fluxo de transição
    /// </summary>
    private IEnumerator TransitionCoroutine(string nextSceneName)
    {
        // Carrega a cena de transição normalmente (substitui a cena atual)
        SceneManager.LoadScene("transition");

        // Aguarda o tempo da animação
        yield return new WaitForSeconds(transitionDuration);

        // Carrega a cena alvo (substitui a transição)
        SceneManager.LoadScene(nextSceneName);
    }
}
