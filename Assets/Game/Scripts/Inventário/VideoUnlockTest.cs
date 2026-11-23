
using UnityEngine;

public class VideoUnlockTest : MonoBehaviour
{
    [Header("Obra para testar")]
    [SerializeField] private ItemData obraTeste;

    [ContextMenu("Testar conquista e desbloqueio")]
    public void TestarObra()
    {
        if (obraTeste == null)
        {
            Debug.LogWarning("Nenhuma obra atribuída!");
            return;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("InventoryManager não encontrado!");
            return;
        }

        if (SaveManager.Instance == null)
        {
            Debug.LogWarning("SaveManager não encontrado!");
            return;
        }

        // Adiciona a obra, caso ainda não tenha sido conquistada.
        InventoryManager.Instance.AdicionarItem(obraTeste);

        // Desbloqueia o vídeo correspondente.
        SaveManager.Instance.DesbloquearVideo(
            obraTeste.ItemID
        );

        Debug.Log(
            "Obra no inventário: " +
            InventoryManager.Instance.PossuiItem(
                obraTeste.ItemID
            )
        );

        Debug.Log(
            "Vídeo desbloqueado: " +
            SaveManager.Instance.VideoDesbloqueado(
                obraTeste.ItemID
            )
        );
    }
}
