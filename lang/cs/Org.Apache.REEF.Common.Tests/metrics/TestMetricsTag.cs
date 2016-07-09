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
using Xunit;

namespace Org.Apache.REEF.Common.Tests.metrics
{
    public sealed class TestMetricsTag
    {
        /// <summary>
        /// Tests different functions of <see cref="MetricsTag"/>
        /// </summary>
        [Fact]
        public void TestMetricsTagClass()
        {
            const string name = "tagtest";
            const string otherName = "othertagtest";
            const string desc = "tagtestdesc";
            const string otherDesc = "othertagtestdesc";
            const string tagValue = "tagvalue";
            const string otherTagValue = "othertagvalue";

            MetricsInfoTestClass info = new MetricsInfoTestClass(name, desc);
            MetricsTag tag = new MetricsTag(info, tagValue);

            Assert.Equal(name, tag.Name);
            Assert.Equal(desc, tag.Description);
            Assert.Equal(tagValue, tag.Value);

            MetricsTag sameTag = new MetricsTag(info, tagValue);
            Assert.True(tag.Equals(sameTag));

            MetricsTag otherTag = new MetricsTag(info, otherTagValue);
            Assert.False(tag.Equals(otherTag));

            otherTag = new MetricsTag(new MetricsInfoTestClass(otherName, desc), tagValue);
            Assert.False(tag.Equals(otherTag));

            otherTag = new MetricsTag(new MetricsInfoTestClass(name, otherDesc), otherTagValue);
            Assert.False(tag.Equals(otherTag));

            string expectedToString = "Tag Information: " + "Name: " + info.Name + ", Description: " + info.Description +
                                      ", Tag Value: " + tagValue;
            Assert.Equal(expectedToString, tag.ToString());
        }

        private class MetricsInfoTestClass : IMetricsInfo
        {
            public MetricsInfoTestClass(string name, string desc)
            {
                Name = name;
                Description = desc;
            }

            public string Name { get; private set; }
            
            public string Description { get; private set; }

            public override string ToString()
            {
                return "Name: " + Name + ", Description: " + Description;
            }

            public override bool Equals(object obj)
            {
                var infoObj = obj as MetricsInfoTestClass;

                if (infoObj != null)
                {
                    return infoObj.Name.Equals(Name) && infoObj.Description.Equals(Description);
                }
                
                return false;
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }
    }
}
