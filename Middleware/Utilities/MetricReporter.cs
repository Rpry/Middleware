using System;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace Middleware.Utilities
{
    public class MetricReporter
    {
        private readonly ILogger<MetricReporter> _logger;
        private readonly Counter _requestCounter;
        private readonly Histogram _responseTimeHistogram;

        public MetricReporter(ILogger<MetricReporter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _requestCounter =
                Metrics.CreateCounter("total_requests", "The total number of requests serviced by this API.");

            _responseTimeHistogram = Metrics.CreateHistogram("request_duration_seconds",
                "The duration in seconds between the response to a request.", new HistogramConfiguration
                {
                    Buckets = Histogram.ExponentialBuckets(0.01, 2, 10),
                    LabelNames = new[] { "status_code", "method" }
                });
        }

        public void IncrementRequestCounter()
        {
            _requestCounter.Inc();
        }

        public void AddResponseTime(int statusCode, string method, TimeSpan elapsed)
        {
            _responseTimeHistogram.Labels(statusCode.ToString(), method).Observe(elapsed.TotalSeconds);
        }
    }
}