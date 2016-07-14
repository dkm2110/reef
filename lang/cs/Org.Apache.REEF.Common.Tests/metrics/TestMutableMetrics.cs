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

using System;
using System.Collections.Generic;
using System.Linq;
using Org.Apache.REEF.Common.metrics.Api;
using Org.Apache.REEF.Common.metrics.MutableMetricsLib;
using Org.Apache.REEF.Common.metrics.MutableMetricsLib.ConfigurationModules;
using Org.Apache.REEF.Common.metrics.MutableMetricsLib.NamedParameters;
using Org.Apache.REEF.Tang.Implementations.Tang;
using Org.Apache.REEF.Tang.Interface;
using Xunit;

namespace Org.Apache.REEF.Common.Tests.metrics
{
    /// <summary>
    /// Tests various classes in REEF.Common.metrics.MutableMetricsLib.
    /// </summary>
    public sealed class TestMutableMetrics
    {
        /// <summary>
        /// Tests <see cref="MetricsInfoImpl"/>.
        /// </summary>
        [Fact]
        public void TestMetricsInfoImpl()
        {
            const string name = "testname";
            const string desc = "testdesc";

            MetricsInfoImpl impl = new MetricsInfoImpl(name, desc);
            Assert.Equal(name, impl.Name);
            Assert.Equal(desc, impl.Description);
        }

        /// <summary>
        /// Tests <see cref="StatsInfoImpl"/>
        /// </summary>
        [Fact]
        public void TestStatsInfoImpl()
        {
            const string name = "testname";
            const string desc = "testdesc";
            const string valueName = "testvalue";

            StatsInfoImpl impl = new StatsInfoImpl(new MetricsInfoImpl(name, desc), valueName);
            Assert.Equal(name, impl.Name);
            Assert.Equal(desc, impl.Description);
            Assert.Equal(valueName, impl.ValueName);
        }

        /// <summary>
        /// Tests various functions of <see cref="MutableCounterInt"/> and <see cref="MutableCounterLong"/>.
        /// </summary>
        [Fact]
        public void TestCounterMetrics()
        {
            const string name = "countertest";
            const string desc = "countertestdesc";

            var info = new MetricsInfoImpl(name, desc);
            MutableCounterInt intCounter = new MutableCounterInt(info, 5);
            Assert.Equal(name, info.Name);
            Assert.Equal(desc, info.Description);

            RecordBuilderForTests recordBuilder = new RecordBuilderForTests();
            intCounter.TakeSnapshot(recordBuilder);
            recordBuilder.Validate(name, 5);
            recordBuilder.Reset();

            intCounter.Increment();
            intCounter.TakeSnapshot(recordBuilder);
            recordBuilder.Validate(name, 6);
            recordBuilder.Reset();

            intCounter.TakeSnapshot(recordBuilder);
            Assert.False(recordBuilder.IntKeyPresent(name), "Metric is not supposed to be recorded.");

            intCounter.TakeSnapshot(recordBuilder, true);
            recordBuilder.Validate(name, 6);
            recordBuilder.Reset();

            MutableCounterLong longCounter = new MutableCounterLong(info, 5);
            Assert.Equal(name, info.Name);
            Assert.Equal(desc, info.Description);

            longCounter.TakeSnapshot(recordBuilder);
            recordBuilder.Validate(name, (long)5);
            recordBuilder.Reset();

            longCounter.Increment();
            longCounter.TakeSnapshot(recordBuilder);
            recordBuilder.Validate(name, (long)6);
            recordBuilder.Reset();

            longCounter.TakeSnapshot(recordBuilder);
            Assert.False(recordBuilder.LongKeyPresent(name), "Metric is not supposed to be recorded.");

            longCounter.TakeSnapshot(recordBuilder, true);
            recordBuilder.Validate(name, (long)6);
            recordBuilder.Reset();
        }

