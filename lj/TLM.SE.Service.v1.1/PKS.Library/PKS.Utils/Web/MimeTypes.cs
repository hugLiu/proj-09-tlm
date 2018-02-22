namespace PKS.Web
{
    /// <summary>HTTP应答内容类型常量</summary>
    public static class MimeTypes
    {
        /// <summary>流</summary>
        public const string Stream = "application/octet-stream";
        /// <summary>JSON</summary>
        public const string JSON = "application/json";
        /// <summary>Url</summary>
        public const string Url = "application/jurassic-url";
        /// <summary>异常</summary>
        public const string Exception = "application/jurassic-exception+json";
        /// <summary>MIME类型是否流</summary>
        public static bool IsStream(this string mediaType)
        {
            if (mediaType.StartsWith("text/", System.StringComparison.OrdinalIgnoreCase)) return false;
            if (mediaType.EndsWith("json", System.StringComparison.OrdinalIgnoreCase)) return false;
            return true;
        }
    }
}
