using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;

namespace DapperCrud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _config;
        public EmployeeController(IConfiguration config)
        {
            _config = config;
        }

        private static async Task<IEnumerable<Employee>> SelectAllEmployees(SqlConnection connection)
        {
            return await connection.QueryAsync<Employee>("Select * From Employee");
        }

        [HttpGet]

        public async Task<ActionResult<List<Employee>>> GetAllEmployees()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Employee> employees = await SelectAllEmployees(connection);
            return Ok(employees);
        }



        [HttpGet("{empId}")]

        public async Task<ActionResult<Employee>> GetEmployee(int empId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var employee = await connection.QueryFirstAsync<Employee>("Select * From Employee Where Id = @Id", new { Id = empId });
            return Ok(employee);
        }

        [HttpPost]

        public async Task<ActionResult<List<Employee>>> CreateEmployee(Employee emp)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Insert into Employee(Id,FirstName,LastName,City) values (@Id,@FirstName,@LastName,@City)", emp);
            return Ok(await SelectAllEmployees(connection));
        }

        [HttpPut]

        public async Task<ActionResult<List<Employee>>> UpdateEmployee(Employee emp)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Update Employee set FirstName=@FirstName,LastName=@LastName,City=@City where Id=@Id", emp);
            return Ok(await SelectAllEmployees(connection));
        }

        [HttpDelete("{empId}")]

        public async Task<ActionResult<Employee>> DeleteEmployee(int empId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Delete From Employee Where Id=@Id", new {Id=empId});
            return Ok(await SelectAllEmployees(connection));
        }

    }
}
