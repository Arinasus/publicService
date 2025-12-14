using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Backend.DTOs;

namespace Backend.Services
{
    public static class ReceiptPdfGenerator
    {
        public static byte[] Generate(ReceiptDto receipt)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(14));

                    page.Content().Column(col =>
                    {
                        col.Item().Text(receipt.StoreName).Bold().FontSize(20);
                        col.Item().Text($"Дата: {receipt.Date}");
                        col.Item().Text($"Клиент: {receipt.Customer}");
                        col.Item().Text($"Счёт №: {receipt.InvoiceNumber}");
                        col.Item().Text($"Сумма: {receipt.Amount} BYN");
                        col.Item().Text($"Метод оплаты: {receipt.Method}");
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
