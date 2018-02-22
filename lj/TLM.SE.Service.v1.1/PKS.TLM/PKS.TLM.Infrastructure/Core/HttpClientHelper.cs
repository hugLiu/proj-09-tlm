using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PKS.Core;
using PKS.Services;
using System;
using PKS.Utils;
using System.Collections;
using System.Web;
using PKS.Web;

namespace PKS.TLM.Infrastructure.Core
{
    /// <summary>令牌提供者</summary>
    public interface ITokenProvider
    {
        /// <summary>令牌</summary>
        string Token { get; }
    }

    /// <summary>Http客户端包装器基类</summary>
    /// <remarks>
    /// 如果不指定应答结果，
    ///     结果有三种，一种是流，一种是字符串，最后一种是HttpResponseMessage(无法解析ContentType)
    /// 如果指定应答结果，将尝试转换为应答结果，如果无法转换将生成<see cref="System.InvalidOperationException"/>异常
    /// </remarks>
    public class HttpClientHelper
    {
        #region 构造函数
        /// <summary>构造函数</summary>
        static HttpClientHelper()
        {
            DefaultInstance = CreateClient();
        }
        /// <summary>构造函数</summary>
        public HttpClientHelper(bool useDefault = true)
        {
            this.Instance = useDefault ? DefaultInstance : CreateClient();
            this.ReadContentHandlers = new Dictionary<Type, Func<HttpResponseMessage, Task<object>>>();
            InitReadHandlers();
        }
        /// <summary>Http客户端</summary>
        public HttpClient Instance { get; private set; }
        /// <summary>JSON发送序列化参数</summary>
        public JsonSerializerSettings JsonSendSerializerSettings { get; set; }
        /// <summary>JSON接收序列化参数</summary>
        public JsonSerializerSettings JsonRecvSerializerSettings { get; set; }
        /// <summary>默认Http客户端</summary>
        public static HttpClient DefaultInstance { get; }
        /// <summary>创建Http客户端</summary>
        private static HttpClient CreateClient()
        {
            var handler = new HttpClientHandler();
            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            handler.UseProxy = false;
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            client.DefaultRequestHeaders.Connection.Add("keep-alive");
            client.Timeout = TimeSpan.FromMinutes(30);
            return client;
        }

        /// <summary>构造带查询串的URL(类似http://www.temp.com/Index?a=***&amp;b=***)</summary>
        private string GetQueryUrl(string url, Dictionary<string, object> queryParams)
        {
            if (queryParams.IsNullOrEmpty()) return url;
            var url2 = new StringBuilder(url);
            url2.Append("?");
            foreach (var pair in queryParams)
            {
                var value = HttpUtility.UrlEncode(pair.Value.ToString());
                url2.Append(pair.Key)
                    .Append("=")
                    .Append(value)
                    .Append("&");
            }
            url2.Length -= 1;
            return url2.ToString();
        }
        #endregion

        #region 发送方法
        /// <summary>令牌提供者</summary>
        public ITokenProvider TokenProvider { get; set; }
        /// <summary>设置授权</summary>
        public void SetDefaultAuthorization(string token, bool allowClear = false)
        {
            if (token.IsNullOrEmpty())
            {
                if (allowClear) this.Instance.DefaultRequestHeaders.Authorization = null;
            }
            else
            {
                this.Instance.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(PKSWebExtension.AuthorizationSchema, token);
            }
        }
        /// <summary>发送</summary>
        private async Task<HttpResponseMessage> SendAsync(HttpMethod httpMethod, string url, HttpContent httpContent)
        {
            var request = new HttpRequestMessage(httpMethod, url);
            request.Method = httpMethod;
            request.Content = httpContent;
            if (this.TokenProvider != null && !this.TokenProvider.Token.IsNullOrEmpty())
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(PKSWebExtension.AuthorizationSchema, this.TokenProvider.Token);
            }
            return await this.Instance.SendAsync(request).ConfigureAwait(false);
        }
        #endregion

