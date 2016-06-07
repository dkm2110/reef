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

namespace Org.Apache.REEF.Network.NetworkService
{
    /// <summary>
    /// Exception class used to indicate that error occured in Streaming 
    /// NetworkService layer.
    /// </summary>
    internal sealed class StreamingNetworkServiceException : NetworkServiceException
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Error message given by the user.</param>
        internal StreamingNetworkServiceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">User error message.</param>
        /// <param name="innerException">Inner exception that caused the failure.</param>
        internal StreamingNetworkServiceException(string message, Exception innerException)
            : base(string.Format("{0}\n{1}", MessageToAppend(), message), innerException)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="innerException">Inner exception that caused the failure.</param>
        internal StreamingNetworkServiceException(Exception innerException)
            : base(MessageToAppend(), innerException)
        {
        }

        private static string MessageToAppend()
        {
            return "Error in StreamingNetworkService.";
        }
    }
}
