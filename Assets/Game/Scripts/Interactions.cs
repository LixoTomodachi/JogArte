
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Interactions : MonoBehaviour, IIntegravel
{
    private enum TipoInteracao
    {
        TrocarCena,
        Dialogo,
        Pergunta
    }

    [Header("Tipo de Interação")]
    [SerializeField] private TipoInteracao tipoInteracao;

    [Header("Indicador de Interação")]
    [SerializeField] private GameObject indicadorInteracao;

    [Header("Troca de Cena")]
    [SerializeField] private string nomeCenaDestino;

    [Header("Diálogo")]
    [SerializeField] private Animator animatorPersonagemEmCena;
    [SerializeField] private string triggerAnimacao = "Interact";

    [SerializeField] private GameObject painelDialogo;
    [SerializeField] private TMP_Text textoDialogo;

    [SerializeField, TextArea(2, 4)]
    private string[] falas;

    [Header("Pergunta")]
    [SerializeField] private string perguntaID;
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private TMP_Text questionText;

    [SerializeField, TextArea(2, 4)]
    private string enunciadoPergunta;

    [SerializeField] private Button resposta1;
    [SerializeField] private Button resposta2;
    [SerializeField] private Button resposta3;

    [SerializeField] private TMP_Text textoResposta1;
    [SerializeField] private TMP_Text textoResposta2;
    [SerializeField] private TMP_Text textoResposta3;

    [SerializeField] private string alternativa1;
    [SerializeField] private string alternativa2;
    [SerializeField] private string alternativa3;

    [Header("Resposta Correta")]
    [SerializeField, Range(1, 3)]
    private int alternativaCorreta = 1;

    [Header("Feedback")]
    [SerializeField] private GameObject feedbackPanel;
    [SerializeField] private TMP_Text feedbackText;

    [SerializeField, TextArea(2, 5)]
    private string textoAcerto;

    [SerializeField, TextArea(2, 5)]
    private string textoErro;

    [Header("Prêmio")]
    [SerializeField] private ItemData obraPremio;

    // Controle interno
    private CharacterMovement personagemNaArea;

    private readonly HashSet<Collider2D> collidersDoPersonagem =
        new HashSet<Collider2D>();

    private int indiceFala;
    private bool dialogoAberto;
    private bool perguntaAberta;
    private bool feedbackAberto;

    private SpriteRenderer spriteRendererIndicador;

    // =========================
    // INICIALIZAÇÃO
    // =========================

    private void Awake()
    {
        if (indicadorInteracao == gameObject)
        {
            spriteRendererIndicador = GetComponent<SpriteRenderer>();

            if (spriteRendererIndicador != null)
                spriteRendererIndicador.enabled = false;
        }
        else if (indicadorInteracao != null)
        {
            indicadorInteracao.SetActive(false);
        }

        if (painelDialogo != null)
            painelDialogo.SetActive(false);

        if (questionPanel != null)
            questionPanel.SetActive(false);

        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);

        if (resposta1 != null)
            resposta1.onClick.AddListener(() => Responder(1));

        if (resposta2 != null)
            resposta2.onClick.AddListener(() => Responder(2));

        if (resposta3 != null)
            resposta3.onClick.AddListener(() => Responder(3));
    }

    private void OnDestroy()
    {
        if (resposta1 != null)
            resposta1.onClick.RemoveAllListeners();

        if (resposta2 != null)
            resposta2.onClick.RemoveAllListeners();

        if (resposta3 != null)
            resposta3.onClick.RemoveAllListeners();
    }

    // =========================
    // INDICADOR
    // =========================

    private void DefinirIndicador(bool visivel)
    {
        if (indicadorInteracao == null)
            return;

        if (indicadorInteracao == gameObject)
        {
            if (spriteRendererIndicador == null)
                spriteRendererIndicador = GetComponent<SpriteRenderer>();

            if (spriteRendererIndicador != null)
                spriteRendererIndicador.enabled = visivel;
        }
        else
        {
            indicadorInteracao.SetActive(visivel);
        }
    }

    // =========================
    // PROXIMIDADE
    // =========================

    private void OnTriggerEnter2D(Collider2D other)
    {
        CharacterMovement personagem =
            other.GetComponentInParent<CharacterMovement>();

        if (personagem == null)
            return;

        if (personagemNaArea != null &&
            personagemNaArea != personagem)
            return;

        bool primeiraEntrada = personagemNaArea == null;

        personagemNaArea = personagem;

        collidersDoPersonagem.Add(other);

        if (primeiraEntrada &&
            animatorPersonagemEmCena != null &&
            !string.IsNullOrWhiteSpace(triggerAnimacao))
        {
            animatorPersonagemEmCena.SetTrigger(triggerAnimacao);
        }

        DefinirIndicador(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!collidersDoPersonagem.Remove(other))
            return;

        if (collidersDoPersonagem.Count > 0)
            return;

        personagemNaArea = null;

        DefinirIndicador(false);

        if (dialogoAberto)
            FecharDialogo();

        if (perguntaAberta || feedbackAberto)
            FecharPergunta();
    }

    // =========================
    // INTERACT
    // =========================

    public void Interagir(GameObject personagem)
    {
        if (personagemNaArea == null)
            return;

        if (personagemNaArea.gameObject != personagem)
            return;

        switch (tipoInteracao)
        {
            case TipoInteracao.TrocarCena:
                TrocarCena();
                break;

            case TipoInteracao.Dialogo:
                InteragirDialogo();
                break;

            case TipoInteracao.Pergunta:
                InteragirPergunta();
                break;
        }
    }

    // =========================
    // TROCA DE CENA
    // =========================

    private void TrocarCena()
    {
        if (string.IsNullOrWhiteSpace(nomeCenaDestino))
        {
            Debug.LogWarning(
                "Nome da cena não foi configurado!",
                this
            );

            return;
        }

        SceneManager.LoadScene(nomeCenaDestino);
    }

    // =========================
    // DIÁLOGO ORIGINAL
    // =========================

    private void InteragirDialogo()
    {
        if (!dialogoAberto)
        {
            AbrirDialogo();
            return;
        }

        indiceFala++;

        if (indiceFala >= falas.Length)
        {
            FecharDialogo();
            return;
        }

        AtualizarTexto();
    }

    private void AbrirDialogo()
    {
        if (painelDialogo == null || textoDialogo == null)
        {
            Debug.LogWarning(
                "Painel ou componente de texto não configurado!",
                this
            );

            return;
        }

        if (falas == null || falas.Length == 0)
        {
            Debug.LogWarning(
                "Nenhuma fala foi cadastrada!",
                this
            );

            return;
        }

        dialogoAberto = true;
        indiceFala = 0;

        painelDialogo.SetActive(true);

        AtualizarTexto();
    }

    private void AtualizarTexto()
    {
        textoDialogo.text = falas[indiceFala];
    }

    private void FecharDialogo()
    {
        dialogoAberto = false;
        indiceFala = 0;

        if (painelDialogo != null)
            painelDialogo.SetActive(false);
    }

    // =========================
    // SISTEMA DE PERGUNTAS
    // =========================

    private void InteragirPergunta()
    {
        // Interact fecha o feedback.
        if (feedbackAberto)
        {
            FecharPergunta();
            return;
        }

        // Não avança enquanto aguarda uma resposta.
        if (perguntaAberta)
            return;

        // Impede repetir perguntas já concluídas.
        if (PerguntaConcluida())
        {
            Debug.Log("Pergunta já concluída: " + perguntaID);
            return;
        }

        AbrirPergunta();
    }

    private void AbrirPergunta()
    {
        if (string.IsNullOrWhiteSpace(perguntaID))
        {
            Debug.LogWarning("Configure o ID da pergunta!", this);
            return;
        }

        if (questionPanel == null || questionText == null ||
            resposta1 == null || resposta2 == null || resposta3 == null ||
            textoResposta1 == null || textoResposta2 == null ||
            textoResposta3 == null)
        {
            Debug.LogWarning(
                "Referências do painel de perguntas incompletas!",
                this
            );
            return;
        }

        perguntaAberta = true;
        feedbackAberto = false;

        questionText.text = enunciadoPergunta;

        textoResposta1.text = alternativa1;
        textoResposta2.text = alternativa2;
        textoResposta3.text = alternativa3;

        if (painelDialogo != null)
            painelDialogo.SetActive(true);

        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);

        questionPanel.SetActive(true);
    }

    private void Responder(int alternativaEscolhida)
    {
        if (!perguntaAberta || feedbackAberto)
            return;

        bool correta = alternativaEscolhida == alternativaCorreta;

        perguntaAberta = false;

        if (questionPanel != null)
            questionPanel.SetActive(false);

        // Registra todas as tentativas.
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.RegistrarResposta(
                perguntaID,
                alternativaEscolhida,
                correta
            );
        }
        else
        {
            Debug.LogWarning(
                "SaveManager não encontrado. Resposta não salva!",
                this
            );
        }

        if (correta)
        {
            // Concede a obra somente após o acerto.
            if (obraPremio != null &&
                InventoryManager.Instance != null)
            {
                InventoryManager.Instance.AdicionarItem(obraPremio);
            }

            MostrarFeedback(textoAcerto);
        }
        else
        {
            MostrarFeedback(textoErro);
        }
    }

    private void MostrarFeedback(string mensagem)
    {
        if (feedbackPanel == null || feedbackText == null)
        {
            Debug.LogWarning(
                "Painel de feedback não configurado!",
                this
            );

            FecharPergunta();
            return;
        }

        feedbackAberto = true;

        feedbackText.text = mensagem;

        if (painelDialogo != null)
            painelDialogo.SetActive(true);

        feedbackPanel.SetActive(true);
    }

    private void FecharPergunta()
    {
        perguntaAberta = false;
        feedbackAberto = false;

        if (questionPanel != null)
            questionPanel.SetActive(false);

        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);

        if (painelDialogo != null)
            painelDialogo.SetActive(false);
    }

    // =========================
    // VERIFICAÇÃO DE CONCLUSÃO
    // =========================

    private bool PerguntaConcluida()
    {
        if (SaveManager.Instance == null ||
            string.IsNullOrWhiteSpace(perguntaID))
            return false;

        List<SaveManager.RespostaSalva> historico =
            SaveManager.Instance.ObterHistoricoRespostas();

        foreach (SaveManager.RespostaSalva resposta in historico)
        {
            if (resposta.perguntaID == perguntaID &&
                resposta.correta)
            {
                return true;
            }
        }

        return false;
    }
}
