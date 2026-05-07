using Microsoft.Data.SqlClient;
using Dapper;

public class OrderWorkflowRepository
{
    private readonly string _connectionString;

    public OrderWorkflowRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<OrderWorkflowRoute> GetOrderWorkflowRoutes()
    {
        using var conn = new SqlConnection(_connectionString);

        var sql = @"SELECT StartOrderWorkflowStepName AS [From], EndOrderWorkflowStepName AS [To], RouteCode FROM dbo.OrderWorkflowRoute";

        return conn.Query<OrderWorkflowRoute>(sql);
    }

    public IEnumerable<OrderWorkflowStep> GetOrderWorkflowSteps()
    {
        // Placeholder for database retrieval logic
        return new List<OrderWorkflowStep>();
    }

}

public class OrderWorkflowRoute
{
    public required string From { get; set; }
    public required string To { get; set; }
    public string? RouteCode { get; set; }
    public int? CompanyId { get; set; }
    public string? CompanyName { get; set; }
}

public class OrderWorkflowStep
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsAutomated { get; set; }
    public bool IsActive { get; set; }
}
