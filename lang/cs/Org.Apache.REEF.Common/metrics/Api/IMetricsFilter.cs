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
    /// Metrics filter interface used to filter metrics at different levels - 
    /// source and sink names, record, individual tag, individual metric.
    /// </summary>
    public interface IMetricsFilter
    {
        /// <summary>
        /// Whether to accept the name (can be from metric, source, sink, record etc.)
        /// </summary>
        /// <param name="name">Name to filter on.</param>
        /// <returns>True if accepted, false otherwise.</returns>
        bool AcceptsName(string name);

        /// <summary>
        /// Whether to accept the tag.
        /// </summary>
        /// <param name="tag">Tag to filter on.</param>
        /// <returns>True if accepted, false otherwise.</returns>
        bool AcceptsTag(MetricsTag tag);

        /// <summary>
        /// Whether to accept a group of tags.
        /// </summary>
        /// <param name="tags">Tags to filter on.</param>
        /// <returns>True if accepted, false otherwise.</returns>
        bool AcceptsTags(IEnumerable<MetricsTag> tags);

        /// <summary>
        /// Whether to accept the record.
        /// </summary>
        /// <param name="record">Record to filter on.</param>
        /// <returns>True if accepted, false otherwise.</returns>
        bool AcceptsRecord(IMetricsRecord record);
    }
}
