using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

public class UserService
{
    private readonly string _connectionString;

    public UserService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new InvalidOperationException("The connection string has not been initialized.");
        }
    }

    public async Task<bool> ValidateUserAsync(string username, string password)
    {
        // WARNING: This method is vulnerable to SQL Injection
        string query = $"SELECT COUNT(1) FROM Users WHERE Username = '{username}' AND PasswordHash = '{password}'";

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(query, connection))
            {
                var result = (int)await command.ExecuteScalarAsync();
                return result > 0;
            }
        }
    }

    public async Task<bool> CreateUserAsync(string username, string password)
    {
        // WARNING: This method is vulnerable to SQL Injection
        string query = $"INSERT INTO Users (Username, PasswordHash) VALUES ('{username}', '{password}')";

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(query, connection))
            {
                var result = await command.ExecuteNonQueryAsync();
                return result > 0;
            }
        }
    }

    public async Task<User> GetUserAsync(string username)
    {
        // WARNING: This method is vulnerable to SQL Injection
        string query = $"SELECT Username, PasswordHash FROM Users WHERE Username = '{username}'";

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(query, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new User
                        {
                            Username = reader.GetString(0),
                            PasswordHash = reader.GetString(1)
                        };
                    }
                }
            }
        }
        return null;
    }

    public async Task<bool> UpdateUserAsync(string username, string newPassword)
    {
        // WARNING: This method is vulnerable to SQL Injection
        string query = $"UPDATE Users SET PasswordHash = '{newPassword}' WHERE Username = '{username}'";

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(query, connection))
            {
                var result = await command.ExecuteNonQueryAsync();
                return result > 0;
            }
        }
    }

    public async Task<bool> DeleteUserAsync(string username)
    {
        // WARNING: This method is vulnerable to SQL Injection
        string query = $"DELETE FROM Users WHERE Username = '{username}'";

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(query, connection))
            {
                var result = await command.ExecuteNonQueryAsync();
                return result > 0;
            }
        }
    }
}

public class User
{
    public string Username { get; set; }
    public string PasswordHash { get; set; }
}
