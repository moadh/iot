using System;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Text;

namespace DeviceOne
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


    class Device
    {

        static DeviceClient deviceClient;
        static string iotHubUri = "AndrewTestHub.azure-devices.net";
        static string deviceKey = "J0ZhpTr9WmCvX2EbTV6GwznoAQWZkY6zaSXggvb3DHE=";
        static string deviceName = "myFirstDevice";


        static void Main(string[] args)
        {
            DeviceSpec device = new DeviceSpec()
            {
                _deviceKey = deviceKey,
                _deviceName = deviceName,
                _messageId = 0
            };

            Console.WriteLine("Device Two reporting for duty.\n");
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceName, deviceKey), TransportType.Mqtt);
            deviceClient.SetMethodHandlerAsync("calculate", Calculate, null).Wait();
            Console.WriteLine("Waiting for direct method call\n Press enter to exit.");
            Console.ReadLine();

            deviceClient.ProductInfo = "HappyPath_Simulated-CSharp";
            //ReceiveC2dAsync(device);
            while (true)
            {
                Console.ReadLine();
                SendDeviceToCloudMessagesAsync(device);
            }

        }

        static Task<MethodResponse> Calculate(MethodRequest methodRequest, object userContext)
        {
            Console.WriteLine();
            MetricsPayload payload = JsonConvert.DeserializeObject<MetricsPayload>(methodRequest.DataAsJson);
            Console.WriteLine("Direct Call recieved. DateTime: {0} Counter: {1} ", payload.dateTime, payload.counter);
            Console.WriteLine("\nReturning response for method {0}", methodRequest.Name);
            payload.counter++;

            string result = JsonConvert.SerializeObject(payload);
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
        }

        private static async void SendDeviceToCloudMessagesAsync(DeviceSpec device)
        {
            while (true)
            {
                device._messageId++;
                device._dateTime = DateTime.Now;
                string output = JsonConvert.SerializeObject(device);

                var message = new Message(Encoding.ASCII.GetBytes(output));

                await deviceClient.SendEventAsync(message);

                Console.WriteLine("Message Sent. DateTime: {0} MessageId: {1} Key: {2} DeviceName: '{3}'", device._dateTime, device._messageId, device._deviceKey, device._deviceName);
            }
        }

        private static async void ReceiveC2dAsync(DeviceSpec device)
        {
            Console.WriteLine("\nReceiving cloud to device messages from service");
            while (true)
            {

                Message receivedMessage = await deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received message: {0}", Encoding.ASCII.GetString(receivedMessage.GetBytes()));

                Console.ResetColor();

                await deviceClient.CompleteAsync(receivedMessage);

                SendDeviceToCloudMessagesAsync(device);
            }
        }


    }
}
