using System;
using System.Text.Json.Serialization;

namespace EventBus.Base.Events
{
    public class IntegrationEvent
    {
        [JsonPropertyName("id")]
        public Guid Id { get; private set; }

        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; private set; }

        [JsonConstructor]
        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
        }

        public IntegrationEvent(Guid id, DateTime createdDate)
        {
            Id = id;
            CreatedDate = createdDate;
        }
    }
}
