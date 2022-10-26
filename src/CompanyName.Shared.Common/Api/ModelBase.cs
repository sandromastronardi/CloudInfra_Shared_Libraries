// Copyright (c) Mastronardi Software (mastrosoft.com). All Rights Reserved.
// Licensed under Mozilla Public License, Version 2.0

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace CompanyName.Shared.Common.Api
{
    public class ModelBase<MLB>
    {
        [DataMember(Name = "_links")]
        [JsonProperty("_links")]
        public MLB Links { get; set; }
    }
}