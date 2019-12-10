namespace AoC_2019.IntCode.Instructions
{
    public class InstructionOutput
    {
        public const int DefaultValue = int.MinValue;

        public long Result { get; set; } = DefaultValue;

        public int InstructionPointer { get; set; } = DefaultValue;

        public int RelativeBase { get; set; } = DefaultValue;
    }
}
