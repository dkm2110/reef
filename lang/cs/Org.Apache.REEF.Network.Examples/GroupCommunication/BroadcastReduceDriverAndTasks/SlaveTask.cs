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
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Org.Apache.REEF.Common.Tasks;
using Org.Apache.REEF.Common.Tasks.Events;
using Org.Apache.REEF.Network.Group.Operators;
using Org.Apache.REEF.Network.Group.Task;
using Org.Apache.REEF.Tang.Annotations;
using Org.Apache.REEF.Utilities;
using Org.Apache.REEF.Utilities.Logging;

namespace Org.Apache.REEF.Network.Examples.GroupCommunication.BroadcastReduceDriverAndTasks
{
    public class SlaveTask : ITask, IObserver<ICloseEvent>, ITaskMessageSource
    {
        private static readonly Logger Logger = Logger.GetLogger(typeof(SlaveTask));

        private readonly int _numIterations;
        private readonly IGroupCommClient _groupCommClient;
        private readonly ICommunicationGroupClient _commGroup;
        private readonly IBroadcastReceiver<int> _broadcastReceiver;
        private readonly IReduceSender<int> _triangleNumberSender;
        private readonly ManualResetEventSlim _waitToCloseEvent = new ManualResetEventSlim(false);
        private int _disposed = 0;
        private int _doneMessage = 0;

        [Inject]
        public SlaveTask(
            [Parameter(typeof(GroupTestConfig.NumIterations))] int numIters,
            IGroupCommClient groupCommClient)
        {
            Logger.Log(Level.Info, "Hello from slave task");

            _numIterations = numIters;
            _groupCommClient = groupCommClient;
            _commGroup = _groupCommClient.GetCommunicationGroup(GroupTestConstants.GroupName);
            _broadcastReceiver = _commGroup.GetBroadcastReceiver<int>(GroupTestConstants.BroadcastOperatorName);
            _triangleNumberSender = _commGroup.GetReduceSender<int>(GroupTestConstants.ReduceOperatorName);
        }

        public byte[] Call(byte[] memento)
        {
            Stopwatch broadcastTime = new Stopwatch();
            Stopwatch reduceTime = new Stopwatch();

            for (int i = 0; i < _numIterations; i++)
            {
                broadcastTime.Start();

                // Receive n from Master Task
                int n = _broadcastReceiver.Receive();
                broadcastTime.Stop();

                Logger.Log(Level.Info, "Calculating TriangleNumber({0}) on slave task...", n);

                // Calculate the nth Triangle number and send it back to driver
                int triangleNum = TriangleNumber(n);
                Logger.Log(Level.Info, "Sending sum: {0} on iteration {1}.", triangleNum, i);
                
                reduceTime.Start();
                _triangleNumberSender.Send(triangleNum);
                reduceTime.Stop();
                
                if (i >= 1)
                {
                    var msg = string.Format("Average time (milliseconds) taken for broadcast: {0} and reduce: {1}",
                            broadcastTime.ElapsedMilliseconds / ((double)i),
                            reduceTime.ElapsedMilliseconds / ((double)i));
                    Logger.Log(Level.Info, msg);
                }
            }

            _doneMessage = 1;
            _waitToCloseEvent.Wait();
            return null;
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                _groupCommClient.Dispose();
            }
        }

        /// <summary>
        /// Signals the task to exit
        /// </summary>
        /// <param name="value">Close event. Does not matter in this case.</param>
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

        /// <summary>
        /// Message to be sent to driver. Sends done signal once task is 
        /// ready to exit.
        /// </summary>
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

        private int TriangleNumber(int n)
        {
            return Enumerable.Range(1, n).Sum();
        }
    }
}
