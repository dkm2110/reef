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
using System.Threading;
using System.Threading.Tasks;
using Org.Apache.REEF.Tang.Annotations;
using Org.Apache.REEF.Wake.Remote;
using Org.Apache.REEF.Wake.StreamingCodec;
using Org.Apache.REEF.Wake.StreamingCodec.CommonStreamingCodecs;

namespace Org.Apache.REEF.Network.Tests
{
    /// <summary>
    /// Buggy string streaming codec. The Reads always fail
    /// </summary>
    internal class BuggyStringStreamingCodec : IStreamingCodec<string>
    {
        private readonly StringStreamingCodec _actualCodec;

        [Inject]
        private BuggyStringStreamingCodec(StringStreamingCodec actualStringCodec)
        {
            _actualCodec = actualStringCodec;
        }

        public string Read(IDataReader reader)
        {
            int length = reader.ReadInt32();
            throw new Exception("I am supposed to fail in Read");
        }

        public void Write(string obj, IDataWriter writer)
        {
            _actualCodec.Write(obj, writer);
        }

        public async Task<string> ReadAsync(IDataReader reader, CancellationToken token)
        {
            int length = await reader.ReadInt32Async(token);
            throw new Exception("I am supposed to fail in Read");
        }

        public async Task WriteAsync(string obj, IDataWriter writer, CancellationToken token)
        {
            await _actualCodec.WriteAsync(obj, writer, token);
        }
    }
}
