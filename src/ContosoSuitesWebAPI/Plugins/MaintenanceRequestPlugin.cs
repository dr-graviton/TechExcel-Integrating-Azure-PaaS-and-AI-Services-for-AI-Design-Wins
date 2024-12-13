using System.ComponentModel; 
using ContosoSuitesWebAPI.Entities;
using Microsoft.Azure.Cosmos;
 using Microsoft.SemanticKernel;
 using Microsoft.SemanticKernel.ChatCompletion;
 using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace ContosoSuitesWebAPI.Plugins
{
    /// <summary>
    /// The maintenance request plugin for creating and saving maintenance requests.
    /// </summary>
    public class MaintenanceRequestPlugin(CosmosClient cosmosClient)
    {
        private readonly CosmosClient _cosmosClient = cosmosClient;

        [KernelFunction("create_maintenance_request")]
        [Description("Creates a new maintenance request for a hotel.")]
        /// <summary>
        /// Creates a new maintenance request for a hotel.
        /// </summary>
        public async Task<MaintenanceRequest> CreateMaintenanceRequest(int HotelId, string Hotel, string Details, int? RoomNumber, string? location)
        {
            try
            {
                Console.WriteLine($"Creating a new maintenance request for the {Hotel}.");

                var request = new MaintenanceRequest
                {
                    id = Guid.NewGuid().ToString(),
                    hotel_id = HotelId,
                    hotel = Hotel,
                    details = Details,
                    room_number = RoomNumber,
                    source = "customer",
                    location = location
                };
                return request;
            }
            catch (Exception ex)
            {
                throw new Exception($"An exception occurred while generating a new maintenance request: {ex}");
            }
        }

        [KernelFunction("save_maintenance_request")]
        [Description("Saves a maintenance request to the database for a hotel.")]
        /// <summary>
        /// Saves a maintenance request to the database for a hotel.
        /// </summary>
        public async Task SaveMaintenanceRequest(MaintenanceRequest maintenanceRequest)
        {
            var db = _cosmosClient.GetDatabase("ContosoSuites");
            var container = db.GetContainer("MaintenanceRequests");

            var response = await container.CreateItemAsync(maintenanceRequest, new PartitionKey(maintenanceRequest.hotel_id));
        }
    }
}
