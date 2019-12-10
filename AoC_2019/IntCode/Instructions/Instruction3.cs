using System.Threading.Tasks;

namespace AoC_2019.IntCode.Instructions
{
    public class Instruction3 : BaseInstruction
    {
        public override int Length => 2;

        public async override Task<InstructionOutput> Run(InstructionInput instructionInput)
        {
            await instructionInput.InputReader.WaitToReadAsync().ConfigureAwait(false);
            instructionInput.InputReader.TryRead(out long input);

            instructionInput.IntCode[ExtractMemoryAddress(instructionInput, 1)] = input;

            return await Nothing(instructionInput.InstructionPointer + Length).ConfigureAwait(false);
        }
    }
}
