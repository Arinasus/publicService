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
    col.Item().Text("КАРТ-ЧЕК").Bold().FontSize(22).AlignCenter();
    col.Item().Text($"Номер операции: {receipt.OperationId}");
    col.Item().Text($"Дата/Время: {receipt.Date}");
    col.Item().Text($"Метод оплаты: {receipt.Method}");
    col.Item().Text($"Плательщик: {receipt.Customer}");
    col.Item().Text($"Лицевой счёт: {receipt.ContractNumber}");
    col.Item().Text($"Получатель: {receipt.ProviderName}");
    col.Item().Text($"УНП: {receipt.ProviderUNP}");
    col.Item().Text($"IBAN: {receipt.ProviderIBAN}");
    col.Item().Text("");

    // таблица показаний
    col.Item().Table(table =>
    {
       table.ColumnsDefinition(columns =>
{
    columns.RelativeColumn(2); // услуга
    columns.RelativeColumn(1); // предыдущее
    columns.RelativeColumn(1); // текущее
    columns.RelativeColumn(1); // расход
    columns.RelativeColumn(1); // тариф
    columns.RelativeColumn(2); // сумма
});


        table.Header(header =>
        {
            header.Cell().Text("Услуга").Bold();
            header.Cell().Text("Предыдущее").Bold();
            header.Cell().Text("Текущее").Bold();
            header.Cell().Text("Расход").Bold();
            header.Cell().Text("Тариф").Bold();
            header.Cell().Text("Начислено").Bold();
        });

        foreach (var m in receipt.Meters)
        {
            var consumption = m.CurrentValue - m.PreviousValue;
            var amount = consumption * m.Price;

            table.Cell().Text(m.ServiceName);
            table.Cell().Text(m.PreviousValue.ToString("F2"));
            table.Cell().Text(m.CurrentValue.ToString("F2"));
            table.Cell().Text($"{consumption:F2} {m.Unit}");
            table.Cell().Text($"{m.Price:F2} BYN");
            table.Cell().Text($"{amount:F2} BYN");
        }
    });

    col.Item().Text($"Итого к оплате: {receipt.Amount:F2} BYN").Bold().FontSize(18).AlignRight();
});

                });
            });

            return document.GeneratePdf();
        }
    }
}
