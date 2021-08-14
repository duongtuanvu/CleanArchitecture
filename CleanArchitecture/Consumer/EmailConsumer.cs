using Domain.Share;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consumer
{
    public class EmailConsumer : IConsumer<Email>
    {
        public async Task Consume(ConsumeContext<Email> context)
        {
            var data = context.Message;
        }
    }
}
