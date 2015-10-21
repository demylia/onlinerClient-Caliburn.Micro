using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OnlinerServices
{
    public class DataManager
    {

        private string adress = "http://tech.onliner.by/feed";

         // Get the list all tech articles from onliner.by to show them on the Page1
        public  IEnumerable<NewsItem> GetNews(string adress = "http://tech.onliner.by/feed")
        {


            var stream = GetStreamOnlinerRSS(adress);
            XElement xml = XElement.Load(stream);
            XNamespace media = "http://search.yahoo.com/mrss/";
            var news = xml.Element("channel").Elements("item").
                           Select(s => new NewsItem()
                           {
                               Title = s.Element("title").Value,
                               Link = s.Element("link").Value,
                               ImagePath = s.Element(media + "thumbnail").Attribute("url").Value
                           });

            return  news;
        }
        public IEnumerable<NewsItem> GetNews(int numberOfpage)
        {

            return GetNews(adress + "/page/" + numberOfpage.ToString());
        }

        private Stream GetStreamOnlinerRSS(string adress)
        {
            WebRequest request = WebRequest.Create(adress);
            WebResponse response = request.GetResponseAsync().Result;
            Stream stream = response.GetResponseStream();

            return stream;
        }

        //Old methods
        private IHtmlDocument GetHtmlDocument(string address)
        {

            WebRequest request = WebRequest.Create(address);
            WebResponse response = request.GetResponseAsync().Result;
            Stream stream = response.GetResponseStream();

            StreamReader sr = new StreamReader(stream);
            string s = sr.ReadToEnd();
            HtmlParser parser = new HtmlParser();

            return parser.Parse(s);
        }
        //Here I must parse the news page whatever, because XML-tree availeble only for list of tech news
        public string GetContentByLink(string link)
        {
            //TODO: Get one  article by link to show it on the Second Page
            WebRequest request = WebRequest.Create(link);
            WebResponse response = request.GetResponseAsync().Result;

            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string s = sr.ReadToEnd();

            HtmlParser parser = new HtmlParser();
            string selector = "div.b-posts-1-item__text";
            var article = parser.Parse(s).QuerySelector(selector).TextContent;
            article = Regex.Replace(article, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);


            return article;
        }
    }
}
