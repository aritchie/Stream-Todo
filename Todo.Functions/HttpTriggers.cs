using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
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
            // TODO: incoming soft delete - have to trigger delete back to client
            var remote = await req.ReadAs<TodoItem>();
            var local = await this.data.Items.FindAsync(remote.Id);
            if (local == null)
            {
                // new item
                this.data.Items.Add(remote);
                await this.data.SaveChangesAsync();
            }
            else if (local.DateUpdatedUtc > remote.DateUpdatedUtc)
            {
                // merge conflict
            }
            else
            {
                // update local
                local.Title = remote.Title;
                local.Notes = remote.Notes;
                local.DueDateUtc = remote.DueDateUtc;
                local.CompletionDateUtc = remote.CompletionDateUtc;
                local.DateUpdatedUtc = remote.DateUpdatedUtc;
                local.IsDeleted = remote.IsDeleted;
                local.GpsLatitude = remote.GpsLatitude;
                local.GpsLongitude = remote.GpsLongitude;

                await this.data.SaveChangesAsync();
            }

            return new OkObjectResult("");
        }


        [FunctionName("Get")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var query = this.data
                .Items
                .AsQueryable();

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
