using Microsoft.AspNetCore.Mvc;
using OneUni.DTOs.University;
using OneUni.Interfaces.Repositories;

namespace OneUni.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UniversitiesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UniversitiesController> _logger;

    public UniversitiesController(IUnitOfWork unitOfWork, ILogger<UniversitiesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UniversityDTO>>> GetAll()
    {
        try
        {
            var universities = await _unitOfWork.Universities.GetAllAsync();
            var universitiesDto = universities.Select(u => new UniversityDTO
            {
                Id = u.UniversityId,
                Name = u.Name,
                ShortName = u.ShortName,
                Type = u.Type,
                City = u.City,
                Province = u.Province,
                Country = u.Country,
                EstablishedYear = u.EstablishedYear,
                WebsiteUrl = u.WebsiteUrl,
                Email = u.Email,
                Phone = u.Phone,
                LogoUrl = u.LogoUrl,
                RankingNational = u.RankingNational,
                RankingInternational = u.RankingInternational,
                IsActive = u.IsActive ?? false
            });

            return Ok(universitiesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving universities");
            return StatusCode(500, "An error occurred while retrieving universities");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UniversityDTO>> GetById(Guid id)
    {
        try
        {
            var university = await _unitOfWork.Universities.GetByIdAsync(id);
            
            if (university == null)
            {
                return NotFound();
            }

            var universityDto = new UniversityDTO
            {
                Id = university.UniversityId,
                Name = university.Name,
                ShortName = university.ShortName,
                Type = university.Type,
                City = university.City,
                Province = university.Province,
                Country = university.Country,
                EstablishedYear = university.EstablishedYear,
                WebsiteUrl = university.WebsiteUrl,
                Email = university.Email,
                Phone = university.Phone,
                LogoUrl = university.LogoUrl,
                RankingNational = university.RankingNational,
                RankingInternational = university.RankingInternational,
                IsActive = university.IsActive ?? false
            };

            return Ok(universityDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving university with id {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the university");
        }
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<UniversityDTO>>> GetActive()
    {
        try
        {
            var universities = await _unitOfWork.Universities.GetActiveUniversitiesAsync();
            var universitiesDto = universities.Select(u => new UniversityDTO
            {
                Id = u.UniversityId,
                Name = u.Name,
                ShortName = u.ShortName,
                Type = u.Type,
                City = u.City,
                Province = u.Province,
                Country = u.Country,
                IsActive = u.IsActive ?? false
            });

            return Ok(universitiesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active universities");
            return StatusCode(500, "An error occurred while retrieving active universities");
        }
    }
}

