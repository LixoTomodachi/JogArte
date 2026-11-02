using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float velocidadeMovimento = 5f;
    [SerializeField] private float forcaPulo = 10f;

    [Header("Referências")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float distanciaVerificadorChao = 0.1f;
    [SerializeField] private LayerMask camadaChao;

    [Header("Interação")]
    [SerializeField] private float raioInteracao = 1f;
    [SerializeField] private LayerMask camadaIntegravel;

    private bool estaNoChao;
    private float direcaoHorizontal;

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        // Debug
        Debug.Log("CharacterMovement iniciado. Rigidbody2D: " + (rb != null ? "Encontrado" : "NÃO ENCONTRADO"));
        Debug.Log("Gravity Scale: " + (rb != null ? rb.gravityScale : 0));
    }

    void Update()
    {
        VerificarChao();
    }

    void FixedUpdate()
    {
        AplicarMovimento();
    }

    // Métodos públicos para os botões UI chamarem
    public void MoverEsquerda(bool pressionado)
    {
        if (pressionado)
            direcaoHorizontal = -1f;
        else if (direcaoHorizontal < 0)
            direcaoHorizontal = 0f;
    }

    public void MoverDireita(bool pressionado)
    {
        if (pressionado)
            direcaoHorizontal = 1f;
        else if (direcaoHorizontal > 0)
            direcaoHorizontal = 0f;
    }

    public void PularBotao()
    {
        Debug.Log("Botão pulo pressionado!");
        Pular();
    }

    public void InteragirBotao()
    {
        Interagir();
    }

    void VerificarChao()
    {
        if (rb == null) return;

        // Posição ligeiramente abaixo do personagem
        Vector2 posicao = (Vector2)transform.position + new Vector2(0, -0.6f);

        // Tenta múltiplos raycasts para garantir detecção
        RaycastHit2D hit = Physics2D.Raycast(posicao, Vector2.down, 0.5f);

        // Se não achar com layer específica, tenta sem filtro
        if (!hit.collider && camadaChao != 0)
        {
            hit = Physics2D.Raycast(posicao, Vector2.down, 0.5f);
        }

        bool estaNoChaoNovo = hit.collider != null;

        // Só loga quando o estado muda (evita spam)
        if (estaNoChaoNovo != estaNoChao)
        {
            if (estaNoChaoNovo)
            {
                Debug.Log($"✓ Chão DETECTADO: {hit.collider.name}");
            }
            else
            {
                Debug.LogWarning("✗ Chão perdido!");
            }
        }

        estaNoChao = estaNoChaoNovo;

        // Desenha debug na cena
        Debug.DrawRay(posicao, Vector2.down * 0.5f, estaNoChao ? Color.green : Color.red);
    }

    void AplicarMovimento()
    {
        if (rb == null) return;

        Vector2 velocidade = rb.linearVelocity;
        velocidade.x = direcaoHorizontal * velocidadeMovimento;
        rb.linearVelocity = velocidade;

        // Virar o personagem na direção do movimento
        if (direcaoHorizontal != 0)
            transform.localScale = new Vector3(direcaoHorizontal > 0 ? 1 : -1, 1, 1);
    }

    void Pular()
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D não atribuído!");
            return;
        }

        Debug.Log("Executando pulo! Velocidade Y atual: " + rb.linearVelocity.y);

        // Zera a velocidade Y e aplica o pulo
        Vector2 velocidade = rb.linearVelocity;
        velocidade.y = 0; // Reset antes de pular
        rb.linearVelocity = velocidade;

        // Aplica a força do pulo
        rb.linearVelocity += Vector2.up * forcaPulo;

        Debug.Log("Após pulo - Velocidade Y: " + rb.linearVelocity.y);
    }

    void Interagir()
    {
        Collider2D[] objetos = Physics2D.OverlapCircleAll(transform.position, raioInteracao, camadaIntegravel);

        foreach (Collider2D obj in objetos)
        {
            IIntegravel integravel = obj.GetComponent<IIntegravel>();
            if (integravel != null)
                integravel.Interagir(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raioInteracao);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * distanciaVerificadorChao);
    }
}

// Interface para objetos que podem ser interagidos
public interface IIntegravel
{
    void Interagir(GameObject personagem);
}
