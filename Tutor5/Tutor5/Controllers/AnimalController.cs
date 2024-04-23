using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Tutor5.DTOs;
using Tutor5.Models;

namespace Tutor5.Controllers;
[ApiController]
[Route("api/animals")]
//[Route("api/[controller]")]
public class AnimalController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public AnimalController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [HttpGet]
    public IActionResult GetAnimals([FromQuery] string? orderBy)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        
        //Defenicja command
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        if (string.IsNullOrEmpty(orderBy)) command.CommandText = "SELECT * FROM Animal ORDER BY Name";
        else command.CommandText = "SELECT * FROM Animal ORDER BY " + orderBy;
        
        //Wykonanie zapytania
        var reader = command.ExecuteReader();

        List<Animal> animals = new List<Animal>();

        int idAnimalOrdinal = reader.GetOrdinal("IdAnimal");
        int NameOrdinal = reader.GetOrdinal("Name"); 
        int DescriptionOrdinal = reader.GetOrdinal("Description");
        int CategoryOrdinal = reader.GetOrdinal("Category"); 
        int AreaOrdinal = reader.GetOrdinal("Area");
        
        while (reader.Read())
        {
            animals.Add(new Animal()
            {
                IdAnimal = reader.GetInt32(idAnimalOrdinal),
                Name = reader.GetString(NameOrdinal),
                Description = !reader.IsDBNull(DescriptionOrdinal) ? reader.GetString(DescriptionOrdinal) : null,
                Category = reader.GetString(CategoryOrdinal),
                Area = reader.GetString(AreaOrdinal)
            });
        }
        return Ok(animals);
    }

    [HttpPost]
    public IActionResult AddAnimal(AddAnimal addAnimal)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        
        //Sql command
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "INSERT INTO Animal(Name, Description, Category, Area) VALUES (@animalName, @animalDescription, @animalCategory, @animalArea)";
        command.Parameters.AddWithValue("@animalName", addAnimal.Name);
        command.Parameters.AddWithValue("@animalDescription", addAnimal.Description);
        command.Parameters.AddWithValue("@animalCategory", addAnimal.Category);
        command.Parameters.AddWithValue("@animalArea", addAnimal.Area);

        //wykonanie
        command.ExecuteNonQuery();

        // _repository.AddAnimal(addAnimal);
        return Created();
    }

    [HttpPut("{idAnimal}")]
    public IActionResult UpdateAnimal(int idAnimal, UpdateAnimal updateAnimal)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        
        //Sql command
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "UPDATE Animal SET ";
        bool isFirstParameter = true;
        if (string.IsNullOrEmpty(updateAnimal.Name) && string.IsNullOrEmpty(updateAnimal.Description) &&
            string.IsNullOrEmpty(updateAnimal.Category) && string.IsNullOrEmpty(updateAnimal.Area)) return BadRequest();
        
        if (!string.IsNullOrEmpty(updateAnimal.Name))
        {
            command.CommandText += "Name = @animalName";
            command.Parameters.AddWithValue("@animalName", updateAnimal.Name);
            isFirstParameter = false;
        }
        
        if (!string.IsNullOrEmpty(updateAnimal.Description))
        {
            if (isFirstParameter)
            {
                command.CommandText += "Description = @animalDescription";
                isFirstParameter = false;
            }
            else
            {
                command.CommandText += ", Description = @animalDescription";
            }
            command.Parameters.AddWithValue("@animalDescription", updateAnimal.Description);
        }
        
        if (!string.IsNullOrEmpty(updateAnimal.Category))
        {
            if (isFirstParameter)
            {
                command.CommandText += "Category = @animalCategory";
                isFirstParameter = false;
            }
            else
            {
                command.CommandText += ", Category = @animalCategory";
            }
            command.Parameters.AddWithValue("@animalCategory", updateAnimal.Category);
        }
        
        if (!string.IsNullOrEmpty(updateAnimal.Area))
        {
            if (isFirstParameter)
            {
                command.CommandText += "Area = @animalArea";
                isFirstParameter = false;
            }
            else
            {
                command.CommandText += ", Area = @animalArea";
            }
            command.Parameters.AddWithValue("@animalArea", updateAnimal.Area);
        }

        command.CommandText += " WHERE IdAnimal = " + idAnimal;
        
        //wykonanie
        command.ExecuteNonQuery();

        // _repository.AddAnimal(addAnimal);
        return Ok("Updated");
    }

    [HttpDelete("{idAnimal}")]
    public IActionResult DeleteAnimal(int idAnimal)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        
        //Sql command
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "DELETE FROM Animal WHERE IdAnimal = " + idAnimal;
        
        //wykonanie
        command.ExecuteNonQuery();

        // _repository.AddAnimal(addAnimal);
        return Ok("Deleted");
    }
}