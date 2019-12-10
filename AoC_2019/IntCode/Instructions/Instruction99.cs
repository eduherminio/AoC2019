﻿using System.Threading.Tasks;

namespace AoC_2019.IntCode.Instructions
{
    public class Instruction99 : BaseInstruction
    {
        public override int Length => -1;

        public override Task<InstructionOutput> Run(InstructionInput instructionInput)
        {
            return Nothing(InstructionPointerValueWhenHalt);
        }
    }
}
