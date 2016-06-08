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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Org.Apache.REEF.Common.Tasks;
using Org.Apache.REEF.Driver;
using Org.Apache.REEF.Driver.Context;
using Org.Apache.REEF.Driver.Evaluator;
using Org.Apache.REEF.Driver.Task;
using Org.Apache.REEF.Wake.StreamingCodec.CommonStreamingCodecs;
using Org.Apache.REEF.Network.Group.Config;
using Org.Apache.REEF.Network.Group.Driver;
using Org.Apache.REEF.Network.Group.Driver.Impl;
using Org.Apache.REEF.Network.Group.Operators;
using Org.Apache.REEF.Network.Group.Pipelining.Impl;
using Org.Apache.REEF.Network.Group.Topology;
using Org.Apache.REEF.Tang.Annotations;
using Org.Apache.REEF.Tang.Implementations.Configuration;
using Org.Apache.REEF.Tang.Implementations.Tang;
using Org.Apache.REEF.Tang.Interface;
using Org.Apache.REEF.Tang.Util;
using Org.Apache.REEF.Wake.Remote.Parameters;

namespace Org.Apache.REEF.Network.Examples.GroupCommunication.BroadcastReduceDriverAndTasks
{
    public class BroadcastReduceDriver : 
        IObserver<IAllocatedEvaluator>, 
        IObserver<IActiveContext>, 
        IObserver<IDriverStarted>,
        IObserver<ITaskMessage>,
        IObserver<IRunningTask>
    {
        private readonly int _numEvaluators;
        private readonly int _numIterations;

        private readonly IGroupCommDriver _groupCommDriver;
        private readonly ICommunicationGroupDriver _commGroup;
        private readonly TaskStarter _groupCommTaskStarter;
        private readonly IConfiguration _tcpPortProviderConfig;
        private readonly IConfiguration _codecConfig;
        private readonly IEvaluatorRequestor _evaluatorRequestor;
        private readonly ConcurrentBag<IRunningTask> _runningTasks = new ConcurrentBag<IRunningTask>();
        private readonly ISet<string> _doneTaskIds = new HashSet<string>();
        private readonly object _lockForDoneTasks = new object(); 
        private const string ClosingMessage = "please close";

        [Inject]
        private BroadcastReduceDriver(
            [Parameter(typeof(GroupTestConfig.NumEvaluators))] int numEvaluators,
            [Parameter(typeof(GroupTestConfig.NumIterations))] int numIterations,
            [Parameter(typeof(GroupTestConfig.StartingPort))] int startingPort,
            [Parameter(typeof(GroupTestConfig.PortRange))] int portRange,
            GroupCommDriver groupCommDriver,
            IEvaluatorRequestor evaluatorRequestor)
        {
            _numEvaluators = numEvaluators;
            _numIterations = numIterations;
            _groupCommDriver = groupCommDriver;
            _evaluatorRequestor = evaluatorRequestor;

            _tcpPortProviderConfig = TangFactory.GetTang().NewConfigurationBuilder()
                .BindNamedParameter<TcpPortRangeStart, int>(GenericType<TcpPortRangeStart>.Class,
                    startingPort.ToString(CultureInfo.InvariantCulture))
                .BindNamedParameter<TcpPortRangeCount, int>(GenericType<TcpPortRangeCount>.Class,
                    portRange.ToString(CultureInfo.InvariantCulture))
                .Build();

            _codecConfig = StreamingCodecConfiguration<int>.Conf
                .Set(StreamingCodecConfiguration<int>.Codec, GenericType<IntStreamingCodec>.Class)
                .Build();

            IConfiguration reduceFunctionConfig = ReduceFunctionConfiguration<int>.Conf
                .Set(ReduceFunctionConfiguration<int>.ReduceFunction, GenericType<SumFunction>.Class)
                .Build();

            IConfiguration dataConverterConfig = PipelineDataConverterConfiguration<int>.Conf
                .Set(PipelineDataConverterConfiguration<int>.DataConverter, GenericType<DefaultPipelineDataConverter<int>>.Class)
                .Build();

            _commGroup = _groupCommDriver.DefaultGroup
                    .AddBroadcast<int>(
                        GroupTestConstants.BroadcastOperatorName,
                        GroupTestConstants.MasterTaskId, 
                        TopologyTypes.Tree,
                        dataConverterConfig)
                    .AddReduce<int>(
                        GroupTestConstants.ReduceOperatorName,
                        GroupTestConstants.MasterTaskId,
                        TopologyTypes.Tree,
                        reduceFunctionConfig,
                        dataConverterConfig)
                    .Build();

            _groupCommTaskStarter = new TaskStarter(_groupCommDriver, numEvaluators);
        }

        /// <summary>
        /// Submits Group communication configuration as part of service
        /// </summary>
        /// <param name="allocatedEvaluator">Allocated evaluator</param>
        public void OnNext(IAllocatedEvaluator allocatedEvaluator)
        {
            IConfiguration contextConf = _groupCommDriver.GetContextConfiguration();
            IConfiguration serviceConf = _groupCommDriver.GetServiceConfiguration();
            serviceConf = Configurations.Merge(serviceConf, _tcpPortProviderConfig, _codecConfig);
            allocatedEvaluator.SubmitContextAndService(contextConf, serviceConf);
        }

        /// <summary>
        /// Submits master or slave task.
        /// </summary>
        /// <param name="activeContext">Active context to which task is submitted.</param>
        public void OnNext(IActiveContext activeContext)
        {
            if (_groupCommDriver.IsMasterTaskContext(activeContext))
            {
                // Configure Master Task
                IConfiguration partialTaskConf = TangFactory.GetTang().NewConfigurationBuilder(
                    TaskConfiguration.ConfigurationModule
                        .Set(TaskConfiguration.Identifier, GroupTestConstants.MasterTaskId)
                        .Set(TaskConfiguration.Task, GenericType<MasterTask>.Class)
                        .Set(TaskConfiguration.OnSendMessage, GenericType<MasterTask>.Class)
                        .Set(TaskConfiguration.OnClose, GenericType<MasterTask>.Class)
                        .Build())
                    .BindNamedParameter<GroupTestConfig.NumEvaluators, int>(
                        GenericType<GroupTestConfig.NumEvaluators>.Class,
                        _numEvaluators.ToString(CultureInfo.InvariantCulture))
                    .BindNamedParameter<GroupTestConfig.NumIterations, int>(
                        GenericType<GroupTestConfig.NumIterations>.Class,
                        _numIterations.ToString(CultureInfo.InvariantCulture))
                    .Build();

                _commGroup.AddTask(GroupTestConstants.MasterTaskId);
                _groupCommTaskStarter.QueueTask(partialTaskConf, activeContext);
            }
            else
            {
                // Configure Slave Task
                string slaveTaskId = "SlaveTask-" + activeContext.Id;
                IConfiguration partialTaskConf = TangFactory.GetTang().NewConfigurationBuilder(
                    TaskConfiguration.ConfigurationModule
                        .Set(TaskConfiguration.Identifier, slaveTaskId)
                        .Set(TaskConfiguration.Task, GenericType<SlaveTask>.Class)
                        .Set(TaskConfiguration.OnSendMessage, GenericType<SlaveTask>.Class)
                        .Set(TaskConfiguration.OnClose, GenericType<SlaveTask>.Class)
                        .Build())
                    .BindNamedParameter<GroupTestConfig.NumEvaluators, int>(
                        GenericType<GroupTestConfig.NumEvaluators>.Class,
                        _numEvaluators.ToString(CultureInfo.InvariantCulture))
                    .BindNamedParameter<GroupTestConfig.NumIterations, int>(
                        GenericType<GroupTestConfig.NumIterations>.Class,
                        _numIterations.ToString(CultureInfo.InvariantCulture))
                    .Build();

                _commGroup.AddTask(slaveTaskId);
                _groupCommTaskStarter.QueueTask(partialTaskConf, activeContext);
            }
        }

        /// <summary>
        /// Requests evaluators once driver starts
        /// </summary>
        public void OnNext(IDriverStarted value)
        {
            var request =
                _evaluatorRequestor.NewBuilder()
                    .SetNumber(_numEvaluators)
                    .SetMegabytes(512)
                    .SetCores(2)
                    .SetRackName("WonderlandRack")
                    .SetEvaluatorBatchId("BroadcastEvaluator")
                    .Build();
            _evaluatorRequestor.Submit(request);
        }

        public void OnError(Exception error)
        {
            throw error;
        }

        /// <summary>
        /// Adds running task to the list. This is needed later on to send the stop message.
        /// </summary>
        /// <param name="runningTask">Running task.</param>
        public void OnNext(IRunningTask runningTask)
        {
            _runningTasks.Add(runningTask);
        }

        /// <summary>
        /// Specifies what to do when the message is received from task.
        /// If we receive message from task that it is done (done = 1), and 
        /// we got these messages from all the evaluators we send close signal to 
        /// all the tasks.
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(ITaskMessage value)
        {
            lock (_lockForDoneTasks)
            {
                int done = BitConverter.ToInt32(value.Message, 0);
                if (!_doneTaskIds.Contains(value.TaskId) && done == 1)
                {
                    _doneTaskIds.Add(value.TaskId);
                    if (_doneTaskIds.Count == _numEvaluators)
                    {
                        foreach (var runningTask in _runningTasks)
                        {
                            runningTask.Dispose(Encoding.UTF8.GetBytes(ClosingMessage));
                        }
                    }
                }
            }
        }

        public void OnCompleted()
        {
        }

        private class SumFunction : IReduceFunction<int>
        {
            [Inject]
            public SumFunction()
            {
            }

            public int Reduce(IEnumerable<int> elements)
            {
                return elements.Sum();
            }
        }
    }
}
