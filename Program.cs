// See https://aka.ms/new-console-template for more information
using System.Collections;
using Microsoft.Extensions.Configuration;


internal class Program
{
    public static readonly List<string> _StreamingSources = new();
    private static async Task Main(string[] args)
    {
        Console.WriteLine($"Thread is {Thread.CurrentThread.ManagedThreadId}");
        IConfigurationRoot configurationRoot = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appSettings.json")
        .Build();
        foreach (var url in configurationRoot["envirnment:StreamingUrls"].Split(','))
        {
            Program._StreamingSources.Add(url);
        }

        Console.WriteLine("Hello, World!. We are going to learn IAsyncEnumerable in this Session");
          var YeieldNoAsyncValue = YieldWithAsync();
          await foreach (var Count in YeieldNoAsyncValue)
          {
              Console.WriteLine($"Thread is {Thread.CurrentThread.ManagedThreadId}");
              Console.WriteLine(Count);
          }

        var YieldAsyncValue = YieldWithoutAsync();
        foreach (var Count in YieldAsyncValue)
        {
            Console.WriteLine($"Thread is {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine(Count);
        }

        /// <summary>
        /// Method that returns yield values from multiple http source. 
        /// IAsyncEnumerable make sure main thread is not blocked but streaming happens asyncnony way.
        /// As soon as streaming service sends the data, yield async will stream it to the client.
        /// </summary>
        static async IAsyncEnumerable<int> YieldWithAsync()
        {
            HttpClient httpClient = new();
            foreach (var url in Program._StreamingSources)
            {
                var StreamingData = await httpClient.GetAsync(url);
                if (StreamingData.IsSuccessStatusCode)
                {
                    var response = await StreamingData.Content.ReadAsStringAsync();
                    yield return response.Count();
                }
            }
        }

         /// <summary>
        /// Yield method without async.
        /// Here Main thread will be blocked until the task completes.
        /// </summary>
        static IEnumerable<int> YieldWithoutAsync()
        {
            HttpClient httpClient = new();


            foreach (var url in Program._StreamingSources)
            {
                var StreamingData = httpClient.GetAsync(url).Result;
                if (StreamingData.IsSuccessStatusCode)
                {
                    var response = StreamingData.Content.ReadAsStringAsync().Result;
                    yield return response.Count();
                }
            }
        }

    }
}


