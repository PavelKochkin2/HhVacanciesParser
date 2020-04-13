using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HhVacanciesParser
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            services.AddSingleton<VacanciesParser>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env, VacanciesParser vacanciesParser)
        {
            var salaryNodes = await vacanciesParser.GetSalaryNodes(AreasWithCodes.Minsk, "designer");

            var usdSalaries = salaryNodes.Where(x => x.InnerText.Contains("USD"));
            
            var salaryValues = new List<int>();

            foreach (var salary in usdSalaries)
            {
                string salaryString = salary.InnerText;

                if (salaryString.StartsWith("от")) //от 500 USD
                {
                    //очистить стрингу оставив только цифры
                    //спарсить ее в инт
                    //добавить к этому значению 25% (т.к. зп от, значит минимальная, но тут хз)
                    //добавить в salaryValues
                }

                if (salaryString.StartsWith("до")) //до 1000 USD
                {
                    //очистить стрингу оставив только цифры
                    //спарсить ее в инт
                    //отнять 25% (логика та же что и выше)
                    //добавить в salaryValues
                }

                if (salaryString.Contains('-')) // 1000-2 000 USD 
                {
                    //переписать говнокод внизу
                    //он берет два значения, парсит их и берет среднее
                    var withoutUsd = salaryString.Remove(salary.InnerText.Length - 3, 3);
                    var withoutSpaces = Regex.Replace(withoutUsd, @"\s+", "");
                    string[] subs = withoutSpaces.Split('-');

                    var x = int.Parse(subs[0]);
                    var y = int.Parse(subs[1]);

                    var result = ((y + x) / 2);
                }
                else
                {
                    //TODO: process salaries with just digit value, like 1000USD
                }
            }
        }
    }
}
