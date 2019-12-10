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
        private readonly Dictionary<long, Func<IInstruction>> _supportedInstructions =
            new Dictionary<long, Func<IInstruction>>
            {
                [0] = () => new Instruction0(),
                [1] = () => new Instruction1(),
                [2] = () => new Instruction2(),
                [3] = () => new Instruction3(),
                [4] = () => new Instruction4(),
                [5] = () => new Instruction5(),
                [6] = () => new Instruction6(),
                [7] = () => new Instruction7(),
                [8] = () => new Instruction8(),
                [9] = () => new Instruction9(),
                [99] = () => new Instruction99()
            };

        private readonly ChannelReader<long> _inputReader;

        private enum ParameterMode
        {
            Position = 0,
            Immediate = 1,
            Relative = 2
        }

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
                InstructionPayload payload = new InstructionPayload()
                {
                    IntCode = intCode,
                    InstructionPointer = instructionPointer,
                    RelativeBase = relativeBase,
                    InputReader = _inputReader
                };

                var output = await ExecuteInstruction(payload).ConfigureAwait(false);

                long outputCode = output.Result;

                instructionPointer = output.InstructionPointer != InstructionOutput.DefaultValue
                    ? output.InstructionPointer
                    : instructionPointer;

                relativeBase = output.RelativeBase != InstructionOutput.DefaultValue
                    ? output.RelativeBase
                    : relativeBase;

                if (outputCode != InstructionOutput.DefaultValue && instructionPointer != BaseInstruction.InstructionPointerValueWhenHalt)
                {
                    yield return outputCode;
                }
            }
        }

        private async Task<InstructionOutput> ExecuteInstruction(InstructionPayload instructionPayload)
        {
            long opCode = instructionPayload.OpCode;
            int opCodeLength = opCode.ToString().Length;

            if (opCodeLength > 2)
            {
                opCode = int.Parse(opCode.ToString().Substring(opCodeLength - 2));
            }

            return await GetInstruction((int)opCode)
                .Run(instructionPayload).ConfigureAwait(false);
        }

        private static int ExtractMemoryAddress(InstructionPayload instructionPayload, int offset)
        {
            long opCode = instructionPayload.OpCode;

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

            long memoryAddress = parameterMode switch
            {
                ParameterMode.Position => instructionPayload.IntCode[instructionPayload.InstructionPointer + offset],
                ParameterMode.Immediate => instructionPayload.InstructionPointer + offset,
                ParameterMode.Relative => instructionPayload.RelativeBase + instructionPayload.IntCode[instructionPayload.InstructionPointer + offset],
                _ => throw new SolvingException($"Unknown ParameterMode: {parameterMode}")
            };

            if (memoryAddress >= instructionPayload.IntCode.Count)
            {
                for (int i = instructionPayload.IntCode.Count; i < memoryAddress + 1; ++i)
                {
                    instructionPayload.IntCode.Add(0);
                }
            }

            return (int)memoryAddress;
        }

        private IInstruction GetInstruction(int instructionCode)
        {
            if (_supportedInstructions.TryGetValue(instructionCode, out var instruction))
            {
                return instruction.Invoke();
            }

            throw new ArgumentException($"Instruction {instructionCode} not supported");
        }

        private interface IInstruction
        {
            int Length { get; }

            Task<InstructionOutput> Run(InstructionPayload instructionPayload);
        }

        #region Instructions

        private class InstructionPayload
        {
            public long OpCode => IntCode[InstructionPointer];

            public int InstructionPointer { get; set; }

            public int RelativeBase { get; set; }

            public List<long> IntCode { get; set; }

            public ChannelReader<long> InputReader { get; set; }
        }

        private class InstructionOutput
        {
            public const int DefaultValue = int.MinValue;

            public long Result { get; set; } = DefaultValue;

            public int InstructionPointer { get; set; } = DefaultValue;

            public int RelativeBase { get; set; } = DefaultValue;
        }

        private abstract class BaseInstruction : IInstruction
        {
            public const int InstructionPointerValueWhenHalt = -1;

            public abstract int Length { get; }

            public abstract Task<InstructionOutput> Run(InstructionPayload instructionPayload);

            protected Task<InstructionOutput> Nothing(int newInstructionPointer) =>
                Task.FromResult(new InstructionOutput()
                {
                    InstructionPointer = newInstructionPointer
                });
        }

        private class Instruction0 : BaseInstruction
        {
            public override int Length => 1;

            public override Task<InstructionOutput> Run(InstructionPayload instructionPayload)
            {
                return Nothing(instructionPayload.InstructionPointer + Length);
            }
        }

        private class Instruction1 : BaseInstruction
        {
            public override int Length => 4;

            public override Task<InstructionOutput> Run(InstructionPayload instructionPayload)
            {
                instructionPayload.IntCode[ExtractMemoryAddress(instructionPayload, 3)] =
                    instructionPayload.IntCode[ExtractMemoryAddress(instructionPayload, 1)]
                    + instructionPayload.IntCode[ExtractMemoryAddress(instructionPayload, 2)];

                return Nothing(instructionPayload.InstructionPointer + Length);
            }
        }

        private class Instruction2 : BaseInstruction
        {
            public override int Length => 4;

            public override Task<InstructionOutput> Run(InstructionPayload instructionPayload)
            {
                instructionPayload.IntCode[ExtractMemoryAddress(instructionPayload, 3)] =
                    instructionPayload.IntCode[ExtractMemoryAddress(instructionPayload, 1)]
                    * instructionPayload.IntCode[ExtractMemoryAddress(instructionPayload, 2)];

                return Nothing(instructionPayload.InstructionPointer + Length);
            }
        }

        private class Instruction3 : BaseInstruction
        {
            public override int Length => 2;

            public async override Task<InstructionOutput> Run(InstructionPayload instructionPayload)
            {
                await instructionPayload.InputReader.WaitToReadAsync().ConfigureAwait(false);
                instructionPayload.InputReader.TryRead(out long input);

                instructionPayload.IntCode[ExtractMemoryAddress(instructionPayload, 1)] = input;

                return await Nothing(instructionPayload.InstructionPointer + Length).ConfigureAwait(false);
            }
        }

        private class Instruction4 : BaseInstruction
        {
            public override int Length => 2;

            public override Task<InstructionOutput> Run(InstructionPayload instructionPayload)
            {
                long output = instructionPayload.IntCode[ExtractMemoryAddress(instructionPayload, 1)];

                return Task.FromResult(new InstructionOutput()
                {
                    Result = output,
                    InstructionPointer = instructionPayload.InstructionPointer + Length
                });
            }
        }

        private class Instruction5 : BaseInstruction
        {
            public override int Length => 3;

            public override Task<InstructionOutput> Run(InstructionPayload instructionPayload)
            {
                long param1 = instructionPayload.IntCode[ExtractMemoryAddress(instructionPayload, 1)];

                long newInstructionPointer = param1 != 0
                    ? instructionPayload.IntCode[ExtractMemoryAddress(instructionPayload, 2)]
                    : instructionPayload.InstructionPointer + Length;

                return Nothing((int)newInstructionPointer);
            }
        }

        private class Instruction6 : BaseInstruction
        {
            public override int Length => 3;

            public override Task<InstructionOutput> Run(InstructionPayload instructionPayload)
            {
                long param1 = instructionPayload.IntCode[ExtractMemoryAddress(instructionPayload, 1)];

                long newInstructionPointer = param1 == 0
                    ? instructionPayload.IntCode[ExtractMemoryAddress(instructionPayload, 2)]
                    : instructionPayload.InstructionPointer + Length;

                return Nothing((int)newInstructionPointer);
            }
        }

        private class Instruction7 : BaseInstruction
        {
            public override int Length => 4;

            public override Task<InstructionOutput> Run(InstructionPayload instructionPayload)
            {
                long param1 = instructionPayload.IntCode[ExtractMemoryAddress(instructionPayload, 1)];
                long param2 = instructionPayload.IntCode[ExtractMemoryAddress(instructionPayload, 2)];

                instructionPayload.IntCode[ExtractMemoryAddress(instructionPayload, 3)] = param1 < param2
                    ? 1
                    : 0;

                return Nothing(instructionPayload.InstructionPointer + Length);
            }
        }

        private class Instruction8 : BaseInstruction
        {
            public override int Length => 4;

            public override Task<InstructionOutput> Run(InstructionPayload instructionPayload)
            {
                long param1 = instructionPayload.IntCode[ExtractMemoryAddress(instructionPayload, 1)];
                long param2 = instructionPayload.IntCode[ExtractMemoryAddress(instructionPayload, 2)];

                instructionPayload.IntCode[ExtractMemoryAddress(instructionPayload, 3)] = param1 == param2
                    ? 1
                    : 0;

                return Nothing(instructionPayload.InstructionPointer + Length);
            }
        }

        private class Instruction9 : BaseInstruction
        {
            public override int Length => 2;

            public override Task<InstructionOutput> Run(InstructionPayload instructionPayload)
            {
                return Task.FromResult(new InstructionOutput()
                {
                    RelativeBase = instructionPayload.RelativeBase + (int)instructionPayload.IntCode[ExtractMemoryAddress(instructionPayload, 1)],
                    InstructionPointer = instructionPayload.InstructionPointer + Length
                });
            }
        }

        private class Instruction99 : BaseInstruction
        {
            public override int Length => -1;

            public override Task<InstructionOutput> Run(InstructionPayload instructionPayload)
            {
                return Nothing(InstructionPointerValueWhenHalt);
            }
        }
        #endregion
    }
}
