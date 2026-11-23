
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Catálogo de Obras")]
    [SerializeField]
    private List<ItemData> catalogoItens =
        new List<ItemData>();

    [Header("Obras Conquistadas")]
    [SerializeField]
    private List<ItemData> itensObtidos =
        new List<ItemData>();


    // =========================
    // INICIALIZAÇÃO
    // =========================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        Debug.Log("InventoryManager iniciado!");
    }


    // =========================
    // ADICIONAR OBRA
    // =========================


    public bool AdicionarItem(ItemData item)
    {
        if (item == null)
        {
            Debug.LogWarning("Tentativa de adicionar um item nulo!");
            return false;
        }

        if (string.IsNullOrWhiteSpace(item.ItemID))
        {
            Debug.LogWarning("A obra não possui um ID válido!");
            return false;
        }

        if (PossuiItem(item.ItemID))
        {
            Debug.Log("Obra já conquistada: " + item.NomeObra);
            return false;
        }

        // Adiciona a obra ao inventário.
        itensObtidos.Add(item);

        Debug.Log("Obra conquistada: " + item.NomeObra);

        // Salva automaticamente o inventário atualizado.
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SalvarJogo();
        }
        else
        {
            Debug.LogWarning("SaveManager não encontrado. A obra não foi salva!");
        }

        return true;
    }



    // =========================
    // CONSULTAR INVENTÁRIO
    // =========================

    public bool PossuiItem(string itemID)
    {
        return itensObtidos.Exists(
            item => item != null && item.ItemID == itemID
        );
    }


    public List<ItemData> ObterItensObtidos()
    {
        return new List<ItemData>(itensObtidos);
    }


    // =========================
    // PREPARAÇÃO PARA SALVAMENTO
    // =========================

    public List<string> ObterIDsItens()
    {
        List<string> IDs = new List<string>();

        foreach (ItemData item in itensObtidos)
        {
            if (item != null && !string.IsNullOrWhiteSpace(item.ItemID))
                IDs.Add(item.ItemID);
        }

        return IDs;
    }


    // =========================
    // RECUPERAR ITENS SALVOS
    // =========================

    public void RestaurarInventario(List<string> IDsSalvos)
    {
        itensObtidos.Clear();

        if (IDsSalvos == null)
            return;

        foreach (string ID in IDsSalvos)
        {
            ItemData item = BuscarNoCatalogo(ID);

            if (item != null && !PossuiItem(item.ItemID))
            {
                itensObtidos.Add(item);
            }
        }

        Debug.Log("Inventário restaurado: " +
                  itensObtidos.Count + " obras.");
    }


    // =========================
    // LOCALIZAR OBRA NO CATÁLOGO
    // =========================

    private ItemData BuscarNoCatalogo(string itemID)
    {
        foreach (ItemData item in catalogoItens)
        {
            if (item != null && item.ItemID == itemID)
                return item;
        }

        Debug.LogWarning(
            "Obra não encontrada no catálogo: " + itemID
        );

        return null;
    }
}
