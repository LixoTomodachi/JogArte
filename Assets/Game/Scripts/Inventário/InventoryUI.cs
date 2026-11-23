
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Painéis do Inventário")]
    [SerializeField] private GameObject collectionPanel;
    [SerializeField] private GameObject artworkDetailsPanel;

    [Header("Lista de Obras")]
    [SerializeField] private Transform contentObras;
    [SerializeField] private Button artworkButtonTemplate;
    [SerializeField] private TMP_Text textoColecaoVazia;

    [Header("Detalhes da Obra")]
    [SerializeField] private Image artworkImage;
    [SerializeField] private TMP_Text artworkName;
    [SerializeField] private TMP_Text artworkDescription;

    [Header("Vídeo Demonstrativo")]
    [SerializeField] private Button videoButton;
    [SerializeField] private TMP_Text videoStatusText;

    [Header("Painel de Reprodução")]
    [SerializeField] private GameObject videoPanel;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Button playPauseButton;
    [SerializeField] private TMP_Text playPauseText;

    private readonly List<GameObject> botoesCriados =
        new List<GameObject>();

    private ItemData obraSelecionada;

    // =========================
    // INICIALIZAÇÃO
    // =========================

    private void Awake()
    {
        if (videoButton != null)
            videoButton.onClick.AddListener(AbrirVideo);

        if (playPauseButton != null)
            playPauseButton.onClick.AddListener(AlternarReproducao);

        if (videoPlayer != null)
            videoPlayer.playOnAwake = false;
    }

    private void OnEnable()
    {
        MostrarColecao();
    }

    private void OnDisable()
    {
        PararVideo();
    }

    // =========================
    // ABRIR E FECHAR
    // =========================

    public void AbrirInventario()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        else
        {
            MostrarColecao();
        }
    }

    public void FecharInventario()
    {
        PararVideo();
        gameObject.SetActive(false);
    }

    // =========================
    // COLEÇÃO DE OBRAS
    // =========================

    public void MostrarColecao()
    {
        PararVideo();

        if (videoPanel != null)
            videoPanel.SetActive(false);

        if (collectionPanel != null)
            collectionPanel.SetActive(true);

        if (artworkDetailsPanel != null)
            artworkDetailsPanel.SetActive(false);

        obraSelecionada = null;

        AtualizarColecao();
    }

    private void AtualizarColecao()
    {
        LimparBotoes();

        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning(
                "InventoryManager não encontrado!",
                this
            );
            return;
        }

        if (contentObras == null ||
            artworkButtonTemplate == null)
        {
            Debug.LogWarning(
                "Content ou modelo de botão não configurado!",
                this
            );
            return;
        }

        artworkButtonTemplate.gameObject.SetActive(false);

        List<ItemData> obras =
            InventoryManager.Instance.ObterItensObtidos();

        if (textoColecaoVazia != null)
            textoColecaoVazia.gameObject.SetActive(
                obras.Count == 0
            );

        foreach (ItemData obra in obras)
        {
            if (obra != null)
                CriarBotaoObra(obra);
        }
    }

    private void CriarBotaoObra(ItemData obra)
    {
        Button novoBotao = Instantiate(
            artworkButtonTemplate,
            contentObras
        );

        novoBotao.gameObject.name =
            "Obra_" + obra.ItemID;

        novoBotao.gameObject.SetActive(true);

        Transform miniatura =
            novoBotao.transform.Find("ArtworkThumbnail");

        if (miniatura != null)
        {
            Image imagem = miniatura.GetComponent<Image>();

            if (imagem != null)
            {
                imagem.sprite = obra.ImagemObra;
                imagem.preserveAspect = true;
            }
        }

        Transform titulo =
            novoBotao.transform.Find("ArtworkLabel");

        if (titulo != null)
        {
            TMP_Text texto =
                titulo.GetComponent<TMP_Text>();

            if (texto != null)
                texto.text = obra.NomeObra;
        }

        ItemData item = obra;

        novoBotao.onClick.AddListener(
            () => MostrarDetalhes(item)
        );

        botoesCriados.Add(novoBotao.gameObject);
    }

    private void LimparBotoes()
    {
        foreach (GameObject botao in botoesCriados)
        {
            if (botao != null)
                Destroy(botao);
        }

        botoesCriados.Clear();
    }

    // =========================
    // DETALHES DA OBRA
    // =========================

    private void MostrarDetalhes(ItemData obra)
    {
        if (obra == null)
            return;

        obraSelecionada = obra;

        if (collectionPanel != null)
            collectionPanel.SetActive(false);

        if (videoPanel != null)
            videoPanel.SetActive(false);

        if (artworkDetailsPanel != null)
            artworkDetailsPanel.SetActive(true);

        if (artworkImage != null)
        {
            artworkImage.sprite = obra.ImagemObra;
            artworkImage.preserveAspect = true;
        }

        if (artworkName != null)
            artworkName.text = obra.NomeObra;

        if (artworkDescription != null)
            artworkDescription.text = obra.Descricao;

        AtualizarEstadoVideo(obra);
    }

    // =========================
    // DESBLOQUEIO DE VÍDEOS
    // =========================

    private bool PodeReproduzirVideo(ItemData obra)
    {
        if (obra == null ||
            obra.VideoDemonstrativo == null ||
            SaveManager.Instance == null)
            return false;

        return SaveManager.Instance.VideoDesbloqueado(
            obra.ItemID
        );
    }

    private void AtualizarEstadoVideo(ItemData obra)
    {
        bool disponivel = PodeReproduzirVideo(obra);

        if (videoButton != null)
            videoButton.interactable = disponivel;

        if (videoStatusText == null)
            return;

        if (obra.VideoDemonstrativo == null)
        {
            videoStatusText.text = "Vídeo não disponível";
        }
        else if (disponivel)
        {
            videoStatusText.text = "Vídeo desbloqueado";
        }
        else
        {
            videoStatusText.text = "Vídeo bloqueado";
        }
    }

    // =========================
    // ABRIR VÍDEO
    // =========================

    public void AbrirVideo()
    {
        if (!PodeReproduzirVideo(obraSelecionada))
        {
            Debug.LogWarning(
                "Vídeo indisponível ou bloqueado!",
                this
            );
            return;
        }

        if (videoPlayer == null || videoPanel == null)
        {
            Debug.LogWarning(
                "VideoPlayer ou VideoPanel não configurado!",
                this
            );
            return;
        }

        PararVideo();

        videoPlayer.clip =
            obraSelecionada.VideoDemonstrativo;

        if (artworkDetailsPanel != null)
            artworkDetailsPanel.SetActive(false);

        videoPanel.SetActive(true);

        videoPlayer.Play();

        AtualizarTextoReproducao();
    }

    // =========================
    // REPRODUZIR E PAUSAR
    // =========================

    public void AlternarReproducao()
    {
        if (videoPlayer == null ||
            videoPlayer.clip == null)
            return;

        if (videoPlayer.isPlaying)
            videoPlayer.Pause();
        else
            videoPlayer.Play();

        AtualizarTextoReproducao();
    }

    private void AtualizarTextoReproducao()
    {
        if (playPauseText == null ||
            videoPlayer == null)
            return;

        playPauseText.text =
            videoPlayer.isPlaying ? "Pausar" : "Reproduzir";
    }

    private void PararVideo()
    {
        if (videoPlayer != null)
            videoPlayer.Stop();

        if (playPauseText != null)
            playPauseText.text = "Reproduzir";
    }

    // =========================
    // VOLTAR AOS DETALHES
    // =========================

    public void VoltarDosVideos()
    {
        PararVideo();

        if (videoPanel != null)
            videoPanel.SetActive(false);

        if (artworkDetailsPanel != null)
            artworkDetailsPanel.SetActive(true);

        if (obraSelecionada != null)
            AtualizarEstadoVideo(obraSelecionada);
    }

    // =========================
    // LIMPEZA
    // =========================

    private void OnDestroy()
    {
        if (videoButton != null)
            videoButton.onClick.RemoveListener(AbrirVideo);

        if (playPauseButton != null)
            playPauseButton.onClick.RemoveListener(
                AlternarReproducao
            );

        LimparBotoes();
    }
}
