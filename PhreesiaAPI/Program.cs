using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonDiffer;


namespace PhreesiaAPI 
{
    class Program
    {
        static void Main()
        {
            // Tests 3.1
            string LookingForBook = "Goodnight Moon";
            var booksSearchedJson = GetJsonFromTheSearch(LookingForBook);
            booksSearchedJson.Wait();
            
            var booksFound = GetTotalBooks(booksSearchedJson, LookingForBook, 2000);
            foreach (KeyValuePair<int, List<string>> kvp in booksFound)
            {
                Console.WriteLine("----------Results for test 3.1.1 and 3.1.2-----------");
                Console.WriteLine("Total books found: " + kvp.Key.ToString());
                Console.WriteLine("Keys' list: " + String.Join(", ", kvp.Value));
            }

            // Tests 3.2
            var filePath = AppDomain.CurrentDomain.BaseDirectory+ "SampleJson.json";
            string LookingForSpecificBook = "Goodnight Moon 123 Lap Edition";
            var newBooksSearchedJson = GetJsonFromTheSearch(LookingForSpecificBook);
            newBooksSearchedJson.Wait();
            var jsonTarget = ReadJson(filePath);
            var differenceInTheResult = CompareJson(jsonTarget, newBooksSearchedJson.Result);

            Console.WriteLine("\n --------------Results for test 3.2--------------");
            Console.WriteLine("Results doesn't match.");
            Console.WriteLine("Showing the differences with \"*\" for changed properties \"-\" and \"+\" for removed and added ones respectively");
            Console.WriteLine(differenceInTheResult.ToString());

            Console.ReadLine();
        }

        private static async Task<string> GetJsonFromTheSearch(string bookName)
        {
            using HttpClient client = new();
            string url = "http://openlibrary.org/search.json?q=" + bookName;
            var httpResponse = "";
            try
            {
                httpResponse = await client.GetStringAsync(url);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return httpResponse;
        } 

        private static Dictionary<int,List<string>> GetTotalBooks(Task<string> json, string bookName, int year)
        {
            List<string> keys = new();
            int numberOfBooks = 0;
            Dictionary<int, List<string>> result = new();
            var books = JsonConvert.DeserializeObject<Results>(json.Result);

            foreach (var book in books!.Books!)
            {
                if (book.Title!.Contains(bookName))
                {
                    numberOfBooks++;
                    if(PublishedAfterYear(year, book.PublishYear!))
                        keys.Add(book.BookKey!);
                }   
            }
            result.Add(numberOfBooks,keys);
  
            return result;
        }

        private static bool PublishedAfterYear(int year, List<int> publish)
        {
            bool published = false;
            if (publish == null)
                return published;

            foreach (var yearPublished in publish)
            {
                if(yearPublished >= year)
                {
                    published = true;
                    break;
                }
            }
            return published;
        }

        private static string ReadJson(string filePath)
        {
            dynamic jsonFile = JsonConvert.DeserializeObject<object>(File.ReadAllText(filePath))!;
            return jsonFile!.ToString() ;
        }

        private static JToken CompareJson(string expectedJSON, string actualJSON)
        {
            var j1 = JToken.Parse(expectedJSON);
            var j2 = JToken.Parse(actualJSON);
            var diff = JsonDifferentiator.Differentiate(j1, j2, showOriginalValues: true);

            return diff;
        }
    }
}
