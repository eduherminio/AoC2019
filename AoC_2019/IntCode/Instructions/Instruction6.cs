using System.Threading.Tasks;

namespace AoC_2019.IntCode.Instructions
{
    public class Instruction6 : BaseInstruction
    {
        public override int Length => 3;

        public override Task<InstructionOutput> Run(InstructionInput instructionInput)
        {
            long param1 = instructionInput.IntCode[ExtractMemoryAddress(instructionInput, 1)];

            long newInstructionPointer = param1 == 0
                ? instructionInput.IntCode[ExtractMemoryAddress(instructionInput, 2)]
                : instructionInput.InstructionPointer + Length;

            return Nothing(newInstructionPointer);
        }
    }
}
