using RentCommandApi.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Producers
{
    public interface IEventProducer
    {
        Task ProduceAsync<T>(
            string topic,
            T @event) where T : BaseEvent;
    }
}
