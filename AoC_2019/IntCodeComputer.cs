using AoCHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AoC_2019
{
    public class IntCodeComputer
    {
        private readonly Dictionary<int, Func<ChannelReader<int>, IInstruction>> _supportedInstructions =
            new Dictionary<int, Func<ChannelReader<int>, IInstruction>>
            {
                [0] = (_) => new Instruction0(),
                [1] = (_) => new Instruction1(),
                [2] = (_) => new Instruction2(),
                [3] = (inputReader) => new Instruction3(inputReader),
                [4] = (_) => new Instruction4(),
                [5] = (_) => new Instruction5(),
                [6] = (_) => new Instruction6(),
                [7] = (_) => new Instruction7(),
                [8] = (_) => new Instruction8(),
                [99] = (_) => new Instruction99()
            };

        private readonly ChannelReader<int> _inputReader;

        private enum ParameterMode
        {
            Position = 0,
            Immediate = 1
        }

        /// <summary>
        /// Use only if no input instructions are expected
        /// </summary>
        public IntCodeComputer() { }

        public IntCodeComputer(ChannelReader<int> inputReader)
        {
            _inputReader = inputReader;
        }

        public async IAsyncEnumerable<int> RunIntCodeProgram(List<int> intCode)
        {
            for (int instructionPointer = 0; instructionPointer < intCode.Count;)
            {
                var output = await ExecuteInstruction(intCode, instructionPointer).ConfigureAwait(false);

                int outputCode = output.Item1;
                instructionPointer = output.Item2;

                if (instructionPointer == BaseInstruction.InstructionPointerValueWhenHalt)
                {
                    break;
                }

                if (outputCode != BaseInstruction.NullCode)
                {
                    yield return outputCode;
                }
            }
        }

        private async Task<Tuple<int, int>> ExecuteInstruction(List<int> intCode, int instructionPointer)
        {
            int opcode = intCode[instructionPointer];

            List<ParameterMode> parameterModeList = CalculateParametersMode(ref opcode);

            return await GetInstruction(opcode)
                .Run(parameterModeList, intCode, instructionPointer).ConfigureAwait(false);
        }

        private static List<ParameterMode> CalculateParametersMode(ref int opcode)
        {
            List<ParameterMode> parameterModeList = new List<ParameterMode>();
            int opcodeLength = opcode.ToString().Length;

            if (opcodeLength != 1)
            {
                IEnumerable<int> parameterModes = opcode.ToString().Reverse().Select(c => int.Parse(c.ToString()));
                parameterModeList = parameterModes.Skip(2).Select(n => (ParameterMode)n).ToList();

                opcode = int.Parse(opcode.ToString().Substring(opcodeLength - 2));
            }

            return parameterModeList;
        }

        private static int ExtractParameterValue(List<int> input, IEnumerable<ParameterMode> parameterModes, int instructionPointer, int offset)
        {
            ParameterMode parameterMode = offset <= parameterModes.Count()
                ? parameterModes.ElementAt(offset - 1)
                : default;

            return parameterMode switch
            {
                ParameterMode.Position => input[instructionPointer + offset],
                ParameterMode.Immediate => instructionPointer + offset,
                _ => throw new SolvingException($"Unknown ParameterMode: {parameterMode}")
            };
        }

        private IInstruction GetInstruction(int instructionCode)
        {
            if (_supportedInstructions.TryGetValue(instructionCode, out var instruction))
            {
                return instruction.Invoke(_inputReader);
            }

            throw new ArgumentException($"Instruction {instructionCode} not supported");
        }

        private interface IInstruction
        {
            int Length { get; }

            Task<Tuple<int, int>> Run(IEnumerable<ParameterMode> parameterModeList, List<int> intCode, int instructionPointer);
        }

        #region Instructions

        private abstract class BaseInstruction : IInstruction
        {
            public const int NullCode = int.MinValue;
            public const int InstructionPointerValueWhenHalt = -1;

            public abstract int Length { get; }

            public abstract Task<Tuple<int, int>> Run(IEnumerable<ParameterMode> parameterModeList, List<int> intCode, int instructionPointer);

            protected Task<Tuple<int, int>> Nothing(int instructionPointerIncrease) => Task.FromResult(Tuple.Create(NullCode, instructionPointerIncrease));
        }

        private class Instruction0 : BaseInstruction
        {
            public override int Length => 1;

            public override Task<Tuple<int, int>> Run(IEnumerable<ParameterMode> parameterModeList, List<int> intCode, int instructionPointer)
            {
                return Nothing(instructionPointer + Length);
            }
        }

        private class Instruction1 : BaseInstruction
        {
            public override int Length => 4;

            public override Task<Tuple<int, int>> Run(IEnumerable<ParameterMode> parameterModeList, List<int> intCode, int instructionPointer)
            {
                intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 3)] =
                    intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 1)]
                    + intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 2)];

                return Nothing(instructionPointer + Length);
            }
        }

        private class Instruction2 : BaseInstruction
        {
            public override int Length => 4;

            public override Task<Tuple<int, int>> Run(IEnumerable<ParameterMode> parameterModeList, List<int> intCode, int instructionPointer)
            {
                intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 3)] =
                    intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 1)]
                    * intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 2)];

                return Nothing(instructionPointer + Length);
            }
        }

        private class Instruction3 : BaseInstruction
        {
            private readonly ChannelReader<int> _inputReader;

            public override int Length => 2;

            public Instruction3(ChannelReader<int> inputReader)
            {
                _inputReader = inputReader;
            }

            public override async Task<Tuple<int, int>> Run(IEnumerable<ParameterMode> parameterModeList, List<int> intCode, int instructionPointer)
            {
                await _inputReader.WaitToReadAsync().ConfigureAwait(false);
                _inputReader.TryRead(out int input);

                intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 1)] = input;

                return await Nothing(instructionPointer + Length).ConfigureAwait(false);
            }
        }

        private class Instruction4 : BaseInstruction
        {
            public override int Length => 2;

            public override Task<Tuple<int, int>> Run(IEnumerable<ParameterMode> parameterModeList, List<int> intCode, int instructionPointer)
            {
                int output = intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 1)];

                return Task.FromResult(Tuple.Create(output, instructionPointer + Length));
            }
        }

        private class Instruction5 : BaseInstruction
        {
            public override int Length => 3;

            public override Task<Tuple<int, int>> Run(IEnumerable<ParameterMode> parameterModeList, List<int> intCode, int instructionPointer)
            {
                int param1 = intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 1)];

                int newInstructionPointer = param1 != 0
                    ? intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 2)]
                    : instructionPointer + Length;

                return Nothing(newInstructionPointer);
            }
        }

        private class Instruction6 : BaseInstruction
        {
            public override int Length => 3;

            public override Task<Tuple<int, int>> Run(IEnumerable<ParameterMode> parameterModeList, List<int> intCode, int instructionPointer)
            {
                int param1 = intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 1)];

                int newInstructionPointer = param1 == 0
                    ? intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 2)]
                    : instructionPointer + Length;

                return Nothing(newInstructionPointer);
            }
        }

        private class Instruction7 : BaseInstruction
        {
            public override int Length => 4;

            public override Task<Tuple<int, int>> Run(IEnumerable<ParameterMode> parameterModeList, List<int> intCode, int instructionPointer)
            {
                int param1 = intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 1)];
                int param2 = intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 2)];

                intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 3)] = param1 < param2
                    ? 1
                    : 0;

                return Nothing(instructionPointer + Length);
            }
        }

        private class Instruction8 : BaseInstruction
        {
            public override int Length => 4;

            public override Task<Tuple<int, int>> Run(IEnumerable<ParameterMode> parameterModeList, List<int> intCode, int instructionPointer)
            {
                int param1 = intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 1)];
                int param2 = intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 2)];

                intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 3)] = param1 == param2
                    ? 1
                    : 0;

                return Nothing(instructionPointer + Length);
            }
        }

        private class Instruction99 : BaseInstruction
        {
            public override int Length => -1;

            public override Task<Tuple<int, int>> Run(IEnumerable<ParameterMode> parameterModeList, List<int> intCode, int instructionPointer)
            {
                return Nothing(InstructionPointerValueWhenHalt);
            }
        }

        #endregion
    }
}
