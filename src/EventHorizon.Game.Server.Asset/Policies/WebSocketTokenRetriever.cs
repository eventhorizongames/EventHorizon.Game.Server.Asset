namespace EventHorizon.Game.Server.Asset.Policies
{
    using System;

    using IdentityModel.AspNetCore.OAuth2Introspection;

    using Microsoft.AspNetCore.Http;

    public static class WebSocketTokenRetriever
    {
        static Func<HttpRequest, string> AuthHeaderTokenRetriever { get; set; }
        static Func<HttpRequest, string> QueryStringTokenRetriever { get; set; }

        static WebSocketTokenRetriever()
        {
            AuthHeaderTokenRetriever = TokenRetrieval.FromAuthorizationHeader();
            QueryStringTokenRetriever = TokenRetrieval.FromQueryString();
        }

        public static string FromHeaderAndQueryString(HttpRequest request)
        {
            var token = AuthHeaderTokenRetriever(request);

            if (string.IsNullOrEmpty(token))
            {
                token = QueryStringTokenRetriever(request);
            }

            return token;
        }

    }
}
