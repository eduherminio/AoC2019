using AoCHelper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AoC_2019.IntCode.Instructions
{
    public abstract class BaseInstruction : IInstruction
    {
        public const int InstructionPointerValueWhenHalt = -1;

        public abstract int Length { get; }

        public abstract Task<InstructionOutput> Run(InstructionInput instructionInput);

        protected static Task<InstructionOutput> Nothing(int newInstructionPointer) =>
            Task.FromResult(new InstructionOutput()
            {
                InstructionPointer = newInstructionPointer
            });

        protected static Task<InstructionOutput> Nothing(long newInstructionPointer) =>
            Nothing((int)newInstructionPointer);

        protected static int ExtractMemoryAddress(InstructionInput instructionInput, int offset)
        {
            long opCode = instructionInput.OpCode;

            List<ParameterMode> parameterModes = new List<ParameterMode>();
            int opCodeLength = opCode.ToString().Length;

            if (opCodeLength != 1)
            {
                IEnumerable<int> parameters = opCode.ToString().Reverse().Select(c => int.Parse(c.ToString()));
                parameterModes = parameters.Skip(2).Select(n => (ParameterMode)n).ToList();

                opCode = int.Parse(opCode.ToString().Substring(opCodeLength - 2));
            }

            ParameterMode parameterMode = offset <= parameterModes.Count
                ? parameterModes[offset - 1]
                : default;

            int memoryAddress = (int)(parameterMode switch
            {
                ParameterMode.Position => instructionInput.IntCode[instructionInput.InstructionPointer + offset],
                ParameterMode.Immediate => instructionInput.InstructionPointer + offset,
                ParameterMode.Relative => instructionInput.RelativeBase + instructionInput.IntCode[instructionInput.InstructionPointer + offset],
                _ => throw new SolvingException($"Unknown ParameterMode: {parameterMode}")
            });

            if (memoryAddress >= instructionInput.IntCode.Count)
            {
                for (int i = instructionInput.IntCode.Count; i < memoryAddress + 1; ++i)
                {
                    instructionInput.IntCode.Add(0);
                }
            }

            return memoryAddress;
        }
    }
}
