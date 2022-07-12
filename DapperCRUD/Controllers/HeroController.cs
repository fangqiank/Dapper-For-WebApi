using Dapper;
using DapperCRUD.Model;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace DapperCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeroController : ControllerBase
    {
        private readonly IConfiguration _config;

        public HeroController(IConfiguration config)
        {
            _config = config;
        }

        private async Task<IEnumerable<SuperHero>> SelectAllHeroes(SqlConnection connection)
        {
            return await connection.QueryAsync<SuperHero>("select * from dbo.SuperHeroes");
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetAllHeroes()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var results = await connection.QueryAsync<SuperHero>("select * from dbo.SuperHeroes");

            return Ok(results);
        }

        [HttpGet("{heroId}")]
        public async Task<ActionResult<SuperHero>> GetHero(int heroId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var result = await connection.QueryFirstAsync<SuperHero>(
                "select * from dbo.SuperHeroes where id = @Id", new {Id = heroId });

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<SuperHero>> AddHero(SuperHero hero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync(@"Insert dbo.SuperHeroes (Name, FirstName, LastName, Place) 
                values(@Name, @FirstName, @LastName, @Place)", hero);

            return Ok(await SelectAllHeroes(connection));
        }

        [HttpPut]
        public async Task<ActionResult<SuperHero>> UpdateHero(SuperHero hero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync(@"update dbo.SuperHeroes set 
                Name=@Name, FirstName=@FirstName, LastName=@LastName, 
                Place=@Place where Id = @Id" , hero);

            return Ok(await SelectAllHeroes(connection));
        }

        [HttpDelete("{heroId}")]
        public async Task<ActionResult<SuperHero>> DeleteHero(int heroId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync(@"delete from dbo.SuperHeroes where Id = @Id", new {Id = heroId});

            return Ok(await SelectAllHeroes(connection));
        }
    }
}
