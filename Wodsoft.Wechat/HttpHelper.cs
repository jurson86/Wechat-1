﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Wechat
{
    /// <summary>
    /// Http助手。
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        /// Get获取内容。
        /// </summary>
        /// <param name="uri">地址。</param>
        /// <param name="encoding">编码</param>
        /// <param name="timeout">超时（毫秒）。</param>
        /// <returns>返回响应内容。</returns>
        public static async Task<string> GetHttp(Uri uri, Encoding encoding, int timeout)
        {
            HttpWebRequest request = HttpWebRequest.CreateHttp(uri);
            request.Timeout = timeout;
            request.Method = "GET";
            request.AllowAutoRedirect = true;
            var response = await request.GetResponseAsync();
            var stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, encoding);
            string result = await reader.ReadToEndAsync();
            return result;
        }

        /// <summary>
        /// Get获取内容。
        /// </summary>
        /// <param name="uri">地址。</param>
        /// <param name="encoding">编码。</param>
        /// <returns>返回响应内容。</returns>
        public static Task<string> GetHttp(Uri uri, Encoding encoding)
        {
            return GetHttp(uri, encoding, 5000);
        }

        /// <summary>
        /// Get获取内容。
        /// </summary>
        /// <param name="uri">地址。</param>
        /// <param name="timeout">超时（毫秒）。</param>
        /// <returns>返回响应内容。</returns>
        public static Task<string> GetHttp(Uri uri, int timeout)
        {
            return GetHttp(uri, Encoding.UTF8, timeout);
        }

        /// <summary>
        /// Get获取内容。
        /// </summary>
        /// <param name="uri">地址。</param>
        /// <returns>返回响应内容。</returns>
        public static Task<string> GetHttp(Uri uri)
        {
            return GetHttp(uri, Encoding.UTF8, 5000);
        }

        /// <summary>
        /// Get获取内容。
        /// </summary>
        /// <param name="url">地址。</param>
        /// <returns>返回响应内容。</returns>
        public static Task<string> GetHttp(string url)
        {
            return GetHttp(new Uri(url));
        }

        /// <summary>
        /// Get获取内容。
        /// </summary>
        /// <param name="url">地址。</param>
        /// <param name="querystring"></param>
        /// <returns>返回响应内容。</returns>
        public static Task<string> GetHttp(string url, object querystring)
        {
            var type = querystring.GetType();
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (var property in type.GetProperties())
            {
                data.Add(property.Name, property.GetValue(querystring).ToString());
            }
            url += "?" + GetQueryString(data);
            return GetHttp(url);
        }

        /// <summary>
        /// Post提交数据。
        /// </summary>
        /// <param name="uri">地址。</param>
        /// <param name="rawData">原始数据。</param>
        /// <param name="contentType">内容类型。</param>
        /// <param name="encoding">编码。</param>
        /// <param name="timeout">超时（毫秒）。</param>
        /// <param name="cert">客户端证书。</param>
        /// <returns>返回响应内容。</returns>
        public static async Task<string> PostHttp(Uri uri, byte[] rawData, string contentType, Encoding encoding, int timeout, X509Certificate2 cert)
        {
            HttpWebRequest request = HttpWebRequest.CreateHttp(uri);
            request.Timeout = timeout;
            request.Method = "POST";
            request.ContentType = contentType;
            request.ContentLength = rawData.Length;
            if (cert != null)
                request.ClientCertificates.Add(cert);
            request.AllowAutoRedirect = true;
            {
                var stream = await request.GetRequestStreamAsync();
                await stream.WriteAsync(rawData, 0, rawData.Length);
                stream.Close();
            }
            {
                var response = await request.GetResponseAsync();
                var stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, encoding);
                string result = await reader.ReadToEndAsync();
                return result;
            }
        }

        /// <summary>
        /// Post提交数据。
        /// </summary>
        /// <param name="uri">地址。</param>
        /// <param name="rawData">原始数据。</param>
        /// <param name="contentType">内容类型。</param>
        /// <param name="encoding">编码。</param>
        /// <returns>返回响应内容。</returns>
        public static Task<string> PostHttp(Uri uri, byte[] rawData, string contentType, Encoding encoding)
        {
            return PostHttp(uri, rawData, contentType, encoding, 5000, null);
        }

        /// <summary>
        /// Post提交数据。
        /// </summary>
        /// <param name="uri">地址。</param>
        /// <param name="formData">表单数据。</param>
        /// <param name="encoding">编码。</param>
        /// <returns>返回响应内容。</returns>
        public static Task<string> PostHttp(Uri uri, object formData, Encoding encoding)
        {
            var type = formData.GetType();
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (var property in type.GetProperties())
            {
                data.Add(property.Name, property.GetValue(formData).ToString());
            }
            return PostHttp(uri, data, encoding);
        }

        /// <summary>
        /// Post提交数据。
        /// </summary>
        /// <param name="uri">地址。</param>
        /// <param name="formData">表单数据。</param>
        /// <returns>返回响应内容。</returns>
        public static Task<string> PostHttp(Uri uri, object formData)
        {
            return PostHttp(uri, formData, Encoding.UTF8);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri">地址。</param>
        /// <param name="formData"></param>
        /// <param name="encoding">编码。</param>
        /// <returns>返回响应内容。</returns>
        public static Task<string> PostHttp(Uri uri, IDictionary<string, string> formData, Encoding encoding)
        {
            return PostHttp(uri, encoding.GetBytes(GetFormString(formData)), "application/x-www-form-urlencoded", encoding);
        }

        /// <summary>
        /// Post提交数据。
        /// </summary>
        /// <param name="uri">地址。</param>
        /// <param name="formData">表单数据。</param>
        /// <returns>返回响应内容。</returns>
        public static Task<string> PostHttp(Uri uri, IDictionary<string, string> formData)
        {
            return PostHttp(uri, formData, Encoding.UTF8);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">地址。</param>
        /// <param name="formData"></param>
        /// <returns>返回响应内容。</returns>
        public static Task<string> PostHttp(string url, object formData)
        {
            return PostHttp(new Uri(url), formData);
        }

        /// <summary>
        /// Post提交数据。
        /// </summary>
        /// <param name="url">地址。</param>
        /// <param name="formData">表单数据。</param>
        /// <returns>返回响应内容。</returns>
        public static Task<string> PostHttp(string url, IDictionary<string, string> formData)
        {
            return PostHttp(new Uri(url), formData);
        }

        /// <summary>
        /// Post提交文件。
        /// </summary>
        /// <param name="uri">地址。</param>
        /// <param name="timeout">超时（毫秒）。</param>
        /// <param name="cert">客户端证书。</param>
        /// <param name="parts">内容部分。</param>
        /// <returns>返回响应内容。</returns>
        public static async Task<string> PostHttp(Uri uri, int timeout, X509Certificate2 cert, params HttpPart[] parts)
        {
            var boundary = "--" + Guid.NewGuid().ToString();
            var startBoundary = Encoding.UTF8.GetBytes("--" + boundary);
            var endBoundary = Encoding.UTF8.GetBytes("--" + boundary + "--");

            HttpWebRequest request = HttpWebRequest.CreateHttp(uri);
            request.Timeout = timeout;
            request.Method = "POST";
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            if (cert != null)
                request.ClientCertificates.Add(cert);
            request.AllowAutoRedirect = true;
            {
                var stream = await request.GetRequestStreamAsync();
                foreach (var part in parts)
                {
                    await stream.WriteAsync(startBoundary, 0, startBoundary.Length);
                    await part.WriteContent(stream);
                }
                await stream.WriteAsync(endBoundary, 0, endBoundary.Length);
                stream.Close();
            }
            {
                var response = await request.GetResponseAsync();
                var stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string result = await reader.ReadToEndAsync();
                return result;
            }
        }

        /// <summary>
        /// Post提交文件。
        /// </summary>
        /// <param name="url">地址。</param>
        /// <param name="timeout">超时（毫秒）。</param>
        /// <param name="parts">内容部分。</param>
        /// <returns>返回响应内容。</returns>
        public static Task<string> PostHttp(string url, int timeout, params HttpPart[] parts)
        {
            return PostHttp(new Uri(url), timeout, null, parts);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">地址。</param>
        /// <param name="parts"></param>
        /// <returns>返回响应内容。</returns>
        public static Task<string> PostHttp(string url, params HttpPart[] parts)
        {
            return PostHttp(new Uri(url), 10000, null, parts);
        }

        /// <summary>
        /// 获取查询字符串。
        /// </summary>
        /// <param name="dictionary">字典数据。</param>
        /// <returns>返回查询字符串。</returns>
        public static string GetQueryString(IDictionary<string, string> dictionary)
        {
            return string.Join("&", dictionary.Select(t => t.Key + "=" + Uri.EscapeUriString(t.Value)));
        }

        /// <summary>
        /// 获取表单字符串。
        /// </summary>
        /// <param name="dictionary">字典数据。</param>
        /// <returns>返回表单字符串。</returns>
        public static string GetFormString(IDictionary<string, string> dictionary)
        {
            return string.Join("&", dictionary.Select(t => t.Key + "=" + Uri.EscapeDataString(t.Value)));
        }
    }

    /// <summary>
    /// Http部分。
    /// </summary>
    public abstract class HttpPart
    {
        /// <summary>
        /// 获取新行字节数据。
        /// </summary>
        protected static readonly byte[] NewLine = Encoding.UTF8.GetBytes("\r\n");

        /// <summary>
        /// 实例化Http部分。
        /// </summary>
        /// <param name="name">表单名称。</param>
        public HttpPart(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 获取表单名称。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 将数据写入Http流。
        /// </summary>
        /// <param name="stream">Http流。</param>
        /// <returns></returns>
        public abstract Task WriteContent(Stream stream);
    }

    /// <summary>
    /// Http表单部分。
    /// </summary>
    public class HttpFormPart : HttpPart
    {
        /// <summary>
        /// 实例化Http表单部分。
        /// </summary>
        /// <param name="name">表单名称。</param>
        /// <param name="value">表单值。</param>
        public HttpFormPart(string name, string value)
            : base(name)
        {
            Value = value;
        }

        /// <summary>
        /// 获取表单值。
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// 将数据写入Http流。
        /// </summary>
        /// <param name="stream">Http流。</param>
        /// <returns></returns>
        public override async Task WriteContent(Stream stream)
        {
            var disposition = Encoding.UTF8.GetBytes("Content-Disposition: form-data; name=\"" + Name + "\"");
            await stream.WriteAsync(disposition, 0, disposition.Length);
            await stream.WriteAsync(NewLine, 0, NewLine.Length);
            var data = Encoding.UTF8.GetBytes(Value);
            await stream.WriteAsync(data, 0, data.Length);
        }
    }

    /// <summary>
    /// Http文件部分。
    /// </summary>
    public class HttpFilePart : HttpPart
    {
        /// <summary>
        /// 实例化Http文件部分。
        /// </summary>
        /// <param name="name">表单名称。</param>
        /// <param name="stream">文件流。</param>
        public HttpFilePart(string name, Stream stream)
            : this(name, stream, "UploadFile")
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">表单名称。</param>
        /// <param name="stream">文件流。</param>
        /// <param name="filename">文件名。</param>
        public HttpFilePart(string name, Stream stream, string filename)
            : this(name, stream, filename, "application/octet-stream")
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">表单名称。</param>
        /// <param name="stream">文件流。</param>
        /// <param name="filename">文件名。</param>
        /// <param name="mimetype">文件类型。</param>
        public HttpFilePart(string name, Stream stream, string filename, string mimetype)
            : base(name)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (filename == null)
                throw new ArgumentNullException("filename");
            if (mimetype == null)
                throw new ArgumentNullException("mimetype");
            Mimetype = mimetype;
        }

        /// <summary>
        /// 获取文件流。
        /// </summary>
        public Stream File { get; private set; }

        /// <summary>
        /// 获取文件名。
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// 获取文件类型。
        /// </summary>
        public string Mimetype { get; private set; }

        /// <summary>
        /// 将数据写入Http流。
        /// </summary>
        /// <param name="stream">Http流。</param>
        /// <returns></returns>
        public override async Task WriteContent(Stream stream)
        {
            var disposition = Encoding.UTF8.GetBytes("Content-Disposition: form-data; name=\"" + Name + "\"; filename=\"" + Filename + "\"");
            await stream.WriteAsync(disposition, 0, disposition.Length);
            await stream.WriteAsync(NewLine, 0, NewLine.Length);
            var type = Encoding.UTF8.GetBytes("Content-Type: " + Mimetype);
            await stream.WriteAsync(NewLine, 0, NewLine.Length);
            await File.CopyToAsync(stream);
        }
    }
}
