using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using ModelPortable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Windows.Web.Http;

namespace OnlinerServices
{
    public class DataManager: IDataManager
    {

        private string adress = "http://tech.onliner.by/feed";
       

        public async Task<IEnumerable<NewsItem>> GetNewsAsync(string adress = "http://tech.onliner.by/feed")
        {

            var rss = await GetOnlinerRSSAsync(adress);
            
            XElement xml = XElement.Parse(rss);
            XNamespace media = "http://search.yahoo.com/mrss/";
            var news = xml.Element("channel").Elements("item").
                                                         Select(s => new NewsItem()
                                                         {
                                                             Title = s.Element("title").Value,
                                                             Link = s.Element("link").Value,
                                                             ImagePath = s.Element(media + "thumbnail").Attribute("url").Value
                                                         });
         
            return news;
        }
        public async Task<string> GetContentByLinkAsync(string link)
        {
            var rss = await GetOnlinerRSSAsync(link);

            HtmlParser parser = new HtmlParser();
            string selector = "div.b-posts-1-item__text";
            var article = await Task.Run(() => parser.Parse(rss).QuerySelector(selector).TextContent);
            article = Regex.Replace(article, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);

            return article;
        }
       
        private async Task<string> GetOnlinerRSSAsync(string adress)
        {
            HttpClient client = new HttpClient();
          
            var rss =  await client.GetStringAsync(new Uri(adress));

            // await Task.Delay(4000);// just for test
            return  rss;
        }

        
        //public IEnumerable<NewsItem> GetNews(int numberOfpage)
        //{

        //    return GetNews(adress + "/page/" + numberOfpage.ToString());
        //}

       
    }
}
