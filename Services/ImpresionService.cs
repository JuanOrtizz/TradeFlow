using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TradeFlow.Models;

namespace TradeFlow.Services
{
    public class ImpresionService : IImpresionService
    {
        public string GenerarHtmlFactura(FacturaModel factura, IReadOnlyList<DetalleFacturaModel> items)
        {
            var html = new StringBuilder();

            html.Append("""
                <!DOCTYPE html>
                <html lang="es">
                <head>
                <meta charset="utf-8">
                <style>
                    * { box-sizing: border-box; }
                    body { font-family: 'Segoe UI', Arial, sans-serif; color: #000000; margin: 0; padding: 32px; background: #ffffff; }
                    .doc { max-width: 720px; margin: 0 auto; page-break-inside: avoid; break-inside: avoid; }
                    .encabezado { display: flex; justify-content: space-between; align-items: baseline; border-bottom: 2px solid #000000; padding-bottom: 6px; margin-bottom: 12px; font-size: 11px; line-height: 1.4; }
                    .marca { font-size: 13px; font-weight: 700; letter-spacing: .5px; }
                    .marca-sub { font-size: 10px; }
                    .remito { font-weight: 700; }
                    .datos-cliente { display: flex; justify-content: space-between; align-items: baseline; font-size: 11px; line-height: 1.4; margin-bottom: 12px; }
                    h2 { font-size: 12px; text-transform: uppercase; letter-spacing: .5px; margin: 0 0 6px; font-weight: bold; }
                    table { width: 100%; border-collapse: collapse; font-size: 12px; }
                    th { background: transparent; color: #000000; text-align: left; padding: 2px 6px; font-size: 11px; text-transform: uppercase; letter-spacing: .5px; border-bottom: 1px solid #000000; }
                    td { padding: 2px 6px; vertical-align: top; }
                    td.der, th.der { text-align: right; }
                    .codigo { color: #555555; font-size: 11px; }
                    .totales { margin-top: 16px; display: flex; justify-content: flex-end; }
                    .totales table { width: 280px; }
                    .totales td { border-bottom: none; }
                    .fila-total td { font-weight: 700; font-size: 16px; border-top: 2px solid #000000; color: #000000; padding-top: 8px; }
                    .pie { margin-top: 40px; border-top: 1px solid #000000; padding-top: 12px; font-size: 11px; color: #000000; text-align: center; }
                    @media print { 
                        @page { size: A4; margin: 15mm; }
                        body { padding: 0; } 
                    }
                </style>
                </head>
                <body>
                """);

            html.Append("<div class=\"doc\">");

            html.Append("<div class=\"encabezado\">");
            html.Append("<div><span class=\"marca\">Distribuidora Yrigoyen</span> <span class=\"marca-sub\">José y Martín</span></div>");
            html.Append($"<div><span class=\"remito\">Remito #{factura.Id}</span> &middot; {factura.Fecha:dd/MM/yyyy HH:mm}</div>");
            html.Append("</div>");

            var partesCliente = new List<string>();
            if (!string.IsNullOrWhiteSpace(factura.Cliente?.Nombre))
                partesCliente.Add(Escapar(factura.Cliente.Nombre));
            if (!string.IsNullOrWhiteSpace(factura.Cliente?.Localidad?.Nombre))
                partesCliente.Add($"Localidad: {Escapar(factura.Cliente.Localidad.Nombre)}");
            if (!string.IsNullOrWhiteSpace(factura.Cliente?.Direccion))
                partesCliente.Add($"Direccion: {Escapar(factura.Cliente.Direccion)}");
            if (!string.IsNullOrWhiteSpace(factura.Cliente?.Telefono))
                partesCliente.Add($"Tel: {Escapar(factura.Cliente.Telefono)}");

            html.Append("<div class=\"datos-cliente\">");
            html.Append($"<div><b>Cliente:</b> {string.Join(" | ", partesCliente)}</div>");
            html.Append("</div>");

            html.Append("""
                <table>
                <thead><tr><th>Cant.</th><th>Producto</th><th class="der">Precio unit.</th><th class="der">Descuento</th><th class="der">Importe</th></tr></thead>
                <tbody>
                """);

            foreach (var item in items)
            {
                var descuento = item.DescuentoPorcentaje > 0 ? $"{item.DescuentoPorcentaje}%" : "—";
                html.Append("<tr>");
                html.Append($"<td>{item.Cantidad}</td>");
                html.Append("</td>");
                html.Append($"<td>{Escapar(item.ProductoNombre)}");
                html.Append($"<td class=\"der\">${item.PrecioUnitario:N2}</td>");
                html.Append($"<td class=\"der\">{descuento}</td>");
                html.Append($"<td class=\"der\">${item.Subtotal:N2}</td>");
                html.Append("</tr>");
            }

            html.Append("</tbody></table>");

            html.Append("<div class=\"totales\"><table><tr class=\"fila-total\">");
            html.Append($"<td>Total</td><td class=\"der\">${factura.Total:N2}</td>");
            html.Append("</tr></table></div>");

            html.Append($"<div class=\"pie\">Documento generado por TradeFlow &middot; {DateTime.Now:dd/MM/yyyy HH:mm}</div>");
            html.Append("</div>");

            html.Append("</body></html>");

            return html.ToString();
        }

