using HtmlAgilityPack;
using WebAPI.Domain.Entities;
using WebAPI.Domain.Enums;

namespace WebAPI.Repositories
{
    public sealed class LinkedInRepository
    {
        public List<Job> GetJobs(string keyword, string location, int limit = 100)
        {
            var returnedJobList = new List<Job>();

            var jobListHtmlWeb = new HtmlWeb();
            //returns 100 data starting from start parameter
            //note: start parameter should increase by 100
            int start = 0, jobCountPerPage = 100, lastJobCount;
            do
            {
                var jobListHtmlDoc = jobListHtmlWeb
                .Load($@"https://www.linkedin.com/jobs-guest/jobs/api/seeMoreJobPostings/search?keywords={keyword}&location={location}&start={start}")
                .DocumentNode;

                var jobDetailHtmlWeb = new HtmlWeb();
                var jobList = jobListHtmlDoc.SelectNodes("/li");
                if (jobList is null)
                {
                    lastJobCount = 0;
                    continue;
                }

                lastJobCount = jobList.Count;
                foreach (var job in jobList)
                {
                    Thread.Sleep(100);
                    int i = jobList.IndexOf(job) + 1;

                    if (jobListHtmlDoc.SelectSingleNode($"/li[{i}]/div/a") is null)
                        continue;

                    string jobDetailUri = jobListHtmlDoc.SelectSingleNode($"/li[{i}]/div/a").GetAttributeValue("href", String.Empty);
                    var jobDetailHtmlDoc = jobDetailHtmlWeb.Load(jobDetailUri).DocumentNode;

                    if (IsLoginPageLoaded(jobDetailHtmlDoc))
                    {
                        //returnedJobList.Add(new() { Title = "CrashedByLoginPageIsLoaded--" + jobDetailUri }); <= logging
                        continue;
                    }

                    returnedJobList.Add(new(
                        GetValueFromNode(jobDetailHtmlDoc, "//h1[contains(@class, 'top-card-layout__title')]"),
                        GetValueFromNode(jobDetailHtmlDoc, "//a[contains(@class, 'topcard__org-name-link')]"),
                        GetValueFromNode(jobDetailHtmlDoc, "//h4[contains(@class, 'top-card-layout__second-subline')]//div[contains(@class, 'topcard__flavor-row')][1]//span[contains(@class, 'topcard__flavor')][2]"),
                        GetValueFromNode(jobDetailHtmlDoc, "//div[contains(@class, 'description__text')]//div[contains(@class, 'show-more-less-html__markup')]"),
                        "LinkedIn"
                        ));
                }

                start += 100;
            } while (lastJobCount == 100 && (start / 100) * jobCountPerPage < limit);

            return returnedJobList;
        }

        bool IsLoginPageLoaded(HtmlNode doc)
        {
            var node = doc.SelectSingleNode("//a[contains(@class, 'topcard__org-name-link')]");
            return node == null || node.InnerHtml == String.Empty;
        }

        string GetValueFromNode(HtmlNode doc, string nodeXPath)
            => doc.SelectSingleNode(nodeXPath).InnerText.Replace("\n", "").Trim();
    }
}
