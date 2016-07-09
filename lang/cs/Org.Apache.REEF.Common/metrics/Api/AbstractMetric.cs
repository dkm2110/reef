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

namespace Org.Apache.REEF.Common.metrics.Api
{
    /// <summary>
    /// Abstract immutable metric class. All metrics put in the record by the MetricsSource are 
    /// kept as <see cref="IEnumerable{T}" />.
    /// </summary>
    public abstract class AbstractMetric : IMetricsInfo
    {
        private readonly IMetricsInfo _info;

        private readonly long _value;

        /// <summary>
        /// Protected constructor of abstract metric.
        /// </summary>
        /// <param name="info">Meta-data for the metric</param>
        /// <param name="value">The value of the metric. It is assumed that user 
        /// or application has converted float/double values to long via some logic.</param>
        protected AbstractMetric(IMetricsInfo info, long value)
        {
            _info = info;
            _value = value;
        }

        /// <summary>
        /// Name of the metric.
        /// </summary>
        public string Name
        {
            get { return _info.Name; }
        }

        /// <summary>
        /// Description of the metric.
        /// </summary>
        public string Description
        {
            get { return _info.Description; }
        }

        /// <summary>
        /// Meta-data of the metric.
        /// </summary>
        protected IMetricsInfo Info
        {
            get { return _info; }
        }

        /// <summary>
        /// Value of the metric. Always stored as long. Immutable metrics of 
        /// type integers, float, double, bool etc. are all converted to long before storing
        /// them as immutable metrics. For double and float we can multiply by 10^n before 
        /// converting to immutable Abstract metric. Exact logic of this conversion is left 
        /// to user/application implementing <see cref="IMetricsSource"/>.
        /// </summary>
        public long Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Type of metric - counter or gauge. Filled in by exact 
        /// metric type.
        /// </summary>
        public abstract MetricType TypeOfMetric { get; }

        /// <summary>
        /// Accepts a visitor interface
        /// </summary>
        /// <param name="visitor">Metrics visitor interface.</param>
        public abstract void Visit(IMetricsVisitor visitor);

        /// <summary>
        /// String representation of a metric for display.
        /// </summary>
        /// <returns>The string representation of the metric.</returns>
        public override string ToString()
        {
            return "Metric Information: " + _info.ToString() + ", Metric Value: " + Value;
        }

        /// <summary>
        /// Checks whether two metrics are equal. Relies on Equals 
        /// function of <see cref="IMetricsInfo"/> implementations.
        /// </summary>
        /// <param name="obj">Object to compare against.</param>
        /// <returns>True if both represent the same metric.</returns>
        public override bool Equals(object obj)
        {
            var otherMetric = obj as AbstractMetric;
            if (otherMetric != null)
            {
                if (otherMetric.Info.Equals(_info) && otherMetric.Value == Value)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Return hash code of the metric object. Simply uses the hash of ToString() method.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            var hashCode = this.ToString().GetHashCode();
            return hashCode;
        }
    }
}
