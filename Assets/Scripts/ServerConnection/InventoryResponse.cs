using System.Collections.Generic;

/// <summary>
/// Represents a response containing inventory data.
/// </summary>
public class InventoryResponse
{
	/// <summary>
	/// Specifies the type of response received.
	/// </summary>
	public string response_type { get; set; }

	/// <summary>
	/// A list of inventory products included in the response.
	/// </summary>
	public List<InventoryProduct> inventory { get; set; }
}
