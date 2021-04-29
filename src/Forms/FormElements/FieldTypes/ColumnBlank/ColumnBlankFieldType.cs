using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;

namespace YuzuDelivery.Umbraco.Forms
{
    public static class _FormsConstant
    {
        public const string ColumnBlank_Name = "Column Blank";
        public const string ColumnBlank_Blank_Type = "BlankType";
        public const string ColumnBlank_SpanAll = "Span all";
        public const string ColumnBlank_BlankSpace = "Blank Space";
    }

    public class ColumnBlank : FieldType
    {
        public ColumnBlank()
        {
            this.Id = new Guid("21937a5a-42a0-49a4-b37f-dae3d1a65cf1");
            this.Name = _FormsConstant.ColumnBlank_Name;
            this.Description = "A blank field for spacing out fields in columns";
            this.Icon = "icon-backspace";
            this.DataType = FieldDataType.String;
            this.FieldTypeViewName = "Textstring.html";
            this.SortOrder = 123;
            this.SupportsRegex = true;
        }

        [Setting(_FormsConstant.ColumnBlank_Blank_Type, PreValues = "Span all,Blank Space", Description = "Does the blank force the field in the other column to span or is there a blank space", View = "Dropdownlist")]
        public string BlankType { get; set; }

    }
}