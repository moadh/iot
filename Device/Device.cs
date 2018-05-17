using System;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace DeviceOne
{
    
    public class MetricsPayload
    {
        public int counter { get; set; }
        public int VibrationCount { get; set; }
        public DateTime dateTime { get; set; }
        public string deviceName { get; set; }
        public MetricsPayload() { }
    }

    public class DeviceLogic
    {
        public List<double> Vibrations { get; private set; }

        public string DeviceKey { get; set; }
        public string DeviceName { get; set; }
        public DeviceLogic() { }



        public void LoadVibrations()
        {

            var vibrationFileName = "vibrations-m0.txt";
            var data = System.IO.File.ReadAllLines(vibrationFileName).ToList();
            Vibrations = data.Skip(1).Select(s => Double.Parse(s)).ToList();
        }
    }


    class Device
    {

        static DeviceClient deviceClient;
        static string iotHubUri = "team22hub.azure-devices.net";
        static string deviceKey = "gEKe3qXYkeskE5a+d0XllrN1wbDxPJExyF/AvExBydE=";
        static string deviceName = "team22device6";

        static void Main(string[] args)
        {
            DeviceLogic device = new DeviceLogic()
            {
                DeviceKey = deviceKey,
                DeviceName = deviceName
            };

            Console.WriteLine(deviceName + " loading data.\n");
            device.LoadVibrations();

            Console.WriteLine(deviceName + " reporting for duty.\n");
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceName, deviceKey), TransportType.Mqtt);
            deviceClient.SetMethodHandlerAsync("count", Count, device).Wait();
            Console.WriteLine("Waiting for direct method call\n Press enter to exit.");
            Console.ReadLine();

        }

        static Task<MethodResponse> Count(MethodRequest methodRequest, object userContext)
        {
            Console.WriteLine();
            var deviceLogic = (DeviceLogic)userContext;
            MetricsPayload payload = JsonConvert.DeserializeObject<MetricsPayload>(methodRequest.DataAsJson);
            payload.VibrationCount = deviceLogic.Vibrations.Count;
            Console.WriteLine("Direct Call recieved. DateTime: {0} Counter: {1} ", payload.dateTime, payload.counter);
            Console.WriteLine("\nReturning response for method {0}", methodRequest.Name);
            payload.counter ++;

            string result = JsonConvert.SerializeObject(payload);
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
        }   
        
    }
}
