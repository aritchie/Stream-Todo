using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Todo.Functions.Data;

namespace Todo.Functions
{
    public class HttpTriggers
    {
        readonly TodoDbContext data;


        public HttpTriggers(TodoDbContext data)
        {
            this.data = data;
        }


        [FunctionName("Save")]
        public async Task<IActionResult> Save(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            //string name = req.Query["name"];
            var item = await req.ReadAs<object>();

            return new OkObjectResult("");
        }


        [FunctionName("Get")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //string name = req.Query["name"];

            return new OkObjectResult("");
        }
    }
}
