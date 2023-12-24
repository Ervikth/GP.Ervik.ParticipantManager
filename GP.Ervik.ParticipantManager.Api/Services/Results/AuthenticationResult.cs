namespace GP.Ervik.ParticipantManager.Api.Services.Results
{
    public class AuthenticationResult
    {
        public bool IsAuthenticated { get; set; }
        public string Token { get; set; }
    }
}