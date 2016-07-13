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
    /// <summary>
    /// Mutable long counter.
    /// </summary>
    public sealed class MutableCounterLong : MutableCounter
    {
        private long _value;
        private readonly object _lock = new object();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="info">Metadata of the metric.</param>
        /// <param name="initValue">Initial value of the metric.</param>
        [Inject]
        public MutableCounterLong(IMetricsInfo info,
            [Parameter(typeof(MetricsInitValueParameters.LongMetricInitValue))] long initValue) : base(info)
        {
            _value = initValue;
        }

        /// <summary>
        /// Increments the counter value by 1.
        /// </summary>
        public override void Increment()
        {
            Increment(1);
        }

        /// <summary>
        /// Increments the counter value by delta.
        /// </summary>
        /// <param name="delta">The amount by which to increment.</param>
        public void Increment(long delta)
        {
            lock (_lock)
            {
                _value += delta;
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
                    recordBuilder.AddCounter(Info, _value);
                    ClearChanged();
                }
            }
        }
    }
}
