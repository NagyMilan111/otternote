namespace otternote;


class Program
{
    static void Main(string[] args)
    {
        JsonHandler handler = new JsonHandler();
        JsonFile file = handler.Load("jsonexample.json");

        foreach (KeyValuePair<string, string> x in file.Header)
        {
            Console.WriteLine(x.Key + " : " + x.Value);
        }
        
        handler.Save(file, "jsonexample.json");
    }
    
}