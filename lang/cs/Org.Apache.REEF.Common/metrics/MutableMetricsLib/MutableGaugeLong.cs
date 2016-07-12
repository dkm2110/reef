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
    public sealed class MutableGaugeLong : MutableGauge<long>
    {
        private long _value;
        private readonly object _lock = new object();

        [Inject]
        public MutableGaugeLong(IMetricsInfo info,
            [Parameter(typeof(MetricsInitValueParameters.LongMetricInitValue))] long initValue) : base(info)
        {
            _value = initValue;
        }

        public override void Increment(long delta)
        {
            lock (_lock)
            {
                _value += delta;
                SetChanged();
            }
        }

        public override void Decrement(long delta)
        {
            lock (_lock)
            {
                _value -= delta;
                SetChanged();
            }
        }

        public void Set(long value)
        {
            lock (_lock)
            {
                _value = value;
                SetChanged();
            }
        }

        public override void TakeSnapshot(IMetricsRecordBuilder recordBuilder, bool all)
        {
            if (all || Changed)
            {
                recordBuilder.AddGauge(Info, _value);
                ClearChanged();
            }            
        }
    }
}
