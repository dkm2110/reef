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
using Org.Apache.REEF.Tang.Annotations;

namespace Org.Apache.REEF.Common.metrics.Api
{
    /// <summary>
    /// Default implementation of <see cref="ITypeConveter"/>.
    /// Multiplies float and double by 10^precision and then truncates.
    /// </summary>
    public class DefaultTypeConverter : ITypeConveter
    {
        private readonly int _precision;

        [Inject]
        private DefaultTypeConverter([Parameter(typeof(TypeConverterPrecisionParameter))] int precision)
        {
            _precision = precision;
        }

        /// <summary>
        /// Converts double to long.
        /// </summary>
        /// <param name="value">double value</param>
        /// <returns></returns>
        public long FromDouble(double value)
        {
            return Convert.ToInt64(value * Math.Pow(10, _precision));
        }

        /// <summary>
        /// Converts float to long.
        /// </summary>
        /// <param name="value">float value</param>
        /// <returns></returns>
        public long FromFloat(float value)
        {
            return Convert.ToInt64(value * Math.Pow(10, _precision));
        }
    }
}
