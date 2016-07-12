using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.Apache.REEF.Common.metrics.Api;
using Org.Apache.REEF.Common.metrics.MutableMetricsLib.NamedParameters;
using Org.Apache.REEF.Tang.Annotations;

namespace Org.Apache.REEF.Common.metrics.MutableMetricsLib
{
    public sealed class MetricsInfoImpl : IMetricsInfo
    {
        [Inject]
        public MetricsInfoImpl([Parameter(typeof(MetricsInfoParameters.MetricName))] string name,
            [Parameter(typeof(MetricsInfoParameters.MetricDescription))] string desc)
        {
            Name = name;
            Description = desc;
        }

        public string Name { get; private set; }

        public string Description { get; private set; }
    }
}
