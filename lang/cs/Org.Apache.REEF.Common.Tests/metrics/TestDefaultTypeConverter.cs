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

using Org.Apache.REEF.Tang.Implementations.Tang;
using Org.Apache.REEF.Common.metrics.Api;
using Xunit;

namespace Org.Apache.REEF.Common.Tests.metrics
{
    public sealed class TestDefaultTypeConverter
    {
        /// <summary>
        /// Tests <see cref="DefaultTypeConverter"/>
        /// </summary>
        [Fact]
        public void TestDefaultTypeConverterClass()
        {
            const float fValue = 12.13f;
            const double dValue = 12.13;
            const long longValue = 1213000;

            ITypeConveter converter = TangFactory.GetTang().NewInjector().GetInstance<DefaultTypeConverter>();

            Assert.Equal(converter.FromDouble(dValue), longValue);
            Assert.Equal(converter.FromFloat(fValue), longValue);
        }
    }
}
