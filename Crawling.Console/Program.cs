using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;

namespace Crawling
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Web adresi
            string WebUrl = "https://www.sahibinden.com/";
            //text dosyasının yer aldığı klasör ismi
            string folderName = @"C:\Crawler\";
            //text dosyasının ismi
            string fileName = "Sahibinden.txt";

            string fullPath = folderName + fileName;

            ScrapingBrowser Browser = new ScrapingBrowser();
            Browser.AllowMetaRedirect = true;

            //İlgili web sitesine gidiliyor.
            WebPage PageResult = Browser.NavigateToPage(new Uri(WebUrl));
            //Bozuk olan Türkçe karakter sebebiyle encoding işlemi yapıldı.
            PageResult.Browser.Encoding = Encoding.GetEncoding("UTF-8");
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(PageResult);

            //fiyat listesi tutacağımız liste
            List<decimal> priceList = new List<decimal>();

            //text işlemlerini yapacağımız classı ekliyoruz.
            FileStream fs = new FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.Write);

            //StreamWriter sınıfı ile .txt dosya yazma işlemini başlatıyoruz.
            StreamWriter sw = new StreamWriter(fs);

            //Vitrin ilanlarında dönülüyor.
            List<HtmlNode> TitleNodes = PageResult.Html.CssSelect(".homepage .homepage-content li").ToList();
            foreach (var titleNode in TitleNodes)
            {
                // her detay linkine gidildildiğinde, IP kısıtlamasını önlemek için her request öncesi 3 saniye bekletiyoruz.
                Thread.Sleep(3000);

                //bu satırda html sayfasındaki ilanların detay sayfası urline ulaşıyoruz
                string detail = titleNode.Descendants("a").FirstOrDefault().ChildAttributes("href").FirstOrDefault().Value;

                WebPage detailPage = Browser.NavigateToPage(new Uri(WebUrl + detail));
                var htmlDocument1 = new HtmlDocument();
                htmlDocument1.LoadHtml(detailPage);

                //detailNodes değişkeni html sayfasındaki ilgili css classındaki datayı çekiyor.
                List<HtmlNode> detailNodes = detailPage.Html.CssSelect(".classifiedDetailTitle").ToList();

                //detailNodes1 değişkeni html sayfasındaki ilgili css classındaki datayı çekiyor.
                List<HtmlNode> detailNodes1 = detailPage.Html.CssSelect(".classifiedDetailContent .classifiedInfo").ToList();

                //bu döngü detay sayfasındaki ilan isimleri döndürüyor
                foreach (var deHtmlNode in detailNodes)
                {
                    string result = deHtmlNode.Descendants("h1").FirstOrDefault().FirstChild.InnerText;

                    //ilan başlıkları yazdırılıyor.
                    Console.WriteLine(result);

                    //.txt dosyasına yazılıyor.
                    sw.WriteLine(result);
                }

                //bu döngü detay sayfasında fiyatları döndürüyor
                foreach (var detailNode in detailNodes1)
                {
                    string result = detailNode.Descendants("h3").FirstOrDefault().FirstChild.InnerText.Replace("/n", "").Trim();
                    //fiyat ile TL yazısını ayırmak için boşluk ile ayırma işlemi yaptım ve sadece fiyatları elde ettim.
                    var priceArray = result.Split(' ');

                    //fiyatlar yazdırılıyor.
                    Console.WriteLine(priceArray[0]);

                    //fiyat listesine fiyatlar ekleniyor.
                    priceList.Add(Convert.ToDecimal(priceArray[0]));

                    //.txt dosyasına yazılıyor.
                    sw.WriteLine(priceArray[0]);
                }
            }

            //.txt dosyası kapatılıyor.
            sw.Close();

            //fiyatların ortalaması hesaplanıyor ve para formatına çevriliyor.
            string avgPrice = priceList.Average(p => p).ToString("N2");
            Console.WriteLine("Ortalama fiyatı:" + avgPrice);

            Console.ReadLine();
        }
    }
}