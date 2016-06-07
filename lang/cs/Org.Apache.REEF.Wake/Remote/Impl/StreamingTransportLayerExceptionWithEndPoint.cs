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
using System.Net;

namespace Org.Apache.REEF.Wake.Remote.Impl
{
    /// <summary>
    /// Exception class used to indicate that error occured in StreamingTransportLayer along with
    /// the related remote endpoint. This class is needed to pass remote endpoint to ObserverContainer 
    /// so that it can call OnError of appropriate container.
    /// </summary>
    internal sealed class StreamingTransportLayerExceptionWithEndPoint : StreamingTransportLayerException
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The error message given by the user.</param>
        /// <param name="innerException">The inner exception that is the cause of error.</param>
        /// <param name="remoteEndPoint">The remote end point related to the error.</param>
        public StreamingTransportLayerExceptionWithEndPoint(string message,
            Exception innerException,
            IPEndPoint remoteEndPoint) : base(message, innerException)
        {
            RemoteEndPoint = remoteEndPoint;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="innerException">The inner exception that is the cause of error.</param>
        /// <param name="remoteEndPoint">The remote end point related to the error.</param>       
        public StreamingTransportLayerExceptionWithEndPoint(Exception innerException,
            IPEndPoint remoteEndPoint)
            : base(innerException)
        {
            RemoteEndPoint = remoteEndPoint;
        }

        /// <summary>
        /// Remote end point associated with the exception.
        /// </summary>
        internal IPEndPoint RemoteEndPoint { get; private set; }
    }
}
