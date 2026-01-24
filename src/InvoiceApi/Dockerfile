FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY InvoiceApi.sln ./
COPY ./InvoiceExtraction/InvoiceExtraction.csproj ./InvoiceExtraction/InvoiceExtraction.csproj
COPY ./InvoiceApi/InvoiceApi.csproj ./InvoiceApi/InvoiceApi.csproj
RUN dotnet restore

COPY . .

RUN dotnet publish ./InvoiceApi/InvoiceApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

RUN apt-get update
RUN apt-get install -y --no-install-recommends python3-pip tesseract-ocr tesseract-ocr-pol ghostscript poppler-utils

COPY --from=build /app/publish ./

RUN useradd -m appuser
RUN mkdir /app/tmp && chown appuser:appuser /app/tmp
ENV TMPDIR=/app/tmp
USER appuser
ENV PATH="/home/appuser/.local/bin:${PATH}"

RUN pip install --user --upgrade --break-system-packages ocrmypdf

ENTRYPOINT ["dotnet", "InvoiceApi.dll"]