        #region GET方法
        /// <summary>Get请求</summary>
        public object Get(string url, Dictionary<string, object> queryParams = null)
        {
            return GetAsync(url, queryParams).Result;
        }
        /// <summary>Get请求</summary>
        public async Task<object> GetAsync(string url, Dictionary<string, object> queryParams = null)
        {
            var url2 = GetQueryUrl(url, queryParams);
            var response = await SendAsync(HttpMethod.Get, url2, null).ConfigureAwait(false);
            return await ReadContentAsync(response);
        }
        /// <summary>Get请求</summary>
        public T Get<T>(string url, Dictionary<string, object> queryParams = null)
        {
            return GetAsync<T>(url, queryParams).Result;
        }
        /// <summary>Get请求</summary>
        public async Task<T> GetAsync<T>(string url, Dictionary<string, object> queryParams = null)
        {
            var url2 = GetQueryUrl(url, queryParams);
            var response = await SendAsync(HttpMethod.Get, url2, null).ConfigureAwait(false);
            return await ReadContentAsync<T>(response);
        }
        #endregion

        #region POST方法
        /// <summary>生成请求内容</summary>
        protected virtual HttpContent BuildRequestContent(string content)
        {
            if (content == null) content = string.Empty;
            var httpContent = new StringContent(content);
            var mediaTypeHeaderValue = new MediaTypeHeaderValue(MimeTypes.JSON);
            mediaTypeHeaderValue.CharSet = Encoding.UTF8.WebName;
            httpContent.Headers.ContentType = mediaTypeHeaderValue;
            return httpContent;
        }
        /// <summary>序列化内容</summary>
        protected virtual string SerializableContent(object instance)
        {
            if (instance == null) return null;
            return instance.ToJson(this.JsonSendSerializerSettings);
        }
        /// <summary>Post请求</summary>
        public object Post(string url, string content = null)
        {
            return PostAsync(url, content).Result;
        }
        /// <summary>Post请求</summary>
        public object PostObject(string url, object instance = null)
        {
            var content = SerializableContent(instance);
            return PostAsync(url, content).Result;
        }
        /// <summary>Post请求</summary>
        public async Task<object> PostAsync(string url, string content = null)
        {
            var httpContent = BuildRequestContent(content);
            var response = await SendAsync(HttpMethod.Post, url, httpContent).ConfigureAwait(false);
            return await ReadContentAsync(response);
        }
        /// <summary>Post请求</summary>
        public async Task<object> PostObjectAsync(string url, object instance = null)
        {
            var content = SerializableContent(instance);
            return await PostAsync(url, content);
        }
        /// <summary>Post请求</summary>
        public T Post<T>(string url, string content = null)
        {
            return PostAsync<T>(url, content).Result;
        }
        /// <summary>Post请求</summary>
        public T PostObject<T>(string url, object instance = null)
        {
            var content = SerializableContent(instance);
            return PostAsync<T>(url, content).Result;
        }
        /// <summary>Post请求</summary>
        public async Task<T> PostAsync<T>(string url, string content = null)
        {
            var httpContent = BuildRequestContent(content);
            var response = await SendAsync(HttpMethod.Post, url, httpContent).ConfigureAwait(false);
            return await ReadContentAsync<T>(response);
        }
        /// <summary>Post请求</summary>
        public async Task<T> PostObjectAsync<T>(string url, object instance = null)
        {
            var content = SerializableContent(instance);
            return await PostAsync<T>(url, content);
        }
        #endregion
        