        /// <summary>
        /// Tests various functions of <see cref="MutableGaugeInt"/> and <see cref="MutableGaugeLong"/>.
        /// </summary>
        [Fact]
        public void TestGaugeMetrics()
        {
            const string name = "gaugetest";
            const string desc = "guagetestdesc";

            var info = new MetricsInfoImpl(name, desc);
            MutableGaugeInt intGauge = new MutableGaugeInt(info, 5);
            Assert.Equal(name, info.Name);
            Assert.Equal(desc, info.Description);

            RecordBuilderForTests recordBuilder = new RecordBuilderForTests();
            intGauge.TakeSnapshot(recordBuilder);
            recordBuilder.Validate(name, 5);
            recordBuilder.Reset();

            intGauge.Increment(2);
            intGauge.TakeSnapshot(recordBuilder);
            recordBuilder.Validate(name, 7);
            recordBuilder.Reset();

            intGauge.Decrement(1);
            intGauge.TakeSnapshot(recordBuilder);
            recordBuilder.Validate(name, 6);
            recordBuilder.Reset();

            intGauge.TakeSnapshot(recordBuilder);
            Assert.False(recordBuilder.IntKeyPresent(name), "Metric is not supposed to be recorded.");

            intGauge.TakeSnapshot(recordBuilder, true);
            recordBuilder.Validate(name, 6);
            recordBuilder.Reset();

            MutableGaugeLong longGauge = new MutableGaugeLong(info, 5);
            Assert.Equal(name, info.Name);
            Assert.Equal(desc, info.Description);

            longGauge.TakeSnapshot(recordBuilder);
            recordBuilder.Validate(name, (long)5);
            recordBuilder.Reset();

            longGauge.Increment(2);
            longGauge.TakeSnapshot(recordBuilder);
            recordBuilder.Validate(name, (long)7);
            recordBuilder.Reset();

            longGauge.Decrement(1);
            longGauge.TakeSnapshot(recordBuilder);
            recordBuilder.Validate(name, (long)6);
            recordBuilder.Reset();

            longGauge.TakeSnapshot(recordBuilder);
            Assert.False(recordBuilder.LongKeyPresent(name), "Metric is not supposed to be recorded.");

            longGauge.TakeSnapshot(recordBuilder, true);
            recordBuilder.Validate(name, (long)6);
            recordBuilder.Reset();
        }

