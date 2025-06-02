using System.Text.Json;

namespace otternote.Json;

public class JsonHandler
{
    public JsonFile Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", filePath);
        }

        try
        {
            using FileStream fs = File.OpenRead(filePath);
            using JsonDocument document = JsonDocument.Parse(fs);
            var root = document.RootElement;

            var header = new Dictionary<string, string>
            {
                { "version", root.GetProperty("version").ToString() },
                { "algorithm", root.GetProperty("algorithm").ToString() },
                { "salt", root.GetProperty("salt").ToString() },
                { "vault_check", root.GetProperty("vault_check").ToString() },
                { "vault_check_iv", root.GetProperty("vault_check_iv").ToString() },
            };

            List<JsonEntry> entries = new List<JsonEntry>();
            foreach (var entryElement in root.GetProperty("entries").EnumerateArray())
            {
                var entry = new JsonEntry(
                    site: entryElement.GetProperty("site").GetString(),
                    password: ParseCipherEntry(entryElement.GetProperty("password")),
                    username: ParseCipherEntry(entryElement.GetProperty("username"))
                );
                entries.Add(entry);
            }

            return new JsonFile(header, entries);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                $"Invalid JSON format in file {filePath}. Error: {ex.Message}", ex);
        }
    }

   

    private CipherEntry ParseCipherEntry(JsonElement element)
    {
        return new CipherEntry(
            iv: element.GetProperty("iv").GetString(),
            cipherText: element.GetProperty("ciphertext").GetString()
        );
    }
    

    public void Save(JsonFile jsonFile, string filePath)
    {
        using (var fs = File.Create(filePath))
        {
            using (var writer = new Utf8JsonWriter(fs, new JsonWriterOptions { Indented = true }))
            {
                writer.WriteStartObject();

                // Write header
                foreach (var kvp in jsonFile.Header)
                {
                    writer.WriteString(kvp.Key, kvp.Value);
                }

                // Always write "entries" array, even if empty or null
                writer.WriteStartArray("entries");

                if (jsonFile.Entries != null)
                {
                    foreach (var entry in jsonFile.Entries)
                    {
                        writer.WriteStartObject();
                        writer.WriteString("site", entry.Site);
                        WriteCipherEntry(writer, "password", entry.Password);
                        WriteCipherEntry(writer, "username", entry.Username);
                        writer.WriteEndObject();
                    }
                }

                writer.WriteEndArray();
                writer.WriteEndObject();
            }
        }
    }


    private void WriteCipherEntry(Utf8JsonWriter writer, string propertyName, CipherEntry entry)
    {
        writer.WriteStartObject(propertyName);
        writer.WriteString("iv", entry.Iv);
        writer.WriteString("ciphertext", entry.CipherText);
        writer.WriteEndObject();
    }
    

    public JsonFile Delete(string siteName, JsonFile jsonFile)
    {
        if (siteName == null || jsonFile == null)
        {
            throw new ArgumentNullException(nameof(siteName));
        }

        foreach (JsonEntry fileEntry in jsonFile.Entries)
        {
            if (fileEntry.Site == siteName)
            {
                jsonFile.Entries.Remove(fileEntry);
                return jsonFile;
            }
        }
        
        return jsonFile;
    }

    public JsonFile AddOrUpdate(JsonEntry newEntry, JsonFile jsonFile)
    {
        if (newEntry == null || jsonFile == null)
        {
            throw new ArgumentNullException(nameof(newEntry));
        }    
        
        for (int i = 0; i < jsonFile.Entries.Count; i++)
        {
            if (jsonFile.Entries[i].Site == newEntry.Site)
            {
                jsonFile.Entries[i] = newEntry;
                return jsonFile;
            }
        }
        
        jsonFile.Entries.Add(newEntry);
        
        return jsonFile;
    }
}