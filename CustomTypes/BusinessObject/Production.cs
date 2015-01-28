using System;
using System.Collections.Generic;
using CustomTypes.Attributes;
using CustomTypes.Base;

namespace CustomTypes.Objects
{
    public class Production : BaseEntityObject
    {
        [ByPassInsertParam]
        public int ProductionID           { get; set; }
        [ByPassUpdateParam]
        public string VoucherCode         { get; set; }
        [ByPassUpdateParam]
        public DateTime ProductionDate    { get; set; }
        public int DepartementID          { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public string DepartementName     { get; set; }
        public string Note                { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public List<Composite> Composites { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public List<ProductionResult> Results { get; set; }
        [ByPassInsertParam]
        [ByPassUpdateParam]
        public bool Deleted               { get; set; }
    }

    public class Composite : Items
    {
        public int ProductionID  { get; set; }
        public int CompositeType { get; set; }
    }

    public class ProductionResult : Items
    {
        public int ProductionID { get; set; }
    }
}