        /// <summary>
        /// Tests various functions of <see cref="MutableStat"/>.
        /// </summary>
        [Fact]
        public void TestStatMetrics()
        {
            const string name = "stattest";
            const string desc = "stattestdesc";
            const string valueName = "statValName";
            const double delta = 1e-10;

            double[] array1 = new double[3];
            double[] array2 = new double[3];
            Random randGen = new Random();

            array1 = array1.Select(x => randGen.NextDouble()).ToArray();
            array2 = array2.Select(x => randGen.NextDouble()).ToArray();

            MutableStat stat = new MutableStat(new StatsInfoImpl(new MetricsInfoImpl(name, desc), valueName), true);
            RecordBuilderForTests recordBuilder = new RecordBuilderForTests();

            foreach (var entry in array1)
            {
                stat.Add(entry);
            }

            double expectedMean = array1.Sum() / 3;
            double expectedStd = Math.Sqrt(array1.Select(x => Math.Pow(x - expectedMean, 2)).Sum() / 2);
            stat.TakeSnapshot(recordBuilder);
            recordBuilder.Validate(name + "-Num", (long)3);
            recordBuilder.Validate(name + "-RunningAvg", expectedMean, delta);
            recordBuilder.Validate(name + "-RunningStdev", expectedStd, delta);
            recordBuilder.Validate(name + "-IntervalAvg", expectedMean, delta);
            recordBuilder.Validate(name + "-IntervalStdev", expectedStd, delta);
            recordBuilder.Validate(name + "-RunningMin", array1.Min(), delta);
            recordBuilder.Validate(name + "-IntervalMin", array1.Min(), delta);
            recordBuilder.Validate(name + "-RunningMax", array1.Max(), delta);
            recordBuilder.Validate(name + "-IntervalMax", array1.Max(), delta);

            recordBuilder.Reset();
            foreach (var entry in array2)
            {
                stat.Add(entry);
            }

            double expectedIntervalMean = array2.Sum() / 3;
            double expectedIntervalStd = Math.Sqrt(array2.Select(x => Math.Pow(x - expectedIntervalMean, 2)).Sum() / 2);
            double expectedIntervalMin = array2.Min();
            double expectedIntervalMax = array2.Max();
            double expectedRunningMean = (array1.Sum() + array2.Sum()) / 6;
            double expectedRunningStd =
                Math.Sqrt((array1.Select(x => Math.Pow(x - expectedRunningMean, 2)).Sum() +
                           array2.Select(x => Math.Pow(x - expectedRunningMean, 2)).Sum()) / 5);
            double expectedRunningMin = Math.Min(array1.Min(), array2.Min());
            double expectedRunningMax = Math.Max(array1.Max(), array2.Max());

            stat.TakeSnapshot(recordBuilder);
            recordBuilder.Validate(name + "-Num", (long)6);
            recordBuilder.Validate(name + "-RunningAvg", expectedRunningMean, delta);
            recordBuilder.Validate(name + "-RunningStdev", expectedRunningStd, delta);
            recordBuilder.Validate(name + "-IntervalAvg", expectedIntervalMean, delta);
            recordBuilder.Validate(name + "-IntervalStdev", expectedIntervalStd, delta);
            recordBuilder.Validate(name + "-RunningMin", expectedRunningMin, delta);
            recordBuilder.Validate(name + "-IntervalMin", expectedIntervalMin, delta);
            recordBuilder.Validate(name + "-RunningMax", expectedRunningMax, delta);
            recordBuilder.Validate(name + "-IntervalMax", expectedIntervalMax, delta);

            recordBuilder.Reset();
            stat.TakeSnapshot(recordBuilder);
            Assert.False(recordBuilder.LongKeyPresent(name + "-Num"), "Metric is not supposed to be recorded.");
            
            stat.TakeSnapshot(recordBuilder, false);
            Assert.False(recordBuilder.LongKeyPresent(name + "-Num"), "Metric is not supposed to be recorded.");

            stat.TakeSnapshot(recordBuilder, true);
            Assert.True(recordBuilder.LongKeyPresent(name + "-Num"), "Metric is supposed to be recorded.");
        }

