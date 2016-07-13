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
    /// Implementation of <see cref="IStatsInfo"/>
    /// </summary>
    public sealed class StatsInfoImpl : IStatsInfo
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="info">Metadata of the stat.</param>
        /// <param name="valueName">Specific name of the value that this stat represents, e.g."Time"</param>
        [Inject]
        public StatsInfoImpl(IMetricsInfo info, [Parameter(typeof(StatValueNameParameter))] string valueName)
        {
            Name = info.Name;
            Description = info.Description;
            ValueName = valueName;
        }

        /// <summary>
        /// Name of the metric.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Description of the metric.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Name of the value that this stat represents.
        /// </summary>
        public string ValueName { get; private set; }
    }
}
