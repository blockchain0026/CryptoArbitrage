using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Infrastructure.ExchangeAPIs
{
    public class ExchangeDelegatingHandler : DelegatingHandler
    {
        //Obtained from the server earlier, APIKey MUST be stored securely and in App.Config
        private readonly string APIKey;
        private readonly string APISecret;
        private readonly string HashMethod;
        private readonly string ExchangeId;

        public const string HASH_HMAC512 = "hmac512";
        public const string HASH_HMAC256 = "hmac256";
        public const string HASH_HMAC1 = "hmac1";

        public ExchangeDelegatingHandler(ExchangeAPI exchangeAPI)
        {
            this.APIKey = exchangeAPI.ApiKey;
            this.APISecret = exchangeAPI.ApiSecret;
            this.ExchangeId = "0";
        }
        public ExchangeDelegatingHandler(ExchangeAPI exchangeAPI, string hashFunction, string exchangeId)
        {
            this.APIKey = exchangeAPI.ApiKey;
            this.APISecret = exchangeAPI.ApiSecret;
            this.HashMethod = hashFunction;
            this.ExchangeId = exchangeId;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            HttpResponseMessage response = null;
            string requestContentBase64String = string.Empty;

            //create random nonce for each request
            string requestUri = HttpUtility.UrlEncode(request.RequestUri.AbsoluteUri.ToLower());

            //Checking if the request contains body, usually will be null wiht HTTP GET and DELETE
            if (request.Content != null)
            {
                var content = await request.Content.ReadAsStringAsync();
                requestContentBase64String = this.GetEncryptedContent(MD5.Create(), Encoding.UTF8, content);
            }


            /*Encoding encoding = Encoding.ASCII;
            using (var hmac = this.GetHmacFunction(encoding.GetBytes(APISecret)))
            {
                var uri = request.RequestUri.ToString();
                var msg = encoding.GetBytes(uri);
                byte[] hash = hmac.ComputeHash(msg);
                string requestSignatureBase64String = BitConverter.ToString(hash).ToLower().Replace("-", string.Empty);
                var header = this.GetHeaders(requestSignatureBase64String);
                request.Headers.Add(header.Item1, header.Item2);
            }*/

            var fullRequest = await GetRequestMessage(request);
            response = await base.SendAsync(
                fullRequest,
                cancellationToken
                );

            return response;
        }




        private async Task<HttpRequestMessage> GetRequestMessage(HttpRequestMessage oldRequest)
        {
            var request = oldRequest;

            if (ExchangeId == "0")
            {
                Encoding encoding = Encoding.ASCII;
                using (var hmac = this.GetHmacFunction(encoding.GetBytes(APISecret)))
                {
                    var uri = request.RequestUri.ToString();
                    var msg = encoding.GetBytes(uri);
                    byte[] hash = hmac.ComputeHash(msg);
                    string requestSignatureBase64String = BitConverter.ToString(hash).ToLower().Replace("-", string.Empty);
                    var header = this.GetHeaders(requestSignatureBase64String);
                    request.Headers.Add(header.Item1, header.Item2);
                }
            }
            else if (ExchangeId == "1")
            {
                Encoding encoding = Encoding.UTF8;
                using (var hmac = this.GetHmacFunction(encoding.GetBytes(APISecret)))
                {
                    var requestParams = request.RequestUri.Query;
                    //var timestamp = this.GetTimestampInMillis();
                    var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
                    requestParams = String.IsNullOrEmpty(requestParams) ? requestParams : requestParams.Substring(1);
                    requestParams += String.IsNullOrEmpty(requestParams) ? ("timestamp=" + timestamp) : ("&timestamp=" + timestamp);
                    //requestParams += "&recvWindow=" + "30000000";
                    var msg = encoding.GetBytes(requestParams);
                    byte[] hash = hmac.ComputeHash(msg);
                    string requestSignatureBase64String = BitConverter.ToString(hash).ToLower().Replace("-", string.Empty);

                    request.RequestUri = this.AppendQueryStringToUri(request.RequestUri, Tuple.Create("timestamp", timestamp), Tuple.Create("signature", requestSignatureBase64String));

                    var header = this.GetHeaders(APIKey);
                    request.Headers.Add(header.Item1, header.Item2);
                }
            }
            else if (ExchangeId == "2")
            {
                //Encoding encoding = Encoding.UTF8;
                var apiSign = string.Empty;

                #region Api Sign
                Int64 nonce = DateTime.UtcNow.Ticks;
                string nonceWithOriginalPostData = "nonce=" + nonce;

                var requestParams = request.RequestUri.ParseQueryString();
                if (requestParams.Count != 0)
                {
                    var args = new List<KeyValuePair<string, string>>();

                    for (int i = 0; i < requestParams.Count; i++)
                    {


                        args.Add(new KeyValuePair<string, string>(requestParams.Keys[i], requestParams[i]));

                    }

                    request.Content = new FormUrlEncodedContent(args);
                }

                if (request.Content != null)
                {
                    string content = await request.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(content))
                    {
                        nonceWithOriginalPostData = nonceWithOriginalPostData + "&" + content;
                    }
                }



                request.Content = CreateHttpContent(nonceWithOriginalPostData);
                byte[] key = Convert.FromBase64String(APISecret);
                byte[] path = Encoding.UTF8.GetBytes(request.RequestUri.LocalPath);

                string nonceAndContent = nonce + Convert.ToChar(0) + nonceWithOriginalPostData;
                byte[] hashedNonceAndContent;

                using (SHA256 hashFunction = SHA256Managed.Create())
                {
                    hashedNonceAndContent = hashFunction.ComputeHash(Encoding.UTF8.GetBytes(nonceAndContent));
                }

                // HMAC-SHA512 of (URI path + SHA256(nonce + POST data))
                byte[] rawSignature = new byte[path.Count() + hashedNonceAndContent.Count()];
                path.CopyTo(rawSignature, 0);
                hashedNonceAndContent.CopyTo(rawSignature, path.Count());


                //Message signature using HMAC-SHA512 of (URI path + SHA256(nonce + POST data)) and base64 decoded secret API key
                using (HMACSHA512 hmacsha = new HMACSHA512(key))
                {
                    apiSign = Convert.ToBase64String(hmacsha.ComputeHash(rawSignature));
                }

                request.RequestUri = RemoveQueryStringToUri(request.RequestUri);
                #endregion


                #region Headers
                request.Headers.Add("API-Key", APIKey);
                request.Headers.Add("API-Sign", apiSign);
                #endregion


                return request;
                /*var requestParams = request.RequestUri.ParseQueryString();

                var encryptedData = this.GetEncryptedContent(SHA1.Create(), Encoding.UTF8, JsonConvert.SerializeObject(requestParams));

                var msg = encoding.GetBytes(
                    request.Method.Method + "\n"
                    + "application/json" + "\n"
                    + DateTime.Now.ToUniversalTime() + "\n"
                    + request.RequestUri.LocalPath);
                byte[] hash = hmac.ComputeHash(msg);

                string requestSignatureBase64String = BitConverter.ToString(hash).ToLower().Replace("-", string.Empty);


                request.RequestUri = this.RemoveQueryStringToUri(request.RequestUri);

                var header = this.GetHeaders(requestSignatureBase64String);
                request.Headers.Add(header.Item1, header.Item2);
                request.Content = new StringContent("{\"date\":\"" + DateTime.UtcNow.ToUniversalTime() + "\"", Encoding.UTF8, "application/json");*/
            }
            else if (ExchangeId == "3")
            {
                Encoding encoding = Encoding.ASCII;
                using (var hmac = this.GetHmacFunction(encoding.GetBytes(APISecret)))
                {
                    var uri = request.RequestUri;
                    var requestParams = uri.ParseQueryString();

                    var nonce = DateTime.Now.Ticks;

                    var userId = "oehp2797";
                    var key = this.APIKey;

                    var msg = encoding.GetBytes(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}",
                        nonce,
                        userId,
                        key));


                    byte[] hash = hmac.ComputeHash(msg);

                    //string requestSignatureBase64String = BitConverter.ToString(hash).ToUpper(CultureInfo.InvariantCulture).Replace("-", string.Empty);

                    var hex = new StringBuilder(hash.Length * 2);
                    foreach (byte b in hash)
                        hex.AppendFormat("{0:x2}", b);
                    string requestSignatureBase64String = hex.ToString().ToUpper(CultureInfo.InvariantCulture);


                    request.RequestUri = RemoveQueryStringToUri(uri);


                    var args = new List<KeyValuePair<string, string>>();
                    foreach (var a in requestParams)
                    {



                        for (int i = 0; i < requestParams.Count; i++)
                        {
                            args.Add(new KeyValuePair<string, string>(requestParams.Keys[i], requestParams[i]));
                        }
                    }
                    args.Add(new KeyValuePair<string, string>("key", key));
                    args.Add(new KeyValuePair<string, string>("signature", requestSignatureBase64String));
                    args.Add(new KeyValuePair<string, string>("nonce", nonce.ToString(CultureInfo.InvariantCulture)));
                    request.Content = new FormUrlEncodedContent(
                        args
                        );

                    /*var header = this.GetHeaders(APIKey);
                    request.Headers.Add(header.Item1, header.Item2);*/
                    // request.Content = new StringContent("{\"nonce\":\"" + timestamp + "\"" + ",\"customer_id\":\"" + requestParams["curstomer_id"] + "\"" + ",\"key\":\"" + APIKey + "\"", Encoding.UTF8, "application/x-www-form-urlencoded");
                }
            }
            else if(ExchangeId=="5")
            {
                #region Api Sign
                string apiSign = string.Empty;
                var requestParams = request.RequestUri.ParseQueryString();
                if (requestParams.Count != 0)
                {
                    var args = new List<KeyValuePair<string, string>>();

                    for (int i = 0; i < requestParams.Count; i++)
                    {


                        args.Add(new KeyValuePair<string, string>(requestParams.Keys[i], requestParams[i]));

                    }
                    
                    request.Content = new FormUrlEncodedContent(args);
                }

                if (request.Content != null)
                {
                    string content = await request.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(content))
                    {
                        var encoding = new UTF8Encoding();

                        var keyBytes = encoding.GetBytes(APISecret);
                        var formBytes = encoding.GetBytes(content);

                        var sha512 = GetHmacFunction(keyBytes);
                        var hashBytes = sha512.ComputeHash(formBytes);
                        apiSign= BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                    }
                }

                
                #endregion

                #region Headers
                request.Headers.Add("KEY", APIKey);
                request.Headers.Add("SIGN", apiSign);
                #endregion

                request.RequestUri = RemoveQueryStringToUri(request.RequestUri);
            }
            return request;
        }

        private string GetSignature()
        {
            throw new NotImplementedException();
        }

        private HMAC GetHmacFunction(byte[] encodingSecret)
        {
            switch (HashMethod)
            {
                case HASH_HMAC256:
                    return new HMACSHA256(encodingSecret);
                case HASH_HMAC512:
                    return new HMACSHA512(encodingSecret);
                case HASH_HMAC1:
                    return new HMACSHA1(encodingSecret);
                default:
                    return new HMACSHA512(encodingSecret);
            }
        }

        private Tuple<string, string> GetHeaders(string value)
        {
            switch (ExchangeId)
            {
                case "0":
                    return Tuple.Create("apisign", value);
                case "1":
                    return Tuple.Create("X-MBX-APIKEY", value);
                case "2":
                    return Tuple.Create("auth", value);

                default:
                    return Tuple.Create("apisign", value);
            }
        }

        private Uri AppendQueryStringToUri(Uri originalUri, params Tuple<string, string>[] paramToAppend)
        {
            var uriBuilder = new UriBuilder(originalUri.Scheme + "://" + originalUri.Host + originalUri.LocalPath);
            var query = originalUri.ParseQueryString();
            foreach (var p in paramToAppend)
            {
                query[p.Item1] = p.Item2;
            }

            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri;
        }

        private Uri RemoveQueryStringToUri(Uri originalUri)
        {
            string localPathWithoutQueryString = string.Empty;
            //int segmentsAmounts = string.IsNullOrEmpty(originalUri.Query) ? originalUri.Segments.Length : (originalUri.Segments.Length - 1);
            int segmentsAmounts = originalUri.Segments.Length;
            for (int i = 0; i < segmentsAmounts; i++)
            {
                localPathWithoutQueryString += originalUri.Segments[i];
            }

            var uriBuilder = new UriBuilder(originalUri.Scheme + "://" + originalUri.Host + localPathWithoutQueryString);
            return uriBuilder.Uri;
        }

        private string GetEncryptedContent(HashAlgorithm algorithm, Encoding encodingType, string content)
        {
            byte[] bytes_in = encodingType.GetBytes(content);
            byte[] bytes_out = algorithm.ComputeHash(bytes_in);

            string result = BitConverter.ToString(bytes_out);
            result = result.Replace("-", "");
            return result;
        }

        private string GetTimestamp()
        {
            //Calculate UNIX time
            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            return Convert.ToUInt64(timeSpan.TotalSeconds).ToString();
        }

        private string GetTimestampInMillis()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
        }

        private static HttpContent CreateHttpContent(string postData)
        {
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(postData);
            IEnumerable<KeyValuePair<string, string>> result = from string key in nameValueCollection select new KeyValuePair<string, string>(key, nameValueCollection[key]);
            return new FormUrlEncodedContent(result);
        }


        private bool IsResponseValid(HttpResponseMessage response)
        {
            if ((response != null) && (response.StatusCode == HttpStatusCode.OK))
                return true;
            return false;
        }

    }
}
