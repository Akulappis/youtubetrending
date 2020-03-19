using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YoutubeHelper
{
    class Program
    {

        /// <summary>
        /// Kommentoi alla oleva koodi
        /// </summary>  
        static void Main(string[] args)
        {
            var watch = Stopwatch.StartNew(); //Start stopwatch to see how long it takes for program to finish

            string trendingUrl = "https://www.youtube.com/feed/trending"; //URL for Youtube trending
            string trendingHtml = ParseHtml(trendingUrl); //Read HTML from trending page

            int listIndex = 0;
            List<string> urlList = new List<string>(); //List for storing URL's from trending page

            Regex linkParser = new Regex(@"(?:watch\?v=.*?)(?=" + "\"" + ")" + "(?!<)", RegexOptions.Compiled | RegexOptions.IgnoreCase); //Regex for looking watch?v= which is link for video
            foreach (Match m in linkParser.Matches(trendingHtml)) //Using regex to look for watch- links
            {
                if (urlList.Contains("http://www.youtube.com/" + m.ToString()) == false && m.ToString().Length == 19) //If list doesn't contain link and its 19 characters long it is link we are looking for
                {
                    urlList.Add("http://www.youtube.com/" + m.ToString()); //Add link to urlList
                }
            }
            foreach (var videoUrl in urlList) //Get title from every URL
            {
                ExtractTitle(ParseHtml(videoUrl), listIndex);
                listIndex++;
            }
            watch.Stop(); //Stop stopwatch
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Execution time: " + elapsedMs + "ms"); //Show how long execution time was

            Console.WriteLine("Minkä videon haluat katsoa");
            int num = Convert.ToInt32(Console.ReadLine()); //Get userinput

            if (num > urlList.Count)
            {
                Console.WriteLine("Liian suuri listaindexi\n");
                Console.WriteLine("Minkä videon haluat katsoa");
                num = Convert.ToInt32(Console.ReadLine());

            }
            else
            {
                Process.Start(urlList[num]); //Open selected url
            }
            Console.ReadLine();
        }


        public static string ParseHtml(string url) //For getting HTML from webpage
        {
            string html = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd(); //Read and save HTML as string
            }
            return html;
        }


        public static void ExtractTitle(string trendingHtml, int listIndex) //For getting title from websites
        {
            string regex = @"(?<=<title.*>)([\s\S]*)(?=</title>)";
            Regex videoTitle = new Regex(regex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Match k = videoTitle.Match(trendingHtml);
            Console.WriteLine(listIndex + ". " + k);
        }


    }
}
