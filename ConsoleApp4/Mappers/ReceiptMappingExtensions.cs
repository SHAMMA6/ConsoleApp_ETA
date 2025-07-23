using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp_ETA_eReceipts.Data;
using ConsoleApp_ETA_eReceipts.EtaDtos;

namespace ConsoleApp_ETA_eReceipts.Mappers
{
    public static class ReceiptMappingExtensions
    {
        /// <summary>
        /// Convert DB entity to ETA Receipt using fallback template + optional override amount.
        /// If overrideAmount == null we try DB totals; if DB totals <=0 we fallback = 1.
        /// Missing strings => fallback constants. Missing Seller => fallback Seller.
        /// If no items => create single fallback item w/ amount.
        /// </summary>
        public static Receipt ToEtaReceipt(this ConsoleApp_ETA_eReceipts.Data.ReceiptEntity entity, decimal? overrideAmount = null)
        {
            // --- Decide amount ---  
            decimal amount = overrideAmount ?? entity.TotalAmount;
            if (amount <= 0) amount = 1m; // user requirement  

            // --- Date ---  
            var dtIssued = entity.DateTimeIssuedUtc != default
                ? DateTime.Parse(entity.DateTimeIssuedUtc).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'")
                : DateTime.UtcNow.AddSeconds(-5).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");

            // --- Header ---  
            string receiptNumber = Use(entity.ReceiptNumber, $"E-{Random.Shared.Next(1000, 10000)}");
            string currency = Use(entity.Currency, EtaDefaults.Currency);
            string prevUuid = entity.PreviousUuid ?? string.Empty;
            string refOldUuid = entity.ReferenceOldUuid ?? string.Empty;
            var header = new ReceiptHeader(dtIssued, receiptNumber, string.Empty, prevUuid, refOldUuid, currency);

            // --- DocumentType ---  
            var docType = new DocumentType(Use(entity.ReceiptType, EtaDefaults.ReceiptType), Use(entity.TypeVersion, EtaDefaults.TypeVersion));

            // --- Seller ---  
            var s = entity.Seller;
            var seller = new Seller(
                Rin: Use(s?.Rin, EtaDefaults.Rin),
                CompanyTradeName: Use(s?.CompanyTradeName, EtaDefaults.CompanyTradeName),
                BranchCode: Use(s?.BranchCode, EtaDefaults.BranchCode),
                BranchAddress: new BranchAddress(
                    Use(s?.Country, EtaDefaults.Country),
                    Use(s?.Governate, EtaDefaults.Governate),
                    Use(s?.RegionCity, EtaDefaults.RegionCity),
                    Use(s?.Street, EtaDefaults.Street),
                    Use(s?.BuildingNumber, EtaDefaults.BuildingNumber),
                    Use(s?.PostalCode, EtaDefaults.PostalCode)
                ),
                DeviceSerialNumber: Use(s?.DeviceSerialNumber, EtaDefaults.DeviceSerialNumber),
                SyndicateLicenseNumber: Use(s?.SyndicateLicenseNumber, EtaDefaults.SyndicateLicenseNumber),
                ActivityCode: Use(s?.ActivityCode, EtaDefaults.ActivityCode)
            );

            // --- Buyer ---  
            var buyer = new Buyer(Use(entity.BuyerType, EtaDefaults.BuyerType));

            // --- Items ---  
            List<ItemData> items;
            if (entity.Items is { Count: > 0 })
            {
                items = entity.Items.Select(i => new ItemData(
                    InternalCode: Use(i.InternalCode, EtaDefaults.DefaultInternalCode),
                    Description: Use(i.Description, EtaDefaults.DefaultDescription),
                    ItemType: Use(i.ItemType, EtaDefaults.DefaultItemType),
                    ItemCode: Use(i.ItemCode, EtaDefaults.DefaultItemCode),
                    UnitType: Use(i.UnitType, EtaDefaults.DefaultUnitType),
                    Quantity: NonZero(i.Quantity, 1m),
                    UnitPrice: NonZero(i.UnitPrice, amount),
                    NetSale: NonZero(i.NetSale, amount * NonZero(i.Quantity, 1m)),
                    TotalSale: NonZero(i.TotalSale, amount * NonZero(i.Quantity, 1m)),
                    Total: NonZero(i.Total, amount * NonZero(i.Quantity, 1m))
                )).ToList();
            }
            else
            {
                // Single fallback line  
                items = new List<ItemData>
               {
                   new ItemData(
                       EtaDefaults.DefaultInternalCode,
                       EtaDefaults.DefaultDescription,
                       EtaDefaults.DefaultItemType,
                       EtaDefaults.DefaultItemCode,
                       EtaDefaults.DefaultUnitType,
                       1m,
                       amount,
                       amount,
                       amount,
                       amount)
               };
            }

            // --- Totals ---  
            decimal totalSales = entity.TotalSales > 0 ? entity.TotalSales : items.Sum(x => x.TotalSale);
            decimal netAmount = entity.NetAmount > 0 ? entity.NetAmount : items.Sum(x => x.NetSale);
            decimal totalAmount = entity.TotalAmount > 0 ? entity.TotalAmount : items.Sum(x => x.Total);

            return new Receipt(header, docType, seller, buyer, items, totalSales, netAmount, totalAmount, Use(entity.PaymentMethod, EtaDefaults.PaymentMethod));
        }

        private static string Use(string? candidate, string fallback) => string.IsNullOrWhiteSpace(candidate) ? fallback : candidate!;
        private static decimal NonZero(decimal candidate, decimal fallback) => candidate > 0 ? candidate : fallback;
    }
}