        /// <summary>
        /// Tests various functions of <see cref="MetricsRegistry"/>
        /// </summary>
        [Fact]
        public void TestMetricsRegistry()
        {
            const string name = "regtest";
            const string desc = "regtestdesc";
            const string intCounterName = name + "-counterint";
            const string longCounterName = name + "-counterlong";
            const string intGaugeName = name + "-gaugeint";
            const string longGaugeName = name + "-gaugelong";
            const string statName1 = name + "-stat1";
            const string statName2 = name + "-stat2";
            const string statVal = name + "-statval";
            const string rateName = name + "-rate";
            const string tagName1 = name + "-tagName1";
            const string tagVal1 = name + "-tagVal1";
            const string tagVal2 = name + "-tagVal2";

            // const string tagName2 = name + "-tagName2";
            // const string tagVal2 = name + "-tagVal2";
            double[] array1 = new double[3];
            double[] array2 = new double[3];
            double[] array3 = new double[3];

            Random randGen = new Random();
            RecordBuilderForTests recordBuilder = new RecordBuilderForTests();

            array1 = array1.Select(x => randGen.NextDouble()).ToArray();
            array2 = array2.Select(x => randGen.NextDouble()).ToArray();
            array3 = array3.Select(x => randGen.NextDouble()).ToArray();

            MetricsRegistry registry = new MetricsRegistry(new MetricsInfoImpl(name, desc));
            Assert.Equal(registry.RegistryInfo.Name, name);
            Assert.Equal(registry.RegistryInfo.Description, desc);

            // Add different metrics to the registry.
            var intCounter = registry.GetNewCounter(new MetricsInfoImpl(intCounterName, desc), 5);
            var longCounter = registry.GetNewCounter(new MetricsInfoImpl(longCounterName, desc), (long)6);
            var intGauge = registry.GetNewGauge(new MetricsInfoImpl(intGaugeName, desc), 5);
            var longGauge = registry.GetNewGauge(new MetricsInfoImpl(longGaugeName, desc), (long)6);
            var stat1 = registry.GetNewStat(statName1, desc, statVal);
            var stat2 = registry.GetNewStat(statName2, desc, statVal, true);
            var rate = registry.GetNewRate(rateName, desc);
            registry.AddTag(new MetricsInfoImpl(tagName1, desc), tagVal1);

            // Change values of metrics.
            intCounter.Increment();
            longCounter.Increment();
            intGauge.Decrement(2);
            longGauge.Decrement(2);

            foreach (var entry in array1)
            {
                stat1.Add(entry);
            }

            foreach (var entry in array2)
            {
                stat2.Add(entry);
            }

            foreach (var entry in array3)
            {
                rate.Add(entry);
            }

            // Verify integer counter.
            var metric = registry.GetMetric(intCounterName);
            Assert.True(metric is MutableCounterInt);
            metric.TakeSnapshot(recordBuilder);
            recordBuilder.Validate(metric.Info.Name, 6);

            // Verify long counter.
            metric = registry.GetMetric(longCounterName);
            Assert.True(metric is MutableCounterLong);
            metric.TakeSnapshot(recordBuilder);
            recordBuilder.Validate(metric.Info.Name, (long)7);

            // Verify integer gauge.
            metric = registry.GetMetric(intGaugeName);
            Assert.True(metric is MutableGaugeInt);
            metric.TakeSnapshot(recordBuilder);
            recordBuilder.Validate(metric.Info.Name, 3);

            // Verify long gauge.
            metric = registry.GetMetric(longGaugeName);
            Assert.True(metric is MutableGaugeLong);
            metric.TakeSnapshot(recordBuilder);
            recordBuilder.Validate(metric.Info.Name, (long)4);

            // Verify mutable stat with non-extended stats.
            metric = registry.GetMetric(statName1);
            Assert.True(metric is MutableStat);
            metric.TakeSnapshot(recordBuilder);

            // Just checking one field. Others are already tested in other tests.
            recordBuilder.Validate(statName1 + "-IntervalAvg", array1.Sum() / 3, 1e-10);
            
            // Just making sure extended stat is not present.
            Assert.False(recordBuilder.DoubleKeyPresent(statName1 + "-IntervalStdev"));

            // Verify mutable stat with extended stats.
            metric = registry.GetMetric(statName2);
            Assert.True(metric is MutableStat);
            metric.TakeSnapshot(recordBuilder);

            // Just checking one field. Others are already tested in other tests.
            recordBuilder.Validate(statName2 + "-IntervalAvg", array2.Sum() / 3, 1e-10);

            // Just making sure extended stat is present.
            Assert.True(recordBuilder.DoubleKeyPresent(statName2 + "-IntervalStdev"));

            // Verify mutable rate.
            metric = registry.GetMetric(rateName);
            Assert.True(metric is MutableRate);
            metric.TakeSnapshot(recordBuilder);

            // Just checking one field. Others are already tested in other tests.
            recordBuilder.Validate(rateName + "-IntervalAvg", array3.Sum() / 3, 1e-10);

            // Just making sure extended stat is not present.
            Assert.False(recordBuilder.DoubleKeyPresent(rateName + "-IntervalStdev"));

            // Verify tags.
            var tag = registry.GetTag(tagName1);
            Assert.Equal(tagName1, tag.Name);

            // Verify that we can overwrite existing tag.
            registry.AddTag(new MetricsInfoImpl(tagName1, desc), tagVal2, true);
            registry.GetTag(tagName1);
            tag = registry.GetTag(tagName1);
            Assert.Equal(tagVal2, tag.Value);

            recordBuilder.Reset();

            // Verifies that Snapshot function works fine.
            registry.Snapshot(recordBuilder, true);
            recordBuilder.Validate(intCounterName, 6);
            recordBuilder.Validate(longCounterName, (long)7);
            recordBuilder.Validate(intGaugeName, 3);
            recordBuilder.Validate(longGaugeName, (long)4);
            recordBuilder.Validate(statName1 + "-IntervalAvg", array1.Sum() / 3, 1e-10);
            recordBuilder.Validate(statName2 + "-IntervalAvg", array2.Sum() / 3, 1e-10);
            recordBuilder.Validate(rateName + "-IntervalAvg", array3.Sum() / 3, 1e-10);
            recordBuilder.Validate(tagName1, tagVal2);

            recordBuilder.Reset();

            // Verifies that mutable metrics are not recorded but tags are.
            registry.Snapshot(recordBuilder, false);
            Assert.True(recordBuilder.MetricsEmpty());
            recordBuilder.Validate(tagName1, tagVal2);
        }

