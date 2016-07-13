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
using Org.Apache.REEF.Common.metrics.Api;

namespace Org.Apache.REEF.Common.metrics.MutableMetricsLib
{
    /// <summary>
    /// Base mutable metrics class. All mutable metrics should derive from this class.
    /// </summary>
    public abstract class MutableMetric
    {
        private volatile bool _changed = true;

        /// <summary>
        /// Constructor called by derived classes.
        /// </summary>
        /// <param name="info">meta-data of the metric.</param>
        protected MutableMetric(IMetricsInfo info)
        {
            if (info == null)
            {
                throw new MetricsException("Metric info cannot be null", new ArgumentNullException("info"));
            }
            Info = info;
        }

        /// <summary>
        /// Gets the snapshot of the metric.
        /// </summary>
        /// <param name="recordBuilder">The metrics record builder.</param>
        /// <param name="all">If true, record even unchanged metrics.</param>
        public abstract void TakeSnapshot(IMetricsRecordBuilder recordBuilder, bool all);

        /// <summary>
        /// Meta-data of the metric.
        /// </summary>
        public IMetricsInfo Info { get; protected set; }
       
        /// <summary>
        /// Gets the snapshot of the metric if it changed.
        /// </summary>
        /// <param name="recordBuilder">The metrics record builder.</param>
        public void TakeSnapshot(IMetricsRecordBuilder recordBuilder)
        {
            TakeSnapshot(recordBuilder, false);
        }

        /// <summary>
        /// Sets the changed flag. Called in derived classes when metric values are changed.
        /// </summary>
        protected void SetChanged()
        {
            _changed = true;
        }

        /// <summary>
        /// Clears the changed flag. Called by snapshot operations after recording latest values.
        /// </summary>
        protected void ClearChanged()
        {
            _changed = false;
        }

        /// <summary>
        /// True if metric value changed after taking a snapshot, false otherwise.
        /// </summary>
        protected bool Changed
        {
            get { return _changed; }
        }
    }
}
