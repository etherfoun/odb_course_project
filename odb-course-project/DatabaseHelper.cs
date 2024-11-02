using Npgsql;
using System;
using System.Data;

public class DatabaseHelper
{
    private string connectionString = "Host=localhost;Port=5432;Username=your_user;Password=your_password;Database=HotelDB";

    public DataTable ExecuteQuery(string query)
    {
        using var connection = new NpgsqlConnection(connectionString);
        using var command = new NpgsqlCommand(query, connection);
        var dataTable = new DataTable();
        connection.Open();
        using var reader = command.ExecuteReader();
        dataTable.Load(reader);
        return dataTable;
    }

    public int ExecuteNonQuery(string query)
    {
        using var connection = new NpgsqlConnection(connectionString);
        using var command = new NpgsqlCommand(query, connection);
        connection.Open();
        return command.ExecuteNonQuery();
    }
}
