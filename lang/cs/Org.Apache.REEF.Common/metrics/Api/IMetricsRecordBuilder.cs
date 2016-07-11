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

namespace Org.Apache.REEF.Common.metrics.Api
{
    /// <summary>
    /// Builder used to build a metrics record. Used by <see cref="IMetricsCollector"/> to 
    /// add a record, typically in <see cref="IMetricsSource"/>.
    /// </summary>
    public interface IMetricsRecordBuilder
    {
        /// <summary>
        /// Adds metrics tag to the record
        /// </summary>
        /// <param name="info">Meta data for the tag.</param>
        /// <param name="value">Value of the tag.</param>
        /// <returns>Self to add more metrics/tags.</returns>
        IMetricsRecordBuilder AddTag(IMetricsInfo info, string value);

        /// <summary>
        /// Adds an immutable metrics tag object. Avoids making a copy.
        /// </summary>
        /// <param name="tag">A pre-made tags object.</param>
        /// <returns>Self to add more metrics/tags.</returns>
        IMetricsRecordBuilder Add(MetricsTag tag);

        /// <summary>
        /// Adds an immutable metric to the record. Saves making a new metric object.
        /// </summary>
        /// <param name="metric">A pre-made metric object.</param>
        /// <returns>Self to add more metrics/tags.</returns>
        IMetricsRecordBuilder Add(AbstractMetric metric);

        /// <summary>
        /// Sets the special context tag of the record.
        /// </summary>
        /// <param name="value">Value of the context</param>
        /// <returns>Self to add more metrics/tags.</returns>
        IMetricsRecordBuilder SetContext(string value);

        /// <summary>
        /// Adds integer counter metric
        /// </summary>
        /// <param name="info">Meta data of the metric</param>
        /// <param name="value">Value of the metric</param>
        /// <returns></returns>
        IMetricsRecordBuilder AddCounter(IMetricsInfo info, int value);

        /// <summary>
        /// Adds long counter metric
        /// </summary>
        /// <param name="info">Meta data of the metric</param>
        /// <param name="value">Value of the metric</param>
        /// <returns></returns>
        IMetricsRecordBuilder AddCounter(IMetricsInfo info, long value);

        /// <summary>
        /// Adds integer gauge metric
        /// </summary>
        /// <param name="info">Meta data of the metric</param>
        /// <param name="value">Value of the metric</param>
        /// <returns></returns>
        IMetricsRecordBuilder AddGauge(IMetricsInfo info, int value);

        /// <summary>
        /// Adds long gauge metric
        /// </summary>
        /// <param name="info">Meta data of the metric</param>
        /// <param name="value">Value of the metric</param>
        /// <returns></returns>
        IMetricsRecordBuilder AddGauge(IMetricsInfo info, long value);

        /// <summary>
        /// Adds float gauge metric
        /// </summary>
        /// <param name="info">Meta data of the metric</param>
        /// <param name="value">Value of the metric</param>
        /// <returns></returns>
        IMetricsRecordBuilder AddGauge(IMetricsInfo info, float value);

        /// <summary>
        /// Adds double gauge metric
        /// </summary>
        /// <param name="info">Meta data of the metric</param>
        /// <param name="value">Value of the metric</param>
        /// <returns></returns>
        IMetricsRecordBuilder AddGauge(IMetricsInfo info, double value);

        /// <summary>
        /// Returns the parent <see cref="IMetricsCollector"/> object.
        /// </summary>
        /// <returns>Parent <see cref="IMetricsCollector"/> object</returns>
        IMetricsCollector ParentCollector();

        /// <summary>
        /// Finalizes the record and enables adding multiple records in one line.
        /// </summary>
        /// <returns>Parent <see cref="IMetricsCollector"/> object</returns>
        IMetricsCollector EndRecord();
    }
}