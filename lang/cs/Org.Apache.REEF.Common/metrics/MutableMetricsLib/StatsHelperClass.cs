using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Org.Apache.REEF.Common.metrics.MutableMetricsLib
{
    internal sealed class StatsHelperClass
    {
        private double _mean = 0;
        private double _unNormalizedVariance = 0;
        private long _numSamples = 0;
        private readonly MinMax _minMax = new MinMax();

        internal void CopyFrom(StatsHelperClass other)
        {
            _numSamples = other.NumSamples;
            _mean = other.Mean;
            _unNormalizedVariance = other.Variance * (_numSamples - 1);
            _minMax.Reset(other.MinMaxModule);
        }

        internal double Mean
        {
            get { return _mean; }
        }

        internal long NumSamples
        {
            get { return _numSamples; }
        }

        internal double Variance
        {
            get { return _numSamples > 1 ? _unNormalizedVariance / (_numSamples - 1) : 0; }
        }

        internal double Std
        {
            get { return Math.Sqrt(Variance); }
        }

        internal MinMax MinMaxModule
        {
            get { return _minMax; }
        }

        internal void Reset()
        {
            _unNormalizedVariance = _mean = _numSamples = 0;
            _minMax.Reset();
        }

        internal StatsHelperClass Add(double value)
        {
            _minMax.Add(value);
            _numSamples++;

            if (_numSamples == 1)
            {
                _mean = value;
            }
            else
            {
                double oldMean = _mean;
                _mean += (value - _mean) / _numSamples;
                _unNormalizedVariance = (value - oldMean) * (value - _mean);
            }
            return this;
        }

        internal sealed class MinMax
        {
            const double DefaultMinValue = double.MaxValue;
            const double DefaultMaxValue = double.MinValue;

            private double _min = DefaultMinValue;
            private double _max = DefaultMaxValue;

            public void Add(double value)
            {
                _min = Math.Min(_min, value);
                _max = Math.Max(_max, value);
            }

            public void Reset()
            {
                _min = DefaultMinValue;
                _max = DefaultMaxValue;
            }

            public void Reset(MinMax other)
            {
                _min = other.Min;
                _max = other.Max;
            }

            public double Min
            {
                get { return _min; }
            }

            public double Max
            {
                get { return _max; }
            }
        }
    }
}
