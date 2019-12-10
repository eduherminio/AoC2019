using System.Collections.Generic;
using System.Threading.Channels;

namespace AoC_2019.IntCode.Instructions
{
    public class InstructionInput
    {
        public long OpCode => IntCode[InstructionPointer];

        public int InstructionPointer { get; set; }

        public int RelativeBase { get; set; }

        public List<long> IntCode { get; set; }

        public ChannelReader<long> InputReader { get; set; }
    }
}
