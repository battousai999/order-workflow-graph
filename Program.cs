using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

var connectionString = config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

var repository = new OrderWorkflowRepository(connectionString);
var graphGenerator = new OrderWorkflowGraphGenerator(repository);

var graphResults = graphGenerator.GenerateGraph();
var graphRenderer = new GraphRenderer();

var topNode = graphResults.Routes.FirstOrDefault(x => x.From == "CheckSKU")?.From ?? null;
var outputFilename = $"order-workflow_{DateTime.Now:yyyyMMdd_HHmmss}.svg";

graphRenderer.RenderGraph(graphResults.Graph, outputFilename, topNode);

Console.WriteLine($"Graph rendered to {outputFilename}");

