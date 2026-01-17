using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("invoices")]
public class InvoiceController : ControllerBase
{
    private static List<Invoice> _invoices = new();

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadInvoice(IFormFile file)
    {
        if (!Request.HasFormContentType)
        {
            return BadRequest("The request does not contain a valid form.");
        }

        var cancellationToken = HttpContext.RequestAborted;
        var formFeature = Request.HttpContext.Features.GetRequiredFeature<IFormFeature>();
        await formFeature.ReadFormAsync(cancellationToken);

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        _invoices.Add(new Invoice
        {
            FileName = file.FileName,
            Content = memoryStream.ToArray()
        });
        return Ok();
    }

    [HttpGet]
    public OkObjectResult GetInvoices()
    {
        return Ok(_invoices.ToArray());
    }
}

class Invoice
{
    public required string FileName { get; set; }
    public required byte[] Content { get; set; }
}