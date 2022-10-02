# CrawlingApp
Vaka uygulamasında kullanabileceğim iki yöntem vardı. Birincisi Web Crawler ikincisi ise Web Scraping . Uygulama için web scraper daha uygundu. 
Bunun için ScrapySharp eklentisini ekledim. 

ScrapingBrowser sınıfından örnek oluşturdum.

 ScrapingBrowser Browser = new ScrapingBrowser(); 
 
 Daha sonra WebPage sınıfından bir değişken oluşturup browser.navigatepage ile ilgili web sayfasının adresine ulaştım.
 WebPage PageResult = Browser.NavigateToPage(new Uri(WebUrl)); 
 
 Ardından HtmlDocumnet classı ile Page result'ı html sayfasını yükledim.
 
  var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(PageResult);
            
   Daha sonra html sayfasında istenilen dataya ulaşmak için HtmlAgilityPack paketi eklendi.
   HtmlNode classından bir oluşturduğum değişken ile istenilen kısmı ilgili css classı belirtilerek datayı çektim.
   
   list<HtmlNode> TitleNodes = PageResult.Html.CssSelect(".homepage .homepage-content li").ToList();
   
   titleNodes adlı listeyi foreach ile dönüp ilgili ilanların detay sayfası urllerini çekmiş oldum ve bir değişkene atadım. Daha sonra her bir urlyi detay sayfasında
   İlan ismine ve ücretlere ulaşmak için yeniden browser.navigatoPage yaptım.
   
     string detail = titleNode.Descendants("a").FirstOrDefault().ChildAttributes("href").FirstOrDefault().Value;

                WebPage detailPage = Browser.NavigateToPage(new Uri(WebUrl + detail));
                var htmlDocument1 = new HtmlDocument();
                htmlDocument1.LoadHtml(detailPage);

Aynı şekilde HtmlNode classından bir oluşturduğum değişken ile istenilen kısmı ilgili css classı belirtilerek datayı çektim.

 List<HtmlNode> detailNodes = detailPage.Html.CssSelect(".classifiedDetailTitle").ToList();
  List<HtmlNode> detailNodes1 = detailPage.Html.CssSelect(".classifiedDetailContent .classifiedInfo").ToList();
  
  
  Daha sonra detay sayfasından ilan isimlerinin consolda ve txt dosyasına yazdım.
  
  
   foreach (var deHtmlNode in detailNodes)
                {
                    string result = deHtmlNode.Descendants("h1").FirstOrDefault().FirstChild.InnerText;

                    //ilan başlıkları yazdırılıyor.
                    Console.WriteLine(result);

                    //.txt dosyasına yazılıyor.
                    sw.WriteLine(result);
                }
  
  
  Ardından ilan fiyatlarını consol ve txt dosyasına yazdım.
  
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
  
  Son olarak fiyat ortalamasını consolda yazdırdım.
  
