using System.Threading.Tasks;

namespace AoC_2019.IntCode.Instructions
{
    public class Instruction9 : BaseInstruction
    {
        public override int Length => 2;

        public override Task<InstructionOutput> Run(InstructionInput instructionInput)
        {
            return Task.FromResult(new InstructionOutput()
            {
                RelativeBase = instructionInput.RelativeBase + (int)instructionInput.IntCode[ExtractMemoryAddress(instructionInput, 1)],
                InstructionPointer = instructionInput.InstructionPointer + Length
            });
        }
    }
}
