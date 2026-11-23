
using UnityEngine;

public class VideoUnlockReward : MonoBehaviour
{
    [Header("Vídeo a desbloquear")]
    [SerializeField] private ItemData obraVinculada;

    [Header("Configurações")]
    [SerializeField] private bool desbloqueioRealizado;

    public void ConcederRecompensa()
    {
        if (obraVinculada == null)
        {
            Debug.LogWarning(
                "Nenhuma obra vinculada ao desbloqueio!",
                this
            );
            return;
        }

        if (SaveManager.Instance == null)
        {
            Debug.LogWarning(
                "SaveManager não encontrado!",
                this
            );
            return;
        }

        if (obraVinculada.VideoDemonstrativo == null)
        {
            Debug.LogWarning(
                "A obra não possui vídeo demonstrativo!",
                this
            );
            return;
        }

        if (SaveManager.Instance.VideoDesbloqueado(
            obraVinculada.ItemID))
        {
            desbloqueioRealizado = true;

            Debug.Log(
                "O vídeo já estava desbloqueado: " +
                obraVinculada.NomeObra
            );

            return;
        }

        SaveManager.Instance.DesbloquearVideo(
            obraVinculada.ItemID
        );

        desbloqueioRealizado = true;

        Debug.Log(
            "Vídeo desbloqueado com sucesso: " +
            obraVinculada.NomeObra
        );
    }
}
