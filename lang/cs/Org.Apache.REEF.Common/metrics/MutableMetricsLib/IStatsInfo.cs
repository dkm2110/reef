﻿// Licensed to the Apache Software Foundation (ASF) under one
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
    /// Extension of <see cref="IMetricsInfo"/> interface for <see cref="MutableStat"/>
    /// </summary>
    public interface IStatsInfo : IMetricsInfo
    {
        /// <summary>
        /// Specific name of the value that this stat represents. For example, "Time".
        /// Mainly used for constructing name and description of different stats like:
        /// mean, variance, min, max.
        /// </summary>
        string ValueName { get; }
    }
}
