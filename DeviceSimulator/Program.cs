using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator
{
    public class Program
    {

       private const string IotHubUri = "{iot hub hostname}";
        private const string DeviceKey = "{device key}";
        private const string DeviceId = "{myFirstDevice}";

       

        private static readonly Random Rand = new Random();
        private static DeviceClient _deviceClient;
     
        private static void Main(string[] args)
        {
            Console.WriteLine("Device Simulater Started\n");
            _deviceClient = DeviceClient.Create(IotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(DeviceId, DeviceKey), TransportType.Mqtt);
           
            SendDeviceToCloudMessagesAsync();
            Console.ReadLine();
        }
        private static async void SendDeviceToCloudMessagesAsync()
        {
            while (true)
            {
              
                var tdsLevel = Rand.Next(10, 1000);
                var filterStatus = tdsLevel % 2 == 0 ? "Good":"Bad";
                var waterUsage = Rand.Next(0, 500);
                var currentTemperature= Rand.Next(-30, 100);
                var motorStatus= currentTemperature >=50 ? "Good" : "Bad";
                var telemetryDataPoint = new
                {
                    deviceId = DeviceId,
                    temperature = currentTemperature,
                    filter = filterStatus,
                    motor=motorStatus,
                    usage=waterUsage
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));
                message.Properties.Add("Topic", "WaterUsage");

                await _deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                await Task.Delay(1000);
            }
        }

       
    }
}
