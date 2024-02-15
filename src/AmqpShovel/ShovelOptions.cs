﻿using AmqpCommon.Commands;
using CommandLine;

namespace AmqpShovel {
    public class ShovelOptions : BaseOptions {
        [Option(Default = 100, HelpText = "Maximum dlq messages to process")]
        public int Max { get; set; }
    }
}
