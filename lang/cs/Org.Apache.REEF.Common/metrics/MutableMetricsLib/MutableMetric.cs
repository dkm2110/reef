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
using Org.Apache.REEF.Common.metrics.Api;

namespace Org.Apache.REEF.Common.metrics.MutableMetricsLib
{
    public abstract class MutableMetric
    {
        protected MutableMetric(IMetricsInfo info)
        {
            if (info == null)
            {
                throw new MetricsException("Metric info cannot be null", new ArgumentNullException("info"));
            }
            Info = info;
        }

        private volatile bool _changed = true;

        public abstract void TakeSnapshot(IMetricsRecordBuilder recordBuilder, bool all);

        public IMetricsInfo Info { get; protected set; }

        public void TakeSnapshot(IMetricsRecordBuilder recordBuilder)
        {
            TakeSnapshot(recordBuilder, false);
        }

        protected void SetChanged()
        {
            _changed = true;
        }

        protected void ClearChanged()
        {
            _changed = false;
        }

        protected bool Changed
        {
            get { return _changed; }
        }
    }
}
