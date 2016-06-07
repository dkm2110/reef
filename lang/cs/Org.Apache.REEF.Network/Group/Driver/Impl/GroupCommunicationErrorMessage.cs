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

namespace Org.Apache.REEF.Network.Group.Driver.Impl
{
    /// <summary>
    /// Error message created to be part of Group Communication Message. 
    /// This class is used to create a special Group Communication Message 
    /// to indicate errors.
    /// </summary>
    internal sealed class GroupCommunicationErrorMessage
    {
        /// <summary>
        /// Create new CommunicationGroupMessage.
        /// </summary>
        /// <param name="exception">Underlying exception that caused the error.</param>
        internal GroupCommunicationErrorMessage(Exception exception)
        {
            UnderlyingException = exception as GroupCommunicationException ?? new GroupCommunicationException(exception);
        }

        /// <summary>
        /// Underlying group communication exception.
        /// </summary>
        internal GroupCommunicationException UnderlyingException { get; private set; }
    }
}