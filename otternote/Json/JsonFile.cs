namespace otternote.Json;

public class JsonFile
{
    public Dictionary<string, string> Header {get; set;}
    public List<JsonEntry> Entries {get; set;}

    public JsonFile(Dictionary<string, string> header, List<JsonEntry>? entries = null)
    {
        this.Header = header;
        Entries = entries ?? new List<JsonEntry>();   
    }
}