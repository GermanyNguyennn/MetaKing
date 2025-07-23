namespace MetaKing.BackendServer.Helpers
{
    public class ApiConflictResponse : ApiResponse
    {
        public ApiConflictResponse(string message)
            : base(409, message)
        {
        }
    }
}
