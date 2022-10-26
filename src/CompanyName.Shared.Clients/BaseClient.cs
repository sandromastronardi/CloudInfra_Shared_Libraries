// Copyright (c) Mastronardi Software (mastrosoft.com). All Rights Reserved.
// Licensed under Mozilla Public License, Version 2.0

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CompanyName.Shared.Common;
using CompanyName.Shared.Resilience;
using Utf8Json;
using Utf8Json.Resolvers;

namespace CompanyName.Shared.Clients
{
    public abstract class BaseClient
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _clientName;
        private readonly ILogger _log;
        protected IJsonFormatterResolver _formatterResolver = null;

        protected BaseClient()
        {
			if(_formatterResolver == null){
				_formatterResolver = CompositeResolver.Create(
					EnumResolver.Default,
					StandardResolver.ExcludeNullCamelCase
				);
			}
        }

        protected BaseClient(IHttpClientFactory clientFactory, ILogger log, string clientName):this()
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _clientName = clientName ?? throw new ArgumentNullException(nameof(clientName));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        protected abstract ClientResult<TResult> CreateClientErrorResult<TResult>(HttpStatusCode statusCode, string content, string method);
        protected abstract ClientResult CreateClientErrorResult(HttpStatusCode statusCode, string content, string method);

        protected abstract Task<Notification[]> SetClientProperties(HttpClient client);

        private ClientResult<T> CreateClientErrorResult<T>(Exception e, string method)
        {
            _log?.LogError(e, $"Exception occured for {_clientName} {method}.");
            throw e;
        }
        private ClientResult CreateClientErrorResult(Exception e, string method)
        {
            _log?.LogError(e, $"Exception occured for {_clientName} {method}.");
            throw e;
        }

        protected async Task<ClientResult<TResult>> GetAsync<TResult>(string requestUri)
        {
            using (var client = _clientFactory.CreateClient())
            {
                var notifications = await SetClientProperties(client);
                return await Execute<TResult>(() => client.GetAsync(requestUri), requestUri, notifications);
            }
        }

        protected async Task<ClientResult<TResult>> GetAsync<TResult>(string requestUri, object value)
        {
            using (var client = _clientFactory.CreateClient())
            {
                var notifications = await SetClientProperties(client);
                return await Execute<TResult>(() => GetAsync(client, requestUri, value), requestUri, notifications);
            }
        }

        protected async Task<ClientResult<TResult>> PostAsJsonAsync<TResult>(string requestUri, object value)
        {
            using (var client = _clientFactory.CreateClient())
            {
                var notifications = await SetClientProperties(client);
                return await Execute<TResult>(() => client.PostAsJsonAsync(requestUri, value), requestUri, notifications);
            }
        }

        protected async Task<ClientResult<TResult>> PostAsync<TResult>(string requestUri, object value, MediaTypeFormatter formatter)
        {
            using (var client = _clientFactory.CreateClient())
            {
                var notifications = await SetClientProperties(client);
                return await Execute<TResult>(() => client.PostAsync(requestUri, value, formatter), requestUri, notifications);
            }
        }
        protected async Task<ClientResult> PostAsJsonAsync(string requestUri, object value)
        {
            using (var client = _clientFactory.CreateClient())
            {
                var notifications = await SetClientProperties(client);
                return await Execute(() => client.PostAsJsonAsync(requestUri, value), requestUri, notifications);
            }
        }

        protected async Task<ClientResult> PostAsync(string requestUri, object value, MediaTypeFormatter formatter)
        {
            using (var client = _clientFactory.CreateClient())
            {
                var notifications = await SetClientProperties(client);
                return await Execute(() => client.PostAsync(requestUri, value, formatter), requestUri, notifications);
            }
        }

        protected async Task<ClientResult<TResult>> PutAsJsonAsync<TResult>(string requestUri, object value)
        {
            using (var client = _clientFactory.CreateClient())
            {
                var notifications = await SetClientProperties(client);
                return await Execute<TResult>(() => client.PutAsJsonAsync(requestUri, value), requestUri, notifications);
            }
        }

        protected async Task<ClientResult<TResult>> PutAsync<TResult>(string requestUri, object value, MediaTypeFormatter formatter)
        {
            using (var client = _clientFactory.CreateClient())
            {
                var notifications = await SetClientProperties(client);
                return await Execute<TResult>(() => client.PutAsync(requestUri, value, formatter), requestUri, notifications);
            }
        }

        protected async Task<ClientResult> PutAsJsonAsync(string requestUri, object value)
        {
            using (var client = _clientFactory.CreateClient())
            {
                var notifications = await SetClientProperties(client);
                return await Execute(() => client.PutAsJsonAsync(requestUri, value), requestUri, notifications);
            }
        }

        protected async Task<ClientResult> PutAsync(string requestUri, object value, MediaTypeFormatter formatter)
        {
            using (var client = _clientFactory.CreateClient())
            {
                var notifications = await SetClientProperties(client);
                return await Execute(() => client.PutAsync(requestUri, value, formatter), requestUri, notifications);
            }
        }

