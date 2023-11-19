namespace MinesServer.Network
{
    public interface IDataPart<TSelf> : IDataPartBase where TSelf : IDataPart<TSelf>
    {
        public abstract static TSelf Decode(ReadOnlySpan<byte> decodeFrom);
    }
}
