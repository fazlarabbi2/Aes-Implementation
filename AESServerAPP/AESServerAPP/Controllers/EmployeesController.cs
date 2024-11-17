using AESServerAPP.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AESServerAPP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        public static List<Employee> _employees = new List<Employee>
        {
            new Employee{Id=1,Name="Fozle Rabbi", Salary=110000},
            new Employee{Id=2, Name="Sobuj", Salary=100000},
            new Employee{Id=3, Name="Shahin", Salary=50000}
        };

        // Endpoint to get all employees, results are encrypted.
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetEmployee()
        {
            // Extract 'ClientId' from request headers or use 'DefaultClient' if not present.
            var clientId = Request.Headers["ClientId"].FirstOrDefault() ?? "DefaultClient";

            // Serialize and encrypt the list of employees using AES.
            var encryptedEmployees = AesEncryptionService.EncryptString(clientId, JsonSerializer.Serialize(_employees));

            // Return the encrypted string as a successful result.
            return Ok(encryptedEmployees);
        }

        // Endpoint to get a single employee by ID, result is encrypted.
        [HttpGet("{id}")]
        public ActionResult<string> GetEmployee(int id)
        {
            // Extract 'ClientId' from request headers or use 'DefaultClient' if not present.
            var clientId = Request.Headers["ClientId"].FirstOrDefault() ?? "DefaultClient";

            // Find the employee by ID.
            var employee = _employees.FirstOrDefault(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }
            // Serialize and encrypt the employee's data.
            var encryptedEmployee = AesEncryptionService.EncryptString(clientId, JsonSerializer.Serialize(employee));

            return Ok(encryptedEmployee);
        }


        [HttpPost]
        public ActionResult<string> PostEmployee([FromBody] string encryptedEmployee)
        {
            // Endpoint to add a new employee, input and output are encrypted.
            var clientId = Request.Headers["ClientId"].FirstOrDefault() ?? "DefaultClient";

            // Decrypt and deserialize the employee data from the request.
            var decryptedEmployee = JsonSerializer.Deserialize<Employee>(AesEncryptionService.DecryptString(clientId, encryptedEmployee));

            if (decryptedEmployee == null)
            {
                return BadRequest("Invalid Employee Data");
            }

            // Assign a new ID to the employee and add them to the list.
            decryptedEmployee.Id = _employees.Max(e => e.Id) + 1;
            _employees.Add(decryptedEmployee);

            // Encrypt and serialize the new employee's data to send back.
            var encryptedResponse = AesEncryptionService.EncryptString(clientId, JsonSerializer.Serialize(decryptedEmployee));


            return CreatedAtAction("GetEmployee", new { Id = decryptedEmployee.Id }, encryptedResponse);
        }

        // Endpoint to update an existing employee, input is encrypted.
        [HttpPut("{id}")]
        public IActionResult PutEmployee(int id, [FromBody] string encryptedEmployee)
        {
            // Extract 'ClientId' from request headers or use 'DefaultClient' if not present.
            var clientId = Request.Headers["ClientId"].FirstOrDefault() ?? "DefaultClient";

            // Decrypt and deserialize the incoming employee data.
            var decryptedEmployee = JsonSerializer.Deserialize<Employee>(AesEncryptionService.DecryptString(clientId, encryptedEmployee));

            var existingEmployee = _employees.FirstOrDefault(e => e.Id == id);

            if (existingEmployee == null)
            {
                return NotFound();
            }

            // Update the existing employee's details.
            existingEmployee.Name = decryptedEmployee.Name;
            existingEmployee.Salary = decryptedEmployee.Salary;

            return NoContent();
        }


        // Endpoint to delete an employee by ID.
        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            var employee = _employees.FirstOrDefault(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            //Remove the employee from the list.
            _employees.Remove(employee);
            return NoContent();
        }
    }
}
