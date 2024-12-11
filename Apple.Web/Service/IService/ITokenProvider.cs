namespace Apple.Web.Service.IService
{
    public interface ITokenProvider
    {
        public void SetToken(string token);
        public string? GetToken();
        public void ClearedToken();
    }
}