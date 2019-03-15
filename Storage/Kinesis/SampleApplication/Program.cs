using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Newtonsoft.Json;

namespace KinesisPublishSample
{
    public class DeviceData
    {
        public string DeviceId { get; set; }
        public int Humidity { get; set; }
        public int Temperature { get; set; }
    }

    class Program
    {
        // NOTE: replace the value with your Kinesis stream name
        static string _kinesisStreamName ="YOUR-KINESIS-STREAM-NAME-HERE";

        // NOTE: update with the region in which you created your stream
        static Amazon.RegionEndpoint _regionEndpoint = Amazon.RegionEndpoint.USEast1;

        static int _maxExecutionCount = 1000;
        static int _publishInterval = 3000;
        static int _deviceCount = 5;

        static bool _cancelled = false;

        static void Main(string[] args)
        {
            Console.WriteLine($"Now ready to publish data into Kinesis stream : {_kinesisStreamName}\n");
            Console.WriteLine("Press Ctrl+C to exit...\n");

            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

            for (var i = 0; i < _maxExecutionCount; i++)
            {
                List<DeviceData> dataList = GetDeviceData();
                PublishDeviceDataToKinesis(dataList);
                Thread.Sleep(_publishInterval);

                if (_cancelled) break;
            }

            Console.WriteLine("Task Completed!\n");
            Console.Write("To publish more data, please run the application again.\n");

            Console.CancelKeyPress -= new ConsoleCancelEventHandler(Console_CancelKeyPress);
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (e.SpecialKey == ConsoleSpecialKey.ControlC)
            {
                _cancelled = true;
                e.Cancel = true;
            }
        }

        private static void PublishDeviceDataToKinesis(List<DeviceData> dataList)
        {
            // note: this constructor relies on you having set up a credential profile
            // named 'default', or have set credentials in environment variables
            // AWS_ACCESS_KEY_ID & AWS_SECRET_ACCESS_KEY, or have an application settings
            // file. See https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/net-dg-config-creds.html
            // for more details and other constructor options.
            var kinesisClient = new AmazonKinesisClient(_regionEndpoint);

            foreach (DeviceData data in dataList)
            {
                var dataAsJson = JsonConvert.SerializeObject(data);
                var dataAsBytes = Encoding.UTF8.GetBytes(dataAsJson);
                using (var memoryStream = new MemoryStream(dataAsBytes))
                {
                    try
                    {
                        var requestRecord = new PutRecordRequest
                        {
                            StreamName = _kinesisStreamName,
                            PartitionKey = data.DeviceId,
                            Data = memoryStream
                        };

                        var responseRecord = kinesisClient.PutRecordAsync(requestRecord).Result;
                        Console.WriteLine($"Successfully published. Record:{data.DeviceId},{data.Humidity},{data.Temperature} Seq:{responseRecord.SequenceNumber}");

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to publish. Exception: {ex.Message}");
                    }
                }
            }
        }

        private static List<DeviceData> GetDeviceData()
        {
            var dataList = new List<DeviceData>();

            var url = Path.GetRandomFileName();
            for (var i = 0; i < _deviceCount; i++)
            {
                var rnd = new Random(Guid.NewGuid().GetHashCode());

                var data = new DeviceData
                {
                    DeviceId = string.Format("Device{0}", i),
                    Temperature = rnd.Next(0, 40),
                    Humidity = rnd.Next(0, 100)
                };

                dataList.Add(data);
            }
            return dataList;
        }
    }
}
