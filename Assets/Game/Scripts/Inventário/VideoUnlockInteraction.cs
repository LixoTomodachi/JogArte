
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
public class VideoUnlockInteraction : MonoBehaviour
{
    [Header("Sistema de Recompensa")]
    [SerializeField] private VideoUnlockReward recompensaVideo;

    [Header("Botão de Interação")]
    [SerializeField] private Button botaoInteract;

    private bool jogadorProximo = false;

    // =========================
    // INICIALIZAÇÃO
    // =========================

    private void Awake()
    {
        if (recompensaVideo == null)
        {
            recompensaVideo =
                GetComponent<VideoUnlockReward>();
        }

        BoxCollider2D colliderInteracao =
            GetComponent<BoxCollider2D>();

        colliderInteracao.isTrigger = true;
    }

    // =========================
    // DETECTAR APROXIMAÇÃO
    // =========================

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        jogadorProximo = true;

        if (botaoInteract != null)
        {
            botaoInteract.onClick.RemoveListener(
                Interagir
            );

            botaoInteract.onClick.AddListener(
                Interagir
            );
        }

        Debug.Log(
            "Jogador próximo de um objeto de desbloqueio."
        );
    }

    // =========================
    // REALIZAR INTERAÇÃO
    // =========================

    public void Interagir()
    {
        if (!jogadorProximo)
            return;

        if (recompensaVideo == null)
        {
            Debug.LogWarning(
                "VideoUnlockReward não configurado!",
                this
            );

            return;
        }

        recompensaVideo.ConcederRecompensa();
    }

    // =========================
    // SAIR DA ÁREA
    // =========================

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        jogadorProximo = false;

        DesvincularBotao();
    }

    private void OnDisable()
    {
        jogadorProximo = false;

        DesvincularBotao();
    }

    private void DesvincularBotao()
    {
        if (botaoInteract != null)
        {
            botaoInteract.onClick.RemoveListener(
                Interagir
            );
        }
    }
}
