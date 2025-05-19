using System.Collections.Generic;

/// <summary>
/// Represents detailed information about a vending or gaming machine, including slots and event tracking.
/// </summary>
[System.Serializable]
public class MachineData
{
    /// <summary>
    /// Unique identifier for the machine.
    /// </summary>
    public string _id;

    /// <summary>
    /// Hardware-specific identifier for the machine.
    /// </summary>
    public string hardware_id;

    /// <summary>
    /// Name of the machine.
    /// </summary>
    public string name;

    /// <summary>
    /// Type of machine (e.g., vending, gaming).
    /// </summary>
    public string type;

    /// <summary>
    /// Physical location where the machine is deployed.
    /// </summary>
    public string location;

    /// <summary>
    /// Current status of the machine (e.g., active, inactive).
    /// </summary>
    public string status;

    /// <summary>
    /// Specifications or configuration details of the machine.
    /// </summary>
    public string specs;

    /// <summary>
    /// Identifier linking the machine to a client or organization.
    /// </summary>
    public string client_id;

    /// <summary>
    /// Identifier for the currently active event associated with the machine.
    /// </summary>
    public string active_event_id;

    /// <summary>
    /// List of slot IDs available in the machine.
    /// </summary>
    public List<string> slots;

    /// <summary>
    /// Timestamp indicating when the machine data was created.
    /// </summary>
    public string created_at;

    /// <summary>
    /// Timestamp indicating the last update to the machine data.
    /// </summary>
    public string updated_at;

    /// <summary>
    /// Identifier for an event linked to the machine.
    /// </summary>
    public string event_id;

    /// <summary>
    /// List of detailed slot information associated with the machine.
    /// </summary>
    public List<SlotData> slots_data;

    /// <summary>
    /// The theme settings applied to the machine.
    /// </summary>
    public Theme theme;
}

/// <summary>
/// Represents detailed information about a slot within a vending or gaming machine.
/// </summary>
[System.Serializable]
public class SlotData
{
    /// <summary>
    /// Unique identifier for the slot data entry.
    /// </summary>
    public string _id;

    /// <summary>
    /// Internal name of the slot.
    /// </summary>
    public string name;

    /// <summary>
    /// Display name of the slot used for UI or presentation.
    /// </summary>
    public string showName;

    /// <summary>
    /// Price of the product or item in the slot.
    /// </summary>
    public float price;

    /// <summary>
    /// The reward type or name associated with the slot.
    /// </summary>
    public string rewardName;

    /// <summary>
    /// Probability weight affecting the chances of winning or selection.
    /// </summary>
    public int probabilityWeight;

    /// <summary>
    /// Indicates whether the slot contains items that are in stock.
    /// </summary>
    public bool inStock;

    /// <summary>
    /// The quantity of items available in the slot.
    /// </summary>
    public int quantity;

    /// <summary>
    /// Identifier linking the slot to a specific machine.
    /// </summary>
    public string machine_id;

    /// <summary>
    /// Unique identifier for the slot itself.
    /// </summary>
    public string slot_id;

    /// <summary>
    /// Timestamp indicating when the slot data was created.
    /// </summary>
    public string created_at;
}
