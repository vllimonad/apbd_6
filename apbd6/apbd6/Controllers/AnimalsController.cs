using apbd6.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Tutorial5.Models.DTOs;

namespace apbd6.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimalsController : ControllerBase
{
    private readonly IConfiguration _iconfiguration;

    public AnimalsController(IConfiguration configuration)
    {
        _iconfiguration = configuration;
    }

    [HttpGet]
    public IActionResult GetAnimals(string? orderBy)
    {
        // Open connection
        using SqlConnection connection = new SqlConnection(_iconfiguration.GetConnectionString("Docker"));
        connection.Open();

        if (orderBy == null)
        {
            orderBy = "name";
        }
        
        // Create command
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "select * from Animal order by @order asc";
        command.Parameters.AddWithValue("@order", orderBy);
        
        // Execute command
        var reader = command.ExecuteReader();

        var animals = new List<Animal>();

        int idAnimalOrdinal = reader.GetOrdinal("IdAnimal");
        int nameOrdinal = reader.GetOrdinal("Name");

        while (reader.Read())
        {
            animals.Add(new Animal()
            {
                IdAnimal = reader.GetInt32(idAnimalOrdinal),
                Name = reader.GetString(nameOrdinal)
            });
        }

        //var animals = _animalRepository.GetAnimals();
        
        return Ok(animals);
    }
    
    [HttpPost]
    public IActionResult AddAnimal(AddAnimal animal)
    {
        // Open connection
        using SqlConnection connection = new SqlConnection(_iconfiguration.GetConnectionString("Docker"));
        connection.Open();
        
        // Create command
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "insert into Animal values (@animalName,@animalDescription,@animalCategory,@animalArea)";
        command.Parameters.AddWithValue("@animalName", animal.Name);
        command.Parameters.AddWithValue("@animalDescription", animal.Description);
        command.Parameters.AddWithValue("@animalCategory", animal.Category);
        command.Parameters.AddWithValue("@animalArea", animal.Area);
        
        
        // Execute command
        command.ExecuteNonQuery();

        return Created("", animal);
    }
    
    [HttpPut("{IdAnimal}")]
    public IActionResult UpdateAnimal(int IdAnimal, UpdateAnimal animal)
    {
        // Open connection
        using SqlConnection connection = new SqlConnection(_iconfiguration.GetConnectionString("Docker"));
        connection.Open();
        
        // Create command
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "update Animal set Name = @animalName, Description = @animalDescription, Category = @animalCategory, Area = @animalArea where IdAnimal = @animalId";
        command.Parameters.AddWithValue("@animalName", animal.Name);
        command.Parameters.AddWithValue("@animalDescription", animal.Description);
        command.Parameters.AddWithValue("@animalCategory", animal.Category);
        command.Parameters.AddWithValue("@animalArea", animal.Area);
        command.Parameters.AddWithValue("@animalId", IdAnimal);
        
        // Execute command
        var updatedRows = command.ExecuteNonQuery();
        if (updatedRows == 0) return NotFound();
        return NoContent();
    }
    
    [HttpDelete("{IdAnimal}")]
    public IActionResult DeleteAnimal(int IdAnimal)
    {
        // Open connection
        using SqlConnection connection = new SqlConnection(_iconfiguration.GetConnectionString("Docker"));
        connection.Open();
        
        // Create command
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "delete from Animal where IdAnimal = @animalId)";
        command.Parameters.AddWithValue("@animalId", IdAnimal);
        
        // Execute command
        command.ExecuteNonQuery();

        var updatedRows = command.ExecuteNonQuery();
        if (updatedRows == 0) return NotFound();
        return NoContent();
    }
}