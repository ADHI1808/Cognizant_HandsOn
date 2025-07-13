using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters; // Needed for ServiceFilter
using Microsoft.Extensions.Logging;
using MyFirstAPI.filters; // Assuming CustomAuthFilter is in this namespace
using MyFirstAPI.models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyFirstAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(CustomAuthFilter))] // Applies CustomAuthFilter to all actions in this controller
    public class EmployeeController : ControllerBase
    {
        private static List<Employee> _employees = new List<Employee>();
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(ILogger<EmployeeController> logger)
        {
            _logger = logger;
            if (_employees.Count == 0)
            {
                _employees = GetStandardEmployeeList();
            }
        }

        private List<Employee> GetStandardEmployeeList()
        {
            var departments = new List<Department>
            {
                new Department { Id = 1, Name = "Human Resources" },
                new Department { Id = 2, Name = "Engineering" },
                new Department { Id = 3, Name = "Sales" }
            };

            var skills = new List<Skills>
            {
                new Skills { Id = 1, Name = "C#" },
                new Skills { Id = 2, Name = "JavaScript" },
                new Skills { Id = 3, Name = "SQL" },
                new Skills { Id = 4, Name = "Agile" }
            };

            var employeeList = new List<Employee>
            {
                new Employee
                {
                    Id = 1,
                    Name = "Alice Smith",
                    Salary = 60000,
                    Permanent = true,
                    Department = departments.FirstOrDefault(d => d.Id == 2),
                    Skills = new List<Skills> { skills.FirstOrDefault(s => s.Id == 1), skills.FirstOrDefault(s => s.Id == 4) },
                    DateOfBirth = new DateTime(1990, 5, 15)
                },
                new Employee
                {
                    Id = 2,
                    Name = "Bob Johnson",
                    Salary = 75000,
                    Permanent = true,
                    Department = departments.FirstOrDefault(d => d.Id == 3),
                    Skills = new List<Skills> { skills.FirstOrDefault(s => s.Id == 2) },
                    DateOfBirth = new DateTime(1988, 11, 22)
                },
                new Employee
                {
                    Id = 3,
                    Name = "Charlie Brown",
                    Salary = 55000,
                    Permanent = false,
                    Department = departments.FirstOrDefault(d => d.Id == 1),
                    Skills = new List<Skills> { skills.FirstOrDefault(s => s.Id == 3) },
                    DateOfBirth = new DateTime(1995, 1, 10)
                }
            };

            _logger.LogInformation("Initialized standard employee list.");
            return employeeList;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Employee>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public ActionResult<IEnumerable<Employee>> Get()
        {
            _logger.LogInformation("Getting all employees.");
            return Ok(_employees);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Employee))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Employee> Get(int id)
        {
            var employee = _employees.FirstOrDefault(e => e.Id == id);

            if (employee == null)
            {
                _logger.LogWarning($"Employee with ID {id} not found.");
                return NotFound();
            }

            _logger.LogInformation($"Getting employee with ID {id}.");
            return Ok(employee);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Employee))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<Employee> Post(Employee newEmployee)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for creating employee.");
                return BadRequest(ModelState);
            }

            if (_employees.Any(e => e.Id == newEmployee.Id))
            {
                _logger.LogWarning($"Employee with ID {newEmployee.Id} already exists.");
                return Conflict($"Employee with ID {newEmployee.Id} already exists.");
            }

            _employees.Add(newEmployee);
            _logger.LogInformation($"Employee '{newEmployee.Name}' created with ID {newEmployee.Id}.");

            return CreatedAtAction(nameof(Get), new { id = newEmployee.Id }, newEmployee);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Employee))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Employee> Put(int id, Employee updatedEmployee)
        {
            if (id <= 0)
            {
                _logger.LogWarning($"Attempted to update employee with invalid ID: {id}");
                return BadRequest("Invalid employee id");
            }

            var existingEmployee = _employees.FirstOrDefault(e => e.Id == id);

            if (existingEmployee == null)
            {
                _logger.LogWarning($"Employee with ID {id} not found for update.");
                return BadRequest("Invalid employee id");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid model state for updating employee with ID {id}.");
                return BadRequest(ModelState);
            }

            existingEmployee.Name = updatedEmployee.Name;
            existingEmployee.Salary = updatedEmployee.Salary;
            existingEmployee.Permanent = updatedEmployee.Permanent;
            existingEmployee.Department = updatedEmployee.Department;
            existingEmployee.Skills = updatedEmployee.Skills ?? new List<Skills>();
            existingEmployee.DateOfBirth = updatedEmployee.DateOfBirth;

            _logger.LogInformation($"Employee with ID {id} updated successfully.");

            return Ok(existingEmployee);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            var employeeToDelete = _employees.FirstOrDefault(e => e.Id == id);

            if (employeeToDelete == null)
            {
                _logger.LogWarning($"Employee with ID {id} not found for deletion.");
                return NotFound();
            }

            _employees.Remove(employeeToDelete);
            _logger.LogInformation($"Employee with ID {id} deleted successfully.");
            return NoContent();
        }
    }
}