using System.Threading.Tasks;

namespace AoC_2019.IntCode.Instructions
{
    public class Instruction1 : BaseInstruction
    {
        public override int Length => 4;

        public override Task<InstructionOutput> Run(InstructionInput instructionInput)
        {
            instructionInput.IntCode[ExtractMemoryAddress(instructionInput, 3)] =
                instructionInput.IntCode[ExtractMemoryAddress(instructionInput, 1)]
                + instructionInput.IntCode[ExtractMemoryAddress(instructionInput, 2)];

            return Nothing(instructionInput.InstructionPointer + Length);
        }
    }
}
