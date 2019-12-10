using System.Threading.Tasks;

namespace AoC_2019.IntCode.Instructions
{
    public class Instruction4 : BaseInstruction
    {
        public override int Length => 2;

        public override Task<InstructionOutput> Run(InstructionInput instructionInput)
        {
            long output = instructionInput.IntCode[ExtractMemoryAddress(instructionInput, 1)];

            return Task.FromResult(new InstructionOutput()
            {
                Result = output,
                InstructionPointer = instructionInput.InstructionPointer + Length
            });
        }
    }
}
