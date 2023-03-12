using Amazon.CDK;

namespace HotelPicker
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new HotelPickerStack(app, "HotelPickerStack");

            app.Synth();
        }
    }
}
