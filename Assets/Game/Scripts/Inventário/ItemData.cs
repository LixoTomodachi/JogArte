
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(
    fileName = "NewItem",
    menuName = "Game/Inventory/Item Data"
)]
public class ItemData : ScriptableObject
{
    [Header("Identificação")]
    [SerializeField] private string itemID;
    [SerializeField] private string nomeObra;

    [Header("Informações da Obra")]
    [TextArea(3, 6)]
    [SerializeField] private string descricao;

    [SerializeField] private Sprite imagemObra;

    [Header("Vídeo Demonstrativo")]
    [SerializeField] private VideoClip videoDemonstrativo;

    public string ItemID => itemID;
    public string NomeObra => nomeObra;
    public string Descricao => descricao;
    public Sprite ImagemObra => imagemObra;
    public VideoClip VideoDemonstrativo => videoDemonstrativo;
}
