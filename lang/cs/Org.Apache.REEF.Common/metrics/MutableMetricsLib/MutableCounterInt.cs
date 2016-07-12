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
    public sealed class MutableCounterInt : MutableCounter
    {
        private int _value;
        private readonly object _lock = new object();

        [Inject]
        public MutableCounterInt(IMetricsInfo info,
            [Parameter(typeof(MetricsInitValueParameters.IntMetricInitValue))] int initValue) : base(info)
        {
            _value = initValue;
        }

        public override void Increment()
        {
            Increment(1);
        }

        public void Increment(int delta)
        {
            lock (_lock)
            {
                _value += delta;
                SetChanged();
            }
        }

        public override void TakeSnapshot(IMetricsRecordBuilder recordBuilder, bool all)
        {
            if (all || Changed)
            {
                recordBuilder.AddCounter(Info, _value);
                ClearChanged();
            }            
        }
    }
}
