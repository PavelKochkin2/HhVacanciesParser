using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;

namespace HhVacanciesParser
{
    public class VacanciesParser
    {
        private readonly HttpClient _httpClient;
        private const string _hhSalaryNodeXpath = "//span[contains(@data-qa, 'vacancy-compensation')]";

        readonly List<HtmlNode> salaryNodes = new List<HtmlNode>();
        List<HtmlNode> returnedNodes = new List<HtmlNode>();


        public VacanciesParser()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<HtmlNode>> GetSalaryNodes(int areaCode, string vacancyName)
        {
            int pageCount = 0;


            while (returnedNodes != null)
            {
                var url = CombineUrl(areaCode, vacancyName, pageCount);
                var response = await _httpClient.GetAsync(url);
                var responseContentString = await response.Content.ReadAsStringAsync();

                var htmlDoc = new HtmlDocument();

                htmlDoc.LoadHtml(responseContentString);

                returnedNodes = htmlDoc.DocumentNode.SelectNodes(_hhSalaryNodeXpath)?.ToList();

                if (returnedNodes != null)
                    salaryNodes.AddRange(returnedNodes);

                pageCount++;
            }

            return salaryNodes;
        }

        private string CombineUrl(int areaCode, string vacancyName, int page = 0)
        {
            return $"https://hh.ru/search/vacancy?area={areaCode}&st=searchVacancy&text={vacancyName}&only_with_salary=true&page={page}";
        }
    }

    public class AreasWithCodes
    {
        public static int Minsk = 1002;
        public static int Belarus = 16;
        public static int Moscow = 1;
    }
}