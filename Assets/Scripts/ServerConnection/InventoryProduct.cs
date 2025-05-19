/// <summary>
/// Represents a product in inventory, including its name, quantity, and associated slot.
/// </summary>
[System.Serializable]
public class InventoryProduct
{
    /// <summary>
    /// The name of the product in inventory.
    /// </summary>
    public string product_name { get; set; }

    /// <summary>
    /// The available quantity of the product.
    /// </summary>
    public int quantity { get; set; }

    /// <summary>
    /// The slot where the product is stored.
    /// </summary>
    public string slot { get; set; }
}