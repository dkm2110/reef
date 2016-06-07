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
using Org.Apache.REEF.Network.Group.Driver.Impl;
using Org.Apache.REEF.Network.NetworkService;
using Org.Apache.REEF.Tang.Annotations;
using Org.Apache.REEF.Utilities.Diagnostics;
using Org.Apache.REEF.Utilities.Logging;
using Org.Apache.REEF.Wake.Remote;

namespace Org.Apache.REEF.Network.Group.Task.Impl
{
    /// <summary>
    /// Handles incoming messages sent to this Communication Group.
    /// Writable version
    /// </summary>
    internal sealed class CommunicationGroupNetworkObserver : ICommunicationGroupNetworkObserver
    {
        private static readonly Logger LOGGER = Logger.GetLogger(typeof(CommunicationGroupNetworkObserver));
        private readonly Dictionary<string, IObserver<GeneralGroupCommunicationMessage>> _handlers;

        /// <summary>
        /// Creates a new CommunicationGroupNetworkObserver.
        /// </summary>
        [Inject]
        private CommunicationGroupNetworkObserver()
        {
            _handlers = new Dictionary<string, IObserver<GeneralGroupCommunicationMessage>>();
        }

        /// <summary>
        /// Registers the handler with the CommunicationGroupNetworkObserver.
        /// Messages that are to be sent to the operator specified by operatorName
        /// are handled by the given observer.
        /// </summary>
        /// <param name="operatorName">The name of the operator whose handler
        /// will be invoked</param>
        /// <param name="observer">The writable handler to invoke when messages are sent
        /// to the operator specified by operatorName</param>
        void ICommunicationGroupNetworkObserver.Register(string operatorName, IObserver<GeneralGroupCommunicationMessage> observer)
        {
            if (string.IsNullOrEmpty(operatorName))
            {
                throw new GroupCommunicationException(new ArgumentNullException("operatorName"));
            }
            if (observer == null)
            {
                throw new GroupCommunicationException(new ArgumentNullException("observer"));
            }

            _handlers[operatorName] = observer;
        }

        /// <summary>
        /// Handles the incoming GeneralGroupCommunicationMessage sent to this Communication Group.
        /// Looks for the operator that the message is being sent to and invoke its handler.
        /// </summary>
        /// <param name="message">The incoming message</param>
        public void OnNext(GeneralGroupCommunicationMessage message)
        {
            string operatorName = message.OperatorName;

            IObserver<GeneralGroupCommunicationMessage> handler = GetOperatorHandler(operatorName);
            if (handler == null)
            {
                throw new GroupCommunicationException(
                    new ArgumentException("No handler registered with the operator name: " + operatorName));
            }

            handler.OnNext(message);
        }

        /// <summary>
        /// GetOperatorHandler for operatorName
        /// </summary>
        /// <param name="operatorName"></param>
        /// <returns></returns>
        private IObserver<GeneralGroupCommunicationMessage> GetOperatorHandler(string operatorName)
        {
            IObserver<GeneralGroupCommunicationMessage> handler;
            if (!_handlers.TryGetValue(operatorName, out handler))
            {
                throw new GroupCommunicationException(
                    new ApplicationException("No handler registered yet with the operator name: " + operatorName));
            }
            return handler;
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
            foreach (var handler in _handlers)
            {
                handler.Value.OnError(exception);
            }
        }

        public void OnCompleted()
        {
        }
    }
}