        #region 读应答内容方法
        /// <summary>读内容处理器字典</summary>
        private Dictionary<Type, Func<HttpResponseMessage, Task<object>>> ReadContentHandlers { get; set; }
        /// <summary>初始化读内容处理器字典</summary>
        private void InitReadHandlers()
        {
            AddReadHandler(typeof(object), ReadAsObjectAsync);
            AddReadHandler(typeof(HttpResponseMessage), ReadAsSelfAsync);
            AddReadHandler(typeof(Stream), ReadAsStreamAsync);
            AddReadHandler(typeof(byte[]), ReadAsByteArrayAsync);
            AddReadHandler(typeof(string), ReadAsStringAsync);
        }
        /// <summary>加入一个读处理器</summary>
        public void AddReadHandler(Type type, Func<HttpResponseMessage, Task<object>> handler)
        {
            this.ReadContentHandlers[type] = handler;
        }
        /// <summary>读内容处理器_根据MIME返回对象</summary>
        private async Task<object> ReadAsObjectAsync(HttpResponseMessage response)
        {
            return await ReadAsObjectAsync(response, null);
        }
        /// <summary>读内容处理器</summary>
        protected virtual async Task<object> ReadAsObjectAsync<T>(HttpResponseMessage response)
        {
            var result = await ReadAsObjectAsync(response, typeof(T));
            return result == null ? default(T) : result;
        }
        /// <summary>读内容处理器_返回应答对象</summary>
        private async Task<object> ReadAsSelfAsync(HttpResponseMessage response)
        {
            return await Task.FromResult(response);
        }
        /// <summary>读内容处理器_返回流对象</summary>
        protected async Task<object> ReadAsStreamAsync(HttpResponseMessage response)
        {
            return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        }
        /// <summary>读内容处理器_返回字节数组</summary>
        private async Task<object> ReadAsByteArrayAsync(HttpResponseMessage response)
        {
            return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }
        /// <summary>读内容处理器_返回字符串</summary>
        protected async Task<object> ReadAsStringAsync(HttpResponseMessage response)
        {
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
        /// <summary>读内容处理器_返回对象</summary>
        protected async Task<object> ReadAsObjectAsync(HttpResponseMessage response, Type resultType)
        {
            var httpContent = response.Content;
            var mediaType = httpContent.Headers.ContentType.MediaType;
            if (mediaType.IsStream())
            {
                if (resultType != null) return null;
                return await httpContent.ReadAsStreamAsync().ConfigureAwait(false);
            }
            var content = await httpContent.ReadAsStringAsync().ConfigureAwait(false);
            if (mediaType.Equals(MimeTypes.JSON, StringComparison.OrdinalIgnoreCase))
            {
                var jsonContent = content.ToString();
                if (resultType == null)
                {
                    return jsonContent.JsonTo(this.JsonRecvSerializerSettings);
                }
                return JsonConvert.DeserializeObject(jsonContent, resultType, this.JsonRecvSerializerSettings);
            }
            if (resultType != null) return null;
            return content;
        }
        #endregion

        #region 检查应答方法
        /// <summary>检查是否成功</summary>
        protected virtual void EnsureSuccess(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = ReadAsStringAsync(response).Result;
                var mediaType = response.Content.Headers.ContentType.MediaType;
                if (mediaType.Equals(MimeTypes.Exception, StringComparison.OrdinalIgnoreCase))
                {
                    var model = content.ToString().JsonTo<ExceptionModel>();
                    if (model != null && !model.Code.IsNullOrEmpty())
                    {
                        throw new UserFriendlyException(model.Code, model.Message, model.Details);
                    }
                }
            }
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                ex.Throw(response);
            }
        }
        /// <summary>读取应答数据</summary>
        private async Task<object> ReadContentAsync(HttpResponseMessage response)
        {
            EnsureSuccess(response);
            if (response.Content.Headers.ContentType != null)
            {
                return await ReadAsObjectAsync(response);
            }
            return response;
        }
        /// <summary>读取应答数据</summary>
        private async Task<T> ReadContentAsync<T>(HttpResponseMessage response)
        {
            EnsureSuccess(response);
            if (response.Content.Headers.ContentType != null)
            {
                var handler = this.ReadContentHandlers.GetValueBy(typeof(T));
                if (handler == null) handler = ReadAsObjectAsync<T>;
                var result = await handler(response);
                return (T)result;
            }
            return default(T);
        }
        #endregion
    }
}
