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
    public sealed class MutableGaugeDouble : MutableGauge<double>
    {
        private double _value;
        private readonly object _lock = new object();

        [Inject]
        public MutableGaugeDouble(IMetricsInfo info,
            [Parameter(typeof(MetricsInitValueParameters.DoubleMetricInitValue))] double initValue) : base(info)
        {
            _value = initValue;
        }

        public override void Increment(double delta)
        {
            lock (_lock)
            {
                _value += delta;
                SetChanged();
            }
        }

        public override void Decrement(double delta)
        {
            lock (_lock)
            {
                _value -= delta;
                SetChanged();
            }
        }

        public void Set(double value)
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
