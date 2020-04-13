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
            
            var salaryValues = new List<double>();

            
                
            foreach (var salary in usdSalaries)
            {
                string salaryString = salary.InnerText;

                var clearedValue = ClearValue(salary.InnerText);

                if (salaryString.StartsWith("от")) //от 500 USD
                {

                    var loh = clearedValue + (clearedValue * 0.25);
                    salaryValues.Add(loh);
                }

                if (salaryString.StartsWith("до")) //до 1000 USD
                {
                    var loh = clearedValue - (clearedValue * 0.25);
                    salaryValues.Add(loh);
                }

                if (salaryString.Contains('-')) // 1000-2 000 USD 
                {
                    var loshok = (double)clearedValue;
                    salaryValues.Add(loshok);
                }
                else
                {
                    //TODO: process salaries with just digit value, like 1000USD
                }
            }

            var avgUsdSalary = salaryValues.Sum() / salaryValues.Count;
        }

        private int ClearValue(string value)
        {
            Regex betweenRegex = new Regex(@"(\d+)");

            var withoutSpaces = Regex.Replace(value, @"\s+", "");
            var digits = betweenRegex.Matches(withoutSpaces);

            if (digits.Count > 1)
            {
                var x = int.Parse(digits[0].Value);
                var y = int.Parse(digits[1].Value);

                var avg = ((x + y) / 2);//это не обязанность этого метода считать среднее

                return avg;
            }

            var intValue = int.Parse(digits[0].Value);

            return intValue;
        }
    }
}

