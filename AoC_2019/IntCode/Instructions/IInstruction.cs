using System.Threading.Tasks;

namespace AoC_2019.IntCode.Instructions
{
    public interface IInstruction
    {
        int Length { get; }

        Task<InstructionOutput> Run(InstructionInput instructionInput);
    }
}
