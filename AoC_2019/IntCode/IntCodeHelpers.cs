using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AoC_2019.IntCode
{
    public static class IntCodeHelpers
    {
        public static async Task<ICollection<long>> RunIntCodeProgram(List<long> intCode, params long[] input)
        {
            Channel<long> channel = Channel.CreateUnbounded<long>();
            IntCodeComputer computer = new IntCodeComputer(channel);

            foreach (long l in input)
            {
                await channel.Writer.WriteAsync(l).ConfigureAwait(false);
            }

            return await RunIntCodeProgramInternal(intCode, computer).ConfigureAwait(false);
        }

        public static async Task<ICollection<long>> RunIntCodeProgram(List<long> intCode)
        {
            IntCodeComputer computer = new IntCodeComputer();

            return await RunIntCodeProgramInternal(intCode, computer).ConfigureAwait(false);
        }

        private static async Task<ICollection<long>> RunIntCodeProgramInternal(List<long> intCode, IntCodeComputer computer)
        {
            ICollection<long> result = new List<long>();
            await foreach (var item in computer.RunIntCodeProgram(intCode))
            {
                result.Add(item);
            }

            return result;
        }
    }
}
