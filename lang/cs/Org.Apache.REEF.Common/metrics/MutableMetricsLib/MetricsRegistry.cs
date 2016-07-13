// Licensed to the Apache Software Foundation (ASF) under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  The ASF licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

using System.Collections.Generic;
using Org.Apache.REEF.Common.metrics.Api;

namespace Org.Apache.REEF.Common.metrics.MutableMetricsLib
{
    /// <summary>
    /// A helper class to maintain collection of mutable metrics and tags.
    /// Makes writing of <see cref="IMetricsSource"/> easier. Eac associated metric 
    /// or tag is assumed to have a unique name.
    /// </summary>
    public class MetricsRegistry
    {
        private readonly Dictionary<string, MutableMetric> _metricsMap =
            new Dictionary<string, MutableMetric>();
        private readonly Dictionary<string, MetricsTag> _tagsMap =
            new Dictionary<string, MetricsTag>();
        private readonly object _lock = new object();

        /// <summary>
        /// Constructs the registry with metadata.
        /// </summary>
        /// <param name="info">Meta-data for the group of metrics.</param>
        public MetricsRegistry(IMetricsInfo info)
        {
            RegistryInfo = info;
        }

        /// <summary>
        /// Constructs the registry with name of the group of metrics.
        /// </summary>
        /// <param name="name">Name of the group of metrics.</param>
        public MetricsRegistry(string name)
        {
            RegistryInfo = new MetricsInfoImpl(name, name);
        }

        /// <summary>
        /// Information about the registry.
        /// </summary>
        public IMetricsInfo RegistryInfo { get; private set; }

        /// <summary>
        /// Gets metric by name.
        /// </summary>
        /// <param name="name">Name of the metric.</param>
        /// <returns>Metrics object.</returns>
        public MutableMetric GetMetric(string name)
        {
            lock (_lock)
            {
                MutableMetric res;
                _metricsMap.TryGetValue(name, out res);
                return res;
            }
        }

        /// <summary>
        /// Gets tag by name.
        /// </summary>
        /// <param name="name">Name of the tag.</param>
        /// <returns>Tag object.</returns>
        public MetricsTag GetTag(string name)
        {
            lock (_lock)
            {
                MetricsTag res;
                _tagsMap.TryGetValue(name, out res);
                return res;
            }
        }

        /// <summary>
        /// Creates a new mutable integer counter.
        /// </summary>
        /// <param name="name">Name of the counter.</param>
        /// <param name="desc">Description of the counter.</param>
        /// <param name="initValue">Initial value of the metric.</param>
        /// <returns>A new Mutable integer counter.</returns>
        public MutableCounterInt GetNewCounter(string name, string desc, int initValue)
        {
            return GetNewCounter(new MetricsInfoImpl(name, desc), initValue);
        }

        /// <summary>
        /// Creates a new mutable integer counter.
        /// </summary>
        /// <param name="info">Metadata of the counter.</param>
        /// <param name="initValue">Initial value of the metric.</param>
        /// <returns>A new Mutable integer counter.</returns>
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

        /// <summary>
        /// Creates a new mutable long counter.
        /// </summary>
        /// <param name="name">Name of the counter.</param>
        /// <param name="desc">Description of the counter.</param>
        /// <param name="initValue">Initial value of the metric.</param>
        /// <returns>A new Mutable long counter.</returns>
        public MutableCounterLong GetNewCounter(string name, string desc, long initValue)
        {
            return GetNewCounter(new MetricsInfoImpl(name, desc), initValue);
        }

        /// <summary>
        /// Creates a new mutable long counter.
        /// </summary>
        /// <param name="info">Metadata of the counter.</param>
        /// <param name="initValue">Initial value of the metric.</param>
        /// <returns>A new Mutable long counter.</returns>
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

        /// <summary>
        /// Creates a new mutable long gauge.
        /// </summary>
        /// <param name="name">Name of the gauge.</param>
        /// <param name="desc">Description of the gauge.</param>
        /// <param name="initValue">Initial value of the metric.</param>
        /// <returns>A new Mutable long gauge.</returns>
        public MutableGaugeLong GetNewGauge(string name, string desc, long initValue)
        {
            return GetNewGauge(new MetricsInfoImpl(name, desc), initValue);
        }

        /// <summary>
        /// Creates a new mutable long gauge.
        /// </summary>
        /// <param name="info">Metadata of the gauge.</param>
        /// <param name="initValue">Initial value of the metric.</param>
        /// <returns>A new Mutable long guage.</returns>
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

