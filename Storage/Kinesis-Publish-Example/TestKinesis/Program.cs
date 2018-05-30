using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    public class DeviceData
    {
        public string DeviceId { get; set; }
        public int Humidity { get; set; }
        public int Temperature { get; set; }
    }

    class Program
    {
        // Please replace with your Kinesis stream name and region info
        static string _kinesisStreamName ="{YOUR_KINESIS_STREAM_NAME}";
        static Amazon.RegionEndpoint _regionEndpoint = Amazon.RegionEndpoint.USEast1;

        static int _maxExecutionCount = 1000;
        static int _publishInterval = 3000;
        static int _deviceCount = 5;

        static bool _cancelled = false;

        static void Main(string[] args)
        {
            Console.WriteLine("Now ready to publish data into Kinesis Streams : {0}\n", _kinesisStreamName);
            Console.WriteLine("Press Ctrl+C to exit...\n");

            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

            for (int i = 0; i < _maxExecutionCount; i++)
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
            AmazonKinesisConfig config = new AmazonKinesisConfig();
            config.RegionEndpoint = _regionEndpoint;
            AmazonKinesisClient kinesisClient = new AmazonKinesisClient(config);

            foreach (DeviceData data in dataList)
            {
                string dataAsJson = JsonConvert.SerializeObject(data);
                byte[] dataAsBytes = Encoding.UTF8.GetBytes(dataAsJson);
                using (MemoryStream memoryStream = new MemoryStream(dataAsBytes))
                {
                    try
                    {
                        PutRecordRequest requestRecord = new PutRecordRequest();
                        requestRecord.StreamName = _kinesisStreamName;
                        requestRecord.PartitionKey = data.DeviceId;
                        requestRecord.Data = memoryStream;

                        PutRecordResponse responseRecord = kinesisClient.PutRecord(requestRecord);
                        Console.WriteLine("Successfully published. Record:{0},{1},{2} Seq:{3}",
                            data.DeviceId, data.Humidity, data.Temperature, responseRecord.SequenceNumber);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to publish. Exception: {0}", ex.Message);
                    }
                }
            }
        }

        private static List<DeviceData> GetDeviceData()
        {
            List<DeviceData> dataList = new List<DeviceData>();

            string url = Path.GetRandomFileName();
            for (int i = 0; i < _deviceCount; i++)
            {
                DeviceData data = new DeviceData();
                data.DeviceId = string.Format("Device{0}", i);

                Random rnd = new Random(Guid.NewGuid().GetHashCode());
                data.Temperature = rnd.Next(0, 40);
                data.Humidity = rnd.Next(0, 100);
                dataList.Add(data);
            }
            return dataList;
        }
    }
}
