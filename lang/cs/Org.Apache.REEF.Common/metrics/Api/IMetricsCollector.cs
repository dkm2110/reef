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

namespace Org.Apache.REEF.Common.metrics.Api
{
    /// <summary>
    /// Interface for collecting the metrics. This interface is passed to 
    /// the <see cref="IMetricsSource"/> to add and fill in the records.
    /// </summary>
    public interface IMetricsCollector
    {
        /// <summary>
        /// Adds a metric record by name.
        /// </summary>
        /// <param name="name">Name of the record.</param>
        /// <returns>Record builder for the record.</returns>
        IMetricsRecordBuilder AddRecord(string name);

        /// <summary>
        /// Adds a metric record by meta-data info.
        /// </summary>
        /// <param name="info">Meta-data info of the record.</param>
        /// <returns>Record builder for the record.</returns>
        IMetricsRecordBuilder AddRecord(IMetricsInfo info);
    }
}
