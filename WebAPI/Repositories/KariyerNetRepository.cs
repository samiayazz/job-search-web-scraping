using HtmlAgilityPack;
using WebAPI.Domain.Entities;
using WebAPI.Domain.Enums;

namespace WebAPI.Repositories
{
    public sealed class KariyerNetRepository
    {
        public List<Job> GetJobs(string keyword, string location, int limit = 100)
        {
            var returnedJobList = new List<Job>();

            var jobListHtmlWeb = new HtmlWeb();
            //returns 50 data starting from start parameter
            //note: start parameter should increase by 1
            int start = 1, jobCountPerPage = 50, lastJobCount;
            do
            {
                var jobListHtmlDoc = jobListHtmlWeb
                .Load($@"https://www.kariyer.net/is-ilanlari/{location}?kw={keyword}&cp={start}")
                .DocumentNode;

                var jobDetailHtmlWeb = new HtmlWeb();
                var jobList = jobListHtmlDoc.SelectNodes("//div[contains(@class, 'list-items')]"); //a[contains(@class, 'k-ad-card')]
                lastJobCount = jobList.Count;
                foreach (var job in jobList)
                {
                    //Thread.Sleep(50);
                    int i = jobList.IndexOf(job) + 1;

                    if (i > 50)
                        continue;

                    string jobDetailUri = @"https://www.kariyer.net" + jobListHtmlDoc.SelectSingleNode($"//div[contains(@class, 'list-items')][{i}]/a").GetAttributeValue("href", String.Empty);
                    var jobDetailHtmlDoc = jobDetailHtmlWeb.Load(jobDetailUri).DocumentNode;

                    if (IsLoginPageLoaded(jobDetailHtmlDoc))
                    {
                        //returnedJobList.Add(new() { Title = "CrashedByLoginPageIsLoaded--" + jobDetailUri }); <= logging
                        continue;
                    }

                    returnedJobList.Add(new(
                        GetValueFromNode(jobDetailHtmlDoc, "//h1[contains(@class, 'title')]/p/span/span"),
                        GetValueFromNode(jobDetailHtmlDoc, "//a[contains(@class, 'company-name')]"),
                        GetValueFromNode(jobDetailHtmlDoc, "//div[contains(@class, 'company-location')]/span"),
                        GetValueFromNode(jobDetailHtmlDoc, "//div[contains(@class, 'job-sub-detail')]"),
                        "KariyerNet"
                        ));
                }

                start += 1;
            } while (lastJobCount > 0 && start * jobCountPerPage <= limit);

            return returnedJobList;
        }

        bool IsLoginPageLoaded(HtmlNode doc)
        {
            var node = doc.SelectSingleNode("//h1[contains(@class, 'title')]/p/span/span");
            return node == null || node.InnerHtml == String.Empty;
        }

        string GetValueFromNode(HtmlNode doc, string nodeXPath)
            => doc.SelectSingleNode(nodeXPath).InnerText.Replace("\n", "").Trim();
    }
}
