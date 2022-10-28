using System;
using System.Linq;
using System.Runtime.Serialization;

namespace FluentNews.Core.Models
{
    [DataContract(Name = "folders")]
    public class NextcloudNewsFolder
    {
        [DataMember(Name = "id")]
        public int id;
        [DataMember(Name = "name")]
        public string name;
    }

    [DataContract]
    public class NextcloudNewsFeed
    {
        [DataMember(Name = "id")]
        public int id;
        [DataMember(Name = "url")]
        public string url;
        [DataMember(Name = "title")]
        public string title;
        [DataMember(Name = "faviconLink")]
        public string faviconLink;
        [DataMember(Name = "added")]
        public long added;
        [DataMember(Name = "folderId")]
        public int? folderId;
        [DataMember(Name = "unreadCount")]
        public int unreadCount;
        [DataMember(Name = "ordering")]
        public int ordering;
        [DataMember(Name = "link")]
        public string link;
        [DataMember(Name = "pinned")]
        public bool pinned;
        [DataMember(Name = "updateErrorCount")]
        public int updateErrorCount;
        [DataMember(Name = "lastUpdateError")]
        public string lastUpdateError;
    }

    [DataContract]
    public class NextcloudNewsFeeds
    {
        [DataMember(Name = "feeds")]
        public NextcloudNewsFeed[] feeds;
        [DataMember(Name = "starredCount")]
        public int starredCount;

        public NextcloudNewsFeed getFeedForId(int id)
        {
            var result = from NextcloudNewsFeed feed in feeds
                         where feed.id == id
                         select feed;
            return result.First();
        }
    }

    public class NextcloudNewsItem
    {
        //NOTE: title, author, url, enclosureMime, and enclosureLink are not sanitized
        [DataMember(Name = "id")]
        public int id;
        [DataMember(Name = "guid")]
        public string guid;
        [DataMember(Name = "guidHash")]
        public string guidHash;
        [DataMember(Name = "url")]
        public string url;
        [DataMember(Name = "title")]
        public string title;
        [DataMember(Name = "author")]
        public string author;
        [DataMember(Name = "pubDate")]
        public long pubDate;
        [DataMember(Name = "body")]
        public string body;
        [DataMember(Name = "enclosureMime")]
        public string enclosureMime;
        [DataMember(Name = "enclosureLink")]
        public string enclosureLink;
        [DataMember(Name = "feedId")]
        public int feedId;
        [DataMember(Name = "unread")]
        public bool unread;
        [DataMember(Name = "starred")]
        public bool starred;
        [DataMember(Name = "lastModified")]
        public long lastModified;
        [DataMember(Name = "fingerprint")]
        public string fingerprint;

        //public async void markRead(NextcloudNewsInterface interface)
        //{
        //}
    }

    public class NextcloudNewsItems
    {
        [DataMember(Name = "items")]
        public NextcloudNewsItem[] items;

        public NextcloudNewsItem getForId(int id)
        {
            var result = from NextcloudNewsItem item in items
                         where item.id == id
                         select item;
            return result.First();
        }
    }

    public class NextcloudNewsUser
    {
    }
}
