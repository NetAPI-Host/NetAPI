using Microsoft.AspNetCore.Mvc;
using NetApi.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        // Path to store the data
        private const string FilePath = "users.json";

        // In-memory data list
        private static List<User> _users = new List<User>();

        // Constructor to load data from the file
        public UsersController()
        {
            LoadFromFile();
        }

        // Helper method: Load data from the file
        private void LoadFromFile()
        {
            if (System.IO.File.Exists(FilePath))
            {
                var jsonData = System.IO.File.ReadAllText(FilePath);
                _users = JsonConvert.DeserializeObject<List<User>>(jsonData) ?? new List<User>();
            }
        }

        // Helper method: Save data to the file
        private void SaveToFile()
        {
            var jsonData = JsonConvert.SerializeObject(_users, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(FilePath, jsonData);
        }

        // GET: api/users
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return Ok(_users);
        }

        // GET: api/users/username?id={id}
        [HttpGet("username")]
        public ActionResult<string> GetUsernameById([FromQuery] int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user.Name);
        }

        // POST: api/users
        [HttpPost]
        public ActionResult<int> AddUser([FromBody] User newUser)
        {
            if (newUser == null || string.IsNullOrWhiteSpace(newUser.Name))
            {
                return BadRequest("Invalid user data.");
            }

            newUser.Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;

            _users.Add(newUser);
            SaveToFile();

            // Return the ID of the new user
            return Ok(newUser.Id);
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateUser(int id, [FromBody] User updatedUser)
        {
            var existingUser = _users.FirstOrDefault(u => u.Id == id);
            if (existingUser == null)
            {
                return NotFound("User not found.");
            }

            if (string.IsNullOrWhiteSpace(updatedUser.Name))
            {
                return BadRequest("Name cannot be empty.");
            }

            existingUser.Name = updatedUser.Name;
            SaveToFile();

            return NoContent();
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            _users.Remove(user);
            SaveToFile();

            return NoContent();
        }
    }
}
