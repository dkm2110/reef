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
    public interface IMetricsSystem
    {
        /// <summary>
        /// (Re)Start the whole Metrics system.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the metrics system. The system can be started and stopped again.
        /// </summary>
        void Stop();

        /// <summary>
        /// Register the metrics source.
        /// </summary>
        /// <param name="name">Name of the source. Must be unique.</param>
        /// <param name="desc">Description of the source.</param>
        /// <param name="source">Metrics source to register.</param>
        /// <returns>Metrics source</returns>
        IMetricsSource RegisterSource(string name, string desc, IMetricsSource source);
        
        /// <summary>
        /// Register the metrics sink.
        /// </summary>
        /// <param name="name">Name of the sink. Must be unique.</param>
        /// <param name="desc">Description of the sink.</param>
        /// <param name="sink">Metrics sink to register.</param>
        /// <returns>Metrics source</returns>
        IMetricsSink RegisterSink(string name, string desc, IMetricsSink sink);

        /// <summary>
        /// Unregister the metrics source.
        /// </summary>
        /// <param name="name">Name of the source.</param>
        void UnRegisterSource(string name);

        /// <summary>
        /// Gets the metrics source.
        /// </summary>
        /// <param name="name">Name of the metrics source.</param>
        /// <returns>Metrics source.</returns>
        IMetricsSource GetSource(string name);

        /// <summary>
        /// Requests an immediate publish of all metrics from sources to sinks.
        /// Best effort will be done to push metrics from source to sink synchronously 
        /// before returning. However, if it cannot be done in reasonable time 
        /// it is ok to return to the caller before everything is done. 
        /// </summary>
        void PublishMetricsNow();

        /// <summary>
        /// Completely shuts down the metrics system.
        /// </summary>
        /// <returns>True if proper shutdown happened.</returns>
        bool ShutDown();
    }
}