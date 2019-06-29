using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;


namespace Todo.Functions
{
    public static class Extensions
    {
        public static async Task<T> ReadAs<T>(this HttpRequest request)
        {
            using (var reader = new StreamReader(request.Body))
            {
                var requestBody = await reader.ReadToEndAsync();
                var result = JsonConvert.DeserializeObject<T>(requestBody);
                return result;
            }
        }
    }
}
