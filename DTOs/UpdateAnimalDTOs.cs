using System.ComponentModel.DataAnnotations;

namespace APBD_zaj6.DTOs;

public record UpdateAnimalRequest(
    [Required] [MaxLength(50)] string Name,
    string Description,
    [Required] [MaxLength(75)] string Category,
    [Required] [MaxLength(100)] string Area
    );