namespace OneUniBackend.DTOs.University;

public class UniversityDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? ShortName { get; set; }
    public string? Type { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? Country { get; set; }
    public int? EstablishedYear { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? LogoUrl { get; set; }
    public int? RankingNational { get; set; }
    public int? RankingInternational { get; set; }
    public bool IsActive { get; set; }
}

