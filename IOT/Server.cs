using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Threading;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;

namespace IOT
{

    public class DeviceSpec
    {
        public string _deviceKey { get; set; }
        public string _deviceName { get; set; }
        public int _messageId { get; set; }
        public DateTime _dateTime { get; set; }
        public DeviceSpec() { }
    }

    public class MetricsPayload
    {
        public int counter { get; set; }
        public DateTime dateTime { get; set; }
        public string deviceName { get; set; }
        public MetricsPayload() { }
    }


    class Server
    {

        static string connectionString = "HostName=AndrewTestHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=56LhSZUhlbC+dxfMj3bYIspEHF3MhFCTvXakn/dWJIs=";
        static string iotHubD2cEndpoint = "messages/events";
        static EventHubClient eventHubClient;
        static ServiceClient serviceClient;

        public static MetricsPayload payload = new MetricsPayload()
        {
            counter = 0,
            deviceName = null
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Call Direct Method\n");
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            Console.ReadLine();

            int x = 0;
            do
            {
                payload.deviceName = "test123";
                InvokeMethod(payload).Wait();
                payload.deviceName = "myFirstDevice";
                InvokeMethod(payload).Wait();
                x++;
            } while (x < 150);

            
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

        }

        private static async Task InvokeMethod(MetricsPayload dataToSend)
        {
            string output = JsonConvert.SerializeObject(dataToSend);
            var methodInvocation = new CloudToDeviceMethod("calculate") { ResponseTimeout = TimeSpan.FromSeconds(30) };

            methodInvocation.SetPayloadJson(output);

            var response = await serviceClient.InvokeDeviceMethodAsync(dataToSend.deviceName, methodInvocation);
            payload = JsonConvert.DeserializeObject<MetricsPayload>(response.GetPayloadAsJson());
            Console.WriteLine("Response status: {0}, payload:", response.Status);
            Console.WriteLine(response.GetPayloadAsJson());
            
        }

    }
}
