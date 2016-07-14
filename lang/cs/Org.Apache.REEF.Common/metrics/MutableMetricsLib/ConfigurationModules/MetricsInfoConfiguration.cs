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
using Org.Apache.REEF.Tang.Formats;
using Org.Apache.REEF.Tang.Util;

namespace Org.Apache.REEF.Common.metrics.MutableMetricsLib.ConfigurationModules
{
    /// <summary>
    /// Configuration Module for the <see cref="MetricsInfoImpl"/>
    /// </summary>
    public sealed class MetricsInfoConfiguration : ConfigurationModuleBuilder
    {
        /// <summary>
        /// The name of the metric.
        /// </summary>
        public static readonly OptionalParameter<string> Name = new OptionalParameter<string>();

        /// <summary>
        /// Description of the metric.
        /// </summary>
        public static readonly OptionalParameter<string> Desc = new OptionalParameter<string>();

        /// <summary>
        /// Set all parameters needed for injecting <see cref="MetricsInfoImpl"/>
        /// </summary>
        public static readonly ConfigurationModule ConfigurationModule = new MetricsInfoConfiguration()
            .BindNamedParameter(GenericType<NamedParameters.MetricsInfoParameters.MetricName>.Class,
                Name)
            .BindNamedParameter(GenericType<NamedParameters.MetricsInfoParameters.MetricDescription>.Class,
                Desc)
            .BindImplementation(GenericType<IMetricsInfo>.Class, GenericType<MetricsInfoImpl>.Class)
            .Build();
    }
}