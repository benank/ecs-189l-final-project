using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private List<int> ItemsHeld = new List<int>();
    [SerializeField] private Transform ItemDropSpawnPoint;
    [SerializeField] private string ItemSpawnPointTag = "Loot Spawn Point";
    [SerializeField] private string LootableObjectTag = "Lootable";

    private void Start()
    {
        // Set spawn point for all loot.
        // Set to transform position of object, unless a specific point was chosen in its child objects.
        var transformChildren = this.gameObject.GetComponentsInChildren<Transform>();
        ItemDropSpawnPoint = this.transform;
        foreach (var child in transformChildren)
        {
            if(child.gameObject.CompareTag(ItemSpawnPointTag)){
                ItemDropSpawnPoint = child;
                break;
            }
        }
        // Check if this object is a lootable object (or is a child of one).
        // Randomize its loot is yes.
        if (this.gameObject.CompareTag(LootableObjectTag) || this.transform.parent.CompareTag(LootableObjectTag))
        {
            CreateRandomItemTable();
        }
    }

    /*
     * Creates a List of Items to attach to object.
     */
    public void CreateRandomItemTable()
    {
        int numItems = Random.Range(1, 5);
        int numItemTypes = ItemTypes.instance.Count;

        for (int i = 0; i < numItems; ++i)
        {
            ItemsHeld.Add(Random.Range(0, numItemTypes - 1));
        }
    }
    /*
     * Instantiate all items with attached behaviour script 
     */
    public void DropItems()
    {
        var prefabList = ItemTypes.instance.ObjectTypes;
        foreach (var itemIndex in ItemsHeld)
        {
            GameObject instance = Instantiate(prefabList[itemIndex]) as GameObject;
            var itemBehaviour = instance.AddComponent<ItemBehaviour>();
            itemBehaviour.ItemIndex = itemIndex;

            instance.transform.SetPositionAndRotation(ItemDropSpawnPoint.position, ItemDropSpawnPoint.rotation);
        }
        ItemsHeld.Clear();
    }

    public void CollectItem(int itemIndex)
    {
        ItemsHeld.Add(itemIndex);
    }
    public string ItemsToString()
    {
        string text = "";
        for(int i = 0; i < ItemTypes.instance.Count; ++i)
        {
            var subCount = GetCountOfItemType(i);
            if (subCount > 0)
            {
                text += ItemTypes.instance.ObjectTypes[i].name;
                text += ": ";
                text += subCount;
                text += "  ";
            }
        }

        return text;
    }
    public int GetCountOfItemType(int itemIndex)
    {
        int count = 0;
        foreach (var item in ItemsHeld)
        {
            if(item == itemIndex)
            {
                ++count;
            }
        }
        return count;
    }
    public List<int> GetInventory()
    {
        return ItemsHeld;
    }
    public void SetInventory(List<int> items)
    {
        ItemsHeld = items;
    }
}