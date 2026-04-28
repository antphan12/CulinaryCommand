using System.Net.Http.Headers;
using System.Text;
using Amazon;
using Amazon.Lambda;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Auth;
using Amazon.Runtime.Internal.Util;

namespace CulinaryCommandApp.SmartTask.Services
{
    public sealed class SigV4SigningHandler : DelegatingHandler
    {
        private const string LambdaServiceName = "lambda";

        private readonly AWSCredentials _awsCredentials;
        private readonly RegionEndpoint _awsRegion;

        public SigV4SigningHandler(AWSCredentials awsCredentials, RegionEndpoint awsRegion)
        {
            _awsCredentials = awsCredentials;
            _awsRegion = awsRegion;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage outboundRequest,
            CancellationToken cancellationToken)
        {
            var bodyPayload = outboundRequest.Content == null
                ? string.Empty
                : await outboundRequest.Content.ReadAsStringAsync(cancellationToken);

            var awsRequestForSigning = new DefaultRequest(new EmptyRequest(), LambdaServiceName)
            {
                HttpMethod = outboundRequest.Method.Method,
                Endpoint = new Uri(outboundRequest.RequestUri!.GetLeftPart(UriPartial.Authority)),
                ResourcePath = outboundRequest.RequestUri.AbsolutePath,
                Content = Encoding.UTF8.GetBytes(bodyPayload)
            };

            // Required headers for Lambda Function URL signing
            awsRequestForSigning.Headers["host"] = outboundRequest.RequestUri.Host;
            awsRequestForSigning.Headers["content-type"] = "application/json";

            var awsV4Signer = new AWS4Signer();
            var lambdaConfig = new AmazonLambdaConfig
            {
                RegionEndpoint = _awsRegion
            };

            // Use the supported Sign overload (IClientConfig + RequestMetrics)
            awsV4Signer.Sign(
                awsRequestForSigning,
                lambdaConfig,
                new RequestMetrics(),
                _awsCredentials);

            // Apply signed headers to the outbound HttpRequestMessage
            foreach (var signedHeader in awsRequestForSigning.Headers)
            {
                if (signedHeader.Key.StartsWith("content-", StringComparison.OrdinalIgnoreCase))
                {
                    outboundRequest.Content ??= new StringContent(string.Empty);
                    outboundRequest.Content.Headers.TryAddWithoutValidation(signedHeader.Key, signedHeader.Value);
                    continue;
                }

                outboundRequest.Headers.TryAddWithoutValidation(signedHeader.Key, signedHeader.Value);
            }

            outboundRequest.Content ??= new StringContent(string.Empty);
            outboundRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return await base.SendAsync(outboundRequest, cancellationToken);
        }

        private sealed class EmptyRequest : AmazonWebServiceRequest { }
    }
}