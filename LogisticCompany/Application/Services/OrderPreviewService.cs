using Humanizer;
using LogisticCompany.Domain.Entities.Location;
using LogisticCompany.Domain.Entities.Orders;
using LogisticCompany.DTO;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LogisticCompany.Application.Services
{
    public class OrderPreviewService
    {
        private readonly IEnumerable<Country> _countries;
        private readonly IEnumerable<Branch> _branches;
        private readonly IEnumerable<Town> _towns;
        private readonly IEnumerable<DeliveryType> _deliveryTypes;
        private readonly IEnumerable<TransportType> _transportTypes;
        private readonly IEnumerable<PaymentMethod> _paymentMethods;
        private readonly IEnumerable<ParcelTemplate> _parcelTemplates;

        public OrderPreviewService(
            IEnumerable<Country> countries,
            IEnumerable<Branch> branches,
            IEnumerable<Town> towns,
            IEnumerable<DeliveryType> deliveryTypes,
            IEnumerable<TransportType> transportTypes,
            IEnumerable<PaymentMethod> paymentMethods,
            IEnumerable<ParcelTemplate> parcelTemplates)
        {
            _countries = countries;
            _branches = branches;
            _towns = towns;
            _deliveryTypes = deliveryTypes;
            _transportTypes = transportTypes;
            _paymentMethods = paymentMethods;
            _parcelTemplates = parcelTemplates;
        }

        public string GenerateHtml(OrdersDTO order, Client sender, int selectedTypeParcel)
        {
            string senderFio = "-";
            string senderPhone = sender?.Phone ?? "-";

            if (sender != null)
            {
                if (sender.ClientTypeId == 1 && sender.IndividualClients?.Any() == true)
                {
                    var indiv = sender.IndividualClients.First();
                    senderFio = $"{indiv.FirstName} {indiv.PatronymicName} {indiv.LastName}";
                }
                else if (sender.ClientTypeId == 2 && sender.CompanyClients?.Any() == true)
                {
                    var comp = sender.CompanyClients.First();
                    senderFio = $"{comp.CompanyName} ({comp.Inn})";
                }
            }

            string length = "-", width = "-", height = "-", weight = "-";
            string parcelType = selectedTypeParcel == 1 ? "Стандартный шаблон" : "Нестандартный размер";

            if (selectedTypeParcel == 1 && order.ParcelTemplateId > 0)
            {
                var template = _parcelTemplates.FirstOrDefault(t => t.TemplateId == order.ParcelTemplateId);
                if (template != null)
                {
                    length = template.LengthCm?.ToString() ?? "-";
                    width = template.WidthCm?.ToString() ?? "-";
                    height = template.HeightCm?.ToString() ?? "-";
                    weight = template.MaxWeight?.ToString() ?? "-";
                }
            }
            else if (selectedTypeParcel == 2)
            {
                length = order.LengthCm?.ToString() ?? "-";
                width = order.WidthCm?.ToString() ?? "-";
                height = order.HeightCm?.ToString() ?? "-";
                weight = order.Weight?.ToString() ?? "-";
            }

            var originCountry = _countries.FirstOrDefault(c => c.CountryId == order.OriginCountryId)?.CountryName ?? "-";
            var originBranch = _branches.FirstOrDefault(b => b.BranchesId == order.PickupBranchesId)?.NameBranches ?? "-";
            var destBranch = _branches.FirstOrDefault(b => b.BranchesId == order.DestinationBranchesId)?.NameBranches ?? "-";
            var originTown = _towns.FirstOrDefault(t => t.TownId == order.OriginTownId)?.TownName ?? "-";
            var destCountry = _countries.FirstOrDefault(c => c.CountryId == order.DestinationCountryId)?.CountryName ?? "-";
            var destTown = _towns.FirstOrDefault(t => t.TownId == order.DestinationTownId)?.TownName ?? "-";
            var deliveryType = _deliveryTypes.FirstOrDefault(d => d.DeliveryTypeId == order.DeliveryTypeId)?.NameDeliveryType ?? "-";
            var transportType = _transportTypes.FirstOrDefault(t => t.TransportTypeId == order.TransportTypeId)?.NameTransportType ?? "-";
            var paymentMethod = _paymentMethods.FirstOrDefault(p => p.PaymentMethodId == order.PaymentMethodId)?.MethodName ?? "-";
            var paymentDate = order.PaymentDate.ToString("dd.MM.yyyy");

            return $@"
<!DOCTYPE html>
<html lang='ru'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        /* Сброс и базовые стили */
        * {{ 
            margin: 0; 
            padding: 0; 
            box-sizing: border-box; 
        }}
        body {{ 
            font-family: 'Segoe UI', 'Helvetica Neue', Arial, sans-serif; 
            line-height: 1.4; 
            color: #333; 
            background: #f5f5f5; 
            margin: 0; 
            padding: 20px; 
        }}

        /* Страница A4 */
        .page {{ 
            width: 210mm; 
            min-height: 297mm; 
            padding: 15mm; 
            margin: 10mm auto; 
            background: white; 
            box-shadow: 0 5px 15px rgba(0,0,0,0.1);
            position: relative;
        }}

        /* Шапка документа */
        .header {{ 
            text-align: center; 
            margin-bottom: 30px; 
            padding-bottom: 20px; 
            border-bottom: 2px solid #2c3e50;
        }}
        .header h1 {{ 
            color: #2c3e50; 
            font-size: 24px; 
            font-weight: 600; 
            margin-bottom: 10px;
        }}
        .header .document-info {{
            display: flex;
            justify-content: space-between;
            font-size: 12px;
            color: #666;
            margin-top: 10px;
        }}
        .header .document-info div {{ 
            text-align: left; 
        }}

        /* Логотип и информация о компании */
        .company-info {{
            text-align: center;
            margin-bottom: 30px;
            padding: 15px;
            background: #f8f9fa;
            border-radius: 4px;
        }}
        .company-name {{
            font-size: 18px;
            font-weight: 600;
            color: #2c3e50;
            margin-bottom: 5px;
        }}
        .company-details {{
            font-size: 11px;
            color: #666;
            line-height: 1.6;
        }}

        /* Секции документа */
        .section {{
            margin-bottom: 25px;
            page-break-inside: avoid;
        }}
        .section-title {{
            font-size: 14px;
            font-weight: 600;
            color: #2c3e50;
            margin-bottom: 12px;
            padding-bottom: 6px;
            border-bottom: 1px solid #eaeaea;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }}

        /* Таблицы */
        .table-wrapper {{
            margin: 0;
            overflow: visible;
        }}
        .info-table {{
            width: 100%;
            border-collapse: collapse;
            font-size: 12px;
        }}
        .info-table th {{
            background: #f8f9fa;
            color: #2c3e50;
            font-weight: 600;
            padding: 10px 12px;
            border: 1px solid #dee2e6;
            text-align: left;
            width: 35%;
            vertical-align: top;
        }}
        .info-table td {{
            padding: 10px 12px;
            border: 1px solid #dee2e6;
            vertical-align: top;
            word-break: break-word;
        }}
        .info-table tr:nth-child(even) td {{
            background-color: #fafafa;
        }}

        /* Специальные блоки */
        .highlight-box {{
            background: #fff8e1;
            border-left: 4px solid #ffc107;
            padding: 12px 15px;
            margin: 15px 0;
            font-size: 11px;
        }}
        .highlight-box strong {{
            color: #d84315;
        }}

        /* QR код и дополнительная информация */
        .qr-section {{
            text-align: center;
            margin: 25px 0;
            padding: 15px;
            background: #f8f9fa;
            border-radius: 4px;
        }}
        .qr-label {{
            font-size: 11px;
            color: #666;
            margin-bottom: 8px;
        }}

        /* Подвал документа */
        .footer {{
            margin-top: 40px;
            padding-top: 20px;
            border-top: 1px solid #eaeaea;
            font-size: 11px;
            color: #666;
        }}
        .signature-area {{
            margin: 30px 0;
            page-break-inside: avoid;
        }}
        .signature-line {{
            display: flex;
            justify-content: space-between;
            margin-bottom: 15px;
        }}
        .signature-block {{
            width: 45%;
        }}
        .signature-label {{
            font-size: 11px;
            color: #666;
            margin-bottom: 5px;
        }}
        .signature-space {{
            border-bottom: 1px solid #333;
            height: 40px;
            margin-top: 10px;
        }}
        .signature-note {{
            font-size: 10px;
            color: #999;
            margin-top: 5px;
        }}

        /* Номера страниц */
        .page-number {{
            position: absolute;
            bottom: 20px;
            right: 20px;
            font-size: 10px;
            color: #999;
        }}

        /* Для печати */
        @@media print {{
            body {{ 
                background: white; 
                padding: 0; 
                margin: 0; 
            }}
            .page {{ 
                width: 100%; 
                min-height: auto; 
                margin: 0; 
                padding: 15mm; 
                box-shadow: none;
            }}
            .no-print {{ 
                display: none !important; 
            }}
            .page-break {{ 
                page-break-before: always; 
            }}
            
            /* Улучшение читаемости при печати */
            * {{ 
                -webkit-print-color-adjust: exact !important;
                color-adjust: exact !important;
            }}
            .info-table th {{
                background: #f5f5f5 !important;
                -webkit-print-color-adjust: exact;
            }}
        }}

        /* Второй лист (если нужен) */
        .second-sheet {{
            margin-top: 50px;
            padding-top: 30px;
            border-top: 2px dashed #ddd;
        }}

        /* Условия и положения */
        .terms {{
            font-size: 9px;
            color: #666;
            line-height: 1.6;
            margin-top: 20px;
        }}
        .terms h4 {{
            font-size: 10px;
            color: #333;
            margin-bottom: 5px;
        }}

        /* Статус документа */
        .status-badge {{
            display: inline-block;
            padding: 4px 8px;
            background: #28a745;
            color: white;
            border-radius: 3px;
            font-size: 10px;
            font-weight: 600;
            margin-left: 10px;
        }}
    </style>
</head>
<body>
    <div class='page'>
        <!-- Шапка документа -->
        <div class='header'>
            <h1>НАКЛАДНАЯ № <span style='color: #28a745;'>{order.OrderNumber}</span></h1>
        </div>
        <!-- Данные отправителя -->
        <div class='section'>
            <div class='section-title'>Отправитель</div>
            <div class='table-wrapper'>
                <table class='info-table'>
                    <tr>
                        <th>ФИО / Название компании</th>
                        <td>{senderFio}</td>
                    </tr>
                    <tr>
                        <th>Контактный телефон</th>
                        <td>{senderPhone}</td>
                    </tr>
                    <tr>
                        <th>Тип клиента</th>
                        <td>{(sender?.ClientTypeId == 1 ? "Физическое лицо" : "Юридическое лицо")}</td>
                    </tr>
                </table>
            </div>
        </div>

        <!-- Данные получателя -->
        <div class='section'>
            <div class='section-title'>Получатель</div>
            <div class='table-wrapper'>
                <table class='info-table'>
                    <tr>
                        <th>ФИО получателя</th>
                        <td>{order.LastRecepientName} {order.FirstRecepientName} {order.MiddleRecepientName}</td>
                    </tr>
                    <tr>
                        <th>Контактный телефон</th>
                        <td>{order.PhoneRecepient}</td>
                    </tr>
                </table>
            </div>
        </div>

        <!-- Информация о посылке -->
        <div class='section'>
            <div class='section-title'>Характеристики груза</div>
            <div class='table-wrapper'>
                <table class='info-table'>
                    <tr>
                        <th>Описание содержимого</th>
                        <td>{order.DescriptionParcel}</td>
                    </tr>
                    <tr>
                        <th>Тип упаковки</th>
                        <td>{parcelType}</td>
                    </tr>
                    <tr>
                        <th>Габариты (Д×Ш×В)</th>
                        <td>{length} × {width} × {height} см</td>
                    </tr>
                    <tr>
                        <th>Вес брутто</th>
                        <td>{weight} кг</td>
                    </tr>
                    <tr>
                        <th>Объёмный вес</th>
                        <td>{(length != "-" && width != "-" && height != "-" ?
                             (Convert.ToDouble(length) * Convert.ToDouble(width) * Convert.ToDouble(height) / 5000).ToString("F2") : "-")} кг</td>
                    </tr>
                </table>
            </div>
            
            <div class='highlight-box'>
                <strong>Примечание:</strong> {(selectedTypeParcel == 1 ? "Стандартная упаковка" : "Нестандартные габариты")}. Требуется осторожное обращение.
            </div>
        </div>

        <!-- Информация о доставке -->
        <div class='section'>
            <div class='section-title'>Маршрут доставки</div>
            <div class='table-wrapper'>
                <table class='info-table'>
                    <tr>
                        <th>Пункт отправления</th>
                        <td>{originCountry}, {originTown}</td>
                    </tr>
                    <tr>
                        <th>Филиал отправки</th>
                        <td>{originBranch}</td>
                    </tr>
                    <tr>
                        <th>Пункт назначения</th>
                        <td>{destCountry}, {destTown}</td>
                    </tr>
                    <tr>
                        <th>Филиал назначения</th>
                        <td>{destBranch}</td>
                    </tr>
                    <tr>
                        <th>Тип доставки</th>
                        <td>{deliveryType}</td>
                    </tr>
                    <tr>
                        <th>Вид транспорта</th>
                        <td>{transportType}</td>
                    </tr>
                    <tr>
                        <th>Адрес доставки</th>
                        <td>{order.CourierDestAddress ?? "-"}</td>
                    </tr>
                    
                </table>
            </div>
        </div>

        <!-- Финансовая информация -->
        <div class='section'>
            <div class='section-title'>Финансовые реквизиты</div>
            <div class='table-wrapper'>
                <table class='info-table'>
                    <tr>
                        <th>Способ оплаты</th>
                        <td>{paymentMethod}</td>
                    </tr>
                    <tr>
                        <th>Сумма к оплате</th>
                        <td style='font-weight: bold; color: #28a745;'>{order.Amount} ₽</td>
                    </tr>
                    <tr>
                        <th>Дата оплаты</th>
                        <td>{paymentDate}</td>
                    </tr>
                    <tr>
                        <th>Номер платежа</th>
                        <td>INV-{order.OrderNumber}</td>
                    </tr>
                </table>
            </div>
        </div>
        <!-- Подписи -->
        <div class='footer'>
            <div class='signature-area'>
                <div class='signature-line'>
                    <div class='signature-block'>
                        <div class='signature-label'>Отправитель/Клиент</div>
                        <div class='signature-space'></div>
                        <div class='signature-note'>(ФИО, подпись, дата)</div>
                    </div>
                    <div class='signature-block'>
                        <div class='signature-label'>Представитель перевозчика</div>
                        <div class='signature-space'></div>
                        <div class='signature-note'>(ФИО, подпись, печать, дата)</div>
                    </div>
                </div>
            </div>

            <div class='terms'>
                <h4>УСЛОВИЯ ПЕРЕВОЗКИ:</h4>
                <p>1. Перевозчик несет ответственность за сохранность груза с момента приема до момента выдачи получателю.</p>
                <p>2. Клиент обязан обеспечить правильную упаковку груза, соответствующую условиям перевозки.</p>
                <p>3. Срок доставки указан ориентировочно и может меняться в зависимости от погодных условий и загруженности маршрутов.</p>
            </div>

            <div style='text-align: center; margin-top: 15px;'>
                <hr style='border: none; border-top: 1px solid #ddd; margin: 10px 0;'>
                <div style='font-size: 10px; color: #999;'>
                    Документ сформирован автоматически в системе LogisticCompany. {DateTime.Now:dd.MM.yyyy HH:mm:ss}
                </div>
            </div>
        </div>
    </div>
</body>
</html>";

        }
    }
}
