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

namespace Org.Apache.REEF.Wake.Remote.Impl
{
    /// <summary>
    /// Exception class used to indicate that error occured in StreamingTransportLayer.
    /// </summary>
    internal class StreamingTransportLayerException : WakeRemoteException
    {
        /// <summary>
        /// Constructor. Simply calls Wake exception constructor
        /// </summary>
        /// <param name="message">User error message.</param>
        /// <param name="innerException">Inner exception that caused the failure.</param>
        internal StreamingTransportLayerException(string message, Exception innerException)
            : base(string.Format("{0}\n{1}", MessageToAppend(), message), innerException)
        {
        }

        /// <summary>
        /// Constructor. Simply calls Wake Remote exception constructor
        /// </summary>
        /// <param name="innerException">Inner exception that caused the failure.</param>
        internal StreamingTransportLayerException(Exception innerException) 
            : base(innerException)
        {
        }

        private static string MessageToAppend()
        {
            return "Error in Streaming Transport Layer.";
        }
    }
}
