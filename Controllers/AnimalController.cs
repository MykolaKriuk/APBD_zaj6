using System.Data.SqlClient;
using APBD_zaj6.DTOs;

namespace APBD_zaj6.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

[ApiController]
[Route("controller-animals")]
public class DatabaseController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly List<string> _neededStringsForHttpGet = ["name", "description", "category", "area"];

    public DatabaseController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [HttpGet]
    public ActionResult GetAllAnimals(string orderBy = "Name")
    {
        if (!_neededStringsForHttpGet.Contains(orderBy.ToLower())) return BadRequest("Invalid 'orderBy' parameter is given.");
        var response = new List<GetAnimalsResponse>();
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            var sqlCommand = new SqlCommand($"SELECT * FROM Animal ORDER BY {orderBy}", sqlConnection);
            sqlCommand.Connection.Open();
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                response.Add(new GetAnimalsResponse(
                    reader.GetInt32(0), 
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetString(4))
                );
                
            }
        }

        return Ok(response);
    }

    [HttpGet("{id}")]
    public ActionResult GetSpecificAnimal(int id)
    {
        using var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default"));
        var sqlCommand = new SqlCommand("SELECT * FROM Animal WHERE Id_Animal = @1", sqlConnection);
        sqlCommand.Parameters.AddWithValue("@1", id);
        sqlCommand.Connection.Open();
        var reader = sqlCommand.ExecuteReader();
        if (!reader.Read()) return NotFound();
        return Ok(new GetAnimalsResponse(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetString(3),
            reader.GetString(4))
        );
    }

    [HttpPost]
    public ActionResult AddAnimal(CreateAnimalRequest request)
    {
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            var sqlCommand = new SqlCommand(
                "INSERT INTO Animal VALUES (@1, @2, @3, @4); SELECT CAST(SCOPE_IDENTITY() as int)",
                sqlConnection
            );
            sqlCommand.Parameters.AddWithValue("@1", request.Name);
            sqlCommand.Parameters.AddWithValue("@2", request.Description);
            sqlCommand.Parameters.AddWithValue("@3", request.Category);
            sqlCommand.Parameters.AddWithValue("@4", request.Area);
            sqlCommand.Connection.Open();

            var id = sqlCommand.ExecuteScalar();

            return Created($"students/{id}", new CreateAnimalResponse((int)id, request));
        }
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteAnimal(int id)
    {
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            var sqlCommand = new SqlCommand($"DELETE FROM Animal WHERE Id_Animal = @1", sqlConnection);
            sqlCommand.Parameters.AddWithValue("@1", id);
            sqlCommand.Connection.Open();

            var affectedRows = sqlCommand.ExecuteNonQuery();

            return affectedRows == 0 ? NotFound() : NoContent();
        }
    }

    [HttpPut("{id}")]
    public ActionResult UpdateAnimal(int id, UpdateAnimalRequest request)
    {
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            var sqlCommand = new SqlCommand(
                "UPDATE Animal SET Name = @1, Description = @2, Category = @3, Area = @4 WHERE Id_Animal = @5",
                sqlConnection
            );
            sqlCommand.Parameters.AddWithValue("@1", request.Name);
            sqlCommand.Parameters.AddWithValue("@2", request.Description);
            sqlCommand.Parameters.AddWithValue("@3", request.Category);
            sqlCommand.Parameters.AddWithValue("@4", request.Area);
            sqlCommand.Parameters.AddWithValue("@5", id);
            sqlCommand.Connection.Open();

            var affectedRows = sqlCommand.ExecuteNonQuery();
            
            return affectedRows == 0 ? NotFound() : NoContent();
        }
    }
}
