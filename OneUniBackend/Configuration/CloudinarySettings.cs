namespace OneUniBackend.Configuration;

public class CloudinarySettings
{
    public string CloudName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    public string Folder { get; set; } = "oneuni/students";
    public int PresignExpiryMinutes { get; set; } = 15;
    public long MaxUploadBytes { get; set; } = 10 * 1024 * 1024;
    public string[] AllowedContentTypes { get; set; } = new[] { "application/pdf", "image/jpeg", "image/png" };
}

