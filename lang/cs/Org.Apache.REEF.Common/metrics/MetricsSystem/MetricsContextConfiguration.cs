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

using System;
using Org.Apache.REEF.Common.Context;
using Org.Apache.REEF.Common.Events;
using Org.Apache.REEF.Common.Metrics.Api;
using Org.Apache.REEF.Common.Metrics.MetricsSystem.Parameters;
using Org.Apache.REEF.Tang.Formats;
using Org.Apache.REEF.Tang.Util;
using Org.Apache.REEF.Utilities.Attributes;

namespace Org.Apache.REEF.Common.Metrics.MetricsSystem
{
    /// <summary>
    /// This configuration module defines configuration for <see cref="MetricsSystemContextStartHandler"/>.
    /// Metrics system configuration via. <see cref="MetricsSystemConfiguration"/> and source 
    /// configuration still needs to be merged with configuration given by this module.
    /// </summary>
    [Unstable("0.16", "Contract may change.")]
    public sealed class MetricsContextConfiguration : ConfigurationModuleBuilder
    {
        /// <summary>
        /// Set of sinks we want to add to metrics system before it starts.
        /// </summary>
        public static readonly OptionalImpl<IObserver<IMetricsRecord>> Sink =
            new OptionalImpl<IObserver<IMetricsRecord>>();

        /// <summary>
        /// Name of the source to be added to the metrics system by default.
        /// </summary>
        public static readonly OptionalParameter<string> SourceName = new OptionalParameter<string>();

        /// <summary>
        /// Description of the source to be added to the metrics system by default.
        /// </summary>
        public static readonly OptionalParameter<string> SourceDescription = new OptionalParameter<string>();

        /// <summary>
        /// Configuration module for the context.
        /// </summary>
        public static ConfigurationModule ConfigurationModule
        {
            get
            {
                return new MetricsContextConfiguration()
                    .BindSetEntry(GenericType<SinkSetNameInStartHandler>.Class, Sink)
                    .BindSetEntry<ContextConfigurationOptions.StartHandlers, MetricsSystemContextStartHandler, IObserver<IContextStart>>(
                        GenericType<ContextConfigurationOptions.StartHandlers>.Class,
                        GenericType<MetricsSystemContextStartHandler>.Class)
                    .BindNamedParameter(GenericType<SourceNameInStartHandler>.Class, SourceName)
                    .BindNamedParameter(GenericType<SourceDescriptionInStartHandler>.Class, SourceDescription)
                    .Build();
            }
        }
    }
}