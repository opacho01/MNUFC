/// <summary>
/// Represents the response received after a vending request.
/// </summary>
public class VendResponse
{
    /// <summary>
    /// Specifies the type of response received.
    /// </summary>
    public string response_type { get; set; }

    /// <summary>
    /// Indicates the status of the vending operation (e.g., "Success", "Failed").
    /// </summary>
    public string status { get; set; }

    /// <summary>
    /// Provides additional information or an error message related to the vending operation.
    /// </summary>
    public string message { get; set; }
}
