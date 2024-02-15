﻿using CommandLine;

namespace AmqpCommon.Commands.Publish {
    public class PublishOptions : BaseOptions {
        [Option("data", Required = false, HelpText = "Message data/json")]
        public string Data { get; set; }
        [Option("file", Required = false, HelpText = "filename for Message data/json")]
        public string File { get; set; }
        [Option("eventtype", Required = true, HelpText = "Event type (event class full name)")]
        public string EventType { get; set; }
    }
}