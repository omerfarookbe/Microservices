using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apple.MessageBus
{
    public interface IMessageBus
    {

        public Task PublishMessage(object messageContent, string queueName);
    }
}
