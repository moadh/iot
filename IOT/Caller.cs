﻿using System;
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
    
    public class MetricsPayload
    {
        public int counter { get; set; }
        public int VibrationCount { get; set; }
        public DateTime dateTime { get; set; }
        public string deviceName { get; set; }
        public MetricsPayload() { }
    }



    class Caller
    {

        static string connectionString = "HostName=team22hub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=T0bsidvuqV6euGRR5W3QocTm84IT2P6C0zY300Y2tuk=";
        static ServiceClient serviceClient;

        public static MetricsPayload payload = new MetricsPayload()
        {
            counter = 0,
            deviceName = null
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Call Direct Method by pressing enter\n");
            //Connection to hub
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            Console.ReadLine();

            payload.deviceName = "team22device6";
            payload.dateTime = DateTime.Now;

            //invokes direct method on device 10 times
            for (int i = 0; i < 10; i++)
            {
                InvokeMethod(payload).Wait();
            }
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

        }

        private static async Task InvokeMethod(MetricsPayload dataToSend)
        {
            string output = JsonConvert.SerializeObject(dataToSend);
            var methodInvocation = new CloudToDeviceMethod("count") { ResponseTimeout = TimeSpan.FromSeconds(30) };

            methodInvocation.SetPayloadJson(output);

            var response = await serviceClient.InvokeDeviceMethodAsync(dataToSend.deviceName, methodInvocation);
            payload = JsonConvert.DeserializeObject<MetricsPayload>(response.GetPayloadAsJson());
            Console.WriteLine("Response status: {0}, payload:", response.Status);
            Console.WriteLine(response.GetPayloadAsJson());
            
        }

    }
}
