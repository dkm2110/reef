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

namespace Org.Apache.REEF.Wake.Remote
{
    /// <summary>
    /// General Exception class to be used to throw any exception in the 
    /// Wake Remote layer. Any errors caught in Transport layer, Remote Manager etc. 
    /// will throw exception of this kind.
    /// </summary>
    public class WakeRemoteException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Error message given by the user.</param>
        internal WakeRemoteException(string message)
            : base(string.Format("{0}\n{1}", ExceptionMessage(), message))
        {           
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">Inner exception responsible for error.</param>
        internal WakeRemoteException(string message, Exception innerException)
            : base(string.Format("{0}\n{1}", ExceptionMessage(), message), innerException)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="innerException">Inner exception responsible for error.</param>
        internal WakeRemoteException(Exception innerException)
            : base(ExceptionMessage(), innerException)
        {
        }

        private static string ExceptionMessage()
        {
            return "Error in Wake Remote Layer.";
        }
    }
}
