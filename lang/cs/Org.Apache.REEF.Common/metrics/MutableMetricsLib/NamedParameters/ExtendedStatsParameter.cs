using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.Apache.REEF.Tang.Annotations;

namespace Org.Apache.REEF.Common.metrics.MutableMetricsLib.NamedParameters
{
    [NamedParameter("Set to true if you want every stat like variance, min, max, False if only mean is needed",
        "extstat", "false")]
    public sealed class ExtendedStatsParameter : Name<bool>
    {
    }
}
