using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.Apache.REEF.Common.metrics.Api;

namespace Org.Apache.REEF.Common.metrics.MutableMetricsLib
{
    public class MutableRate : MutableStat
    {
        public MutableRate(IMetricsInfo info, bool extended) : base(GenerateStatsInfo(info), extended)
        {
        }

        private static IStatsInfo GenerateStatsInfo(IMetricsInfo info)
        {
            return new StatsInfoImpl(info, "Time");
        }
    }
}
