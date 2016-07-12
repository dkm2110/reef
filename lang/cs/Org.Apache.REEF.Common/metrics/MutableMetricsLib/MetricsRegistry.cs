using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.Apache.REEF.Common.metrics.Api;

namespace Org.Apache.REEF.Common.metrics.MutableMetricsLib
{
    public sealed class MetricsRegistry
    {
        private readonly Dictionary<string, MutableMetric> _metricsMap = new Dictionary<string, MutableMetric>();
        private readonly Dictionary<string, MetricsTag> _tagsMap = new Dictionary<string, MetricsTag>();
        private readonly IMetricsInfo _registryInfo;

        public MetricsRegistry()
        {
            
        }
    }
}
