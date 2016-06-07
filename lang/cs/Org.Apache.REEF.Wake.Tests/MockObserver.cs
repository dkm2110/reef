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

namespace Org.Apache.REEF.Wake.Tests
{
    /// <summary>
    /// Mock observer class used in various tests. Takes account of 
    /// how many times each function is called.
    /// </summary>
    /// <typeparam name="T">Type of observer</typeparam>
    internal sealed class MockObserver<T> : IObserver<T>
    {
        internal MockObserver()
        {
            OnNextCounter = 0;
            OnErrorCounter = 0;
            OnCompletedCounter = 0;
        }

        internal int OnNextCounter { get; private set; }

        internal int OnErrorCounter { get; private set; }

        internal int OnCompletedCounter { get; private set; }

        internal Exception ThrownException { get; private set; }

        public void OnNext(T value)
        {
            OnNextCounter++;
        }

        public void OnError(Exception error)
        {
            ThrownException = error;
            OnErrorCounter++;
        }

        public void OnCompleted()
        {
            OnCompletedCounter++;
        }
    }
}
