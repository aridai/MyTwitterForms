namespace MyTwitterForms.Model
{
    //  APIキー
    public class ApiKeys
    {
        public string ApiKey { get; }

        public string ApiSecretKey { get; }

        public ApiKeys(string apiKey, string apiSecretKey)
        {
            this.ApiKey = apiKey;
            this.ApiSecretKey = apiSecretKey;
        }
    }
}