        /// <summary>
        /// Tests whether metrics can be properly injected.
        /// </summary>
        [Fact]
        public void TestMutableMetricsConfigurations()
        {
            string name = "testname";
            string desc = "tesdesc";
            string valueName = "testvalname";

            IConfiguration metricsInfoConfig =
                MetricsInfoConfiguration.ConfigurationModule
                    .Set(MetricsInfoConfiguration.Name, name)
                    .Set(MetricsInfoConfiguration.Desc, desc)
                    .Build();

            // Tests metrics info configuration
            IMetricsInfo metricsInfo = TangFactory.GetTang().NewInjector(metricsInfoConfig).GetInstance<IMetricsInfo>();
            Assert.Equal(name, metricsInfo.Name);
            Assert.Equal(desc, metricsInfo.Description);

            IConfiguration statsInfoConfig =
                StatsInfoConfiguration.ConfigurationModule
                    .Set(StatsInfoConfiguration.ValueName, valueName)
                    .Build();

            // Test Stats info configuration.
            IStatsInfo statsInfo =
                TangFactory.GetTang().NewInjector(metricsInfoConfig, statsInfoConfig).GetInstance<IStatsInfo>();
            Assert.Equal(name, statsInfo.Name);
            Assert.Equal(desc, statsInfo.Description);
            Assert.Equal(valueName, statsInfo.ValueName);

            RecordBuilderForTests builder = new RecordBuilderForTests();
            int initValue = 5;

            // Test int counter with non default value.
            MutableCounterInt intCounter =
                TangFactory.GetTang().NewInjector(
                    TangFactory.GetTang()
                        .NewConfigurationBuilder(metricsInfoConfig)
                        .BindNamedParameter(typeof(MetricsInitValueParameters.IntMetricInitValue),
                            initValue.ToString())
                        .Build()).GetInstance<MutableCounterInt>();
            Assert.NotNull(intCounter);
            intCounter.TakeSnapshot(builder);
            builder.Validate(name, initValue);
            builder.Reset();

            // Test int counter with default value.
            intCounter =
                TangFactory.GetTang().NewInjector(metricsInfoConfig).GetInstance<MutableCounterInt>();
            Assert.NotNull(intCounter);
            intCounter.TakeSnapshot(builder);
            builder.Validate(name, 0);
            builder.Reset();

            // Test long counter with non default value.
            MutableCounterLong longCounter =
                TangFactory.GetTang().NewInjector(
                    TangFactory.GetTang()
                        .NewConfigurationBuilder(metricsInfoConfig)
                        .BindNamedParameter(typeof(MetricsInitValueParameters.LongMetricInitValue),
                            initValue.ToString())
                        .Build()).GetInstance<MutableCounterLong>();
            Assert.NotNull(longCounter);
            longCounter.TakeSnapshot(builder);
            builder.Validate(name, (long)initValue);
            builder.Reset();

            // Test long counter with default value.
            longCounter =
                TangFactory.GetTang().NewInjector(metricsInfoConfig).GetInstance<MutableCounterLong>();
            Assert.NotNull(longCounter);
            longCounter.TakeSnapshot(builder);
            builder.Validate(name, (long)0);
            builder.Reset();

            // Test integer gauge with non default value.
            MutableGaugeInt intGauge =
                TangFactory.GetTang().NewInjector(
                    TangFactory.GetTang()
                        .NewConfigurationBuilder(metricsInfoConfig)
                        .BindNamedParameter(typeof(MetricsInitValueParameters.IntMetricInitValue),
                            initValue.ToString())
                        .Build()).GetInstance<MutableGaugeInt>();
            Assert.NotNull(intGauge);
            intGauge.TakeSnapshot(builder);
            builder.Validate(name, initValue);
            builder.Reset();

            // Test integer gauge with default value.
            intGauge =
                TangFactory.GetTang().NewInjector(metricsInfoConfig).GetInstance<MutableGaugeInt>();
            Assert.NotNull(intGauge);
            intGauge.TakeSnapshot(builder);
            builder.Validate(name, 0);
            builder.Reset();

            // Test long gauge with non default value.
            MutableGaugeLong longGauge =
                TangFactory.GetTang().NewInjector(
                    TangFactory.GetTang()
                        .NewConfigurationBuilder(metricsInfoConfig)
                        .BindNamedParameter(typeof(MetricsInitValueParameters.LongMetricInitValue),
                            initValue.ToString())
                        .Build()).GetInstance<MutableGaugeLong>();
            Assert.NotNull(longGauge);
            longGauge.TakeSnapshot(builder);
            builder.Validate(name, (long)initValue);
            builder.Reset();

            // Test long gauge with default value.
            longGauge =
                TangFactory.GetTang().NewInjector(metricsInfoConfig).GetInstance<MutableGaugeLong>();
            Assert.NotNull(longGauge);
            longGauge.TakeSnapshot(builder);
            builder.Validate(name, (long)0);
            builder.Reset();

            // Test Mutable stat with extended stats set to false.
            MutableStat stat = TangFactory.GetTang()
                .NewInjector(
                    TangFactory.GetTang()
                        .NewConfigurationBuilder(statsInfoConfig, metricsInfoConfig)
                        .BindNamedParameter(typeof(ExtendedStatsParameter), "false")
                        .Build())
                .GetInstance<MutableStat>();
            stat.TakeSnapshot(builder);
            builder.Validate(name + "-IntervalAvg", 0, 1e-10);

            // Verify that extended metrics were not recorded.
            Assert.False(builder.DoubleKeyPresent(name + "-RunningAvg"), "Extended stats should not be present");
            builder.Reset();

            // Test Mutable stat with extended stats set to true.
            stat = TangFactory.GetTang()
                .NewInjector(
                    TangFactory.GetTang()
                        .NewConfigurationBuilder(statsInfoConfig, metricsInfoConfig)
                        .BindNamedParameter(typeof(ExtendedStatsParameter), "true")
                        .Build())
                .GetInstance<MutableStat>();
            stat.TakeSnapshot(builder);
            builder.Validate(name + "-IntervalAvg", 0, 1e-10);

            // Verify that extended metrics were recorded.
            Assert.True(builder.DoubleKeyPresent(name + "-RunningAvg"), "Extended stats should be present");
            builder.Reset();

            // Test Mutable rate with extended stats set to false.
            MutableRate rate = TangFactory.GetTang()
                .NewInjector(
                    TangFactory.GetTang()
                        .NewConfigurationBuilder(metricsInfoConfig)
                        .BindNamedParameter(typeof(ExtendedStatsParameter), "false")
                        .Build())
                .GetInstance<MutableRate>();
            rate.TakeSnapshot(builder);
            builder.Validate(name + "-IntervalAvg", 0, 1e-10);

            // Verify that extended metrics were not recorded.
            Assert.False(builder.DoubleKeyPresent(name + "-RunningAvg"), "Extended stats should not be present");
            builder.Reset();

            // Test Mutable rate with extended stats set to true.
            rate = TangFactory.GetTang()
                .NewInjector(
                    TangFactory.GetTang()
                        .NewConfigurationBuilder(metricsInfoConfig)
                        .BindNamedParameter(typeof(ExtendedStatsParameter), "true")
                        .Build())
                .GetInstance<MutableRate>();
            rate.TakeSnapshot(builder);
            builder.Validate(name + "-IntervalAvg", 0, 1e-10);

            // Verify that extended metrics were recorded.
            Assert.True(builder.DoubleKeyPresent(name + "-RunningAvg"), "Extended stats should be present");
            builder.Reset();
        }

