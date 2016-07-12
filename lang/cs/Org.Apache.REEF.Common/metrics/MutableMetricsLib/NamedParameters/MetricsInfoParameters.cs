using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.Apache.REEF.Common.Io;
using Org.Apache.REEF.Tang.Annotations;

namespace Org.Apache.REEF.Common.metrics.MutableMetricsLib.NamedParameters
{
    public sealed class MetricsInfoParameters
    {
        [NamedParameter("Name of the metric","mname")]
        public sealed class MetricName : Name<string>
        {
        }

        [NamedParameter("Description of the metric", "mdesc")]
        public sealed class MetricDescription : Name<string>
        {
        }
    }
}
