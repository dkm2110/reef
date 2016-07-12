using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.Apache.REEF.Tang.Annotations;

namespace Org.Apache.REEF.Common.metrics.MutableMetricsLib.NamedParameters
{
    [NamedParameter("Value name for the stat (e.g. Time) used for display","valname")]
    public sealed class StatValueNameParameter: Name<string>
    {
    }
}
