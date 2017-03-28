using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using OfficeOpenXml;
using Sfa.Roatp.Indexer.ApplicationServices.Settings;
using Sfa.Roatp.Indexer.Core.Models;
using SFA.DAS.NLog.Logger;

namespace Sfa.Roatp.Indexer.ApplicationServices
{
    public class RoatpProvidersXlsxService : IGetRoatpProviders
    {
        private const int UkprnPosition = 1;
        private const int NamePosition = 2;
        private const int ProviderTypePosition = 3;
        private const int ContractedForNonLeviedEmployersPosition = 4;
        private const int ParentCompanyGuaranteePosition = 5;
        private const int NewOrganisationWithoutFinancialTrackRecordPosition = 6;
        private const int StartDatePosition = 7;
        private const int EndDatePosition = 8;

        private readonly IAppServiceSettings _appServiceSettings;
        private readonly ILog _log;

        public RoatpProvidersXlsxService(IAppServiceSettings appServiceSettings, ILog log)
        {
            _appServiceSettings = appServiceSettings;
            _log = log;
        }

        public IEnumerable<RoatpProvider> GetRoatpData()
        {
            var roatpProviders = new List<RoatpProvider>();
            IDictionary<string, object> extras = new Dictionary<string, object>();
            extras.Add("DependencyLogEntry.Url", _appServiceSettings.VstsRoatpUrl);

            using (var client = new WebClient())
            {
                if (!string.IsNullOrEmpty(_appServiceSettings.GitUsername))
                {
                    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_appServiceSettings.GitUsername}:{_appServiceSettings.GitPassword}"));
                    client.Headers[HttpRequestHeader.Authorization] = $"Basic {credentials}";
                }

                var filePath = Path.GetTempFileName();
                try
                {
                    client.DownloadFile(new Uri(_appServiceSettings.VstsRoatpUrl), filePath);

                    using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
                    {
                        GetRoatpProviders(package, roatpProviders);
                    }
                }
                catch (WebException wex)
                {
                    extras.Add("DependencyLogEntry.ResponseCode", ((HttpWebResponse) wex.Response).StatusCode);
                    _log.Error(wex, "Problem downloading ROATP from VSTS", extras);
                    return null;
                }
                catch (Exception ex)
                {
                    _log.Error(ex, "Problem downloading ROATP from VSTS", extras);
                }
            }

            return roatpProviders.Where(roatpProviderResult => roatpProviderResult.Ukprn != string.Empty);
        }

        private void GetRoatpProviders(ExcelPackage package, List<RoatpProvider> roatpProviders)
        {
            var roatpWorkSheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "RoATP");
            if (roatpWorkSheet == null) return;

            for (var i = roatpWorkSheet.Dimension.Start.Row + 1; i <= roatpWorkSheet.Dimension.End.Row; i++)
            {
                var roatpData = new RoatpProvider
                {
                    Ukprn = roatpWorkSheet.Cells[i, UkprnPosition].Value != null ? roatpWorkSheet.Cells[i, UkprnPosition].Value.ToString() : string.Empty,
                    Name = roatpWorkSheet.Cells[i, NamePosition].Value != null ? roatpWorkSheet.Cells[i, NamePosition].Value.ToString() : string.Empty,
                    ProviderType = GetProviderType(roatpWorkSheet.Cells[i, ProviderTypePosition]),
                    ContractedForNonLeviedEmployers = GetBooleanValue(roatpWorkSheet.Cells[i, ContractedForNonLeviedEmployersPosition]),
                    ParentCompanyGuarantee = GetBooleanValue(roatpWorkSheet.Cells[i, ParentCompanyGuaranteePosition]),
                    NewOrganisationWithoutFinancialTrackRecord = GetBooleanValue(roatpWorkSheet.Cells[i, NewOrganisationWithoutFinancialTrackRecordPosition]),
                    StartDate = GetDateTimeValue(roatpWorkSheet.Cells[i, StartDatePosition]),
                    EndDate = GetDateTimeValue(roatpWorkSheet.Cells[i, EndDatePosition]),
                };

                roatpProviders.Add(roatpData);
            }
        }

        private static DateTime? GetDateTimeValue(ExcelRange excelRange)
        {
            var value = excelRange.Value?.ToString();
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Contains("/"))
                {
                    return DateTime.Parse(value);
                }

                return DateTime.FromOADate(double.Parse(excelRange.Value.ToString()));
            }

            return null;
        }

        private static bool GetBooleanValue(ExcelRange excelRange)
        {
            if (excelRange.Value != null)
            {
                return excelRange.Value.ToString().ToUpper() == "Y";
            }

            return false;
        }

        private static ProviderType GetProviderType(ExcelRange excelRange)
        {
            if (excelRange.Value != null)
            {
                switch (excelRange.Value.ToString().ToLower())
                {
                    case "main provider":
                        return ProviderType.MainProvider;
                    case "employer provider":
                        return ProviderType.EmployerProvider;
                    case "supporting provider":
                        return ProviderType.SupportingProvider;
                    default:
                        return ProviderType.Unknown;
                }
            }

            return ProviderType.Unknown;
        }
    }
}
