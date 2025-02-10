using BlobStorageApi.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;

public class CosmosDbService
{
    private CosmosClient _cosmosClient;
    private Database _database;

    public CosmosDbService(IConfiguration configuration)
    {
        var connectionString = configuration["CosmosDb:ConnectionString"];
        _cosmosClient = new CosmosClient(connectionString);
    }

    public async Task<Database> CreateDatabaseAsync(string databaseName)
    {
        _database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
        return _database;
    }

    public async Task DeleteDatabaseAsync(string databaseName)
    {
        await _cosmosClient.GetDatabase(databaseName).DeleteAsync();
    }

    public async Task<Container> CreateContainerAsync(string databaseName, string containerName)
    {
        var database = _cosmosClient.GetDatabase(databaseName);
        var containerResponse = await database.CreateContainerIfNotExistsAsync(containerName, "/id");
        return containerResponse;
    }

    public async Task CreateDocument(string databaseName, string container)
    {
        var item = new { id = "2", Name = "Item1", Description = "This is a sample item." };

        await _cosmosClient.GetContainer(databaseName, container).CreateItemAsync(item);

    }

    public async Task<object> ReadItemAsync(string databaseName, string containerName, string id)
    {
        try
        {
            var container = _cosmosClient.GetContainer(databaseName, containerName);
            var response = await container.ReadItemAsync<Object>(id, new PartitionKey("id"));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Console.WriteLine($"Item with id {id} not found");
            return null;
        }
    }

    /// <summary>
    /// It creates a new stored procedure in the specified container
    /// </summary>
    /// <param name="databaseName">Data base name</param>
    /// <param name="containerName">Container Name</param>
    /// <param name="storedProcedureName">Store Procedure Name</param>
    /// <param name="storedProcedureBody">Store Procedure Body (JavaScript)</param>
    /// <returns>Task<StoredProcedureResponse></returns>
    public async Task<StoredProcedureResponse> CreateStoredProcedureAsync(string databaseName, string containerName, string storedProcedureName, string storedProcedureBody)
    {
        var container = _cosmosClient.GetContainer(databaseName, containerName);

        var storedProcedureProperties = new StoredProcedureProperties
        {
            Id = storedProcedureName,
            Body = storedProcedureBody
        };

        // Crear o actualizar el Stored Procedure
        var response = await container.Scripts.CreateStoredProcedureAsync(storedProcedureProperties);
        return response;
    }

    /// <summary>
    /// It executes a stored procedure in the specified container
    /// </summary>
    /// <param name="databaseName"></param>
    /// <param name="containerName"></param>
    /// <param name="storedProcedureName"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public async Task ExecuteStoredProcedureAsync(string databaseName, string containerName, string storedProcedureName, dynamic[] items)
    {
        var container = _cosmosClient.GetContainer(databaseName, containerName);
        var partitionKey = new PartitionKey("items"); // Valor de partición usado en todos los documentos
        var response = await container.Scripts.ExecuteStoredProcedureAsync<dynamic>(
            storedProcedureName,
            partitionKey,
            new object[] { items });

        Console.WriteLine($"Stored procedure executed successfully with response: {response.Resource}");
    }

    public async Task ExecuteSecondStoredProcedureAsync(StoredProcedureRequest request, dynamic[] items)
    {
        var container = _cosmosClient.GetContainer(request.DatabaseName, request.ContainerName);
        var partitionKey = new PartitionKey(request.PartitionName); 

        var response = await container.Scripts.ExecuteStoredProcedureAsync<dynamic>(
            request.ProcedureName,
            partitionKey,
            new object[] { items });

        Console.WriteLine($"Stored procedure executed successfully with response: {response.Resource}");
    }

    /// <summary>
    /// Start the Change Feed Processor to listen for changes and process them with the HandleChangesAsync implementation.
    /// </summary>
    private  async Task<ChangeFeedProcessor> StartChangeFeedProcessorAsync(
        CosmosClient cosmosClient,
        IConfiguration configuration)
    {
        string databaseName = "";
        string sourceContainerName = "";
        string leaseContainerName = configuration["LeasesContainerName"];

        Container leaseContainer = _cosmosClient.GetContainer(databaseName, leaseContainerName);
        ChangeFeedProcessor changeFeedProcessor = cosmosClient.GetContainer(databaseName, sourceContainerName)
            .GetChangeFeedProcessorBuilder<ToDoItem>(processorName: "changeFeedSample", onChangesDelegate: HandleChangesAsync)
                .WithInstanceName("consoleHost")
                .WithLeaseContainer(leaseContainer)
                .Build();

        Console.WriteLine("Starting Change Feed Processor...");
        await changeFeedProcessor.StartAsync();
        Console.WriteLine("Change Feed Processor started.");
        return changeFeedProcessor;
    }

    /// <summary>
    /// The delegate receives batches of changes as they are generated in the change feed and can process them.
    /// </summary>
    static async Task HandleChangesAsync(
        ChangeFeedProcessorContext context,
        IReadOnlyCollection<ToDoItem> changes,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Started handling changes for lease {context.LeaseToken}...");
        Console.WriteLine($"Change Feed request consumed {context.Headers.RequestCharge} RU.");
        // SessionToken if needed to enforce Session consistency on another client instance
        Console.WriteLine($"SessionToken ${context.Headers.Session}");

        // We may want to track any operation's Diagnostics that took longer than some threshold
        if (context.Diagnostics.GetClientElapsedTime() > TimeSpan.FromSeconds(1))
        {
            Console.WriteLine($"Change Feed request took longer than expected. Diagnostics:" + context.Diagnostics.ToString());
        }

        foreach (ToDoItem item in changes)
        {
            //Console.WriteLine($"Detected operation for item with id {item.id}, created at {item.creationTime}.");
            // Simulate some asynchronous operation
            await Task.Delay(10);
        }

        Console.WriteLine("Finished handling changes.");
    }









}
