using System.ComponentModel.DataAnnotations;

namespace APBD_zaj6.DTOs;

public record CreateAnimalRequest(
    [Required] [MaxLength(50)] string Name,
    string Description,
    [Required] [MaxLength(75)] string Category,
    [Required] [MaxLength(100)] string Area
    );

public record CreateAnimalResponse(int Id, string Name, string Description, string Category, string Area)
{
    public CreateAnimalResponse(int id, CreateAnimalRequest request) : 
        this(id, request.Name, request.Description, request.Category, request.Area){}
}