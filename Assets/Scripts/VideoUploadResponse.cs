/// <summary>
/// Represents a response received after uploading a video.
/// </summary>
[System.Serializable]
public class VideoUploadResponse
{
    /// <summary>
    /// Message indicating the status of the video upload.
    /// </summary>
    public string message;

    /// <summary>
    /// The public URL of the uploaded video stored in an S3 bucket.
    /// </summary>
    public string s3_url;

    /// <summary>
    /// The internal URL for accessing the uploaded video.
    /// </summary>
    public string s3_internal_url;

    /// <summary>
    /// The unique file key assigned to the uploaded video.
    /// </summary>
    public string file_key;

    /// <summary>
    /// The asset ID associated with the uploaded video.
    /// </summary>
    public string asset_id;
}
