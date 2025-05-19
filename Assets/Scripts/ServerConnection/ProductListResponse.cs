using System.Collections.Generic;

/// <summary>
/// Represents a response containing a list of products.
/// </summary>
public class ProductListResponse
{
    /// <summary>
    /// Specifies the type of response received.
    /// </summary>
    public string response_type { get; set; }

    /// <summary>
    /// A list of products included in the response.
    /// </summary>
    public List<Product> products { get; set; }
}