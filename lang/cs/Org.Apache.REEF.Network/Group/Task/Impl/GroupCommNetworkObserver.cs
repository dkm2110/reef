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
using System.Collections.Generic;
using System.Linq;
using Org.Apache.REEF.Network.Group.Driver.Impl;
using Org.Apache.REEF.Network.NetworkService;
using Org.Apache.REEF.Tang.Annotations;
using Org.Apache.REEF.Utilities.Logging;
using Org.Apache.REEF.Wake.Remote;

namespace Org.Apache.REEF.Network.Group.Task.Impl
{
    /// <summary>
    /// Handles all incoming messages for this Task.
    /// Writable version
    /// </summary>
    internal sealed class GroupCommNetworkObserver : IGroupCommNetworkObserver
    {
        private static readonly Logger LOGGER = Logger.GetLogger(typeof(GroupCommNetworkObserver));

        private readonly Dictionary<string, IObserver<GeneralGroupCommunicationMessage>> _commGroupHandlers;

        /// <summary>
        /// Creates a new GroupCommNetworkObserver.
        /// </summary>
        [Inject]
        private GroupCommNetworkObserver()
        {
            _commGroupHandlers = new Dictionary<string, IObserver<GeneralGroupCommunicationMessage>>();
        }

        /// <summary>
        /// Handles the incoming WritableNsMessage for this Task.
        /// Delegates the GeneralGroupCommunicationMessage to the correct 
        /// WritableCommunicationGroupNetworkObserver.
        /// </summary>
        /// <param name="nsMessage"></param>
        public void OnNext(NsMessage<GeneralGroupCommunicationMessage> nsMessage)
        {
            // The exceptions being thrown in OnNext are indicative of bugs in REEF 
            // or wrong user code like codecs. It is not a good practice to expect 
            // to handle exceptions in OnNext calls.
            // https://msdn.microsoft.com/en-us/library/ff519622(v=vs.110).aspx
            // So there is no need to propagate these exceptions up to group 
            // communicaton operator calls.
            if (nsMessage == null)
            {
                throw new GroupCommunicationException(new ArgumentNullException("nsMessage"));
            }

            IObserver<GeneralGroupCommunicationMessage> observer = null;
            GeneralGroupCommunicationMessage gcm = null;
            try
            {
                gcm = nsMessage.Data.First();
                observer = _commGroupHandlers[gcm.GroupName];
            }
            catch (InvalidOperationException e)
            {
                LOGGER.Log(Level.Error, "Group Communication Network Handler received message with no data");
                throw new GroupCommunicationException(e);
            }
            catch (KeyNotFoundException e)
            {
                LOGGER.Log(Level.Error, "Group Communication Network Handler received message for nonexistant group");
                throw new GroupCommunicationException(e);
            }

            observer.OnNext(gcm);
        }

        /// <summary>
        /// Registers the network handler for the given CommunicationGroup.
        /// When messages are sent to the specified group name, the given handler
        /// will be invoked with that message.
        /// </summary>
        /// <param name="groupName">The group name for the network handler</param>
        /// <param name="commGroupHandler">The network handler to invoke when
        /// messages are sent to the given group.</param>
        public void Register(string groupName, IObserver<GeneralGroupCommunicationMessage> commGroupHandler)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                throw new GroupCommunicationException(new ArgumentNullException("groupName"));
            }
            if (commGroupHandler == null)
            {
                throw new GroupCommunicationException(new ArgumentNullException("commGroupHandler"));
            }

            _commGroupHandlers[groupName] = commGroupHandler;
        }

        /// <summary>
        /// Specifies what to do if error is received. In this case notify 
        /// to all the obsevers.
        /// </summary>
        /// <param name="error">The error message</param>
        public void OnError(Exception error)
        {
            var exception = error;

            if (!(error is GroupCommunicationException))
            {
                if (!(error is NetworkServiceException || error is WakeRemoteException))
                {
                    LOGGER.Log(Level.Info,
                        "Exception should have been of type NetworkServiceException or WakeRemoteException. Wrapping it with GroupCommunicationException.");
                }
                exception = new GroupCommunicationException(error);
            }

            foreach (var handler in _commGroupHandlers)
            {
                handler.Value.OnError(exception);
            }
        }

        public void OnCompleted()
        {
        }
    }
}
