using System;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.Apache.REEF.Common.metrics.Api;
using Org.Apache.REEF.Utilities.Attributes;

namespace Org.Apache.REEF.Common.metrics.MutableMetricsLib
{
    public class MetricsRegistry
    {
        private readonly Dictionary<string, MutableMetric> _metricsMap =
            new Dictionary<string, MutableMetric>();
        private readonly Dictionary<string, MetricsTag> _tagsMap =
            new Dictionary<string, MetricsTag>();
        private readonly object _lock = new object();

        public MetricsRegistry(IMetricsInfo info)
        {
            RegistryInfo = info;
        }

        public MetricsRegistry(string name)
        {
            RegistryInfo = new MetricsInfoImpl(name, name);
        }

        public IMetricsInfo RegistryInfo { get; private set; }

        public MutableMetric GetMetric(string name)
        {
            lock (_lock)
            {
                MutableMetric res;
                _metricsMap.TryGetValue(name, out res);
                return res;
            }
        }

        public MetricsTag GetTag(string name)
        {
            lock (_lock)
            {
                MetricsTag res;
                _tagsMap.TryGetValue(name, out res);
                return res;
            }
        }

        public MutableCounterInt GetNewCounter(string name, string desc, int initValue)
        {
            return GetNewCounter(new MetricsInfoImpl(name, desc), initValue);

        }

        public MutableCounterInt GetNewCounter(IMetricsInfo info, int initValue)
        {
            lock (_lock)
            {
                CheckMetricName(info.Name);
                var value = new MutableCounterInt(info, initValue);
                _metricsMap[info.Name] = value;
                return value;
            }
        }

        public MutableCounterLong GetNewCounter(string name, string desc, long initValue)
        {
            return GetNewCounter(new MetricsInfoImpl(name, desc), initValue);

        }

        public MutableCounterLong GetNewCounter(IMetricsInfo info, long initValue)
        {
            lock (_lock)
            {
                CheckMetricName(info.Name);
                var value = new MutableCounterLong(info, initValue);
                _metricsMap[info.Name] = value;
                return value;
            }
        }

        public MutableGaugeLong GetNewGauge(string name, string desc, long initValue)
        {
            return GetNewGauge(new MetricsInfoImpl(name, desc), initValue);

        }

        public MutableGaugeLong GetNewGauge(IMetricsInfo info, long initValue)
        {
            lock (_lock)
            {
                CheckMetricName(info.Name);
                var value = new MutableGaugeLong(info, initValue);
                _metricsMap[info.Name] = value;
                return value;
            }
        }

        public MutableGaugeInt GetNewGauge(string name, string desc, int initValue)
        {
            return GetNewGauge(new MetricsInfoImpl(name, desc), initValue);

        }

        public MutableGaugeInt GetNewGauge(IMetricsInfo info, int initValue)
        {
            lock (_lock)
            {
                CheckMetricName(info.Name);
                var value = new MutableGaugeInt(info, initValue);
                _metricsMap[info.Name] = value;
                return value;
            }
        }

        public MutableStat GetNewStat(string name, string desc, string valueName, bool extended = false)
        {
            lock (_lock)
            {
                CheckMetricName(name);
                var value = new MutableStat(new StatsInfoImpl(new MetricsInfoImpl(name, desc), valueName), extended);
                _metricsMap[name] = value;
                return value;
            }
        }

        private void CheckMetricName(string name)
        {
            // Check if name has already been registered
            if (_metricsMap.ContainsKey(name))
            {
                throw new MetricsException("Metric name " + name + " already exists!");
            }
        }

        private void CheckTagName(string name)
        {
            if (_tagsMap.ContainsKey(name))
            {
                throw new MetricsException("Tag " + name + " already exists!");
            }
        }
    }
}
