using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float velocidadeMovimento = 5f;
    [SerializeField] private float forcaPulo = 10f;
    [SerializeField] private int maxPulos = 1;

    [Header("Referências")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform peDoPersonagem;
    [SerializeField] private float distanciaVerificadorChao = 0.2f;
    [SerializeField] private LayerMask camadaChao;

    [Header("Interação")]
    [SerializeField] private float raioInteracao = 1f;
    [SerializeField] private LayerMask camadaIntegravel;

    private bool estaNoChao;
    private float direcaoHorizontal;
    private int pulosRestantes;
    private SpriteRenderer spriteRenderer;

    //Agora conseguimos animar a "andada" do personagem com base na velocidade horizontal
    public bool Walk => Mathf.Abs(direcaoHorizontal) > 0.1f;

    void Start()
    {
        // Rigidbody2D - Procura em vários lugares
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = GetComponentInChildren<Rigidbody2D>();
        if (rb == null)
            rb = GetComponentInParent<Rigidbody2D>();

        // SpriteRenderer - Procura em vários lugares
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInParent<SpriteRenderer>();

        pulosRestantes = maxPulos;
        estaNoChao = true;

        Debug.Log("=== CharacterMovement Iniciado ===");
        Debug.Log("Rigidbody2D: " + (rb != null ? "✓" : "✗"));
        Debug.Log("SpriteRenderer: " + (spriteRenderer != null ? "✓" : "✗"));
        Debug.Log("Pe Do Personagem: " + (peDoPersonagem != null ? "✓" : "✗"));
    }

    void Update()
    {
        VerificarChao();
    }

    void FixedUpdate()
    {
        AplicarMovimento();
    }

    public void MoverEsquerda(bool pressionado)
    {
        if (pressionado)
        {
            direcaoHorizontal = -1f;
            VirarParaEsquerda();
            Debug.Log("← ESQUERDA");
        }
        else if (direcaoHorizontal < 0)
        {
            direcaoHorizontal = 0f;
            Debug.Log("□ Parado");
        }
    }

    public void MoverDireita(bool pressionado)
    {
        if (pressionado)
        {
            direcaoHorizontal = 1f;
            VirarParaDireita();
            Debug.Log("→ DIREITA");
        }
        else if (direcaoHorizontal > 0)
        {
            direcaoHorizontal = 0f;
            Debug.Log("□ Parado");
        }
    }

    public void PularBotao()
    {
        Debug.Log($"[PULO] Chão: {estaNoChao} | Pulos: {pulosRestantes}/{maxPulos}");

        if (pulosRestantes > 0)
        {
            Pular();
        }
        else
        {
            Debug.LogWarning("⚠ Sem pulos!");
        }
    }

    public void InteragirBotao()
    {
        Debug.Log("[INTERAÇÃO]");
        Interagir();
    }

    void VerificarChao()
    {
        if (rb == null || peDoPersonagem == null) return;

        Vector2 posicao = (Vector2)peDoPersonagem.position;
        RaycastHit2D hit = Physics2D.Raycast(posicao, Vector2.down, distanciaVerificadorChao, camadaChao);

        bool estaNoChaoNovo = hit.collider != null;

        if (estaNoChaoNovo && !estaNoChao)
        {
            pulosRestantes = maxPulos;
            Debug.Log($"✓ CHÃO! Pulos: {pulosRestantes}");
        }

        estaNoChao = estaNoChaoNovo;
        Debug.DrawRay(posicao, Vector2.down * distanciaVerificadorChao, estaNoChao ? Color.green : Color.red);
    }

    void AplicarMovimento()
    {
        if (rb == null) return;

        Vector2 velocidade = rb.linearVelocity;
        velocidade.x = direcaoHorizontal * velocidadeMovimento;
        rb.linearVelocity = velocidade;
    }

    void VirarParaDireita()
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("❌ SpriteRenderer NULL!");
            return;
        }

        spriteRenderer.flipX = false;
        Debug.Log("✓ DIREITA (flipX = false)");
    }

    void VirarParaEsquerda()
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("❌ SpriteRenderer NULL!");
            return;
        }

        spriteRenderer.flipX = true;
        Debug.Log("✓ ESQUERDA (flipX = true)");
    }

    void Pular()
    {
        if (rb == null || pulosRestantes <= 0) return;

        Vector2 velocidade = rb.linearVelocity;
        velocidade.y = 0;
        rb.linearVelocity = velocidade;

        rb.linearVelocity += Vector2.up * forcaPulo;
        pulosRestantes--;

        Debug.Log($"✈️ PULO! Restantes: {pulosRestantes}");
    }

    void Interagir()
    {
        Collider2D[] objetos = Physics2D.OverlapCircleAll(transform.position, raioInteracao, camadaIntegravel);

        foreach (Collider2D obj in objetos)
        {
            IIntegravel integravel = obj.GetComponent<IIntegravel>();
            if (integravel != null)
            {
                integravel.Interagir(gameObject);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raioInteracao);

        if (peDoPersonagem != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(peDoPersonagem.position, 0.1f);
        }
    }
}

public interface IIntegravel
{
    void Interagir(GameObject personagem);
}