        /// <summary>
        /// Implementation of <see cref="IMetricsRecordBuilder"/> to test Snapshot functions of 
        /// different Mutable metrics.
        /// </summary>
        class RecordBuilderForTests : IMetricsRecordBuilder
        {
            private readonly Dictionary<string, int> _intMetricVals = new Dictionary<string, int>();
            private readonly Dictionary<string, long> _longMetricVals = new Dictionary<string, long>();
            private readonly Dictionary<string, double> _doubleMetricVals = new Dictionary<string, double>();
            private readonly Dictionary<string, string> _tagVals = new Dictionary<string, string>();

            public IMetricsRecordBuilder AddTag(IMetricsInfo info, string value)
            {
                throw new NotImplementedException();
            }

            public IMetricsRecordBuilder Add(MetricsTag tag)
            {
                _tagVals[tag.Name] = tag.Value;
                return this;
            }

            public IMetricsRecordBuilder Add(AbstractMetric metric)
            {
                throw new NotImplementedException();
            }

            public IMetricsRecordBuilder SetContext(string value)
            {
                throw new NotImplementedException();
            }

            public IMetricsRecordBuilder AddCounter(IMetricsInfo info, int value)
            {
                _intMetricVals[info.Name] = value;
                return this;
            }

            public IMetricsRecordBuilder AddCounter(IMetricsInfo info, long value)
            {
                _longMetricVals[info.Name] = value;
                return this;
            }

