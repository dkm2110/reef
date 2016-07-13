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

namespace Org.Apache.REEF.Common.metrics.MutableMetricsLib
{
    /// <summary>
    /// Base class for gauge (arbitrarily varying).
    /// </summary>
    public abstract class MutableGauge<T> : MutableMetric
    {
        /// <summary>
        /// Cosntructor called by derived classes.
        /// </summary>
        /// <param name="info">Meta-data of the metric.</param>
        protected MutableGauge(IMetricsInfo info) : base(info)
        {
        }

        /// <summary>
        /// Increments the value of the metric.
        /// </summary>
        /// <param name="delta">Amount by which to increment.</param>
        public abstract void Increment(T delta);

        /// <summary>
        /// Decrements the value of the metric.
        /// </summary>
        /// <param name="delta">Amount by which to decrement.</param>
        public abstract void Decrement(T delta);
    }
}
