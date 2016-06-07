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
using System.IO;
using System.Net;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Org.Apache.REEF.Tang.Implementations.Tang;
using Org.Apache.REEF.Tang.Interface;
using Org.Apache.REEF.Wake.Remote;
using Org.Apache.REEF.Wake.Remote.Impl;
using Org.Apache.REEF.Wake.Remote.Parameters;
using Org.Apache.REEF.Wake.StreamingCodec;
using Org.Apache.REEF.Wake.StreamingCodec.CommonStreamingCodecs;
using Xunit;

namespace Org.Apache.REEF.Wake.Tests
{
    /// <summary>
    /// Tests the StreamingTransportServer, StreamingTransportClient and StreamingLink.
    /// Basically the Wake transport layer.
    /// </summary>
    public class StreamingTransportTest
    {
        private readonly ITcpPortProvider _tcpPortProvider = GetTcpProvider(9900, 9940);
        private readonly IInjector _injector = TangFactory.GetTang().NewInjector();
        private readonly ITcpClientConnectionFactory _tcpClientFactory = GetTcpClientFactory(5, 500);

        /// <summary>
        /// Tests whether StreamingTransportServer receives 
        /// string messages from StreamingTransportClient
        /// </summary>
        [Fact]
        public void TestStreamingTransportServer()
        {
            BlockingCollection<string> queue = new BlockingCollection<string>();
            List<string> events = new List<string>();
            IStreamingCodec<string> stringCodec = _injector.GetInstance<StringStreamingCodec>();

            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
            var remoteHandler = Observer.Create<TransportEvent<string>>(tEvent => queue.Add(tEvent.Data));

            using (
                var server = new StreamingTransportServer<string>(endpoint.Address,
                    remoteHandler,
                    _tcpPortProvider,
                    stringCodec))
            {
                server.Run();

                IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), server.LocalEndpoint.Port);
                using (var client = new StreamingTransportClient<string>(remoteEndpoint, stringCodec, _tcpClientFactory))
                {
                    client.Send("Hello");
                    client.Send(", ");
                    client.Send("World!");

                    events.Add(queue.Take());
                    events.Add(queue.Take());
                    events.Add(queue.Take());
                }
            }

