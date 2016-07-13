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

using Org.Apache.REEF.Common.metrics.Api;
using Org.Apache.REEF.Common.metrics.MutableMetricsLib.NamedParameters;
using Org.Apache.REEF.Tang.Annotations;

namespace Org.Apache.REEF.Common.metrics.MutableMetricsLib
{
    public sealed class MutableGaugeLong : MutableGauge<long>
    {
        private long _value;
        private readonly object _lock = new object();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="info">Metadata of the metric.</param>
        /// <param name="initValue">Initial value of the metric.</param>
        [Inject]
        public MutableGaugeLong(IMetricsInfo info,
            [Parameter(typeof(MetricsInitValueParameters.LongMetricInitValue))] long initValue) : base(info)
        {
            _value = initValue;
        }

        /// <summary>
        /// Increments the gauge value by delta.
        /// </summary>
        /// <param name="delta">Increment by which to update.</param>
        public override void Increment(long delta)
        {
            lock (_lock)
            {
                _value += delta;
                SetChanged();
            }
        }

        /// <summary>
        /// Decrements the gauge value by delta.
        /// </summary>
        /// <param name="delta">Increment by which to update.</param>
        public override void Decrement(long delta)
        {
            lock (_lock)
            {
                _value -= delta;
                SetChanged();
            }
        }

        /// <summary>
        /// Sets the value of the gauge.
        /// </summary>
        /// <param name="value">Value to set.</param>
        public void Set(long value)
        {
            lock (_lock)
            {
                _value = value;
                SetChanged();
            }
        }

        /// <summary>
        /// Gets the snapshot of the metric.
        /// </summary>
        /// <param name="recordBuilder">The metrics record builder.</param>
        /// <param name="all">If true, record even unchanged metrics.</param>
        public override void TakeSnapshot(IMetricsRecordBuilder recordBuilder, bool all)
        {
            lock (_lock)
            {
                if (all || Changed)
                {
                    recordBuilder.AddGauge(Info, _value);
                    ClearChanged();
                }
            }
        }
    }
}
