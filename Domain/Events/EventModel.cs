﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RentCommandApi.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events
{
    public class EventModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid AggregateIdentifier { get; set; }
        public string AggregateType { get; set; }
        public int Version { get; set; }
        public string EventType { get; set; }
        public Guid CarId { get ; set; } 
        public BaseEvent EventData { get; set; }
    }
}
