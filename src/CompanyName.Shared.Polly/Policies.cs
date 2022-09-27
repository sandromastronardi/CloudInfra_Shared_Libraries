using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;
using System;
using System.Net.Http;

namespace CompanyName.Shared.Resilience
{
    public class Policies
    {
        public static AsyncRetryPolicy<HttpResponseMessage> GetDefaultServiceHttpRetryPolicy(IServiceProvider services)
        {
            // Break the circuit after the specified number of consecutive exceptions
            // and keep circuit broken for the specified duration,
            // calling an action on change of circuit state.
            Action<Exception, TimeSpan> onBreak = (exception, timespan) => { };
            Action onReset = () => {  };


            var policy = HttpPolicyExtensions
                  .HandleTransientHttpError() // HttpRequestException, 5XX and 408
                  //.CircuitBreaker(2, TimeSpan.FromMinutes(1), onBreak, onReset)
                  .OrResult(response => (int)response.StatusCode == 429) // RetryAfter
                  //.CircuitBreaker(2, TimeSpan.FromMinutes(1), onBreak, onReset)
                  .WaitAndRetryAsync(new[]{
                      TimeSpan.FromSeconds(1),
                      TimeSpan.FromSeconds(5),
                      TimeSpan.FromSeconds(10)
                    }, onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        services.GetService<ILogger<Policies>>()?
                            .LogWarning("Delaying for {delay}ms, then making retry {retry}.", timespan.TotalMilliseconds, retryAttempt);
                    });
                return policy;
        }

        public static AsyncTimeoutPolicy<HttpResponseMessage> GetDefaultServiceHttpTimeoutPolicy(int seconds = 30)
        {
            var policy = Policy.TimeoutAsync<HttpResponseMessage>(seconds);
            return policy;
        }

        public static AsyncCircuitBreakerPolicy<HttpResponseMessage> GetDefaultServiceCircuitBreakerPolicy(int handledEventsAllowedBeforeBreaking = 5, int seconds = 30)
        {
            var policy = HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(handledEventsAllowedBeforeBreaking, TimeSpan.FromSeconds(seconds));
            return policy;
        }
    }
    public static class PollyContextExtensions
    {
        private static readonly string LoggerKey = "ILogger";

        public static Context WithLogger<T>(this Context context, ILogger logger)
        {
            context[LoggerKey] = logger;
            return context;
        }

        public static ILogger GetLogger(this Context context)
        {
            if (context.TryGetValue(LoggerKey, out object logger))
            {
                return logger as ILogger;
            }

            return null;
        }
    }
    }
