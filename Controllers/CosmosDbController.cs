using BlobStorageApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlobStorageApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CosmosDbController : ControllerBase
    {
        private readonly CosmosDbService _cosmosDbService;

        public CosmosDbController(CosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpPost("createDatabase")]
        public async Task<IActionResult> CreateDatabase(string databaseName)
        {
            var result = await _cosmosDbService.CreateDatabaseAsync(databaseName);
            return Ok(result);
        }

        [HttpDelete("deleteDatabase")]
        public async Task<IActionResult> DeleteDatabase(string databaseName)
        {
            await _cosmosDbService.DeleteDatabaseAsync(databaseName);
            return Ok();
        }

        [HttpPost("createContainer")]
        public async Task<IActionResult> CreateContainer(string databaseName, string containerName)
        {
            var result = await _cosmosDbService.CreateContainerAsync(databaseName, containerName);
            return Ok(result);
        }

        [HttpPost("createDocument")]
        public async Task<IActionResult> CreateDocument(string databaseName, string container)
        {
            await _cosmosDbService.CreateDocument(databaseName, container);
            return Ok();
        }

        [HttpGet("readItem")]
        public async Task<IActionResult> ReadItem(string databaseName, string container, string id)
        {
            var result = await _cosmosDbService.ReadItemAsync(databaseName, container, id);
            return Ok(result);
        }

        [HttpPost("createStoredProcedure")]
        public async Task<IActionResult> CreateStoredProcedure(string databaseName, string containerName, string procedureName)
        {
            string storedProcedureCode = @"
                    function insertItems(items) {
                            var context = getContext();
                            var collection = context.getCollection();
                            var response = context.getResponse();
    
                            // Variable para llevar un conteo de las inserciones
                            var count = 0;

                            // Función para intentar insertar un elemento
                            function tryCreate(item) {
                                var isAccepted = collection.createDocument(collection.getSelfLink(), item, function (err, doc) {
                                    if (err) throw new Error('Error inserting item: ' + err.message);
                                    count++;
            
                                    // Llamar a la función recursivamente para el siguiente elemento
                                    if (count < items.length) {
                                        tryCreate(items[count]);
                                    } else {
                                        response.setBody('Todos los elementos se insertaron correctamente');
                                    }
                                });

                                if (!isAccepted) {
                                    throw new Error('La solicitud de inserción fue rechazada.');
                                }
                            }

                            // Empezar con el primer elemento
                            if (items.length > 0) {
                                tryCreate(items[count]);
                            } else {
                                response.setBody('No se proporcionaron elementos para insertar.');
                            }
                        }";

            var createResponse = await _cosmosDbService.CreateStoredProcedureAsync(databaseName, containerName, procedureName, storedProcedureCode);
            return Ok(createResponse);
        }


        [HttpPost("executeStoredProcedure")]
        public async Task<IActionResult> ExecuteStoredProcedure([FromBody]  StoredProcedureRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var itemsToInsert = new dynamic[]
                {
                    new { id = "101", category = "enlatados", name = "Lata de frijoles negros", description = "Frijoles negros enlatados de 400g", price = 1.50, brand = "Marca A" },
                    new { id = "102", category = "enlatados", name = "Lata de maíz dulce", description = "Maíz dulce enlatado de 300g", price = 1.20, brand = "Marca B" },
                    new { id = "103", category = "enlatados", name = "Lata de atún", description = "Atún en agua de 150g", price = 2.00, brand = "Marca C" },
                    new { id = "104", category = "enlatados", name = "Sopa de tomate", description = "Sopa de tomate enlatada de 500ml", price = 1.75, brand = "Marca D" },
                    new { id = "105", category = "enlatados", name = "Lata de garbanzos", description = "Garbanzos enlatados de 400g", price = 1.40, brand = "Marca A" },
                    new { id = "106", category = "enlatados", name = "Lata de guisantes", description = "Guisantes enlatados de 350g", price = 1.10, brand = "Marca E" },
                    new { id = "107", category = "enlatados", name = "Lata de champiñones", description = "Champiñones enteros enlatados de 300g", price = 1.80, brand = "Marca F" },
                    new { id = "108", category = "enlatados", name = "Lata de sardinas", description = "Sardinas en aceite de 125g", price = 1.90, brand = "Marca G" },
                    new { id = "109", category = "enlatados", name = "Sopa de pollo con fideos", description = "Sopa de pollo con fideos enlatada de 500ml", price = 2.20, brand = "Marca D" },
                    new { id = "110", category = "enlatados", name = "Lata de lentejas", description = "Lentejas enlatadas de 400g", price = 1.35, brand = "Marca H" },
                    new { id = "111", category = "enlatados", name = "Lata de puré de tomate", description = "Puré de tomate enlatado de 500g", price = 1.25, brand = "Marca I" },
                    new { id = "112", category = "enlatados", name = "Lata de espinacas", description = "Espinacas enlatadas de 300g", price = 1.45, brand = "Marca J" },
                    new { id = "113", category = "enlatados", name = "Lata de duraznos en almíbar", description = "Duraznos en almíbar de 400g", price = 2.30, brand = "Marca K" },
                    new { id = "114", category = "enlatados", name = "Lata de chiles jalapeños", description = "Chiles jalapeños en escabeche de 200g", price = 1.60, brand = "Marca L" },
                    new { id = "115", category = "enlatados", name = "Lata de sopa de champiñones", description = "Sopa de champiñones enlatada de 400ml", price = 1.85, brand = "Marca M" },
                    new { id = "116", category = "enlatados", name = "Lata de frutas mixtas", description = "Frutas mixtas en almíbar de 450g", price = 2.50, brand = "Marca K" },
                    new { id = "117", category = "enlatados", name = "Lata de calamares en su tinta", description = "Calamares en su tinta de 150g", price = 2.75, brand = "Marca N" },
                    new { id = "118", category = "enlatados", name = "Lata de albóndigas en salsa", description = "Albóndigas en salsa de tomate de 400g", price = 2.10, brand = "Marca O" },
                    new { id = "119", category = "enlatados", name = "Lata de habas", description = "Habas enlatadas de 350g", price = 1.50, brand = "Marca P" },
                    new { id = "120", category = "enlatados", name = "Lata de peras en almíbar", description = "Peras en almíbar de 400g", price = 2.40, brand = "Marca Q" }
                };


            await _cosmosDbService.ExecuteSecondStoredProcedureAsync(request, itemsToInsert);
            return Ok();
        }




    }
}
