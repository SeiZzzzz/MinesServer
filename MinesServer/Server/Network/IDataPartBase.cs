namespace MinesServer.Network
{
    public interface IDataPartBase
    {
        public abstract int Encode(Span<byte> output);

        /// <summary>
        /// Predictive length caclulation. This *should* be faster than Encode().Length in all cases
        /// </summary>
        public abstract int Length { get; }

        /// <summary>
        /// Packet name. Use a constant value for implementation!
        /// </summary>
        public abstract string PacketName { get; }
    }
}
