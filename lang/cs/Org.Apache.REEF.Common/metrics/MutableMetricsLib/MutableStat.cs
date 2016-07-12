using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.Apache.REEF.Common.metrics.Api;
using Org.Apache.REEF.Common.metrics.MutableMetricsLib.NamedParameters;
using Org.Apache.REEF.Tang.Annotations;

namespace Org.Apache.REEF.Common.metrics.MutableMetricsLib
{
    public class MutableStat : MutableMetric
    {
        private readonly IMetricsInfo _numSamplesInfo;
        private readonly IMetricsInfo _runningMeanInfo;
        private readonly IMetricsInfo _currentMeanInfo;
        private readonly IMetricsInfo _runningMinInfo;
        private readonly IMetricsInfo _runningMaxInfo;
        private readonly IMetricsInfo _currentMinInfo;
        private readonly IMetricsInfo _currentMaxInfo;
        private readonly IMetricsInfo _runningStdInfo;
        private readonly IMetricsInfo _currentStdInfo;

        private readonly StatsHelperClass _runningStat = new StatsHelperClass();
        private readonly StatsHelperClass _intervalStat = new StatsHelperClass();
        private readonly StatsHelperClass _prevStat = new StatsHelperClass();
        private readonly bool _showExtendedStats;
        private readonly object _lock = new object();

        [Inject]
        public MutableStat(IStatsInfo info, [Parameter(typeof(ExtendedStatsParameter))] bool extended) : base(info)
        {
            _showExtendedStats = extended;

            string name = info.Name + "-Num";
            string desc = "Number of samples for " + info.Description;
            _numSamplesInfo = new MetricsInfoImpl(name, desc);

            name = info.Name + "-RunningAvg";
            desc = "Average " + info.ValueName + " for " + info.Description;
            _runningMeanInfo = new MetricsInfoImpl(name, desc);

            name = info.Name + "-RunningStdev";
            desc = "Standard deviation of " + info.ValueName + " for " + info.Description;
            _runningStdInfo = new MetricsInfoImpl(name, desc);

            name = info.Name + "-IntervalAvg";
            desc = "Interval Average " + info.ValueName + " for " + info.Description;
            _currentMeanInfo = new MetricsInfoImpl(name, desc);

            name = info.Name + "-IntervalStdev";
            desc = "Interval Standard deviation of " + info.ValueName + " for " + info.Description;
            _currentStdInfo = new MetricsInfoImpl(name, desc);

            name = info.Name + "-RunningMin";
            desc = "Min " + info.ValueName + " for " + info.Description;
            _runningMinInfo = new MetricsInfoImpl(name, desc);

            name = info.Name + "-RunningMax";
            desc = "Max " + info.ValueName + " for " + info.Description;
            _runningMaxInfo = new MetricsInfoImpl(name, desc);

            name = info.Name + "-IntervalMin";
            desc = "Interval Min " + info.ValueName + " for " + info.Description;
            _currentMinInfo = new MetricsInfoImpl(name, desc);

            name = info.Name + "-IntervalMax";
            desc = "Interval Max " + info.ValueName + " for " + info.Description;
            _currentMaxInfo = new MetricsInfoImpl(name, desc);
        }

        public void Add(double value)
        {
            lock (_lock)
            {
                _runningStat.Add(value);
                _intervalStat.Add(value);
                SetChanged();
            }
        }

        public override void TakeSnapshot(IMetricsRecordBuilder recordBuilder, bool all)
        {
            lock (_lock)
            {
                var lastStat = Changed ? _intervalStat : _prevStat;
                if (all || Changed)
                {
                    recordBuilder.AddCounter(_numSamplesInfo, lastStat.NumSamples)
                        .AddGauge(_currentMeanInfo, lastStat.Mean);

                    if (_showExtendedStats)
                    {
                        recordBuilder.AddGauge(_currentMaxInfo, _intervalStat.MinMaxModule.Max)
                            .AddGauge(_currentMinInfo, _intervalStat.MinMaxModule.Min)
                            .AddGauge(_currentStdInfo, _intervalStat.Std)
                            .AddGauge(_runningMaxInfo, _runningStat.MinMaxModule.Max)
                            .AddGauge(_runningMinInfo, _runningStat.MinMaxModule.Min)
                            .AddGauge(_runningMeanInfo, _runningStat.Mean)
                            .AddGauge(_runningStdInfo, _runningStat.Std);
                    }

                    if (Changed)
                    {
                        if (_runningStat.NumSamples > 0)
                        {
                            _prevStat.CopyFrom(_intervalStat);
                            _intervalStat.Reset();
                        }
                        ClearChanged();
                    }
                }
            }
        }
    }
}
