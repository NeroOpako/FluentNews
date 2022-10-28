using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using FluentNews.Core.Models;

namespace FluentNews.Core.Services
{
    public class NextcloudNewsInterface
    {
        private const string url_base_string = "index.php/apps/news/api/v1-2/";
        private string host;
        private string username;
        private string password;
        private NextcloudNewsFeeds feedCache = null;
        private NextcloudNewsItems itemCache = null;
        private static NextcloudNewsInterface instance = null;

        public NextcloudNewsInterface(string host, string username, string password)
        {
            if (!host.EndsWith("/"))
                host = host + "/";
            this.host = host;
            this.username = username;
            this.password = password;
        }

        public static NextcloudNewsInterface getInstance(string host = null, string username = null, string password = null)
        {
            if (instance == null)
            {
                if (host == null || username == null || password == null)
                {
                    throw new Exception("NextcloudNewsInterface parameters not valid.");
                }
                instance = new NextcloudNewsInterface(host, username, password);
            }
            return instance;
        }

        private static void setHeader(HttpWebRequest request, string header, string value)
        {
            //Stupid work around for user-agent not being exposed in the PCL version of HttpWebRequest
            //---STUPID!---
            var propertyInfo = request.GetType().GetRuntimeProperty(header.Replace("-", string.Empty));
            if (propertyInfo != null)
                propertyInfo.SetValue(request, value, null);
            else
                request.Headers[header] = value;
        }

        private string buildFullURL(string method)
        {
            return string.Format("{0}{1}{2}", host, url_base_string, method);
        }


        public async Task<NextcloudNewsItems> getItems(bool getRead = false, int batchSize = -1, int offset = 0, int id = 0)
        {
            if (itemCache != null)
                return itemCache;
            var getReadString = "false";
            if (getRead)
                getReadString = "true";
            var url = string.Format(buildFullURL("items") + "?type={4}&batchSize={0}&offset={1}&getRead={2}&id={3}", batchSize, offset, getReadString, id, id == 0 ? 3 : 0);
            var request = WebRequest.Create(url) as HttpWebRequest;
            var hash = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            request.Headers["Authorization"] = "Basic " + hash;
            using (var response = await request.GetResponseAsync() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception(string.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                var jsonSerializer = new DataContractJsonSerializer(typeof(NextcloudNewsItems));
                var jsonResponse = jsonSerializer.ReadObject(response.GetResponseStream()) as NextcloudNewsItems;
                itemCache = jsonResponse;
                return jsonResponse;
            }
            throw new Exception("getItems failed.");
        }

        public async Task<NextcloudNewsFeeds> getFeeds()
        {
            if (feedCache == null)
            {
                var request = WebRequest.Create(buildFullURL("feeds")) as HttpWebRequest;
                var hash = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                request.Headers["Authorization"] = "Basic " + hash;
                using (var response = await request.GetResponseAsync() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(string.Format(
                            "Server error (HTTP {0}: {1}).",
                            response.StatusCode,
                            response.StatusDescription));
                    var jsonSerializer = new DataContractJsonSerializer(typeof(NextcloudNewsFeeds));
                    var jsonResponse = jsonSerializer.ReadObject(response.GetResponseStream()) as NextcloudNewsFeeds;
                    feedCache = jsonResponse;
                    return feedCache;
                }
                throw new Exception("getFeeds failed.");
            }
            return feedCache;
        }

        public async void toggleItemRead(NextcloudNewsItem item)
        {
            var request = WebRequest.Create(buildFullURL(String.Format("items/{0}/multiple",item.unread ? "read" : "unread"))) as HttpWebRequest;
            request.Method = "PUT";
            request.ContentType = "application/json";
            var hash = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            request.Headers["Authorization"] = "Basic " + hash;
            using (var writer = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                var payload = string.Format("{{\"items\": [{0}]}}", item.id);
                writer.Write(payload);
            }
            using (var response = await request.GetResponseAsync() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception(string.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
            }
            item.unread = !item.unread;
        }

        public void invalidateCache()
        {
            itemCache = null;
        }
    }
}
