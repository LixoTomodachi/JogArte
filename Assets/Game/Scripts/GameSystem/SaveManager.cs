
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private string caminhoArquivo;

    // =========================
    // ESTRUTURA DOS DADOS
    // =========================

    [Serializable]
    public class RespostaSalva
    {
        public string perguntaID;
        public int respostaEscolhida;
        public bool correta;
    }

    [Serializable]
    public class DadosSalvos
    {
        public List<string> itensObtidos = new List<string>();

        public List<RespostaSalva> respostas =
            new List<RespostaSalva>();

        public List<string> videosDesbloqueados =
            new List<string>();

        public int totalPenalidades;

        public string progressoHistoria = "inicio";
    }

    private DadosSalvos dados = new DadosSalvos();


    // =========================
    // INICIALIZAÇÃO
    // =========================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        caminhoArquivo = Path.Combine(
            Application.persistentDataPath,
            "savegame.json"
        );

        Debug.Log("SaveManager iniciado!");
    }

    private void Start()
    {
        CarregarJogo();
    }


    // =========================
    // SALVAMENTO
    // =========================

    public void SalvarJogo()
    {
        // Atualiza os itens do inventário
        if (InventoryManager.Instance != null)
        {
            dados.itensObtidos =
                InventoryManager.Instance.ObterIDsItens();
        }

        try
        {
            string json = JsonUtility.ToJson(dados, true);

            File.WriteAllText(caminhoArquivo, json);

            Debug.Log("Jogo salvo com sucesso!");
        }
        catch (Exception e)
        {
            Debug.LogError("Erro ao salvar o jogo: " + e.Message);
        }
    }


    // =========================
    // CARREGAMENTO
    // =========================

    public void CarregarJogo()
    {
        if (!File.Exists(caminhoArquivo))
        {
            Debug.Log("Nenhum salvamento anterior encontrado.");
            return;
        }

        try
        {
            string json = File.ReadAllText(caminhoArquivo);

            DadosSalvos dadosCarregados =
                JsonUtility.FromJson<DadosSalvos>(json);

            if (dadosCarregados == null)
            {
                Debug.LogWarning("Arquivo de salvamento inválido.");
                return;
            }

            dados = dadosCarregados;

            // Garante que as listas existam
            if (dados.itensObtidos == null)
                dados.itensObtidos = new List<string>();

            if (dados.respostas == null)
                dados.respostas = new List<RespostaSalva>();

            if (dados.videosDesbloqueados == null)
                dados.videosDesbloqueados = new List<string>();

            // Restaura o inventário
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.RestaurarInventario(
                    dados.itensObtidos
                );
            }

            Debug.Log("Jogo carregado com sucesso!");
        }
        catch (Exception e)
        {
            Debug.LogError("Erro ao carregar o jogo: " + e.Message);
        }
    }


    // =========================
    // REGISTRO DE RESPOSTAS
    // =========================

    public void RegistrarResposta(
        string perguntaID,
        int respostaEscolhida,
        bool correta)
    {
        RespostaSalva resposta = new RespostaSalva();

        resposta.perguntaID = perguntaID;
        resposta.respostaEscolhida = respostaEscolhida;
        resposta.correta = correta;

        dados.respostas.Add(resposta);

        if (!correta)
        {
            dados.totalPenalidades++;
        }

        SalvarJogo();
    }


    // =========================
    // DESBLOQUEIO DE VÍDEOS
    // =========================

    public void DesbloquearVideo(string itemID)
    {
        if (string.IsNullOrWhiteSpace(itemID))
            return;

        if (dados.videosDesbloqueados.Contains(itemID))
            return;

        dados.videosDesbloqueados.Add(itemID);

        SalvarJogo();

        Debug.Log("Vídeo desbloqueado: " + itemID);
    }


    public bool VideoDesbloqueado(string itemID)
    {
        return dados.videosDesbloqueados.Contains(itemID);
    }


    // =========================
    // PROGRESSÃO DA HISTÓRIA
    // =========================

    public void AtualizarProgresso(string novoProgresso)
    {
        dados.progressoHistoria = novoProgresso;

        SalvarJogo();
    }


    public string ObterProgresso()
    {
        return dados.progressoHistoria;
    }


    // =========================
    // CONSULTAS
    // =========================

    public int ObterTotalPenalidades()
    {
        return dados.totalPenalidades;
    }

    public List<RespostaSalva> ObterHistoricoRespostas()
    {
        return new List<RespostaSalva>(dados.respostas);
    }
}
