var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddControllers();
services.AddSingleton<IInvoiceAnalyzer, InvoiceAnalyzer>();

var app = builder.Build();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