        public async Task ImprimirAsync(object controlWebView)
        {
            if (controlWebView is not Microsoft.Maui.Controls.WebView webView)
            {
                throw new ArgumentException("El control de vista previa no es valido.", nameof(controlWebView));
            }

#if WINDOWS
            if (webView.Handler?.PlatformView is not Microsoft.UI.Xaml.Controls.WebView2 webView2)
            {
                throw new PlatformNotSupportedException("La impresion solo esta disponible en Windows.");
            }

            var core = webView2.CoreWebView2;
            if (core is null)
            {
                var inicializado = new TaskCompletionSource();
                webView2.CoreWebView2Initialized += (_, _) => inicializado.TrySetResult();
                await inicializado.Task.WaitAsync(TimeSpan.FromSeconds(5));
                core = webView2.CoreWebView2;
            }

            core?.ShowPrintUI(Microsoft.Web.WebView2.Core.CoreWebView2PrintDialogKind.System);
#else
            await Task.CompletedTask;
            throw new PlatformNotSupportedException("La impresion solo esta disponible en Windows.");
#endif
        }

        public string GenerarHtmlCatalogo(IReadOnlyList<ProductoModel> productos)
        {
            var html = new StringBuilder();

            html.Append("""
                <!DOCTYPE html>
                <html lang="es">
                <head>
                <meta charset="utf-8">
                <style>
                    * { box-sizing: border-box; }
                    body { font-family: 'Segoe UI', Arial, sans-serif; color: #1f2937; margin: 0; padding: 32px; background: #ffffff; }
                    .doc { max-width: 720px; margin: 0 auto; }
                    header { display: flex; justify-content: space-between; align-items: flex-start; border-bottom: 3px solid #008B8B; padding-bottom: 16px; margin-bottom: 8px; }
                    .marca { font-size: 26px; font-weight: 700; color: #008B8B; letter-spacing: 1px; }
                    .marca-sub { color: #6b7280; font-size: 12px; margin-top: 2px; }
                    .titulo { text-align: right; }
                    .titulo h1 { font-size: 20px; margin: 0; color: #111827; }
                    .titulo p { margin: 4px 0 0; font-size: 13px; color: #6b7280; }
                    table { width: 100%; border-collapse: collapse; font-size: 14px; }
                    th { background: #008B8B; color: #fff; text-align: left; padding: 10px 12px; font-size: 11px; text-transform: uppercase; letter-spacing: .5px; }
                    td { padding: 9px 12px; border-bottom: 1px solid #e5e7eb; vertical-align: top; }
                    td.der, th.der { text-align: right; }
                    .codigo { color: #6b7280; }
                    tr:nth-child(even) td { background: #f8fafc; }
                    .resumen { margin-top: 16px; font-size: 13px; color: #6b7280; text-align: right; }
                    .pie { margin-top: 48px; border-top: 1px solid #e5e7eb; padding-top: 12px; font-size: 11px; color: #9ca3af; text-align: center; }
                    @media print { body { padding: 0; } }
                </style>
                </head>
                <body>
                <div class="doc">
                """);

            html.Append("<header><div><div class=\"marca\">TRADEFLOW</div><div class=\"marca-sub\">Comprobantes y control de ventas</div></div>");
            html.Append($"<div class=\"titulo\"><h1>Cat&aacute;logo de productos</h1><p>{DateTime.Now:dd/MM/yyyy HH:mm}</p></div></header>");

            html.Append("""
                <table>
                <thead><tr><th style="width:130px">C&oacute;digo</th><th>Producto</th><th class="der" style="width:120px">Precio</th></tr></thead>
                <tbody>
                """);

            foreach (var producto in productos)
            {
                html.Append("<tr>");
                html.Append($"<td class=\"codigo\">{Escapar(producto.Codigo)}</td>");
                html.Append($"<td>{Escapar(producto.Nombre)}</td>");
                html.Append($"<td class=\"der\">${producto.Precio:N2}</td>");
                html.Append("</tr>");
            }

            html.Append("</tbody></table>");
            html.Append($"<div class=\"resumen\">{productos.Count} {(productos.Count == 1 ? "producto" : "productos")}</div>");
            html.Append($"<div class=\"pie\">Documento generado por TradeFlow &middot; {DateTime.Now:dd/MM/yyyy HH:mm}</div>");
            html.Append("</div></body></html>");

            return html.ToString();
        }

