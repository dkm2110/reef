using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Org.Apache.REEF.Common.metrics.Api;

namespace Org.Apache.REEF.Common.metrics.MutableMetricsLib
{
    public abstract class MutableMetric
    {
        private bool _changed = true;

        public abstract void TakeSnapshot(IMetricsRecordBuilder recordBuilder, bool all);

        public void TakeSnapshot(IMetricsRecordBuilder recordBuilder)
        {
            TakeSnapshot(recordBuilder, false);
        }

        protected void SetChanged()
        {
            _changed = true;
        }

        protected void ClearChanged()
        {
            _changed = false;
        }

        public bool Changed
        {
            get { return _changed; }
        }
    }
}
