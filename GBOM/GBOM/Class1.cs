using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBOM
{
    public class Error
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public string ResourceKey { get; set; }
        public string ResourceFormatString { get; set; }
        public string ResourceFormatString2 { get; set; }
        public string PropertyName { get; set; }
    }

    public class ProductAttribute
    {
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
    }

    public class PriceBreak
    {
        public int Quantity { get; set; }
        public string Price { get; set; }
        public string Currency { get; set; }
    }

    public class AlternatePackaging
    {
        public string APMfrPN { get; set; }
    }

    public class UnitWeightKg
    {
        public int UnitWeight { get; set; }
    }

    public class Part
    {
        public string Availability { get; set; }
        public string DataSheetUrl { get; set; }
        public string Description { get; set; }
        public string FactoryStock { get; set; }
        public string ImagePath { get; set; }
        public string Category { get; set; }
        public string LeadTime { get; set; }
        public string LifecycleStatus { get; set; }
        public string Manufacturer { get; set; }
        public string ManufacturerPartNumber { get; set; }
        public string Min { get; set; }
        public string Mult { get; set; }
        public string MouserPartNumber { get; set; }
        public List<ProductAttribute> ProductAttributes { get; set; }
        public List<PriceBreak> PriceBreaks { get; set; }
        public List<AlternatePackaging> AlternatePackagings { get; set; }
        public string ProductDetailUrl { get; set; }
        public bool Reeling { get; set; }
        public string ROHSStatus { get; set; }
        public string SuggestedReplacement { get; set; }
        public int MultiSimBlue { get; set; }
        public UnitWeightKg UnitWeightKg { get; set; }
        public string RestrictionMessage { get; set; }
        public string PID { get; set; }
    }

    public class SearchResults
    {
        public int NumberOfResult { get; set; }
        public List<Part> Parts { get; set; }
    }

    public class RootObject
    {
        public List<Error> Errors { get; set; }
        public SearchResults SearchResults { get; set; }
    }
}
