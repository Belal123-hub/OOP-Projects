using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MultilevelMenuExample.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Currency
    {
        USD=1,
        EUR,
        Rub
    }
}