        protected async Task<ClientResult<TResult>> DeleteAsync<TResult>(string requestUri)
        {
            using (var client = _clientFactory.CreateClient())
            {
                var notifications = await SetClientProperties(client);
                return await Execute<TResult>(() => client.DeleteAsync(requestUri), requestUri, notifications);
            }

        }
        protected async Task<ClientResult> DeleteAsync(string requestUri)
        {
            using (var client = _clientFactory.CreateClient())
            {
                var notifications = await SetClientProperties(client);
                return await Execute(() => client.DeleteAsync(requestUri), requestUri, notifications);
            }
        }

        public async Task<ClientResult<TResult>> Execute<TResult>(Func<Task<HttpResponseMessage>> func, string method, Notification[] notifications)
        {
            if (!notifications.IsSuccessful(false))
            {
                return new ClientResult<TResult>(HttpStatusCode.BadRequest, notifications);
            }
            try
            {
                var result = await func();
                //var content = await result.Content.ReadAsStringAsync();
                var stream = await result.Content.ReadAsStreamAsync();
                if (result.IsSuccessStatusCode)// && !string.IsNullOrWhiteSpace(content))
                {
                    //return new ClientResult<TResult>(JsonConvert.DeserializeObject<TResult>(content));
                    var data = Utf8Json.JsonSerializer.Deserialize<TResult>(stream, _formatterResolver);
                    if(data == null)
                    {
                        _log.LogDebug("Deserialized data is null");
                    }
                    return new ClientResult<TResult>(result.StatusCode, data);
                }
                var content = await result.Content.ReadAsStringAsync();
                return CreateClientErrorResult<TResult>(result.StatusCode, content, method);

            }
            catch (Exception e)
            {
                return CreateClientErrorResult<TResult>(e, method);
            }
        }

        public async Task<ClientResult> Execute(Func<Task<HttpResponseMessage>> func, string method, Notification[] notifications)
        {
            if (!notifications.IsSuccessful(false))
            {
                return new ClientResult(notifications);
            }
            try
            {
                var result = await func();
                var content = await result.Content.ReadAsStringAsync();
                if (result.IsSuccessStatusCode)
                {
                    return new EmptyClientResult(result.StatusCode);
                }

                return CreateClientErrorResult(result.StatusCode, content, method);
            }
            catch (Exception e)
            {
                return CreateClientErrorResult(e, method);
            }
        }

        protected async Task<ClientResult<TResult>> GetAsync<TResult>(string requestUri, string clientName)
        {
            using (var client = _clientFactory.CreateClient(clientName))
            {
                var notifications = await SetClientProperties(client);
                return await Execute<TResult>(() => client.GetAsync(requestUri), requestUri, notifications);
            }
        }

        protected async Task<ClientResult<TResult>> GetAsync<TResult>(string requestUri, object value, string clientName)
        {
            using (var client = _clientFactory.CreateClient(clientName))
            {
                var notifications = await SetClientProperties(client);
                return await Execute<TResult>(() => GetAsync(client, requestUri, value), requestUri, notifications);
            }
        }

        private async Task<HttpResponseMessage> GetAsync(HttpClient httpClient, string requestUri, object value)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(httpClient.BaseAddress, requestUri),
                //Content = new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json"),
                Content = new ByteArrayContent(Utf8Json.JsonSerializer.NonGeneric.Serialize(value, _formatterResolver)),//, Encoding.UTF8, "application/json"),
            };
            //var context = new Polly.Context().WithLogger(_log);
            //request.SetPolicyExecutionContext(context);
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            return response;
        }

        protected async Task<ClientResult<TResult>> PostAsJsonAsync<TResult>(string requestUri, object value, string clientName)
        {
            using (var client = _clientFactory.CreateClient(clientName))
            {
                var notifications = await SetClientProperties(client);
                return await Execute<TResult>(() => client.PostAsJsonAsync(requestUri, value), requestUri, notifications);
            }
        }

        protected async Task<ClientResult> DeleteAsJsonAsync(string requestUri, object value)
        {
            using (var client = _clientFactory.CreateClient())
            {
                var notifications = await SetClientProperties(client);
                return await Execute(() => client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri)
                {
                    Content = new StringContent(Utf8Json.JsonSerializer.ToJsonString(value), Encoding.UTF8,
                            "application/json")
                })
                , requestUri, notifications);
            }
        }

        protected async Task<ClientResult<TResult>> DeleteAsync<TResult>(string requestUri, string clientName)
        {
            using (var client = _clientFactory.CreateClient(clientName))
            {
                var notifications = await SetClientProperties(client);
                return await Execute<TResult>(() => client.DeleteAsync(requestUri), requestUri, notifications);
            }
        }

        private async Task<HttpResponseMessage> DeleteAsync(HttpClient httpClient, string requestUri, object value)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(httpClient.BaseAddress, requestUri),
                //Content = new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json"),
                Content = new ByteArrayContent(Utf8Json.JsonSerializer.NonGeneric.Serialize(value, _formatterResolver)),//, Encoding.UTF8, "application/json"),
            };
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            return response;
        }
    }
}