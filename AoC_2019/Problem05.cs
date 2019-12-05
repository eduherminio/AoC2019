using AoCHelper;
using AoCHelper.Helpers;
using FileParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC_2019
{
    public class Problem05 : BaseProblem
    {
        private enum ParameterMode
        {
            Position = 0,
            Immediate = 1
        }

        public override string Solve_1()
        {
            var intCode = ParseInput().ToList();

            var outputSequence = RunIntcodeProgram(intCode, input: 1).ToList();

            return string.Join(string.Empty, outputSequence.SkipWhile(n => n == 0));
        }

        public override string Solve_2()
        {
            var intCode = ParseInput().ToList();

            var outputSequence = RunIntcodeProgram(intCode, input: 5).ToList();

            return string.Join(string.Empty, outputSequence.SkipWhile(n => n == 0));
        }

        private static IEnumerable<int> RunIntcodeProgram(List<int> intCode, int input)
        {
            for (int i = 0; i < intCode.Count;)
            {
                int output = ExecuteInstruction(ref intCode, ref i, input);

                if (output >= 0)
                {
                    yield return output;
                }

                if (i == -1)
                {
                    break;
                }
            }
        }

        private static int ExecuteInstruction(ref List<int> intCode, ref int instructionPointer, int input)
        {
            List<ParameterMode> parameterModeList = new List<ParameterMode>();
            int opcode = intCode[instructionPointer];
            int opcodeLength = opcode.ToString().Length;

            if (opcodeLength != 1)
            {
                IEnumerable<int> parameterModes = opcode.ToString().Reverse().Select(c => int.Parse(c.ToString()));
                parameterModeList = parameterModes.Skip(2).Select(n => (ParameterMode)n).ToList();

                opcode = int.Parse(opcode.ToString().Substring(opcodeLength - 2));
            }

            switch (opcode)
            {
                case 0:
                    ++instructionPointer;
                    break;
                case 1:
                    intCode[ExtractParameter(intCode, parameterModeList, instructionPointer, 3)] =
                        intCode[ExtractParameter(intCode, parameterModeList, instructionPointer, 1)]
                        + intCode[ExtractParameter(intCode, parameterModeList, instructionPointer, 2)];
                    instructionPointer += 4;
                    break;

                case 2:
                    intCode[ExtractParameter(intCode, parameterModeList, instructionPointer, 3)] =
                        intCode[ExtractParameter(intCode, parameterModeList, instructionPointer, 1)]
                        * intCode[ExtractParameter(intCode, parameterModeList, instructionPointer, 2)];
                    instructionPointer += 4;
                    break;

                case 3:
                    intCode[ExtractParameter(intCode, parameterModeList, instructionPointer, 1)] = input;
                    instructionPointer += 2;
                    break;

                case 4:
                    int output = intCode[ExtractParameter(intCode, parameterModeList, instructionPointer, 1)];
                    instructionPointer += 2;
                    return output;

                case 5:
                    {
                        int param1 = intCode[ExtractParameter(intCode, parameterModeList, instructionPointer, 1)];
                        instructionPointer = param1 != 0
                            ? intCode[ExtractParameter(intCode, parameterModeList, instructionPointer, 2)]
                            : instructionPointer + 3;
                        break;
                    }

                case 6:
                    {
                        int param1 = intCode[ExtractParameter(intCode, parameterModeList, instructionPointer, 1)];
                        instructionPointer = param1 == 0
                            ? intCode[ExtractParameter(intCode, parameterModeList, instructionPointer, 2)]
                            : instructionPointer + 3;
                        break;
                    }

                case 7:
                    {
                        int param1 = intCode[ExtractParameter(intCode, parameterModeList, instructionPointer, 1)];
                        int param2 = intCode[ExtractParameter(intCode, parameterModeList, instructionPointer, 2)];

                        intCode[ExtractParameter(intCode, parameterModeList, instructionPointer, 3)] = param1 < param2
                            ? 1
                            : 0;

                        instructionPointer += 4;
                        break;
                    }

                case 8:
                    {
                        int param1 = intCode[ExtractParameter(intCode, parameterModeList, instructionPointer, 1)];
                        int param2 = intCode[ExtractParameter(intCode, parameterModeList, instructionPointer, 2)];

                        intCode[ExtractParameter(intCode, parameterModeList, instructionPointer, 3)] = param1 == param2
                            ? 1
                            : 0;

                        instructionPointer += 4;
                        break;
                    }
                case 99:
                    instructionPointer = -1;
                    return -1;

                default:
                    throw new SolvingException("Something went wrong");
            }

            return -2;
        }

        private static int ExtractParameter(List<int> input, IEnumerable<ParameterMode> parameterModes, int instructionPointer, int offset)
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

        private IEnumerable<int> ParseInput()
        {
            return new ParsedFile(FilePath)
                .ToSingleString()
                .Split(',')
                .Select(int.Parse);
        }
    }
}
