using AoCHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC_2019
{
    public static class IntCodeComputer
    {
        private static readonly Dictionary<int, Func<ICollection<int>, IInstruction>> _supportedInstructions =
            new Dictionary<int, Func<ICollection<int>, IInstruction>>
            {
                [0] = (_) => new Instruction0(),
                [1] = (_) => new Instruction1(),
                [2] = (_) => new Instruction2(),
                [3] = (input) => new Instruction3(input),
                [4] = (_) => new Instruction4(),
                [5] = (_) => new Instruction5(),
                [6] = (_) => new Instruction6(),
                [7] = (_) => new Instruction7(),
                [8] = (_) => new Instruction8(),
                [99] = (_) => new Instruction99()
            };

        private enum ParameterMode
        {
            Position = 0,
            Immediate = 1
        }

        public static IEnumerable<int> RunIntcodeProgram(List<int> intCode, ICollection<int> input)
        {
            for (int instructionPointer = 0; instructionPointer < intCode.Count;)
            {
                int output = ExecuteInstruction(ref intCode, ref instructionPointer, ref input);

                if (instructionPointer == BaseInstruction.InstructionPointerValueWhenHalt)
                {
                    break;
                }

                if (output != BaseInstruction.NullCode)
                {
                    yield return output;
                }
            }
        }

        private static int ExecuteInstruction(ref List<int> intCode, ref int instructionPointer, ref ICollection<int> input)
        {
            int opcode = intCode[instructionPointer];

            List<ParameterMode> parameterModeList = CalculateParametersMode(ref opcode);

            return GetInstruction(opcode, input)
                .Run(parameterModeList, ref intCode, ref instructionPointer);
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

        private static IInstruction GetInstruction(int instructionCode, ICollection<int> input)
        {
            if (_supportedInstructions.TryGetValue(instructionCode, out var instruction))
            {
                return instruction.Invoke(input);
            }

            throw new ArgumentException($"Instruction {instructionCode} not supported");
        }

        private interface IInstruction
        {
            int Run(IEnumerable<ParameterMode> parameterModeList, ref List<int> intCode, ref int instructionPointer);
        }

        #region Instructions

        private abstract class BaseInstruction : IInstruction
        {
            public const int NullCode = int.MinValue;
            public const int InstructionPointerValueWhenHalt = -1;

            public abstract int Run(IEnumerable<ParameterMode> parameterModeList, ref List<int> intCode, ref int instructionPointer);

            protected int Nothing() => NullCode;
        }

        private class Instruction0 : BaseInstruction
        {
            public override int Run(IEnumerable<ParameterMode> parameterModeList, ref List<int> intCode, ref int instructionPointer)
            {
                ++instructionPointer;
                return Nothing();
            }
        }

        private class Instruction1 : BaseInstruction
        {
            public override int Run(IEnumerable<ParameterMode> parameterModeList, ref List<int> intCode, ref int instructionPointer)
            {
                intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 3)] =
                    intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 1)]
                    + intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 2)];

                instructionPointer += 4;

                return Nothing();
            }
        }

        private class Instruction2 : BaseInstruction
        {
            public override int Run(IEnumerable<ParameterMode> parameterModeList, ref List<int> intCode, ref int instructionPointer)
            {
                intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 3)] =
                    intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 1)]
                    * intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 2)];

                instructionPointer += 4;

                return Nothing();
            }
        }

        private class Instruction3 : BaseInstruction
        {
            private IEnumerable<int> _input;

            public Instruction3(IEnumerable<int> input)
            {
                _input = input;
            }

            public override int Run(IEnumerable<ParameterMode> parameterModeList, ref List<int> intCode, ref int instructionPointer)
            {
                if (!_input.Any())
                {
                    throw new SolvingException("IntCode computer has run out of inputs!");
                }

                var nextInput = _input.FirstOrDefault();
                _input = _input.Skip(1);

                intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 1)] = nextInput;

                instructionPointer += 2;

                return Nothing();
            }
        }

        private class Instruction4 : BaseInstruction
        {
            public override int Run(IEnumerable<ParameterMode> parameterModeList, ref List<int> intCode, ref int instructionPointer)
            {
                int output = intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 1)];
                instructionPointer += 2;

                return output;
            }
        }

        private class Instruction5 : BaseInstruction
        {
            public override int Run(IEnumerable<ParameterMode> parameterModeList, ref List<int> intCode, ref int instructionPointer)
            {
                int param1 = intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 1)];

                instructionPointer = param1 != 0
                    ? intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 2)]
                    : instructionPointer + 3;

                return Nothing();
            }
        }

        private class Instruction6 : BaseInstruction
        {
            public override int Run(IEnumerable<ParameterMode> parameterModeList, ref List<int> intCode, ref int instructionPointer)
            {
                int param1 = intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 1)];

                instructionPointer = param1 == 0
                    ? intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 2)]
                    : instructionPointer + 3;

                return Nothing();
            }
        }

        private class Instruction7 : BaseInstruction
        {
            public override int Run(IEnumerable<ParameterMode> parameterModeList, ref List<int> intCode, ref int instructionPointer)
            {
                int param1 = intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 1)];
                int param2 = intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 2)];

                intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 3)] = param1 < param2
                    ? 1
                    : 0;

                instructionPointer += 4;

                return Nothing();
            }
        }

        private class Instruction8 : BaseInstruction
        {
            public override int Run(IEnumerable<ParameterMode> parameterModeList, ref List<int> intCode, ref int instructionPointer)
            {
                int param1 = intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 1)];
                int param2 = intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 2)];

                intCode[ExtractParameterValue(intCode, parameterModeList, instructionPointer, 3)] = param1 == param2
                    ? 1
                    : 0;

                instructionPointer += 4;

                return Nothing();
            }
        }

        private class Instruction99 : BaseInstruction
        {
            public override int Run(IEnumerable<ParameterMode> parameterModeList, ref List<int> intCode, ref int instructionPointer)
            {
                instructionPointer = InstructionPointerValueWhenHalt;

                return Nothing();
            }
        }

        #endregion
    }
}
