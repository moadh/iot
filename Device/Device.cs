using System;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Text;

namespace DeviceOne
{
    
    public class MetricsPayload
    {
        public int counter { get; set; }
        public DateTime dateTime { get; set; }
        public string deviceName { get; set; }
        public MetricsPayload() { }
    }


    class Device
    {

        static DeviceClient deviceClient;
        static string iotHubUri = "{ Azure hub uri without https:// }";
        static string deviceKey = "{ Azure Device Key }";
        static string deviceName = "{ Azure device id }";


        static void Main(string[] args)
        {
    
            Console.WriteLine(deviceName + " reporting for duty.\n");
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceName, deviceKey), TransportType.Mqtt);
            deviceClient.SetMethodHandlerAsync("calculate", Calculate, null).Wait();
            Console.WriteLine("Waiting for direct method call\n Press enter to exit.");
            Console.ReadLine();

        }

        static Task<MethodResponse> Calculate(MethodRequest methodRequest, object userContext)
        {
            Console.WriteLine();
            MetricsPayload payload = JsonConvert.DeserializeObject<MetricsPayload>(methodRequest.DataAsJson);
            Console.WriteLine("Direct Call recieved. DateTime: {0} Counter: {1} ", payload.dateTime, payload.counter);
            Console.WriteLine("\nReturning response for method {0}", methodRequest.Name);
            payload.counter ++;

            string result = JsonConvert.SerializeObject(payload);
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
        }   
        
    }
}
