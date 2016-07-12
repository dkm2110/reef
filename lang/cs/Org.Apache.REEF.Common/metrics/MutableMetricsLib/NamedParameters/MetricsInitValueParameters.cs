using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.Apache.REEF.Tang.Annotations;

namespace Org.Apache.REEF.Common.metrics.MutableMetricsLib.NamedParameters
{
    /// <summary>
    /// Class with named parameters for specifying intial values of the metric.
    /// </summary>
    public sealed class MetricsInitValueParameters
    {
        [NamedParameter("Initial value for any metric with integer value", "initint", "0")]
        public sealed class IntMetricInitValue : Name<int>
        {
        }

        [NamedParameter("Initial value for any metric with float value", "initfloat", "0")]
        public sealed class FloatMetricInitValue : Name<float>
        {
        }

        [NamedParameter("Initial value for any metric with double value", "initdouble", "0")]
        public sealed class DoubleMetricInitValue : Name<double>
        {
        }

        [NamedParameter("Initial value for any metric with long value", "initlong", "0")]
        public sealed class LongMetricInitValue : Name<long>
        {
        }
    }
}
