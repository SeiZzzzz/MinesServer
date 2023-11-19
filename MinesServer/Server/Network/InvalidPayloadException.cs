namespace MinesServer.Network
{
    public class InvalidPayloadException : Exception
    {
        public InvalidPayloadException(string message) : base(message) { }
    }
}
