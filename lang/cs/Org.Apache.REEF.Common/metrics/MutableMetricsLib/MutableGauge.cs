using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.Apache.REEF.Common.metrics.Api;

namespace Org.Apache.REEF.Common.metrics.MutableMetricsLib
{
    public abstract class MutableGauge<T> : MutableMetric
    {
        protected MutableGauge(IMetricsInfo info) : base(info)
        {
        }

        public abstract void Increment(T delta);

        public abstract void Decrement(T delta);
    }
}
