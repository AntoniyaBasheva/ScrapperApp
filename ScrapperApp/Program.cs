namespace ScrapperApp
{
    using CsvHelper;
    using HtmlAgilityPack;
    using Newtonsoft.Json;
    using ScrapySharp.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;

    public class Program
    {
        public static async Task<HtmlDocument> DownloadAsync()
        {
            var web = new HtmlWeb();

            return await web.LoadFromWebAsync("https://emag.bg");
        }

        public static async Task<IEnumerable<EmagModel>> ParseAsync(HtmlDocument document)
        {
            var titles = document.DocumentNode.CssSelect("div.card > div > a");

            var products = new List<EmagModel>();

            foreach (var title in titles)
            {
                products.Add(new EmagModel { Title = title.InnerText });
            }

            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

            using var streamWriter = new StreamWriter(Path.Combine(projectDirectory, "products.csv"));
            using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

            await csvWriter.WriteRecordsAsync(products);

            return products;
        }

        static async Task Main()
        {
            var document = await DownloadAsync();
            var products = await ParseAsync(document);

            foreach (var product in products)
            {
                Console.WriteLine(JsonConvert.SerializeObject(product));
            }

            Console.ReadKey();
        }
    }
}
