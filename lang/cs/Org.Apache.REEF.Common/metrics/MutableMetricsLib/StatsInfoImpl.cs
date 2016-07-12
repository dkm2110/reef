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
    public sealed class StatsInfoImpl : IStatsInfo
    {
        [Inject]
        public StatsInfoImpl(IMetricsInfo info, [Parameter(typeof(StatValueNameParameter))] string valueName)
        {
            Name = info.Name;
            Description = info.Description;
            ValueName = valueName;
        }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public string ValueName { get; private set; }
    }
}
