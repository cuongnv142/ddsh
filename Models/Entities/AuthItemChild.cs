﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DongThucVatQuangTri.Models.Entities
{
    public partial class AuthItemChild
    {
        public string Parent { get; set; }
        public string Child { get; set; }

        public virtual AuthItem ParentNavigation { get; set; }
    }
}