            public IMetricsRecordBuilder AddGauge(IMetricsInfo info, int value)
            {
                _intMetricVals[info.Name] = value;
                return this;
            }

            public IMetricsRecordBuilder AddGauge(IMetricsInfo info, long value)
            {
                _longMetricVals[info.Name] = value;
                return this;
            }

            public IMetricsRecordBuilder AddGauge(IMetricsInfo info, float value)
            {
                throw new NotImplementedException();
            }

            public IMetricsRecordBuilder AddGauge(IMetricsInfo info, double value)
            {
                _doubleMetricVals[info.Name] = value;
                return this;
            }

            public IMetricsCollector ParentCollector()
            {
                throw new NotImplementedException();
            }

            public IMetricsCollector EndRecord()
            {
                throw new NotImplementedException();
            }

            public void Validate(string name, int expected)
            {
                if (!_intMetricVals.ContainsKey(name))
                {
                    Assert.True(false, "Metric name not present");
                }
                Assert.Equal(expected, _intMetricVals[name]);
            }

            public void Validate(string name, long expected)
            {
                if (!_longMetricVals.ContainsKey(name))
                {
                    Assert.True(false, "Metric name not present");
                }
                Assert.Equal(expected, _longMetricVals[name]);
            }

            public void Validate(string name, double expected, double delta)
            {
                if (!_doubleMetricVals.ContainsKey(name))
                {
                    Assert.True(false, "Metric name not present");
                }
                Assert.True(Math.Abs(expected - _doubleMetricVals[name]) < delta);
            }

            public void Validate(string tagName, string expectedTagVal)
            {
                if (!_tagVals.ContainsKey(tagName))
                {
                    Assert.True(false, "Tag name not present");
                }
                Assert.Equal(expectedTagVal, _tagVals[tagName]);
            }

            public bool IntKeyPresent(string name)
            {
                return _intMetricVals.ContainsKey(name);
            }

            public bool LongKeyPresent(string name)
            {
                return _longMetricVals.ContainsKey(name);
            }

            public bool DoubleKeyPresent(string name)
            {
                return _doubleMetricVals.ContainsKey(name);
            }

            public bool MetricsEmpty()
            {
                return _intMetricVals.Count == 0 && _doubleMetricVals.Count == 0 && _longMetricVals.Count == 0;
            }

            public void Reset()
            {
                _intMetricVals.Clear();
                _longMetricVals.Clear();
                _doubleMetricVals.Clear();
                _tagVals.Clear();
            }
        }
    }
}