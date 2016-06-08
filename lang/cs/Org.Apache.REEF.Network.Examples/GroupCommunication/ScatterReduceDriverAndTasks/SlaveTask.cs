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
using System.Threading;
using Org.Apache.REEF.Common.Tasks;
using Org.Apache.REEF.Common.Tasks.Events;
using Org.Apache.REEF.Network.Group.Operators;
using Org.Apache.REEF.Network.Group.Task;
using Org.Apache.REEF.Tang.Annotations;
using Org.Apache.REEF.Utilities;
using Org.Apache.REEF.Utilities.Logging;

namespace Org.Apache.REEF.Network.Examples.GroupCommunication.ScatterReduceDriverAndTasks
{
    public class SlaveTask : ITask, IObserver<ICloseEvent>, ITaskMessageSource
    {
        private static readonly Logger Logger = Logger.GetLogger(typeof(SlaveTask));

        private readonly IGroupCommClient _groupCommClient;
        private readonly ICommunicationGroupClient _commGroup;
        private readonly IScatterReceiver<int> _scatterReceiver;
        private readonly IReduceSender<int> _sumSender;
        private readonly ManualResetEventSlim _waitToCloseEvent = new ManualResetEventSlim(false);
        private int _disposed = 0;
        private int _doneMessage = 0;

        [Inject]
        public SlaveTask(IGroupCommClient groupCommClient)
        {
            Logger.Log(Level.Info, "Hello from slave task");

            _groupCommClient = groupCommClient;
            _commGroup = _groupCommClient.GetCommunicationGroup(GroupTestConstants.GroupName);
            _scatterReceiver = _commGroup.GetScatterReceiver<int>(GroupTestConstants.ScatterOperatorName);
            _sumSender = _commGroup.GetReduceSender<int>(GroupTestConstants.ReduceOperatorName);
        }

        public byte[] Call(byte[] memento)
        {
            List<int> data = _scatterReceiver.Receive();
            Logger.Log(Level.Info, "Received data: {0}", string.Join(" ", data));

            int sum = data.Sum();
            Logger.Log(Level.Info, "Sending back sum: {0}", sum);
            _sumSender.Send(sum);

            _doneMessage = 1;
            _waitToCloseEvent.Wait();
            return null;
        }

        /// <summary>
        /// Disposes the Group comm. client.
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                _groupCommClient.Dispose();
            }
        }

        private List<string> GetScatterOrder()
        {
            return new List<string> { "SlaveTask-4", "SlaveTask-3", "SlaveTask-2", "SlaveTask-1" };
        }

        /// <summary>
        /// Signals the task to exit
        /// </summary>
        /// <param name="value">close event. Does not matter in this case.</param>
        public void OnNext(ICloseEvent value)
        {
            _waitToCloseEvent.Set();
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }

        public Optional<TaskMessage> Message 
        {
            get
            {
                int done = _doneMessage;
                TaskMessage message = TaskMessage.From(
                    "slave",
                    BitConverter.GetBytes(done));
                return Optional<TaskMessage>.Of(message);
            }
            set
            {    
            } 
        }
    }
}
