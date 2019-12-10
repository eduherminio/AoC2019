using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using AoC_2019.IntCode.Instructions;

namespace AoC_2019.IntCode
{
    public class IntCodeComputer
    {
        private readonly Dictionary<long, IInstruction> _supportedInstructions = new Dictionary<long, IInstruction>
        {
            [0] = new Instruction0(),
            [1] = new Instruction1(),
            [2] = new Instruction2(),
            [3] = new Instruction3(),
            [4] = new Instruction4(),
            [5] = new Instruction5(),
            [6] = new Instruction6(),
            [7] = new Instruction7(),
            [8] = new Instruction8(),
            [9] = new Instruction9(),
            [99] = new Instruction99()
        };

        private readonly ChannelReader<long> _inputReader;

        /// <summary>
        /// Use only if no input instructions are expected
        /// </summary>
        public IntCodeComputer() { }

        public IntCodeComputer(ChannelReader<long> inputReader)
        {
            _inputReader = inputReader;
        }

        public async IAsyncEnumerable<long> RunIntCodeProgram(List<long> intCode)
        {
            int relativeBase = 0;
            int instructionPointer = 0;

            while (instructionPointer != BaseInstruction.InstructionPointerValueWhenHalt)
            {
                InstructionInput payload = new InstructionInput()
                {
                    IntCode = intCode,
                    InstructionPointer = instructionPointer,
                    RelativeBase = relativeBase,
                    InputReader = _inputReader
                };

                var output = await ExecuteInstruction(payload).ConfigureAwait(false);

                instructionPointer = output.InstructionPointer != InstructionOutput.DefaultValue
                    ? output.InstructionPointer
                    : instructionPointer;

                relativeBase = output.RelativeBase != InstructionOutput.DefaultValue
                    ? output.RelativeBase
                    : relativeBase;

                long outputCode = output.Result;

                if (outputCode != InstructionOutput.DefaultValue && instructionPointer != BaseInstruction.InstructionPointerValueWhenHalt)
                {
                    yield return outputCode;
                }
            }
        }

        private async Task<InstructionOutput> ExecuteInstruction(InstructionInput instructionInput)
        {
            int opCode = (int)instructionInput.OpCode;
            int opCodeLength = opCode.ToString().Length;

            if (opCodeLength > 2)
            {
                opCode = int.Parse(opCode.ToString().Substring(opCodeLength - 2));
            }

            var instruction = GetInstruction(opCode);

            return await instruction.Run(instructionInput).ConfigureAwait(false);
        }

        private IInstruction GetInstruction(int instructionCode)
        {
            if (_supportedInstructions.TryGetValue(instructionCode, out IInstruction instruction))
            {
                return instruction;
            }

            throw new ArgumentException($"Instruction {instructionCode} not supported");
        }
    }
}
