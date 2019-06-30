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
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
            var item = await req.ReadAs<TodoItem>();

            return new OkObjectResult("");
        }


        [FunctionName("Get")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var query = this.data.Items.AsQueryable();
            var includeCompleted = req.Query["IncludeCompleted"] == "true";
            if (!includeCompleted)
                query = query.Where(x => x.CompletionDateUtc == null);

            if (DateTime.TryParse(req.Query["Delta"], out var delta))
                query = query.Where(x => x.DateUpdatedUtc > delta);

            var list = await query.ToListAsync();
            return new OkObjectResult(list);
        }
    }
}
