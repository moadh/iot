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
           

            // Adding this to call direct method

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
            } while (x < 20);

            
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
            //*****


            Console.WriteLine("Send Cloud-to-Device message\n");
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);

            Console.WriteLine("Press any key to send a C2D message.");
            Console.ReadLine();
            SendCloudToDeviceMessageAsync().Wait();
            eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2cEndpoint);

            var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

            CancellationTokenSource cts = new CancellationTokenSource();

            System.Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };
            
            var tasks = new List<Task>();
            foreach (string partition in d2cPartitions)
            {
                tasks.Add(ReceiveMessagesFromDeviceAsync(partition, cts.Token));
            }
            Task.WaitAll(tasks.ToArray());

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

        private async static Task SendCloudToDeviceMessageAsync()
        {
            Console.WriteLine("Enter device Name:");
            string deviceToMessage = Console.ReadLine();
            Console.WriteLine("Enter Message:");
            string messageToSend = Console.ReadLine();
            var commandMessage = new Message(Encoding.ASCII.GetBytes(messageToSend));
            await serviceClient.SendAsync(deviceToMessage, commandMessage);
        }


        private static async Task ReceiveMessagesFromDeviceAsync(string partition, CancellationToken ct)
        {

            var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.UtcNow);

            while (true)
            {
                if (ct.IsCancellationRequested) break;
                EventData eventData = await eventHubReceiver.ReceiveAsync();
                if (eventData == null) continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                DeviceSpec device = JsonConvert.DeserializeObject<DeviceSpec>(data);

                Console.WriteLine("Message received. DateTime: {0} MessageId: {1} Key: {2} DeviceName: '{3}'", device._dateTime, device._messageId, device._deviceKey, device._deviceName);
            }
        }
    }
}
