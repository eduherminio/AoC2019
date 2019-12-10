using System.Threading.Tasks;

namespace AoC_2019.IntCode.Instructions
{
    public class Instruction8 : BaseInstruction
    {
        public override int Length => 4;

        public override Task<InstructionOutput> Run(InstructionInput instructionInput)
        {
            long param1 = instructionInput.IntCode[ExtractMemoryAddress(instructionInput, 1)];
            long param2 = instructionInput.IntCode[ExtractMemoryAddress(instructionInput, 2)];

            instructionInput.IntCode[ExtractMemoryAddress(instructionInput, 3)] = param1 == param2
                ? 1
                : 0;

            return Nothing(instructionInput.InstructionPointer + Length);
        }
    }
}
