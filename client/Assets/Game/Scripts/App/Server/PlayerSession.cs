namespace DuckDoku.App
{
    public class PlayerSession
    {
        public string PlayerId { get; private set; } = string.Empty;
        public string Token { get; private set; } = string.Empty;

        public PlayerProfile Profile { get; private set; }

        public bool IsAuthenticated => !string.IsNullOrEmpty(Token);

        public void SetAuthentication(string playerId, string token)
        {
            PlayerId = playerId;
            Token = token;
        }

        public void SetProfile(PlayerProfile profile)
        {
            Profile = profile;
        }

        public void Clear()
        {
            PlayerId = string.Empty;
            Token = string.Empty;
            Profile = null;
        }
    }
}