            Assert.Equal(3, events.Count);
            Assert.Equal(events[0], "Hello");
            Assert.Equal(events[1], ", ");
            Assert.Equal(events[2], "World!");
        }

        /// <summary>
        /// Checks whether StreamingTransportClient is able to receive messages from remote host
        /// </summary>
        [Fact]
        public void TestStreamingTransportSenderStage()
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);

            List<string> events = new List<string>();
            BlockingCollection<string> queue = new BlockingCollection<string>();
            IStreamingCodec<string> stringCodec = _injector.GetInstance<StringStreamingCodec>();

            // Server echoes the message back to the client
            var remoteHandler = Observer.Create<TransportEvent<string>>(tEvent => tEvent.Link.Write(tEvent.Data));

            using (
                var server = new StreamingTransportServer<string>(endpoint.Address,
                    remoteHandler,
                    _tcpPortProvider,
                    stringCodec))
            {
                server.Run();

                var clientHandler = Observer.Create<TransportEvent<string>>(tEvent => queue.Add(tEvent.Data));
                IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), server.LocalEndpoint.Port);
                using (
                    var client = new StreamingTransportClient<string>(remoteEndpoint,
                        clientHandler,
                        stringCodec,
                        _tcpClientFactory))
                {
                    client.Send("Hello");
                    client.Send(", ");
                    client.Send(" World");

                    events.Add(queue.Take());
                    events.Add(queue.Take());
                    events.Add(queue.Take());
                }
            }

            Assert.Equal(3, events.Count);
            Assert.Equal(events[0], "Hello");
            Assert.Equal(events[1], ", ");
            Assert.Equal(events[2], " World");
        }

        /// <summary>
        /// Checks whether StreamingTransportClient and StreamingTransportServer works 
        /// in asynchronous condition while sending messages asynchronously from different 
        /// threads
        /// </summary>
        [Fact]
        public void TestStreamingRaceCondition()
        {
            BlockingCollection<string> queue = new BlockingCollection<string>();
            List<string> events = new List<string>();
            IStreamingCodec<string> stringCodec = _injector.GetInstance<StringStreamingCodec>();
            int numEventsExpected = 150;

            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
            var remoteHandler = Observer.Create<TransportEvent<string>>(tEvent => queue.Add(tEvent.Data));

            using (
                var server = new StreamingTransportServer<string>(endpoint.Address,
                    remoteHandler,
                    _tcpPortProvider,
                    stringCodec))
            {
                server.Run();

                StreamingTransportClient<string>[] clients = new StreamingTransportClient<string>[numEventsExpected / 3];
                for (int i = 0; i < numEventsExpected / 3; i++)
                {
                    var index = i;
                    Task.Run(() =>
                    {
                        IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"),
                            server.LocalEndpoint.Port);
                        clients[index] = new StreamingTransportClient<string>(remoteEndpoint,
                            stringCodec,
                            _tcpClientFactory);
                        {
                            clients[index].Send("Hello");
                            clients[index].Send(", ");
                            clients[index].Send("World!");
                        }
                    });
                }

                for (int i = 0; i < numEventsExpected; i++)
                {
                    events.Add(queue.Take());
                }

                foreach (var client in clients)
                {
                    client.Dispose();
                }
            }

            Assert.Equal(numEventsExpected, events.Count);
        }

        /// <summary>
        /// Tests whether StreamingTransportClient's Send function 
        /// throws right exception if link or connection is ended prematurely.
        /// Also tests if OnError() call is rightly executed at the server side.
        /// </summary>
        [Fact]
        public void TestClientWriteErrorAndServerOnError()
        {
            IStreamingCodec<string> stringCodec = _injector.GetInstance<StringStreamingCodec>();
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
            var remoteHandler = new MockObserver<TransportEvent<string>>();

            using (
                var server = new StreamingTransportServer<string>(endpoint.Address,
                    remoteHandler,
                    _tcpPortProvider,
                    stringCodec))
            {
                server.Run();

                IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), server.LocalEndpoint.Port);
                var client = new StreamingTransportClient<string>(remoteEndpoint, stringCodec, _tcpClientFactory);
                client.Send("Hello");
                client.Send(", ");
                client.Link.Dispose();

                try
                {
                    client.Send("World");
                    Assert.True(false, "Expected an exception on writing to disposed server.");
                }
                catch
                {
                    // ignored
                }

                int sleepTimeinMs = 500;
                int reTries = 100;

                for (int i = 0; i < reTries; i++)
                {
                    int errorCount = remoteHandler.OnErrorCounter;
                    if (errorCount > 0)
                    {
                        Assert.NotNull(remoteHandler.ThrownException.InnerException);
                        return;
                    }
                    Thread.Sleep(sleepTimeinMs);
                }
                Assert.True(false, "OnError condition in StreamingTransportServer not reached");
            }
        }

        /// <summary>
        /// Tests whether OnError() call is rightly executed at the server 
        /// side if Client completes itself.
        /// </summary>
        [Fact]
        public void TestServerOnError()
        {
            IStreamingCodec<string> stringCodec = _injector.GetInstance<StringStreamingCodec>();
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
            var remoteHandler = new MockObserver<TransportEvent<string>>();

            int sleepTimeinMs = 500;
            int reTries = 100;
            using (
                var server = new StreamingTransportServer<string>(endpoint.Address,
                    remoteHandler,
                    _tcpPortProvider,
                    stringCodec))
            {
                server.Run();

                IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), server.LocalEndpoint.Port);
                using (
                    var client = new StreamingTransportClient<string>(remoteEndpoint,
                        stringCodec,
                        _tcpClientFactory))
                {
                    client.Send("Hello");
                    client.Send(", ");
                    client.Send("World");

                    for (int i = 0; i < reTries; i++)
                    {
                        int receivedCount = remoteHandler.OnNextCounter;
                        if (receivedCount == 3)
                        {
                            break;
                        }
                        Thread.Sleep(sleepTimeinMs);
                    }

                    if (remoteHandler.OnNextCounter != 3)
                    {
                        Assert.True(false, "Number of received messages are not equal to 3");
                    }
                }

                for (int i = 0; i < reTries; i++)
                {
                    int errorCount = remoteHandler.OnErrorCounter;
                    if (errorCount > 0)
                    {
                        Assert.NotNull(remoteHandler.ThrownException.InnerException);
                        return;
                    }
                    Thread.Sleep(sleepTimeinMs);
                }
                Assert.True(false, "OnError condition in StreamingTransportServer not reached");
            }
        }

        /// <summary>
        /// Tests whether OnCompleted() call is rightly executed at the server 
        /// side if it disposes itself before client.
        /// </summary>
        [Fact]
        public void TestServerOnCompleted()
        {
            IStreamingCodec<string> stringCodec = _injector.GetInstance<StringStreamingCodec>();
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
            var remoteHandler = new MockObserver<TransportEvent<string>>();

            int sleepTimeinMs = 500;
            int reTries = 100;
            var server = new StreamingTransportServer<string>(endpoint.Address,
                remoteHandler,
                _tcpPortProvider,
                stringCodec);
            server.Run();

            IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), server.LocalEndpoint.Port);
            var client = new StreamingTransportClient<string>(remoteEndpoint,
                stringCodec,
                _tcpClientFactory);

            client.Send("Hello");
            client.Send(", ");
            client.Send("World");

            for (int i = 0; i < reTries; i++)
            {
                int receivedCount = remoteHandler.OnNextCounter;
                if (receivedCount == 3)
                {
                    break;
                }
                Thread.Sleep(sleepTimeinMs);
            }

            if (remoteHandler.OnNextCounter != 3)
            {
                Assert.True(false, "Number of received messages are not equal to 3");
            }

            server.Dispose();
            for (int i = 0; i < reTries; i++)
            {
                int completedCount = remoteHandler.OnCompletedCounter;
                if (completedCount > 0)
                {
                    return;
                }
                if (remoteHandler.OnErrorCounter > 0)
                {
                    Assert.True(false, "Reached error condition not sure why");
                }
                Thread.Sleep(sleepTimeinMs);
            }

            Assert.True(false, "OnCompleted condition in StreamingTransportServer not reached");
        }

        private static ITcpPortProvider GetTcpProvider(int portRangeStart, int portRangeEnd)
        {
            var configuration = TangFactory.GetTang().NewConfigurationBuilder()
                .BindImplementation<ITcpPortProvider, TcpPortProvider>()
                .BindIntNamedParam<TcpPortRangeStart>(portRangeStart.ToString())
                .BindIntNamedParam<TcpPortRangeCount>((portRangeEnd - portRangeStart + 1).ToString())
                .Build();
            return TangFactory.GetTang().NewInjector(configuration).GetInstance<ITcpPortProvider>();
        }

        private static ITcpClientConnectionFactory GetTcpClientFactory(int connectionRetryCount, int sleepTimeInMs)
        {
            var config =
                TangFactory.GetTang()
                    .NewConfigurationBuilder()
                    .BindIntNamedParam<ConnectionRetryCount>(connectionRetryCount.ToString())
                    .BindIntNamedParam<SleepTimeInMs>(sleepTimeInMs.ToString())
                    .Build();
            return TangFactory.GetTang().NewInjector(config).GetInstance<ITcpClientConnectionFactory>();
        }
    }
}
