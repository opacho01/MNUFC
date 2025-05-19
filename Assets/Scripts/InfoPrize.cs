/// <summary>
/// Represents detailed information about a prize won during an event.
/// </summary>
[System.Serializable]
public class InfoPrize
{
    /// <summary>
    /// The timestamp indicating when the event started.
    /// </summary>
    public string start_time;

    /// <summary>
    /// The timestamp indicating when the event ended.
    /// </summary>
    public string end_time;

    /// <summary>
    /// The total duration of gameplay in seconds.
    /// </summary>
    public float total_game_time;

    /// <summary>
    /// The total time spent on screens during the event.
    /// </summary>
    public float total_screen_time;

    /// <summary>
    /// The date on which the event took place.
    /// </summary>
    public string date;

    /// <summary>
    /// The unique identifier of the machine used in the event.
    /// </summary>
    public string machine_id;

    /// <summary>
    /// The unique identifier of the event.
    /// </summary>
    public string event_id;

    /// <summary>
    /// The total number of clicks registered during the event.
    /// </summary>
    public int total_clicks;

    /// <summary>
    /// The URL of the recorded video stored in an S3 bucket.
    /// </summary>
    public string s3_video_url;

    /// <summary>
    /// The public URL where event information or media can be accessed.
    /// </summary>
    public string public_web_url;

    /// <summary>
    /// Indicates whether this prize is a highlighted or featured reward.
    /// </summary>
    public bool is_highlight;

    /// <summary>
    /// The prize details associated with this event.
    /// </summary>
    public Prize prize;

    /// <summary>
    /// The final URL generated for sharing or accessing additional details.
    /// </summary>
    public string final_url;

    /// <summary>
    /// The unique identifier assigned to this prize record.
    /// </summary>
    public string _id;
}

/// <summary>
/// Represents a prize item with details about availability, pricing, and characteristics.
/// </summary>
[System.Serializable]
public class Prize
{
    /// <summary>
    /// The unique identifier of the slot in which the prize is stored.
    /// </summary>
    public string slot_id;

    /// <summary>
    /// The internal name of the prize.
    /// </summary>
    public string name;

    /// <summary>
    /// The display name of the prize, used in UI or public presentation.
    /// </summary>
    public string showName;

    /// <summary>
    /// The price of the prize in the relevant currency or point system.
    /// </summary>
    public float price;

    /// <summary>
    /// The name of the reward category or type associated with the prize.
    /// </summary>
    public string rewardName;

    /// <summary>
    /// The probability weight used to determine how likely the prize is to be won.
    /// </summary>
    public int probabilityWeight;

    /// <summary>
    /// Indicates whether the prize is currently in stock.
    /// </summary>
    public bool inStock;

    /// <summary>
    /// The number of units available for this prize.
    /// </summary>
    public int quantity;
}