using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Todo.Functions.Data;


namespace Todo.Functions
{
    public class TimerTriggers
    {
        readonly TodoDbContext data;

        public TimerTriggers(TodoDbContext data)
        {
            this.data = data;
        }


        [FunctionName("Purge")]
        public async Task Purge([TimerTrigger("0 */5 * * * *")] TimerInfo timer, ILogger log)
        {
            var oldestDate = DateTime.UtcNow.AddMonths(-3);
            var purgeItems = await this.data
                .Items
                .Where(x => x.DateUpdatedUtc <= oldestDate && x.IsDeleted)
                .ToListAsync();

            foreach (var item in purgeItems)
                this.data.Remove(item);

            await this.data.SaveChangesAsync();
        }
    }
}
