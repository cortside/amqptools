using System;
using System.Collections.Generic;

namespace AmqpTools.Core.Models {
    /// <summary>
    /// Message Details
    /// </summary>
    public class AmqpToolsMessage {
        /// <summary>
        /// Content, should be json
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Deserialized content
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// MessageId
        /// </summary>
        public string MessageId { get; set; }
        /// <summary>
        /// CorrelationId
        /// </summary>
        public string CorrelationId { get; set; }
        /// <summary>
        /// PartitionKey
        /// </summary>
        public string PartitionKey { get; set; }
        /// <summary>
        /// ExpiresAtUtc
        /// </summary>
        public DateTime? ExpiresAtUtc { get; set; }
        /// <summary>
        /// ContentType
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// ScheduledEnqueueTimeUtc
        /// </summary>
        public DateTime? ScheduledEnqueueTimeUtc { get; set; }
        /// <summary>
        /// UserProperties
        /// </summary>
        public IReadOnlyDictionary<string, object> ApplicationProperties { get; set; }
        /// <summary>
        /// SystemProperties
        /// </summary>
        public object SystemProperties { get; set; }
    }
}
