// Copyright (c) Mastronardi Software (mastrosoft.com). All Rights Reserved.
// Licensed under Mozilla Public License, Version 2.0

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CompanyName.Shared.Common.Api
{
    public abstract class ListResponseBase<T, MLB> where MLB : ModelLinksBase
    {
        public ListResponseBase(PagingParameters paging, long totalItems):this(paging)
        {
            TotalItems = totalItems;
        }
        public ListResponseBase(PagingParameters paging) {
            PageSize = paging.PageSize;
            Page = paging.Page;
        }
        public ListResponseBase() { }

        public IEnumerable<T> Items { get; set; }
        public long Page { get; set; }
        public long PageSize { get; set; } = 25;
        public long Pages { get
            {
                return Math.Max(1,(long)Math.Ceiling((double)TotalItems / PageSize)); // number of pages with 1 as minimum
            }
        }
        public long ItemCount {
            get
            {
                return Items.Count();
            }
        }
        public dynamic NextPageToken { get; set; }
        public long TotalItems { get; set; }
        [DataMember(Name = "_links")]
        [JsonProperty("_links")]
        public MLB Links {get;set;}
    }
}