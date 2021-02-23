using System.Net.Http;

namespace TestHelpers
{
    public class MockHttpClientFactory : IHttpClientFactory
    {
        private static HttpClient _HttpClient = new HttpClient();

        public HttpClient CreateClient(string name) => _HttpClient;
    }
}
