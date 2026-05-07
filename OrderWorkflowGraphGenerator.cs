using Microsoft.Msagl.Drawing;

public class OrderWorkflowGraphGenerator
{
    private readonly OrderWorkflowRepository _repository;

    public OrderWorkflowGraphGenerator(OrderWorkflowRepository repository)
    {
        _repository = repository;
    }

    public GraphResults GenerateGraph()
    {
        var routes = _repository.GetOrderWorkflowRoutes();
        var steps = _repository.GetOrderWorkflowSteps();

        var graph = new Graph("Order Workflow");

        foreach (var route in routes)
        {
            graph.AddEdge(route.From, route.RouteCode ?? string.Empty, route.To);
        }

        graph.Attr.LayerDirection = LayerDirection.TB;

        return new GraphResults { Graph = graph, Routes = routes.ToList(), Steps = steps.ToList() };
    }
}

public class GraphResults
{
    public required Graph Graph { get; set; }
    public List<OrderWorkflowRoute> Routes { get; set; } = [];
    public List<OrderWorkflowStep> Steps { get; set; } = [];
}
