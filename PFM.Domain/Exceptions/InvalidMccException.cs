namespace PFM.Domain.Exceptions
{
    public class InvalidMccException : DomainException
    {
        public InvalidMccException(string mcc)
            : base($"The MCC code '{mcc}' is invalid.") { }
    }
}