        /// <summary>
        /// Creates a new mutable integer gauge.
        /// </summary>
        /// <param name="name">Name of the gauge.</param>
        /// <param name="desc">Description of the gauge.</param>
        /// <param name="initValue">Initial value of the metric.</param>
        /// <returns>A new Mutable integer gauge.</returns>
        public MutableGaugeInt GetNewGauge(string name, string desc, int initValue)
        {
            return GetNewGauge(new MetricsInfoImpl(name, desc), initValue);
        }

        /// <summary>
        /// Creates a new mutable integer gauge.
        /// </summary>
        /// <param name="info">Metadata of the gauge.</param>
        /// <param name="initValue">Initial value of the metric.</param>
        /// <returns>A new Mutable integer guage.</returns>
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

        /// <summary>
        /// Creates a mutable stats metric.
        /// </summary>
        /// <param name="name">Name of the metric.</param>
        /// <param name="desc">Description of the metric.</param>
        /// <param name="valueName">Name of the value that this stat represents (e.g. Time).</param>
        /// <param name="extended">If true, outputs exytended stats (min, max, stdev)</param>
        /// <returns>A new mutable stats metric.</returns>
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

        /// <summary>
        /// Creates a mutable rate metric.
        /// </summary>
        /// <param name="name">Name of the metric.</param>
        /// <param name="desc">Description of the metric.</param>
        /// <param name="extended">If true, outputs exytended stats (min, max, stdev)</param>
        /// <returns>A new mutable rate metric.</returns>
        public MutableRate GetNewRate(string name, string desc, bool extended = false)
        {
            lock (_lock)
            {
                CheckMetricName(name);
                var value = new MutableRate(new MetricsInfoImpl(name, desc), extended);
                _metricsMap[name] = value;
                return value;
            }
        }

        /// <summary>
        /// Creates a mutable rate metric.
        /// </summary>
        /// <param name="name">Name of the metric.</param>
        /// <returns>A new mutable rate metric.</returns>
        public MutableRate GetNewRate(string name)
        {
            return GetNewRate(name, name);
        }

        /// <summary>
        /// Adds <see cref="MutableMetric"/> by name.
        /// </summary>
        /// <param name="name">Name by which to add.</param>
        /// <param name="metric">Metric to add.</param>
        public void AddMetric(string name, MutableMetric metric)
        {
            lock (_lock)
            {
                CheckMetricName(name);
                _metricsMap[name] = metric;
            }
        }

        /// <summary>
        /// Adds a tag to the registry.
        /// </summary>
        /// <param name="name">Name of the tag.</param>
        /// <param name="desc">Description of the tag.</param>
        /// <param name="value">Value of the tag.</param>
        /// <param name="replaceExisting">If true, simply replace the tag with same name 
        /// if it exists. If false and tag name already exists then throws error.</param>
        /// <returns>Metrics tag object.</returns>
        public MetricsRegistry AddTag(string name, string desc, string value, bool replaceExisting = false)
        {
            return AddTag(new MetricsInfoImpl(name, desc), value, replaceExisting);
        }

        /// <summary>
        /// Adds a tag to the registry.
        /// </summary>
        /// <param name="info">Metadata of the tag.</param>
        /// <param name="value">Value of the tag.</param>
        /// <param name="replaceExisting">If true, simply replace the tag with same name 
        /// if it exists. If false and tag name already exists then throws error.</param>
        /// <returns>Metrics tag object.</returns>
        public MetricsRegistry AddTag(IMetricsInfo info, string value, bool replaceExisting = false)
        {
            lock (_lock)
            {
                if (!replaceExisting)
                {
                    CheckTagName(info.Name);
                }
                _tagsMap[info.Name] = new MetricsTag(info, value);
                return this;
            }
        }

        /// <summary>
        /// Sampled all the metrics and put the snapshot in the builder.
        /// </summary>
        /// <param name="recordBuilder">The metrics record builder.</param>
        /// <param name="all">If true, record even unchanged metrics.</param>
        public void Snapshot(IMetricsRecordBuilder recordBuilder, bool all)
        {
            lock (_lock)
            {
                foreach (var tag in _tagsMap.Values)
                {
                    recordBuilder.Add(tag);
                }

                foreach (var metric in _metricsMap.Values)
                {
                    metric.TakeSnapshot(recordBuilder, all);
                }
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
