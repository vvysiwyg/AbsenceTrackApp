using System.Data;
using Microsoft.Data.SqlClient;
using WebApiTest.Models;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/timesheets", async () =>
{
    List<Timesheet> timesheets = new List<Timesheet>();
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        try
        {
            await connection.OpenAsync();
            Console.WriteLine("Подключение открыто");
        }
        catch (SqlException ex)
        {
            Debug.WriteLine(ex.Message);
        }
        string sqlExpression = "SELECT * FROM timesheet";
        SqlCommand command = new SqlCommand(sqlExpression, connection);
        SqlDataReader reader = await command.ExecuteReaderAsync();

        if (reader.HasRows)
        {
            while (await reader.ReadAsync())
            {
                Timesheet timesheet = new Timesheet()
                {
                    Id = reader.GetInt32(0),
                    Reason = reader.GetByte(1),
                    StartDate = reader.GetDateTime(2),
                    Duration = reader.GetByte(3),
                    Discounted = reader.GetBoolean(4),
                    Description = reader.GetString(5)
                };
                timesheets.Add(timesheet);
            }
        }

        await reader.CloseAsync();
    }
    return Results.Json(timesheets);
});

app.MapGet("/api/timesheets/{id:int}", async (int id) =>
{
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        try
        {
            await connection.OpenAsync();
            Console.WriteLine("Подключение открыто");
        }
        catch (SqlException ex)
        {
            Console.WriteLine(ex.Message);
        }
        string sqlExpression = "SELECT * FROM timesheet WHERE id = (@id)";
        SqlCommand command = new SqlCommand(sqlExpression, connection)
        {
            Parameters =
            {
                new("id", id)
            }
        };
        SqlDataReader reader = await command.ExecuteReaderAsync();
        if (reader.HasRows)
        {
            Timesheet timesheet = new Timesheet();
            if (await reader.ReadAsync())
            {
                timesheet = new Timesheet()
                {
                    Id = reader.GetInt32(0),
                    Reason = reader.GetByte(1),
                    StartDate = reader.GetDateTime(2),
                    Duration = reader.GetByte(3),
                    Discounted = reader.GetBoolean(4),
                    Description = reader.GetString(5)
                };
            }
            await reader.CloseAsync();
            return Results.Json(timesheet);
        }
        else
        {
            await reader.CloseAsync();
            return Results.NotFound(new { message = "Запись не найдена" });
        }
    }
});

app.MapPost("/api/timesheets", async (Timesheet timesheet) =>
{
    Console.WriteLine(
        $"Reason = {timesheet.Reason}, StartDate = {timesheet.StartDate}, Duration = {timesheet.Duration}, Discounted = {timesheet.Discounted}, Description = {timesheet.Description}"
        );

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        try
        {
            await connection.OpenAsync();
            Console.WriteLine("Подключение открыто");
        }
        catch (SqlException ex)
        {
            Console.WriteLine(ex.Message);
        }
        string sqlExpression = "INSERT INTO [dbo].[timesheet] " +
        "([reason], " +
        "[start_date], " +
        "[duration], " +
        "[discounted], " +
        "[description]) " +
        "VALUES((@reason), (@start_date), (@duration), (@discounted), (@description)); SET @id=SCOPE_IDENTITY()";

        SqlCommand command = new SqlCommand(sqlExpression, connection)
        {
            Parameters =
            {
                new("reason", timesheet.Reason),
                new("start_date", timesheet.StartDate),
                new("duration", timesheet.Duration),
                new("discounted", timesheet.Discounted),
                new("description", timesheet.Description)
            }
        };
        SqlParameter idParam = new SqlParameter
        {
            ParameterName = "id",
            SqlDbType = SqlDbType.Int,
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(idParam);

        await command.ExecuteNonQueryAsync();

        timesheet.Id = (int)idParam.Value;

        Console.WriteLine("Данные успешно добавлены");
    }
    return timesheet;
});

app.MapPut("/api/timesheets", async (Timesheet timesheet) =>
{
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        try
        {
            await connection.OpenAsync();
            Console.WriteLine("Подключение открыто");
        }
        catch (SqlException ex)
        {
            Console.WriteLine(ex.Message);
        }
        string sqlExpression = "UPDATE [dbo].[timesheet] " +
        "SET [reason] = (@reason), " +
        "[start_date] = (@start_date), " +
        "[duration] = (@duration), " +
        "[discounted] = (@discounted), " +
        "[description] = (@description) " +
        "WHERE id = (@id)";

        SqlCommand command = new SqlCommand(sqlExpression, connection)
        {
            Parameters =
            {
                new("id", timesheet.Id),
                new("reason", timesheet.Reason),
                new("start_date", timesheet.StartDate),
                new("duration", timesheet.Duration),
                new("discounted", timesheet.Discounted),
                new("description", timesheet.Description)
            }
        };

        await command.ExecuteNonQueryAsync();

        Console.WriteLine("Данные успешно обновлены");
    }
    return Results.Json(timesheet);
});

app.MapDelete("/api/timesheets/{id:int}", async (int id) =>
{
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        try
        {
            await connection.OpenAsync();
            Console.WriteLine("Подключение открыто");
        }
        catch (SqlException ex)
        {
            Console.WriteLine(ex.Message);
        }
        string sqlExpression = "DELETE FROM [dbo].[timesheet] WHERE id = (@id)";

        SqlCommand command = new SqlCommand(sqlExpression, connection)
        {
            Parameters =
            {
                new("id", id)
            }
        };

        await command.ExecuteNonQueryAsync();

        Console.WriteLine("Данные успешно обновлены");
    }
    return Results.Json(id);
});

app.Run();
