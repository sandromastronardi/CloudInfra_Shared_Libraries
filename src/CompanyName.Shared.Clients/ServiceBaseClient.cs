// Copyright (c) Mastronardi Software (mastrosoft.com). All Rights Reserved.
// Licensed under Mozilla Public License, Version 2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using IdentityModel.Client;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using CompanyName.Shared.Common;
using CompanyName.Shared.Common.Responses.Error;
using Utf8Json;

namespace CompanyName.Shared.Clients
{
    public class ServiceBaseClient : BaseClient
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IdentitySettings _identitySettings;
        private readonly ILogger _log;
        private readonly IDistributedCache _distributedCache;
        private readonly string _baseUrl;
        private readonly string _scope;
        private readonly string _apiKey;
        //private readonly BaseSettings settings;

        public Func<string> GetBearerToken;

        protected readonly MediaTypeFormatter _formatter = new JsonMediaTypeFormatter
        {
            SerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                NullValueHandling = NullValueHandling.Ignore
            }
        };


        protected ServiceBaseClient(IHttpClientFactory clientFactory, IOptions<IdentitySettings> identitySettings, ILogger log, BaseSettings settings, string scope, IDistributedCache distributedCache) : base(clientFactory, log, scope)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _identitySettings = identitySettings.Value ?? throw new ArgumentNullException(nameof(identitySettings));
            _baseUrl = settings?.BaseUrl ?? throw new ArgumentNullException(nameof(settings)+"."+nameof(settings.BaseUrl));
            _apiKey = settings?.ApiKey;
            _scope = _identitySettings.Scope ?? scope ?? throw new ArgumentNullException(nameof(scope));
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _distributedCache = distributedCache; // https://docs.microsoft.com/en-us/azure/architecture/multitenant-identity/token-cache
        }


        protected override ClientResult<TResult> CreateClientErrorResult<TResult>(HttpStatusCode statusCode, string content, string method)
        {
            if (!string.IsNullOrEmpty(content))
            {
                _log.LogDebug(content);

                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(content);
                var notifications = new List<Notification>(errorResponse.Notifications ?? new Notification[0]);
                if(!string.IsNullOrWhiteSpace(errorResponse.Message))
                {
                    notifications.Add(new Notification(NotificationLevel.Error, errorResponse.Message));
                }
                if (notifications.Any(x => x.Level == NotificationLevel.Error))
                {
                    _log.LogError($"Client {_scope} returned errors on method {method} with response: {errorResponse}");
                }
                else
                {
                    _log.LogWarning($"Client {_scope} returned errors on method {method} with response: {errorResponse}");
                }
                return new ClientResult<TResult>(statusCode, notifications.ToArray());
            }
            return new ClientResult<TResult>(statusCode, default(TResult));
        }

        protected override ClientResult CreateClientErrorResult(HttpStatusCode statusCode, string content, string method)
        {
            if (!string.IsNullOrEmpty(content))
            {
                _log.LogDebug(content);

                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(content);
                var notifications = new List<Notification>(errorResponse.Notifications ?? new Notification[0]);
                if (!string.IsNullOrWhiteSpace(errorResponse.Message))
                {// Catch APIM Error Message
                    notifications.Add(new Notification(NotificationLevel.Error, errorResponse.Message));
                }
                if (notifications.Any(x => x.Level == NotificationLevel.Error))
                {
                    _log.LogError($"Client {_scope} returned errors on method {method} with response: {errorResponse}");
                }
                else
                {
                    _log.LogWarning($"Client {_scope} returned errors on method {method} with response: {errorResponse}");
                }
                return new ClientResult(statusCode,notifications.ToArray());
            }
            return new ClientResult() { StatusCode = statusCode };
        }

        protected override async Task<Notification[]> SetClientProperties(HttpClient client)
        {
            string accessToken = null;
            if (_apiKey == null)
            {

                if (GetBearerToken != null)
                {
                    accessToken = GetBearerToken();
                }
                else
                {
                    var token = await GetJwtToken();
                    accessToken = token?.AccessToken;
                }
            }
            //if (token.IsError)
            //{
            //    return new []{Notification.Error(token.Error)};
            //}
            client.BaseAddress = new Uri(_baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            if(!string.IsNullOrWhiteSpace(_apiKey))
            {
                client.DefaultRequestHeaders.Add("x-functions-key", _apiKey);
            }
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(this.GetType().FullName, this.GetType().Assembly.GetName().Version.ToString()));
#if(DEBUG)
            client.Timeout = TimeSpan.FromMinutes(30);
#endif
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if(!string.IsNullOrWhiteSpace(_apiKey))
            {
                client.DefaultRequestHeaders.Add("Subscription-Key", _apiKey);
            }
            if (_apiKey == null)
            {
                if (accessToken != null)
                {
                    client.SetBearerToken(accessToken);
                }
                else
                {
                    if (GetBearerToken == null)
                    {

                        //client.SetBearerToken(GetBearerToken());
                        //}
                        //else
                        //{
                        throw new Exception("You must provide the GetBearerToken method for returning the token");
                    }
                }
            }


            return new Notification[0];
        }
        
        private async Task<AccessTokenResponse> GetJwtToken()
        {
            string cacheKey = $"{this.GetType().FullName}:ClientID:{_identitySettings.Scope}:{_identitySettings.ClientId}.JwtToken";
            var cachedToken = await _distributedCache.GetAsync(cacheKey);
            if (cachedToken != null && cachedToken.Length>0)//!string.IsNullOrWhiteSpace(cachedToken))
            {
                return Utf8Json.JsonSerializer.Deserialize<AccessTokenResponse>(cachedToken);// System.Text.UTF8Encoding.UTF8.GetBytes(cachedToken));
               // return JsonConvert.DeserializeObject<AccessTokenResponse>(cachedToken);
            }
            else { 
                using (var httpClient = _clientFactory.CreateClient())
                {
                    using (HttpClientAuthenticationConnection connection = new HttpClientAuthenticationConnection(httpClient))
                    {
                        //var tokenEndpoint = $"{_identitySettings.BaseUrl}/connect/token";
                        _log.LogDebug("Authenticating to: " + _identitySettings.BaseUrl);
                        using (Auth0.AuthenticationApi.AuthenticationApiClient client = new Auth0.AuthenticationApi.AuthenticationApiClient(new Uri(_identitySettings.BaseUrl), connection))
                        {
                            try
                            {
                                AccessTokenResponse tokenResponse = null;
                                if (_identitySettings.IsDevice)
                                {
                                    tokenResponse = await client.GetTokenAsync(new Auth0.AuthenticationApi.Models.ClientCredentialsTokenRequest
                                    {
                                        ClientId = _identitySettings.ClientId,
                                        ClientSecret = _identitySettings.ClientSecret,
                                        Audience = _identitySettings.Audience
                                    });
                                    if (tokenResponse.ExpiresIn > 60)
                                    {
                                        using (MemoryStream ms = new MemoryStream()) {
                                            await Utf8Json.JsonSerializer.SerializeAsync(ms, tokenResponse);
                                            ms.Position = 0;
                                            using (StreamReader sr = new StreamReader(ms)) {
                                                await _distributedCache.SetStringAsync(cacheKey, await sr.ReadToEndAsync(), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(tokenResponse.ExpiresIn - 60) }); // expire cache 60 seconds prior to real expiration
                                            }
                                        }
                                    }
                                }
                                return tokenResponse;
                            }
                            catch (Exception e)
                            {
                                _log.LogError(e, "Error while requesting client credentials token.");
                                //return new TokenResponse(HttpStatusCode.InternalServerError, "Error while request client credentials token.", null);
                                //return null;
                                throw;
                            }
                        }

                    }
                }
            }
        }
    }
}