        public async Task<string> ExportarAPdfAsync(object controlWebView, string html, string nombreArchivo)
        {
            if (controlWebView is not Microsoft.Maui.Controls.WebView webView)
            {
                throw new ArgumentException("El control de exportacion no es valido.", nameof(controlWebView));
            }

#if WINDOWS
            if (webView.Handler?.PlatformView is not Microsoft.UI.Xaml.Controls.WebView2 webView2)
            {
                throw new PlatformNotSupportedException("La exportacion a PDF solo esta disponible en Windows.");
            }

            var core = webView2.CoreWebView2;
            if (core is null)
            {
                var inicializado = new TaskCompletionSource();
                webView2.CoreWebView2Initialized += (_, _) => inicializado.TrySetResult();
                await inicializado.Task.WaitAsync(TimeSpan.FromSeconds(10));
                core = webView2.CoreWebView2;
            }

            if (core is null)
            {
                throw new Exception("No se pudo inicializar el motor de PDF.");
            }

            // Cargo el HTML y espero la navegacion antes de imprimir a PDF
            var navegacionTerminada = new TaskCompletionSource();
            void AlNavegar(object? sender, Microsoft.Maui.Controls.WebNavigatedEventArgs e)
                => navegacionTerminada.TrySetResult();

            webView.Navigated += AlNavegar;
            try
            {
                webView.Source = new HtmlWebViewSource { Html = html };
                await navegacionTerminada.Task.WaitAsync(TimeSpan.FromSeconds(10));
            }
            finally
            {
                webView.Navigated -= AlNavegar;
            }

            var rutaArchivo = Path.Combine(FileSystem.AppDataDirectory, nombreArchivo);
            var generado = await core.PrintToPdfAsync(rutaArchivo, null);

            if (!generado)
            {
                throw new Exception("El motor de PDF no pudo generar el archivo.");
            }

            return rutaArchivo;
#else
            await Task.CompletedTask;
            throw new PlatformNotSupportedException("La exportacion a PDF solo esta disponible en Windows.");
#endif
        }

        public async Task<string> GenerarCatalogoPdfAsync(IReadOnlyList<ProductoModel> productos)
        {
            var rutaArchivo = Path.Combine(FileSystem.AppDataDirectory, "catalogo_productos.pdf");

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.MarginHorizontal(40);
                    page.MarginVertical(30);

                    page.Header().Element(header =>
                    {
                        header.Row(row =>
                        {
                            row.RelativeItem().PaddingBottom(5).Column(col =>
                            {
                                col.Item().Text("Distribuidora Yrigoyen")
                                    .FontSize(22)
                                    .Bold()
                                    .FontColor("#000000");

                                col.Item().Text("Catálogo de productos")
                                    .Bold()
                                    .FontSize(12)
                                    .FontColor("#6b7280");
                            });
                        });
                    });

                    page.Content().Element(content =>
                    {
                        content.Column(column =>
                        {
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(4);
                                    columns.RelativeColumn(2);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Border(1).BorderColor("#000000").Padding(8).Text("PRODUCTO")
                                        .FontColor("#000000").FontSize(9).Bold();
                                    header.Cell().Border(1).BorderColor("#000000").Padding(8).AlignRight().Text("PRECIO")
                                        .FontColor("#000000").FontSize(9).Bold();
                                });

                                int indice = 0;
                                foreach (var producto in productos)
                                {
                                    table.Cell().Border(1).BorderColor("#000000").Padding(8)
                                        .Text(producto.Nombre).FontSize(10);
                                    table.Cell().Border(1).BorderColor("#000000").Padding(8).AlignRight()
                                        .Text($"${producto.Precio:N2}").FontSize(10);
                                    indice++;
                                }
                            });
                        });
                    });

                    page.Footer().AlignCenter()
                        .Text("Documento generado por TradeFlow")
                        .FontSize(8).FontColor("#9ca3af");
                });
            }).GeneratePdf(rutaArchivo);

            await Task.CompletedTask;
            return rutaArchivo;
        }

        private static string Escapar(string? texto)
            => System.Net.WebUtility.HtmlEncode(texto ?? string.Empty);
    }
}