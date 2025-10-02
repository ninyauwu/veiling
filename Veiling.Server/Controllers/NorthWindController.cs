using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

[ApiController]
[Route("[controller]")]
public class NorthWindController : ControllerBase {
    [HttpGet]
    public async Task<ActionResult<int>> Get() {
        using var connection = new SqlConnection(ConnectionStore.ConnectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand("SELECT COUNT(*) FROM Products", connection);

        var result = await command.ExecuteScalarAsync();

        int count = (result == null || result == DBNull.Value) ? 0 : (int)result;

        return Ok(count);
    }
}
