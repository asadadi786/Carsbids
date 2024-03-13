using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace SearchService.Consumers
{
    public class AuctionDeletedConsumer : IConsumer<AuctionDeleted>
    {
        public async Task Consume(ConsumeContext<AuctionDeleted> context)
        {
              Console.WriteLine("---> Consuming aution deleted" + context.Message.Id);

              var result = await DB.DeleteAsync<Item>(context.Message.Id);

              if(!result.IsAcknowledged)
                    throw new MessageException(typeof(AuctionDeleted), "Problem deleting auction");
        }
    }
}