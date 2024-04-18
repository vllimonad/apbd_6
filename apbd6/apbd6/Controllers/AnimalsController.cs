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
        using SqlConnection connection = new SqlConnection(_iconfiguration.GetConnectionString("Default"));
        connection.Open();

        if (orderBy == null)
        {
            orderBy = "name";
        }
        
        // Create command
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT * FROM Animal order by @order asc";
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
        using SqlConnection connection = new SqlConnection(_iconfiguration.GetConnectionString("Default"));
        connection.Open();
        
        // Create command
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "INSERT INTO Animal VALUES (@animalName,'','','')";
        command.Parameters.AddWithValue("@animalName", animal.Name);
        
        // Execute command
        command.ExecuteNonQuery();

        return Created("", null);
    }
    
    
}