using System.Text;
using System.Text.Json;
using Invoices;
using Microsoft.Extensions.AI;
using OllamaSharp;
using UglyToad.PdfPig;

public interface IInvoiceAnalyzer
{
    Task<Invoice?> ExtractInvoiceAsync(byte[] content);
}

class InvoiceAnalyzer : IInvoiceAnalyzer
{
    public async Task<Invoice?> ExtractInvoiceAsync(byte[] content)
    {
        var pdfText = ExtractPdfText(content);
        var invoice = await ExtractInvoiceJson(pdfText);
        return invoice;
    }

    public static string ExtractPdfText(byte[] content)
    {
        var sb = new StringBuilder();

        using var pdf = PdfDocument.Open(content);

        foreach (var page in pdf.GetPages())
            sb.AppendLine(page.Text);

        return sb.ToString();
    }

    public async Task<Invoice?> ExtractInvoiceJson(string invoiceText)
    {
        IChatClient chatClient = new OllamaApiClient("http://localhost:11434", "SpeakLeash/bielik-11b-v3.0-instruct:Q4_K_M");

        var prompt = $@"
Oto treść faktury:

{invoiceText}

Wyodrębnij dane.

Zwróć wynik w formacie JSON:
{{
  ""invoiceNumber"": "",
  ""issueDate"": "",
  ""saleDate"": "",
  ""seller"":
  {{
    ""name"": "",
    ""address"": "",
    ""nip"": ""
  }},
  ""buyer"":
  {{
    ""name"": "",
    ""address"": "",
    ""nip"": ""
  }},
  ""items"": [
    {{
      ""name"": "",
      ""quantity"": 0,
      ""unit"": "",
      ""netPrice"": 0.0,
      ""netValue"": 0.0,
      ""vatRate"": "",
      ""vatValue"": 0.0,
      ""grossValue"": 0.0
    }}
  ],
  ""totals"":
  {{
    ""netTotal"": 0.0,
    ""vatTotal"": 0.0,
    ""grossTotal"": 0.0
  }}
}}

Jeśli nie znajdziesz jakiejś wartości, wpisz null.";

        var chatMessage = new ChatMessage(ChatRole.User, prompt);
        var chatResponse = await chatClient.GetResponseAsync(chatMessage);
        return JsonSerializer.Deserialize<Invoice>(chatResponse.Text);
    }
}