using Amazon.CDK;

namespace HotelRecommender
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new HotelRecommenderStack(app, "HotelRecommenderStack");

            app.Synth();
        }
    }
}
